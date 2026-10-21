using UnityEngine;
using UnityEngine.SceneManagement;

// Put this on anything dangerous (red enemies, the pit KillZone).
// When the Player touches it: go back to the last checkpoint,
// or restart the level if no checkpoint was reached yet.
public class Hazard : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) Kill(other);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player")) Kill(collision.collider);
    }

    void Kill(Collider2D player)
    {
        if (Checkpoint.hasCheckpoint)
        {
            player.transform.position = Checkpoint.respawnPoint;
            if (player.attachedRigidbody != null)
                player.attachedRigidbody.linearVelocity = Vector2.zero;
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
