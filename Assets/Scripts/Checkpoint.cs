using UnityEngine;
using UnityEngine.SceneManagement;

// Put this on a checkpoint flag (trigger collider).
// When the Player touches it, it becomes the new respawn point.
public class Checkpoint : MonoBehaviour
{
    public static Vector3 respawnPoint;
    public static bool hasCheckpoint;

    public Color activatedColor = new Color(0.6f, 1f, 1f);

    SpriteRenderer sr;

    // forget old checkpoints every time a scene loads (new game = fresh start)
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Init()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        hasCheckpoint = false;
    }

    static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        hasCheckpoint = false;
    }

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        respawnPoint = transform.position + Vector3.up * 0.5f;
        hasCheckpoint = true;
        if (sr != null) sr.color = activatedColor;
    }
}
