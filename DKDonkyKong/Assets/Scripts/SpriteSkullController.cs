using System.Collections;
using UnityEngine;

public class SpriteSkullController : MonoBehaviour
{
    public GameObject laserPrefab;      // Prefab for the laser
    public Transform player;            // Player transform
    public float totalSpinDuration = 2; // Total time to spin
    public float slowdownDuration = 1;  // Slowdown duration at the end
    public float laserFireDelay = 0.5f; // Delay before firing the laser

    private bool isFiringLaser = false;

    private void Start()
    {
        // Ensure player is assigned
        if (player == null)
        {
            player = GameObject.FindWithTag("Player").transform;  // Find the player using its tag
        }

        StartCoroutine(SpinAndFireLaser());
    }

    IEnumerator SpinAndFireLaser()
    {
        // Spin for the total spin duration
        float spinTime = 0;
        while (spinTime < totalSpinDuration)
        {
            transform.Rotate(0, 0, 360 * Time.deltaTime);  // Spinning
            spinTime += Time.deltaTime;
            yield return null;
        }

        // Slow down the spin before stopping
        float slowdownTime = 0;
        while (slowdownTime < slowdownDuration)
        {
            transform.Rotate(0, 0, 360 * (1 - (slowdownTime / slowdownDuration)) * Time.deltaTime);  // Slowing down
            slowdownTime += Time.deltaTime;
            yield return null;
        }

        // Align the skull to face the player
        Vector3 direction = player.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));

        // Wait for the delay before firing the laser
        yield return new WaitForSeconds(laserFireDelay);

        // Fire the laser
        FireLaser();
    }

    private void FireLaser()
    {
        // Instantiate the laser at the skull's position
        GameObject laser = Instantiate(laserPrefab, transform.position, Quaternion.identity);

        // Align the laser to point at the player
        Vector3 direction = player.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        laser.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90)); // Rotating laser to align

        // Adjust the Z position to make sure it's aligned with the game objects
        laser.transform.position = new Vector3(laser.transform.position.x, laser.transform.position.y, 0);

        // Adjust scale if necessary
        laser.transform.localScale = new Vector3(1, 1, 1);  // Reset scale to make sure it's correct

        // You can add any additional logic here, such as adding force or a visual warning for the player
    }
}
