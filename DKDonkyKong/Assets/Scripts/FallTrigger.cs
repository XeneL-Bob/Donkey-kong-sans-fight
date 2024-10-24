using UnityEngine;

public class FallTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the collider belongs to the Player
        if (collision.CompareTag("Player"))
        {
            // Get the Player component and trigger the death sequence
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.TriggerDeathSequence();
            }
        }
    }
}
