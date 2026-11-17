using System;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	[SerializeField]
	static public GameManager Instance;

	[SerializeField]private LevelData previousLevel;
	[SerializeField]private LevelData currentLevel;
	[SerializeField]private LevelData nextLexel;

	[SerializeField] private LevelData winScene;
	[SerializeField] private LevelData loseScene;


	public int Score;
	public float timer;
	public bool pause;

	private void Awake()
	{
		// makes this a singleton
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			print("xcxcxcxcxcxcxcxcxcxcxcxcxcxcxcxcxcxcxcx");
			DestroyImmediate(this);
		}
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		if (previousLevel != null && currentLevel.carryPreviousLevelScoreOver == true)
		{
				Score = previousLevel.currentScore;
		}
		else
		{
			Score = currentLevel.StartingScore;
		}

		timer = currentLevel.levelTime;
	}

	// Update is called once per frame
	void Update()
	{
		if (Keyboard.current.pKey.wasPressedThisFrame)
		{
			pause = !pause;
			if (!pause) {
				Time.timeScale = 0;
			}
			else {
				Time.timeScale = 1;
			}
		}

		if (!pause)
		{
			if (Keyboard.current.spaceKey.wasPressedThisFrame)
			{
				Score++;
			}
			if (Keyboard.current.backspaceKey.wasPressedThisFrame)
			{
				Score--;
			}

			timer -= Time.deltaTime;

			if (timer <= 0f)
			{
				if (Score >= currentLevel.winScore)
				{
					//print("win");
					currentLevel.currentScore = Score;

					if (nextLexel != null)
					{
						SceneManager.LoadScene(nextLexel.scene.name);
					}
					else if (currentLevel.lastDay)
					{
						winScene.currentScore = Score;
						SceneManager.LoadScene(winScene.scene.name);
					}
				}
				else
				{
					//print("loser");
					SceneManager.LoadScene(loseScene.name);
				}
			}
		}

	}

	public LevelData GetCurrentLevelData() 
		{
			return currentLevel;
		}


}
