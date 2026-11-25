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
    public void SetData(IList<Vector2> data, float minX, float maxX, float minY, float maxY)
    {
        points.Clear();
        float w = Mathf.Max(maxX - minX, 0.0001f);
        float h = Mathf.Max(maxY - minY, 0.0001f);
        foreach (var p in data)
            points.Add(new Vector2((p.x - minX) / w, (p.y - minY) / h));
        SetVerticesDirty();
    }

    /// <summary>Set values spaced evenly on X (e.g. a time series).</summary>
    public void SetValues(IList<float> values, float minY, float maxY)
    {
        points.Clear();
        float h = Mathf.Max(maxY - minY, 0.0001f);
        int n = values.Count;
        for (int i = 0; i < n; i++)
        {
            float x = n > 1 ? (float)i / (n - 1) : 0f;
            points.Add(new Vector2(x, (values[i] - minY) / h));
        }
        SetVerticesDirty();
    }

    /// <summary>Set the goal using a real value and the same Y range as your data.</summary>
    public void SetGoal(float value, float minY, float maxY)
    {
        goalNormalised = Mathf.Clamp01((value - minY) / Mathf.Max(maxY - minY, 0.0001f));
        showGoal = true;
        SetVerticesDirty();
    }

    public void HideGoal()
    {
        showGoal = false;
        SetVerticesDirty();
    }

    public void AddPoint(Vector2 normalised)
    {
        points.Add(normalised);
        SetVerticesDirty();
    }

    public void Clear()
    {
        points.Clear();
        SetVerticesDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        Rect r = GetPixelAdjustedRect();
        Debug.Log($"mesh: points {points.Count}, showGoal {showGoal}, goal {goalNormalised:F2}, rect {r}");

        // Drawn first so the data renders on top of it.
        if (showGoal)
        {
            float y = r.yMin + goalNormalised * r.height;
            Vector2 a = new Vector2(r.xMin, y);
            Vector2 b = new Vector2(r.xMax, y);
            if (goalDashed) AddDashedLine(vh, a, b, goalThickness, goalColor);
            else AddLineQuad(vh, a, b, goalThickness, goalColor);
        }

        if (points.Count == 0) return;

        var pos = new Vector2[points.Count];
        for (int i = 0; i < points.Count; i++)
        {
            pos[i] = new Vector2(
                r.xMin + points[i].x * r.width,
                r.yMin + points[i].y * r.height);
        }

        for (int i = 0; i < pos.Length - 1; i++)
        {
            float dy = points[i + 1].y - points[i].y;
            Color c = dy > 0.0001f ? upColor : (dy < -0.0001f ? downColor : flatColor);
            AddLineQuad(vh, pos[i], pos[i + 1], lineThickness, c);
        }

        if (drawPoints)
            for (int i = 0; i < pos.Length; i++)
                AddSquare(vh, pos[i], pointSize, pointColor);
    }

    private void AddDashedLine(VertexHelper vh, Vector2 a, Vector2 b, float thickness, Color c)
    {
        Vector2 dir = b - a;
        float len = dir.magnitude;
        if (len < 0.01f) return;
        dir /= len;

        float step = Mathf.Max(dashLength + dashGap, 0.01f);
        for (float d = 0f; d < len; d += step)
        {
            float end = Mathf.Min(d + dashLength, len);
            AddLineQuad(vh, a + dir * d, a + dir * end, thickness, c);
        }
    }

    private void AddLineQuad(VertexHelper vh, Vector2 a, Vector2 b, float thickness, Color c)
    {
        Vector2 dir = b - a;
        if (dir.sqrMagnitude < 0.0001f) return;
        Vector2 normal = new Vector2(-dir.y, dir.x).normalized * (thickness * 0.5f);

        int start = vh.currentVertCount;
        vh.AddVert(a - normal, c, Vector2.zero);
        vh.AddVert(a + normal, c, Vector2.zero);
        vh.AddVert(b + normal, c, Vector2.zero);
        vh.AddVert(b - normal, c, Vector2.zero);
        vh.AddTriangle(start, start + 1, start + 2);
        vh.AddTriangle(start, start + 2, start + 3);
    }

    private void AddSquare(VertexHelper vh, Vector2 center, float size, Color c)
    {
        float h = size * 0.5f;
        int start = vh.currentVertCount;
        vh.AddVert(new Vector2(center.x - h, center.y - h), c, Vector2.zero);
        vh.AddVert(new Vector2(center.x - h, center.y + h), c, Vector2.zero);
        vh.AddVert(new Vector2(center.x + h, center.y + h), c, Vector2.zero);
        vh.AddVert(new Vector2(center.x + h, center.y - h), c, Vector2.zero);
        vh.AddTriangle(start, start + 1, start + 2);
        vh.AddTriangle(start, start + 2, start + 3);
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        SetVerticesDirty();
    }
#endif
}
