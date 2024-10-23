using UnityEngine;

public class SkullSpawner : MonoBehaviour
{
    public GameObject skullPrefab;       // Reference to the Skull prefab
    public Camera mainCamera;            // Reference to the Main Camera
    public float horizontalOffset = 10f; // Distance from center to spawn on left/right
    public float spawnDelay = 3f;        // Delay between spawns

    void Start()
    {
        SpawnSkullAtRandomPosition();  // Spawn the first skull
        InvokeRepeating("SpawnSkullAtRandomPosition", spawnDelay, spawnDelay); // Repeat spawning
    }

    void SpawnSkullAtRandomPosition()
    {
        // Get the vertical bounds of the camera
        float cameraHeight = 2f * mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        // Randomly choose whether to spawn on the far left or far right
        bool spawnOnLeft = Random.Range(0, 2) == 0;
        float horizontalPosition = spawnOnLeft ? mainCamera.transform.position.x - (cameraWidth / 2 + horizontalOffset)
                                               : mainCamera.transform.position.x + (cameraWidth / 2 + horizontalOffset);

        // Randomly choose a vertical position within the camera's view
        float verticalPosition = Random.Range(mainCamera.transform.position.y - cameraHeight / 2, mainCamera.transform.position.y + cameraHeight / 2);

        // Set the spawn position
        Vector3 spawnPosition = new Vector3(horizontalPosition, verticalPosition, 0);

        // Spawn the skull at the random position
        Instantiate(skullPrefab, spawnPosition, Quaternion.identity);
    }
}
