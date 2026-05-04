using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    // Start is called before the first frame update
    public float moveSpeed = 5f;
    public float jumpHeight = 5f;
    private Rigidbody2D rb;
    private bool isGrounded = false;
    private bool isRespawning = false;
    void Start()
    {
        //set the rigidbody reference
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isRespawning) return;
        // horizontal movement
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // jump only when grounded
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
        }
    }

    // called when collider touches another collider
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }

    public void Respawn(Vector3 spawnPosition)
    {
        transform.position = spawnPosition;
        rb.velocity = Vector2.zero;
        StartCoroutine(RespawnCooldown());
    }

    private IEnumerator RespawnCooldown()
    {
        isRespawning = true;
        StartCoroutine(FlashPlayer());
        yield return new WaitForSeconds(0.5f);
        //coroutine in Unity,pause this thread of execution here
        isRespawning = false;
    }

    private IEnumerator FlashPlayer()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        float elapsed = 0f;
        while (elapsed < 0.5f)
        {
            sr.enabled = !sr.enabled; // toggle visibility
            yield return new WaitForSeconds(0.1f); // flash every 0.1 seconds
            elapsed += 0.1f;
        }
        sr.enabled = true; // make sure player is visible at the end
    }
}
