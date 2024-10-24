using UnityEngine;
using System.Collections;

public class BonePiston : MonoBehaviour
{
    public float riseDistance = 2f; // How far the bone should rise
    public float riseDuration = 0.5f; // Time it takes to rise
    public float waitDuration = 1f; // Time to wait before disappearing
    private bool isTriggered = false; // To ensure the piston only activates once
    private Rigidbody2D rb; // Reference to Rigidbody2D

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true; // Ensure the bone starts as kinematic
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isTriggered && collision.CompareTag("Player")) // Check for player collision
        {
            isTriggered = true; // Prevent multiple triggers
            StartCoroutine(RiseAndStay());
        }
    }

    private IEnumerator RiseAndStay()
    {
        Vector3 originalPosition = transform.position; // Save original position
        Vector3 targetPosition = originalPosition + new Vector3(0, riseDistance, 0); // Calculate target position

        // Move up
        float elapsedTime = 0f;
        while (elapsedTime < riseDuration)
        {
            transform.position = Vector3.Lerp(originalPosition, targetPosition, elapsedTime / riseDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Snap to the target position
        transform.position = targetPosition;

        // Make the bone kinematic so it doesn't fall
        rb.isKinematic = true;

        // Optionally, wait for a moment before destroying the object
        yield return new WaitForSeconds(waitDuration);

        // Optionally destroy the bone object if desired after staying
        // Destroy(gameObject); // Uncomment this line if you want to remove the bone after a delay
    }
}
