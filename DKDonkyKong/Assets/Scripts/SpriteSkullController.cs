using UnityEngine;
using System.Collections;

public class SpriteSkullController : MonoBehaviour
{
    public Transform player;                 // Player's Transform
    public GameObject laserPrefab;           // Laser Prefab
    public float totalSpinDuration = 2f;     // Duration of the total spin
    public float slowdownDuration = 1f;      // Duration of slow down after spin
    public float laserFireDelay = 0.5f;      // Delay before firing the laser
    public float laserDuration = 2f;         // Duration for how long the laser stays visible

    private Transform skullTransform;        // Skull's Transform
    private bool isMovingToPosition = true;  // Indicates if the skull is moving to its position

    void Start()
    {
        // Find player if not assigned in the inspector
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

        skullTransform = this.transform; // Store the skull's transform

        // Start the coroutine to handle movement, spinning, and laser firing
        StartCoroutine(MoveToPositionAndSpin());
    }

    private IEnumerator MoveToPositionAndSpin()
    {
        // Start at a random off-screen position on the correct side (left or right) based on target position
        Vector3 targetPosition = GetRandomTargetPositionOnSides();
        Vector3 offScreenStartPosition = GetOffScreenStartPosition(targetPosition);

        skullTransform.position = offScreenStartPosition;

        // Move to the target position
        while (isMovingToPosition)
        {
            skullTransform.position = Vector3.MoveTowards(skullTransform.position, targetPosition, 3f * Time.deltaTime);

            if (Vector3.Distance(skullTransform.position, targetPosition) < 0.01f)
            {
                isMovingToPosition = false; // Stop when close enough to the target position
            }

            yield return null;
        }

        // Start spinning after reaching the position
        yield return StartCoroutine(SpinAndFire());
    }

    private Vector3 GetOffScreenStartPosition(Vector3 targetPosition)
    {
        // Randomly decide to spawn off-camera on the side relative to the target position
        float screenWidth = Camera.main.orthographicSize * Camera.main.aspect;
        float screenHeight = Camera.main.orthographicSize;

        // If the target is on the right, spawn off-screen to the right. If on the left, spawn off-screen to the left.
        if (targetPosition.x > 0) // Right side
        {
            return new Vector3(screenWidth + 2f, targetPosition.y, 0f); // Off-screen to the right
        }
        else // Left side
        {
            return new Vector3(-screenWidth - 2f, targetPosition.y, 0f); // Off-screen to the left
        }
    }

    private Vector3 GetRandomTargetPositionOnSides()
    {
        // Calculate screen boundaries based on the camera
        float screenWidth = Camera.main.orthographicSize * Camera.main.aspect;
        float screenHeight = Camera.main.orthographicSize;

        // Limit how far to the sides the skull can go (closer to center)
        float horizontalOffset = screenWidth * 0.5f; // Adjust this multiplier for how far toward the sides it should go (lower = closer to center)
        float verticalRange = screenHeight * 0.8f; // Keep the vertical movement within screen bounds

        // Randomly select either the left or right side of the screen for the skull to move toward
        bool moveToLeftSide = Random.value > 0.5f;

        // Return a position on the left or right side, closer to the center
        if (moveToLeftSide)
        {
            return new Vector3(-horizontalOffset, Random.Range(-verticalRange, verticalRange), 0f); // Left side, closer to center
        }
        else
        {
            return new Vector3(horizontalOffset, Random.Range(-verticalRange, verticalRange), 0f); // Right side, closer to center
        }
    }

    private IEnumerator SpinAndFire()
    {
        // Simulate the skull spinning
        float spinTime = 0f;
        while (spinTime < totalSpinDuration)
        {
            spinTime += Time.deltaTime;
            skullTransform.Rotate(0, 0, 360 * Time.deltaTime); // Full spin
            yield return null;
        }

        // Slow down the spin after the initial fast spin
        float slowdownTime = 0f;
        while (slowdownTime < slowdownDuration)
        {
            slowdownTime += Time.deltaTime;
            skullTransform.Rotate(0, 0, 90 * Time.deltaTime); // Slow spin
            yield return null;
        }

        // Face the player before firing the laser
        FacePlayer();

        // Fire the laser after the spin and delay
        yield return new WaitForSeconds(laserFireDelay);
        FireLaser();

        // Wait for the laser duration, then destroy the skull
        yield return new WaitForSeconds(laserDuration);
        DestroySkull();
    }

    private void FacePlayer()
    {
        // Calculate the direction from the skull to the player
        Vector3 directionToPlayer = player.position - skullTransform.position;
        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;

        // Rotate the skull to face the player (teeth face down toward player)
        skullTransform.rotation = Quaternion.Euler(0f, 0f, angle + 90f); // Adjusted for the teeth direction
    }

    private void FireLaser()
    {
        // Get the bounds of the skull to calculate the bottom position accurately
        float skullHeight = skullTransform.GetComponent<SpriteRenderer>().bounds.size.y;

        // Adjust the laser's start position to be further down, below the skull's bottom
        Vector3 laserStartPosition = skullTransform.position - skullTransform.up * (skullHeight * 2.9f);

        // Create the rotation for the laser with the extra 90-degree adjustment to align the short side
        Quaternion laserRotation = skullTransform.rotation * Quaternion.Euler(0f, 0f, 90f);

        // Instantiate the laser prefab at the bottom of the skull, with the adjusted position and rotation
        GameObject laser = Instantiate(laserPrefab, laserStartPosition, laserRotation);

        Debug.Log("Laser fired from the bottom of the skull, fully beneath the skull.");
    }

    private void DestroySkull()
    {
        // Destroy the skull GameObject to clean up after firing the laser
        Destroy(gameObject);
        Debug.Log("Skull destroyed to reduce clutter.");
    }
}
