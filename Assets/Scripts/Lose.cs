using System;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Lose : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private LevelData loseData;

    [SerializeField] private SceneAsset main;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text.text = $"Loser!\r\nScore: {loseData.currentScore}";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void backToMain()
    {
        SceneManager.LoadScene(main.name);
    }
}
