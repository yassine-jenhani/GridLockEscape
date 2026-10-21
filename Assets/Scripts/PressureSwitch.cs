using UnityEngine;

public class PressureSwitch : MonoBehaviour
{
    public GameObject doorToOpen; // Glissez la porte ici
    private bool isActivated = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Vérifie si le joueur touche l'interrupteur
        if (collision.CompareTag("Player") && !isActivated)
        {
            isActivated = true;
            // Ouvre la porte (ici, on la désactive ou on la déplace)
            if (doorToOpen != null)
            {
                doorToOpen.SetActive(false);
            }
            Debug.Log("Interrupteur activé : Porte ouverte !");
        }
    }
}