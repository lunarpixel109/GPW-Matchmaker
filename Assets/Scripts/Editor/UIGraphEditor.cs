#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// Adds debug buttons to the UIGraph inspector. Place this file in a folder named "Editor".
/// Points are re-spaced evenly along X after each change, so it suits time-series style graphs.
/// </summary>
[CustomEditor(typeof(UIGraph))]
public class UIGraphEditor : Editor
{
    private int randomCount = 10;
    private float maxStep = 0.25f;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Debug", EditorStyles.boldLabel);

        maxStep = EditorGUILayout.Slider("Max Step", maxStep, 0.01f, 1f);

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Add Point")) AddPoints(1);
            if (GUILayout.Button("Remove Last")) RemoveLast();
        }

        using (new EditorGUILayout.HorizontalScope())
        {
            randomCount = Mathf.Max(1, EditorGUILayout.IntField(randomCount, GUILayout.Width(50)));
            if (GUILayout.Button("Generate Random Data")) Generate(randomCount);
        }

        if (GUILayout.Button("Clear")) Generate(0);
    }

    private void AddPoints(int count)
    {
        var points = Modify("Add Graph Point");
        for (int i = 0; i < count; i++)
        {
            float last = points.arraySize > 0
                ? points.GetArrayElementAtIndex(points.arraySize - 1).vector2Value.y
                : 0.5f;
            Append(points, last);
        }
        Respace(points);
        Commit();
    }

    private void RemoveLast()
    {
        var points = Modify("Remove Graph Point");
        if (points.arraySize > 0) points.DeleteArrayElementAtIndex(points.arraySize - 1);
        Respace(points);
        Commit();
    }

    private void Generate(int count)
    {
        var points = Modify("Generate Graph Data");
        points.ClearArray();
        float y = 0.5f;
        for (int i = 0; i < count; i++)
        {
            y = Append(points, y);
        }
        Respace(points);
        Commit();
    }

    private SerializedProperty Modify(string undoName)
    {
        Undo.RecordObject(target, undoName);
        serializedObject.Update();
        return serializedObject.FindProperty("points");
    }

    private void Commit()
    {
        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(target);
    }

    private float Append(SerializedProperty points, float previousY)
    {
        float y = Mathf.Clamp01(previousY + Random.Range(-maxStep, maxStep));
        points.arraySize++;
        points.GetArrayElementAtIndex(points.arraySize - 1).vector2Value = new Vector2(0f, y);
        return y;
    }

    private void Respace(SerializedProperty points)
    {
        int n = points.arraySize;
        for (int i = 0; i < n; i++)
        {
            var element = points.GetArrayElementAtIndex(i);
            element.vector2Value = new Vector2(n > 1 ? (float)i / (n - 1) : 0f, element.vector2Value.y);
        }
    }
}
#endif
