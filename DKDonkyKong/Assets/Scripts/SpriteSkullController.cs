using UnityEngine;
using System.Collections;

public class SpriteSkullController : MonoBehaviour
{
    public Transform player;                 // Player's Transform
    public GameObject laserPrefab;           // Laser Prefab
    public float totalSpinDuration = 2f;     // Duration of the total spin
    public float slowdownDuration = 1f;      // Duration of slow down after spin
    public float laserFireDelay = 0.5f;      // Delay before firing the laser

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
        // Start at a random off-screen position (left or right side)
        Vector3 offScreenStartPosition = GetRandomOffScreenPosition();
        skullTransform.position = offScreenStartPosition;

        // Choose a random target position on the sides (left or right)
        Vector3 targetPosition = GetRandomTargetPositionOnSides();

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

    private Vector3 GetRandomOffScreenPosition()
    {
        // Randomly choose an off-screen position on the left or right side of the screen
        float screenWidth = Camera.main.orthographicSize * Camera.main.aspect;
        float screenHeight = Camera.main.orthographicSize;

        bool spawnOnLeftSide = Random.value > 0.5f; // Randomly decide if it spawns on the left or right

        // Off-screen positions (left or right side, slightly off the screen bounds)
        if (spawnOnLeftSide)
        {
            return new Vector3(-screenWidth - 2f, Random.Range(-screenHeight, screenHeight), 0f); // Left off-screen
        }
        else
        {
            return new Vector3(screenWidth + 2f, Random.Range(-screenHeight, screenHeight), 0f); // Right off-screen
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
        // Calculate the spawn position based on the bottom (teeth) of the skull
        Vector3 laserStartPosition = skullTransform.position + skullTransform.up * -1f; // Adjusted to spawn from the "teeth"

        // Instantiate the laser prefab at the calculated position
        GameObject laser = Instantiate(laserPrefab, laserStartPosition, skullTransform.rotation); // Make the laser follow the skull's rotation

        Debug.Log("Laser fired toward player!");
    }
}
