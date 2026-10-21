using UnityEngine;

// Put this on the Main Camera. The camera follows the player,
// so the level can be much wider than one screen.
public class CameraFollow : MonoBehaviour
{
    public Transform target;                         // drag the Player here
    public Vector3 offset = new Vector3(0f, 1.5f, -10f);
    public float smoothSpeed = 5f;

    [Header("Limits (so the camera doesn't show outside the level)")]
    public bool useLimits = true;
    public Vector2 minPosition = new Vector2(0f, 0f);
    public Vector2 maxPosition = new Vector2(60f, 10f);

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desired = target.position + offset;

        if (useLimits)
        {
            desired.x = Mathf.Clamp(desired.x, minPosition.x, maxPosition.x);
            desired.y = Mathf.Clamp(desired.y, minPosition.y, maxPosition.y);
        }

        transform.position = Vector3.Lerp(transform.position, desired, smoothSpeed * Time.deltaTime);
    }
}
