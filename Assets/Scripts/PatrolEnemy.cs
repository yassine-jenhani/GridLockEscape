using UnityEngine;

// Moves the enemy back and forth around its start position.
// Use together with Hazard.cs on the same object.
public class PatrolEnemy : MonoBehaviour
{
    public Vector2 direction = Vector2.right;   // (1,0) = horizontal, (0,1) = vertical
    public float distance = 3f;                 // how far it goes each way
    public float speed = 2f;                    // how fast

    Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float offset = Mathf.PingPong(Time.time * speed, distance * 2f) - distance;
        transform.position = startPos + (Vector3)(direction.normalized * offset);
    }
}
