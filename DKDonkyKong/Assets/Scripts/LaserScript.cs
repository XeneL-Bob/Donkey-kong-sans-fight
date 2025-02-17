using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour
{
    public float warningDuration = 2f;  // Duration of the warning phase before the laser becomes active
    public float activeDuration = 2f;   // How long the laser remains active
    private bool isActive = false;      // Determines whether the laser can damage the player
    private Collider2D laserCollider;   // Collider for detecting collisions with the player

    void Start()
    {
        // Get the Collider2D component and disable it initially
        laserCollider = GetComponent<Collider2D>();
        laserCollider.enabled = false; // Disable the collider during the warning phase

        // Show the laser (but it can't hurt the player yet)
        StartCoroutine(ActivateLaserAfterDelay());
    }

    private IEnumerator ActivateLaserAfterDelay()
    {
        // Wait for the warning phase to finish
        yield return new WaitForSeconds(warningDuration);

        // Activate the laser (allow it to damage the player)
        isActive = true;
        laserCollider.enabled = true;

        // Keep the laser active for a certain duration
        yield return new WaitForSeconds(activeDuration);

        // Deactivate the laser and remove it after its active duration ends
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the laser hits the player and is active
        if (collision.CompareTag("Player") && isActive)
        {
            // Ensure that the player script handles death sound and animation
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.TriggerDeathSequence(); // Trigger the player's death sequence
            }
        }
    }
}
