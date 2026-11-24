using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TiledScrollingBackground : MonoBehaviour
{
    [SerializeField] private Vector2 scrollSpeed = new Vector2(1f, 0f);

    private Vector3 startLocalPos;
    private Vector2 tileSize;

    void Start()
    {
        startLocalPos = transform.localPosition;
        tileSize = GetComponent<SpriteRenderer>().sprite.bounds.size;
    }

    void Update()
    {
        float x = Mathf.Repeat(Time.time * scrollSpeed.x, tileSize.x);
        float y = Mathf.Repeat(Time.time * scrollSpeed.y, tileSize.y);
        transform.localPosition = startLocalPos + new Vector3(x, y, 0f);
    }
}