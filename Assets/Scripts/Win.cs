using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Win : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private LevelData winData;

    [SerializeField] private SceneAsset main;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text.text = $"Winner\r\nScore: {winData.currentScore}";
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
