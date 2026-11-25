using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Feeds a UIGraph with a running score total. Call AddScore with a positive or
/// negative amount and a new point is plotted. The Y range auto-fits the data.
/// </summary>
public class ScoreGraphFeeder : MonoBehaviour {
    [SerializeField] private UIGraph graph;
    [Tooltip("Current score. Editing this at runtime plots a new point.")]
    [SerializeField] private float score;
    [SerializeField] private int maxPoints = 100;
    [SerializeField, Range(0f, 0.5f)] private float padding = 0.1f;
    
    [Header("Score")]
    [SerializeField] private bool allowNegativeScore = true;
    [SerializeField] private TMP_Text scoreLabel;
    [Tooltip("{0} is the score. For example: Score: {0:N0}")]
    [SerializeField] private string scoreLabelFormat = "{0:0.##}";

    [Header("Goal")]
    [SerializeField] private bool showGoal = true;
    [SerializeField] private float goal = 100f;

    [Header("X axis")]
    [Tooltip("Off: one evenly spaced point per score event. On: points are spaced by real time.")]
    [SerializeField] private bool useTimeAxis;

    private readonly List<float> scoreHistory = new List<float>();
    private readonly List<float> timeHistory = new List<float>();
    private float graphStartTime;

    private void Start() 
    {
        ResetGraph();
    }

    /// <summary>Adds a positive or negative amount to the score and plots the new total.</summary>
    public void AddScore(float scoreChange) {
        score += scoreChange;
        if (!allowNegativeScore) score = Mathf.Max(score, 0f);
        Record();
    }

    /// <summary>Moves the goal line to a real score value.</summary>
    public void SetGoal(float newGoal) {
        goal = newGoal;
        Refresh();
    }

    /// <summary>Resets the score to 0 and clears the graph, e.g. for a new round.</summary>
    public void ResetGraph() {
        score = 0f;
        graphStartTime = Time.time;
        scoreHistory.Clear();
        timeHistory.Clear();
        Record();
    }

    /// <summary>Lets inspector edits (score, goal, toggles) take effect while the game is running.</summary>
    private void OnValidate() {
        if (!Application.isPlaying || scoreHistory.Count == 0) return;
        
        if(!allowNegativeScore) score = Mathf.Max(score, 0f);

        if (!Mathf.Approximately(score, scoreHistory[scoreHistory.Count - 1])) Record();
        else Refresh();
    }

    /// <summary>Stores the current score as a new point, dropping the oldest once maxPoints is exceeded.</summary>
    private void Record() {
        scoreHistory.Add(score);
        timeHistory.Add(Time.time - graphStartTime);

        if (scoreHistory.Count > maxPoints) {
            scoreHistory.RemoveAt(0);
            timeHistory.RemoveAt(0);
        }

        Refresh();
    }

    /// <summary>Fits the Y range to the history (and goal, if shown) and sends everything to the graph.</summary>
    private void Refresh() {
        UpdateScoreLabel();
        
        if (graph == null || scoreHistory.Count == 0) return;

        float lowestValue = scoreHistory[0];
        float highestValue = scoreHistory[0];
        foreach (float total in scoreHistory) {
            if (total < lowestValue) lowestValue = total;
            if (total > highestValue) highestValue = total;
        }

        if (showGoal) {
            lowestValue = Mathf.Min(lowestValue, goal);
            highestValue = Mathf.Max(highestValue, goal);
        }

        float valueRange = Mathf.Max(highestValue - lowestValue, 1f);
        float minY = lowestValue - valueRange * padding;
        float maxY = highestValue + valueRange * padding;

        if (useTimeAxis) {
            var timePoints = new List<Vector2>(scoreHistory.Count);
            for (int pointIndex = 0; pointIndex < scoreHistory.Count; pointIndex++)
                timePoints.Add(new Vector2(timeHistory[pointIndex], scoreHistory[pointIndex]));
            graph.SetData(timePoints, timeHistory[0], timeHistory[timeHistory.Count - 1], minY, maxY);
        }
        else {
            graph.SetValues(scoreHistory, minY, maxY);
        }

        if (showGoal) graph.SetGoal(goal, minY, maxY);
        else graph.HideGoal();

        Debug.Log($"goal {goal}, range {minY} to {maxY}, normalised {(goal - minY) / (maxY - minY)}, showGoal {showGoal}");
    }
    
    /// <summary>Shows the current score in the label, if one is assigned.</summary>
    private void UpdateScoreLabel() {
        if (scoreLabel == null) return;
        scoreLabel.text = string.Format(scoreLabelFormat, score);
    }
}