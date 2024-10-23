using System.Collections;
using UnityEngine;

public class SkullSpawner : MonoBehaviour
{
    public GameObject skullPrefab;
    public Camera mainCamera;
    public float spawnDelay = 3f;
    public float horizontalOffset = 10f;

    void Start()
    {
        StartCoroutine(SpawnSkullRoutine());
    }

    IEnumerator SpawnSkullRoutine()
    {
        while (true)
        {
            SpawnSkull();
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    void SpawnSkull()
    {
        // Randomly choose to spawn on the far left or far right
        float xPos = Random.Range(0, 2) == 0 ? mainCamera.transform.position.x - horizontalOffset : mainCamera.transform.position.x + horizontalOffset;
        float yPos = Random.Range(mainCamera.transform.position.y - mainCamera.orthographicSize, mainCamera.transform.position.y + mainCamera.orthographicSize);

        Vector3 spawnPosition = new Vector3(xPos, yPos, 0);
        Instantiate(skullPrefab, spawnPosition, Quaternion.identity);

        Debug.Log("Skull spawned at: " + spawnPosition);
    }
}
