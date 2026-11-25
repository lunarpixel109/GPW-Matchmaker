using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        showGoal = true;
        SetVerticesDirty();
    }

    public void HideGoal() {
        showGoal = false;
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
    /// Draws a straight line between two points as a rectangle (two triangles),
    /// extending half the thickness to each side of the line.
    /// </summary>
    private void AddLineQuad(VertexHelper vertexHelper, Vector2 lineStart, Vector2 lineEnd, float thickness, Color lineColor) {
        Vector2 direction = lineEnd - lineStart;
        if (direction.sqrMagnitude < 0.0001f) return;
        Vector2 sideOffset = new Vector2(-direction.y, direction.x).normalized * (thickness * 0.5f);

        int firstVertexIndex = vertexHelper.currentVertCount;
        vertexHelper.AddVert(lineStart - sideOffset, lineColor, Vector2.zero);
        vertexHelper.AddVert(lineStart + sideOffset, lineColor, Vector2.zero);
        vertexHelper.AddVert(lineEnd + sideOffset, lineColor, Vector2.zero);
        vertexHelper.AddVert(lineEnd - sideOffset, lineColor, Vector2.zero);
        vertexHelper.AddTriangle(firstVertexIndex, firstVertexIndex + 1, firstVertexIndex + 2);
        vertexHelper.AddTriangle(firstVertexIndex, firstVertexIndex + 2, firstVertexIndex + 3);
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
        SetVerticesDirty();
    }
#endif
}
