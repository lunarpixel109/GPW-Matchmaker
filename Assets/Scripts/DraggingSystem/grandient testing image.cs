using UnityEngine;
using UnityEngine.UI;

public class grandienttestingimage : MonoBehaviour
{
    void Start()
    {
        RandomGradient.Apply(this.gameObject.GetComponent<Image>(), RandomGradient.Direction.Horizontal);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
