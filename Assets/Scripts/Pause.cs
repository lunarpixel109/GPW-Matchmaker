using UnityEngine;

public class Pause : MonoBehaviour
{
    private GameManager gameManager;
    private RectTransform rTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameManager.Instance;
        rTransform = GetComponent<RectTransform>();
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.pause)
        {
            rTransform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        }
        else
        {
            rTransform.localScale = new Vector3 (0, 0, 0);
        }
    }
}
