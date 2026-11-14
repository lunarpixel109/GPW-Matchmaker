using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Startmenu : MonoBehaviour
{
    [SerializeField] SceneAsset level1;
    [SerializeField] SceneAsset levelTutorial;


    public void StartButton()
    {
        SceneManager.LoadScene(level1.name);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
