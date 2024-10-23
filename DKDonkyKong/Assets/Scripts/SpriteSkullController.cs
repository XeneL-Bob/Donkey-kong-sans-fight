using System.Collections;
using UnityEngine;

public class SpriteSkullController : MonoBehaviour
{
    public Transform player; // This will store the player's Transform
    public GameObject laserPrefab;
    public float totalSpinDuration = 2f;
    public float slowdownDuration = 1f;
    public float laserFireDelay = 0.5f;

    private bool laserFired = false;

    void Start()
    {
        // If the player is not assigned in the Inspector, find it by tag
        if (player == null)
        {
            GameObject playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
            else
            {
                Debug.LogError("Player not found! Make sure the player object is tagged as 'Player'.");
            }
        }

        StartCoroutine(SpinAndFire());
    }

    IEnumerator SpinAndFire()
    {
        // Rotate the skull 360 degrees twice
        float timeElapsed = 0f;
        float rotationSpeed = 720f / totalSpinDuration; // Two 360-degree spins

        while (timeElapsed < totalSpinDuration)
        {
            transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Slow down the rotation
        timeElapsed = 0f;
        while (timeElapsed < slowdownDuration)
        {
            float speed = Mathf.Lerp(rotationSpeed, 0, timeElapsed / slowdownDuration);
            transform.Rotate(Vector3.forward, speed * Time.deltaTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Lock the skull's bottom towards the player
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        // Fire the laser after the delay
        yield return new WaitForSeconds(laserFireDelay);
        FireLaser();
    }

    void FireLaser()
    {
        if (!laserFired && laserPrefab != null)
        {
            // Instantiate the laser and point it towards the player
            GameObject laserInstance = Instantiate(laserPrefab, transform.position, transform.rotation);

            Debug.Log("Laser fired at player!");

            laserFired = true;
        }
    }
}
