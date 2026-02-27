using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using UnityEngine;

public class CharacterController_ : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float jumpForce = 5f;

    public float gravity = -20f;
    public float punchRange = 1.5f;
    public AnimationClip punchClip;

    private Animator animator;
    private CharacterController controller;
    private Vector3 velocity;

    private string currentSurface = "";

    private bool isPunching = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        bool isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f; // small negative to keep grounded

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0, v);

        if (move.magnitude > 0.1f)
        {
            controller.Move(move * moveSpeed * Time.deltaTime);
            transform.forward = move; // face direction of movement
        }

        // Animator
        animator.SetFloat("Speed", move.magnitude);

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = jumpForce;
            animator.SetBool("IsJumping", true);
        }

        if (!isGrounded)
        {
            animator.SetBool("IsJumping", false);
        }

        if (isGrounded && !Input.GetKeyDown(KeyCode.Space))
            animator.SetBool("IsJumping", false);

        // Apply gravity manually
        velocity.y += gravity * Time.deltaTime;        
        controller.Move(velocity * Time.deltaTime);

        // Debug.Log(velocity);

        if (Input.GetKeyDown(KeyCode.F) && !isPunching)
        {
            StartCoroutine(Punch());
        }

        DetectSurface();
    }

    System.Collections.IEnumerator Punch()
    {
        isPunching = true;
        animator.SetBool("IsPunching", true);

        float clipLength = punchClip != null ? punchClip.length : 1f;
        Debug.Log(punchClip.name);
        Debug.Log("Punch clip length: " + clipLength);

        yield return null;
    }

    // Called automatically by the Animation Event at impact frame
    void OnPunchImpact()
    {
        if (!isPunching) return;

        Collider[] hits = Physics.OverlapSphere(
            transform.position + transform.forward * punchRange + Vector3.up,
            punchRange
        );

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                Debug.Log("Hit the enemy!");
                hit.GetComponent<EnemyController>().TakeHit();
            }
        }
    }

    void OnPunchEnd()
    {
        animator.SetBool("IsPunching", false);
        isPunching = false;
    }

    // Get's called by the engine
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            transform.position + transform.forward * punchRange + Vector3.up,
            punchRange
        );
    }

    void DetectSurface()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out hit, 1.2f))
        {
            currentSurface = hit.collider.gameObject.name;
            // Debug.Log("Current surface: " + currentSurface);
        }
    }
}
