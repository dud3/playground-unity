using UnityEngine;

public class CubeController : MonoBehaviour
{
    Animator anim;
    int speedHash;
    int jumpHash;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        speedHash = Animator.StringToHash("speed");
        jumpHash  = Animator.StringToHash("jump");
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleJump();
    }

    void HandleMovement()
    {
        /*
            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            // The 0 is the layer index — Layer 0 is the base layer

            bool isJumping = stateInfo.IsName("Jump");

            if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
            {
                anim.SetTrigger(jumpHash);
            }
        */
        
        // Get raw input (-1 to 1)
        float input = Input.GetAxis("Vertical");

        // Map to a speed value: 0, 3 (walk), 6 (run)
        float targetSpeed = 0f;

        if (Mathf.Abs(input) > 0.1f)
        {
            targetSpeed = Input.GetKey(KeyCode.LeftShift) ? 6f : 3f;
        }

        // Smoothly damp the current speed toward target
        float currentSpeed = anim.GetFloat(speedHash);
        float smoothSpeed  = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * 10f);

        anim.SetFloat(speedHash, smoothSpeed);
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            anim.SetTrigger(jumpHash);
        }
    }
}
