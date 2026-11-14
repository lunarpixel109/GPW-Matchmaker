using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEditor;

[CreateAssetMenu(fileName = "Level Data", menuName = "Scriptable Objects/Player Generating/Level Data")]
public class LevelData : ScriptableObject
{
	public List<Property> properties;

	public GameObject PlayerPrefab;

    public bool lastDay;
    public bool winLose;

    public int StartingScore;
    public int winScore;
    public int currentScore;

    public float levelTime;
    public float playerGenerationCooldown;

    public SceneAsset scene;
}
