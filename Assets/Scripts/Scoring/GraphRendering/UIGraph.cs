using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Scalable line graph for Unity UI. Add to a GameObject under a Canvas
/// (it replaces the Image component). Points are stored normalised (0..1),
/// so resizing the RectTransform rescales the whole graph automatically.
///
/// Features: coloured segments (rises vs drops), a horizontal goal line,
/// optional point markers.
/// </summary>
[RequireComponent(typeof(CanvasRenderer))]
public class UIGraph : MaskableGraphic
{
    [Header("Data (normalised 0..1 in both axes)")]
    [SerializeField] private List<Vector2> points = new List<Vector2>();

    [Header("Line")]
    [SerializeField] private float lineThickness = 4f;
    [Tooltip("Width of the soft fade on each side of a line, in canvas units. 0 turns it off.")]
    [SerializeField] private float edgeFeather = 1.5f;
    [SerializeField] private Color upColor = new Color(0.25f, 0.85f, 0.4f, 1f);
    [SerializeField] private Color downColor = new Color(0.95f, 0.3f, 0.3f, 1f);
    [SerializeField] private Color flatColor = new Color(0.8f, 0.8f, 0.8f, 1f);

    [Header("Points")]
    [SerializeField] private bool drawPoints = true;
    [SerializeField] private float pointSize = 10f;
    [SerializeField] private Color pointColor = Color.white;

    [Header("Goal line (horizontal, full width)")]
    [SerializeField] private bool showGoal = true;
    [SerializeField, Range(0f, 1f)] private float goalNormalised = 0.5f;
    [SerializeField] private float goalThickness = 2f;
    [SerializeField] private Color goalColor = new Color(1f, 0.85f, 0.2f, 0.9f);
    [SerializeField] private bool goalDashed = true;
    [SerializeField] private float dashLength = 12f;
    [SerializeField] private float dashGap = 8f;
    
    [Header("Goal label")]
    [Tooltip("Optional text child. Shown on the right edge, just above the goal line.")]
    [SerializeField] private TMP_Text goalLabel;
    [SerializeField] private string goalLabelFormat = "0.##";
    [SerializeField] private Vector2 goalLabelOffset = new Vector2(-6f, 2f);

    private float labelValue;

    public override Texture mainTexture => s_WhiteTexture;

    /// <summary>Set raw data; it is normalised to the given ranges.</summary>
    public void SetData(IList<Vector2> rawPoints, float minX, float maxX, float minY, float maxY) {
        points.Clear();
        float rangeWidth = Mathf.Max(maxX - minX, 0.0001f);
        float rangeHeight = Mathf.Max(maxY - minY, 0.0001f);
        foreach (var rawPoint in rawPoints)
            points.Add(new Vector2((rawPoint.x - minX) / rangeWidth, (rawPoint.y - minY) / rangeHeight));
        SetVerticesDirty();
    }

    /// <summary>Set values spaced evenly on X (e.g. a time series).</summary>
    public void SetValues(IList<float> values, float minY, float maxY) {
        points.Clear();
        float rangeHeight = Mathf.Max(maxY - minY, 0.0001f);
        int valueCount = values.Count;
        for (int valueIndex = 0; valueIndex < valueCount; valueIndex++) {
            float normalisedX = valueCount > 1 ? (float)valueIndex / (valueCount - 1) : 0f;
            points.Add(new Vector2(normalisedX, (values[valueIndex] - minY) / rangeHeight));
        }
        SetVerticesDirty();
    }

    /// <summary>Set the goal using a real value and the same Y range as your data.</summary>
    public void SetGoal(float goalValue, float minY, float maxY) {
        goalNormalised = Mathf.Clamp01((goalValue - minY) / Mathf.Max(maxY - minY, 0.0001f));
        labelValue = goalValue;
        showGoal = true;
        UpdateGoalLabel();
        SetVerticesDirty();
    }

    public void HideGoal() {
        showGoal = false;
        UpdateGoalLabel();
        SetVerticesDirty();
    }

    public void AddPoint(Vector2 normalisedPoint) {
        points.Add(normalisedPoint);
        SetVerticesDirty();
    }

    public void Clear() {
        points.Clear();
        SetVerticesDirty();
    }
    
    protected override void OnEnable() {
        base.OnEnable();
        UpdateGoalLabel();
    }

    /// <summary>
    /// Anchors the label to the right edge at the goal's height, so it follows the line
    /// when the graph is resized. It flips below the line when the goal is near the top.
    /// </summary>
    private void UpdateGoalLabel() {
        if (goalLabel == null) return;

        goalLabel.enabled = showGoal;
        goalLabel.raycastTarget = false;
        goalLabel.text = labelValue.ToString(goalLabelFormat);

        bool placeBelowLine = goalNormalised > 0.9f;
        goalLabel.alignment = placeBelowLine ? TextAlignmentOptions.TopRight : TextAlignmentOptions.BottomRight;

        RectTransform labelRect = goalLabel.rectTransform;
        labelRect.anchorMin = new Vector2(1f, goalNormalised);
        labelRect.anchorMax = new Vector2(1f, goalNormalised);
        labelRect.pivot = new Vector2(1f, placeBelowLine ? 1f : 0f);
        labelRect.anchoredPosition = new Vector2(goalLabelOffset.x, placeBelowLine ? -goalLabelOffset.y : goalLabelOffset.y);
    }

