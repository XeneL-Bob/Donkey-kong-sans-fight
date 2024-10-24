using UnityEngine;
using System.Collections;
public class CutsceneManager : MonoBehaviour
{
    public GameObject textBoxPrefab; // Assign your text box prefab here
    public GameObject player; // Reference to the player object

    private void Start()
    {
        // You can initialize things here if needed
    }

    public void StartCutscene()
    {
        player.SetActive(false); // Disable player control
        GameObject textBox = Instantiate(textBoxPrefab, transform.position, Quaternion.identity);
        Animator textBoxAnimator = textBox.GetComponent<Animator>();
        textBoxAnimator.Play("OpenAnimation"); // Play your opening animation

        StartCoroutine(WaitForCutscene(textBox, 5f)); // Adjust the duration as needed
    }

    private IEnumerator WaitForCutscene(GameObject textBox, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Destroy(textBox); // Destroy the text box after the wait time
        player.SetActive(true); // Enable player control
    }
}
