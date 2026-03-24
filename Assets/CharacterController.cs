using System.Runtime.CompilerServices;
using NUnit.Framework;
using UnityEngine;

public class CharacterController_ : MonoBehaviour
{
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float jumpForce = 8f;
    public float gravity = -20f;
    public float punchRange = 1.5f;

    public Transform aimTarget;
    public Transform lookTarget;

    public AnimationClip punchClip;
    public Transform cameraTransform;

    private Animator animator;
    private CharacterController controller;
    private Vector3 velocity;
    private float jumpingProgress = 0f;
    private float jumpingProgressDelta = 0f;

    private string currentSurface = "";

    private bool isPunching = false;

    private bool inverseKinematicsHandInterpolate = false;
    private float inverseKinematicsHandInterpolateDelta = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();

        // animator.SetLayerWeight(1, 0f);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        bool isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f; // small negative to keep grounded

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0f; // flatten so camera pitch doesn't push us up/down
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 move = (camForward * v + camRight * h);

        // Debug.Log(move.magnitude);

        if (move.magnitude > 0.1f)
        {
            // Vector3 dir = cameraTransform.forward * v + cameraTransform.right * h;
            controller.Move(move * currentSpeed * Time.deltaTime);
            // transform.forward = move; // face direction of movement

            Quaternion targetRotation = Quaternion.LookRotation(camForward);

            if ((camForward * v).magnitude > 0.1f && (camForward * h).magnitude > 0.1f) {

                Vector3 rotate = move;

                if (v < 0.0f) {
                    rotate = -move;
                }

                targetRotation = Quaternion.LookRotation(rotate);
            }

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * 10f
            );
        }

        // Animator
        Vector3 localMove = transform.InverseTransformDirection(move.normalized);

        float velocityX = localMove.x * 0.5f;
        float velocityZ = localMove.z * (isRunning ? 1f : 0.5f);

        animator.SetFloat("VelocityX", velocityX, 0.1f, Time.deltaTime);
        animator.SetFloat("VelocityZ", velocityZ, 0.1f, Time.deltaTime);

        // Jump

        jumpingProgress += jumpingProgressDelta * Time.deltaTime;

        // Debug.Log(jumpingProgress);

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Jumping Up"))
        {
            float normalized = Mathf.Clamp01(stateInfo.normalizedTime);
            float elapsed    = normalized * stateInfo.length;

            // Debug.Log($"Jumping Up: {elapsed:F2}s / {stateInfo.length:F2}s ({normalized * 100f:F1}%)");
        }

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            jumpingProgressDelta = 1f;
            animator.SetBool("IsJumping", true);
        }

        if (isGrounded) {
            animator.SetBool("IsLanding", false);
        }

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

    public void OnJumpPeak()
    {
        velocity.y = jumpForce;
    }

    public void OnJumpEnd()
    {
        animator.SetBool("IsJumping", false);
        animator.SetBool("IsLanding", true);
    }

    public void OnLandEnd()
    {
        animator.SetBool("IsLanding", false);
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

    // IK

    void OnAnimatorIK(int layerIndex)
    {
        /*
        if (aimTarget != null)
        {
            if (Input.GetKeyDown(KeyCode.LeftControl)) {
                inverseKinematicsHandInterpolate = !inverseKinematicsHandInterpolate;
            }

            if (inverseKinematicsHandInterpolate) {
                inverseKinematicsHandInterpolateDelta = Mathf.Lerp(inverseKinematicsHandInterpolateDelta, 1.0f, Time.deltaTime / 4);
            } else {
                inverseKinematicsHandInterpolateDelta = Mathf.Lerp(inverseKinematicsHandInterpolateDelta, 0.0f, Time.deltaTime / 4);
            }

            Debug.Log(inverseKinematicsHandInterpolateDelta);

            // Right hand fully follows the target
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, inverseKinematicsHandInterpolateDelta);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0.2f);

            animator.SetIKPosition(AvatarIKGoal.RightHand, aimTarget.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, aimTarget.rotation);
        }

        if (lookTarget != null) {
            animator.SetLookAtWeight(
                weight:     1f,   // Overall influence
                bodyWeight: 0.1f, // How much the body turns
                headWeight: 0.8f, // How much the head turns
                eyesWeight: 0.5f, // How much the eyes move
                clampWeight: 0.6f // Limits how far the head can turn (0=no limit, 1=full clamp)
            );

            animator.SetLookAtPosition(lookTarget.position);
        } else {
            animator.SetLookAtWeight(0f);
        }
        */
    }
}