    protected override void OnPopulateMesh(VertexHelper vertexHelper) {
        vertexHelper.Clear();
        Rect graphRect = GetPixelAdjustedRect();
        Debug.Log($"mesh: points {points.Count}, showGoal {showGoal}, goal {goalNormalised:F2}, rect {graphRect}");

        // Drawn first so the data renders on top of it.
        if (showGoal) {
            float goalY = graphRect.yMin + goalNormalised * graphRect.height;
            Vector2 goalStart = new Vector2(graphRect.xMin, goalY);
            Vector2 goalEnd = new Vector2(graphRect.xMax, goalY);
            if (goalDashed) AddDashedLine(vertexHelper, goalStart, goalEnd, goalThickness, goalColor);
            else AddLineQuad(vertexHelper, goalStart, goalEnd, goalThickness, goalColor);
        }

        if (points.Count == 0) return;

        var pixelPositions = new Vector2[points.Count];
        for (int pointIndex = 0; pointIndex < points.Count; pointIndex++) {
            pixelPositions[pointIndex] = new Vector2(
                graphRect.xMin + points[pointIndex].x * graphRect.width,
                graphRect.yMin + points[pointIndex].y * graphRect.height);
        }

        for (int segmentIndex = 0; segmentIndex < pixelPositions.Length - 1; segmentIndex++) {
            float heightChange = points[segmentIndex + 1].y - points[segmentIndex].y;
            Color segmentColor = heightChange > 0.0001f ? upColor : (heightChange < -0.0001f ? downColor : flatColor);
            AddLineQuad(vertexHelper, pixelPositions[segmentIndex], pixelPositions[segmentIndex + 1], lineThickness, segmentColor);
        }

        if (drawPoints)
            for (int pointIndex = 0; pointIndex < pixelPositions.Length; pointIndex++)
                AddSquare(vertexHelper, pixelPositions[pointIndex], pointSize, pointColor);
    }

    private void AddDashedLine(VertexHelper vertexHelper, Vector2 lineStart, Vector2 lineEnd, float thickness, Color lineColor) {
        Vector2 direction = lineEnd - lineStart;
        float totalLength = direction.magnitude;
        if (totalLength < 0.01f) return;
        direction /= totalLength;

        float dashStep = Mathf.Max(dashLength + dashGap, 0.01f);
        for (float distanceAlongLine = 0f; distanceAlongLine < totalLength; distanceAlongLine += dashStep) {
            float dashEndDistance = Mathf.Min(distanceAlongLine + dashLength, totalLength);
            AddLineQuad(vertexHelper, lineStart + direction * distanceAlongLine, lineStart + direction * dashEndDistance, thickness, lineColor);
        }
    }

    /// <summary>
    /// Draws a straight line between two points as a strip of three rectangles:
    /// a solid core, plus a fringe on each side that fades to transparent to smooth the edges.
    /// </summary>
    private void AddLineQuad(VertexHelper vertexHelper, Vector2 lineStart, Vector2 lineEnd, float thickness, Color lineColor) {
        Vector2 direction = lineEnd - lineStart;
        if (direction.sqrMagnitude < 0.0001f) return;

        Vector2 sideDirection = new Vector2(-direction.y, direction.x).normalized;
        Vector2 coreOffset = sideDirection * (thickness * 0.5f);
        Vector2 fringeOffset = sideDirection * (thickness * 0.5f + edgeFeather);
        Color fringeColor = new Color(lineColor.r, lineColor.g, lineColor.b, 0f);

        int firstVertexIndex = vertexHelper.currentVertCount;

        // Start of the line: left fringe, left core, right core, right fringe.
        vertexHelper.AddVert(lineStart - fringeOffset, fringeColor, Vector2.zero);
        vertexHelper.AddVert(lineStart - coreOffset, lineColor, Vector2.zero);
        vertexHelper.AddVert(lineStart + coreOffset, lineColor, Vector2.zero);
        vertexHelper.AddVert(lineStart + fringeOffset, fringeColor, Vector2.zero);

        // End of the line, in the same order.
        vertexHelper.AddVert(lineEnd - fringeOffset, fringeColor, Vector2.zero);
        vertexHelper.AddVert(lineEnd - coreOffset, lineColor, Vector2.zero);
        vertexHelper.AddVert(lineEnd + coreOffset, lineColor, Vector2.zero);
        vertexHelper.AddVert(lineEnd + fringeOffset, fringeColor, Vector2.zero);

        for (int stripIndex = 0; stripIndex < 3; stripIndex++) {
            int stripStart = firstVertexIndex + stripIndex;
            vertexHelper.AddTriangle(stripStart, stripStart + 1, stripStart + 5);
            vertexHelper.AddTriangle(stripStart, stripStart + 5, stripStart + 4);
        }
    }

    /// <summary>Draws a filled square marker centred on a point.</summary>
    private void AddSquare(VertexHelper vertexHelper, Vector2 center, float squareSize, Color squareColor) {
        float halfSize = squareSize * 0.5f;
        int firstVertexIndex = vertexHelper.currentVertCount;
        vertexHelper.AddVert(new Vector2(center.x - halfSize, center.y - halfSize), squareColor, Vector2.zero);
        vertexHelper.AddVert(new Vector2(center.x - halfSize, center.y + halfSize), squareColor, Vector2.zero);
        vertexHelper.AddVert(new Vector2(center.x + halfSize, center.y + halfSize), squareColor, Vector2.zero);
        vertexHelper.AddVert(new Vector2(center.x + halfSize, center.y - halfSize), squareColor, Vector2.zero);
        vertexHelper.AddTriangle(firstVertexIndex, firstVertexIndex + 1, firstVertexIndex + 2);
        vertexHelper.AddTriangle(firstVertexIndex, firstVertexIndex + 2, firstVertexIndex + 3);
    }

#if UNITY_EDITOR
    protected override void OnValidate() {
        base.OnValidate();
        UpdateGoalLabel();
        SetVerticesDirty();
    }
#endif
}
