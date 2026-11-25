using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Feeds a UIGraph with a running score total. Call AddScore with a positive or
/// negative amount and a new point is plotted. The Y range auto-fits the data.
/// </summary>
public class ScoreGraphFeeder : MonoBehaviour
{
    [SerializeField] private UIGraph graph;
    [Tooltip("Current score. Editing this at runtime plots a new point.")]
    [SerializeField] private float score;
    [SerializeField] private int maxPoints = 100;
    [SerializeField, Range(0f, 0.5f)] private float padding = 0.1f;

    [Header("Goal")]
    [SerializeField] private bool showGoal = true;
    [SerializeField] private float goal = 100f;

    [Header("X axis")]
    [Tooltip("Off: one evenly spaced point per score event. On: points are spaced by real time.")]
    [SerializeField] private bool useTimeAxis;

    private readonly List<float> totals = new List<float>();
    private readonly List<float> times = new List<float>();
    private float startTime;

    private void Start()
    {
        ResetGraph();
    }

    public void AddScore(float delta) {
        score += delta;
        Record();
    }

    public void SetGoal(float value) {
        goal = value;
        Refresh();
    }

    public void ResetGraph() {
        score = 0f;
        startTime = Time.time;
        totals.Clear();
        times.Clear();
        Record();
    }

    private void OnValidate() {
        if (!Application.isPlaying || totals.Count == 0) return;

        if (!Mathf.Approximately(score, totals[totals.Count - 1])) Record();
        else Refresh();
    }

    private void Record() {
        totals.Add(score);
        times.Add(Time.time - startTime);

        if (totals.Count > maxPoints) {
            totals.RemoveAt(0);
            times.RemoveAt(0);
        }

        Refresh();
    }

    private void Refresh() {
        if (graph == null || totals.Count == 0) return;

        float min = totals[0];
        float max = totals[0];
        foreach (float v in totals) {
            if (v < min) min = v;
            if (v > max) max = v;
        }

        if (showGoal) {
            min = Mathf.Min(min, goal);
            max = Mathf.Max(max, goal);
        }

        float range = Mathf.Max(max - min, 1f);
        float minY = min - range * padding;
        float maxY = max + range * padding;

        if (useTimeAxis) {
            var data = new List<Vector2>(totals.Count);
            for (int i = 0; i < totals.Count; i++)
                data.Add(new Vector2(times[i], totals[i]));
            graph.SetData(data, times[0], times[times.Count - 1], minY, maxY);
        }
        else {
            graph.SetValues(totals, minY, maxY);
        }

        if (showGoal) graph.SetGoal(goal, minY, maxY);
        else graph.HideGoal();
        
        Debug.Log($"goal {goal}, range {minY} to {maxY}, normalised {(goal - minY) / (maxY - minY)}, showGoal {showGoal}");
    }
}