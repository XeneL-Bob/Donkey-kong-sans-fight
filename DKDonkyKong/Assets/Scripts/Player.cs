using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite[] runSprites;
    public Sprite climbSprite;
    private int spriteIndex;

    private Rigidbody2D rb;
    private CapsuleCollider2D capsuleCollider;

    private readonly Collider2D[] overlaps = new Collider2D[4];
    private Vector2 direction;

    private bool grounded;
    private bool climbing;
    private bool isDead = false;  // Prevent multiple death triggers

    public float moveSpeed = 3f;
    public float jumpStrength = 4f;

    // Reference to the death prefab and sound
    public GameObject deathPrefab;
    public AudioClip deathSound;
    private AudioSource audioSource;

    // Delay before resetting level
    public float deathAnimationDuration = 1.5f; // Adjust the duration as needed

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        audioSource = GetComponent<AudioSource>();  // Make sure AudioSource is attached
    }

    private void OnEnable()
    {
        InvokeRepeating(nameof(AnimateSprite), 1f / 12f, 1f / 12f);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void Update()
    {
        if (isDead) return;  // If the player is dead, skip the rest
        CheckCollision();
        SetDirection();
    }

    private void CheckCollision()
    {
        grounded = false;
        climbing = false;

        float skinWidth = 0.1f;
        Vector2 size = capsuleCollider.bounds.size;
        size.y += skinWidth;
        size.x /= 2f;

        int amount = Physics2D.OverlapBoxNonAlloc(transform.position, size, 0f, overlaps);

        for (int i = 0; i < amount; i++)
        {
            GameObject hit = overlaps[i].gameObject;

            if (hit.layer == LayerMask.NameToLayer("Ground"))
            {
                grounded = hit.transform.position.y < (transform.position.y - 0.5f + skinWidth);
                Physics2D.IgnoreCollision(overlaps[i], capsuleCollider, !grounded);
            }
            else if (hit.layer == LayerMask.NameToLayer("Ladder"))
            {
                climbing = true;
            }
        }
    }

    private void SetDirection()
    {
        if (climbing)
        {
            direction.y = Input.GetAxis("Vertical") * moveSpeed;
        }
        else if (grounded && Input.GetButtonDown("Jump"))
        {
            direction = Vector2.up * jumpStrength;
        }
        else
        {
            direction += Physics2D.gravity * Time.deltaTime;
        }

        direction.x = Input.GetAxis("Horizontal") * moveSpeed;

        if (grounded)
        {
            direction.y = Mathf.Max(direction.y, -1f);
        }

        if (direction.x > 0f)
        {
            transform.eulerAngles = Vector3.zero;
        }
        else if (direction.x < 0f)
        {
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }
    }

    private void FixedUpdate()
    {
        if (isDead) return;  // Prevent movement after death
        rb.MovePosition(rb.position + direction * Time.fixedDeltaTime);
    }

    private void AnimateSprite()
    {
        if (climbing)
        {
            spriteRenderer.sprite = climbSprite;
        }
        else if (direction.x != 0f)
        {
            spriteIndex++;

            if (spriteIndex >= runSprites.Length)
            {
                spriteIndex = 0;
            }

            if (spriteIndex > 0 && spriteIndex <= runSprites.Length)
            {
                spriteRenderer.sprite = runSprites[spriteIndex];
            }
        }
    }

    // Called when the player dies from any obstacle
    public void TriggerDeathSequence()
    {
        if (isDead) return;  // Prevent multiple death triggers
        isDead = true;

        Debug.Log("Death sequence triggered");

        // Disable player movement and animation
        enabled = false;

        // Play the death sound
        if (audioSource != null && deathSound != null)
        {
            Debug.Log("Playing death sound");
            audioSource.volume = 1.0f; // Ensure full volume
            audioSource.PlayOneShot(deathSound, 1.0f); // Increase the volume if necessary
        }

        // Instantiate the death prefab at the player's position
        Instantiate(deathPrefab, transform.position, Quaternion.identity);

        // Optionally, you can hide or destroy the player GameObject
        spriteRenderer.enabled = false; // Hides the player sprite

        // Delay the level reset to allow the death animation and sound to play
        StartCoroutine(DelayBeforeReset());
    }

    private IEnumerator DelayBeforeReset()
    {
        // Wait for the duration of the death animation and sound to play
        yield return new WaitForSeconds(deathAnimationDuration);

        // Reset the level or call the GameManager to handle it
        GameManager.Instance.LevelFailed();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Objective"))
        {
            enabled = false;
            GameManager.Instance.LevelComplete();
        }
        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            TriggerDeathSequence();  // Call death sequence for any obstacle
        }
    }
}
