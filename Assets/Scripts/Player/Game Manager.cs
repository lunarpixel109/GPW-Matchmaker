using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	[SerializeField]private LevelData previousLevel;
	[SerializeField]private LevelData currentLevel;
	[SerializeField] private LevelData nextLexel;

	public int Score;
	public float timer;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		if (previousLevel != null)
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
		if (Keyboard.current.spaceKey.wasPressedThisFrame)
		{
			Score++;
		}


		timer -= Time.deltaTime;

		if (timer <= 0f)
		{
			if (Score >= currentLevel.winScore)
			{
				print("win");
				currentLevel.currentScore = Score;

				if (nextLexel != null)
				{
					SceneManager.LoadScene(nextLexel.scene.name);
				}
			}
			else
			{
				print("loser");
			}
		}


	}

	public LevelData GetCurrentLevelData() 
		{
			return currentLevel;
		}


}
