using UnityEngine;

public class InverseKinematicController : MonoBehaviour
{
	private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
		bool isAiming = Input.GetMouseButton(1);

		// Toggle upper body aiming layer
		float targetWeight = isAiming ? 1f : 0f;
		float currentWeight = animator.GetLayerWeight(1);
		animator.SetLayerWeight(1, Mathf.Lerp(currentWeight, targetWeight, Time.deltaTime * 0.5f));

		// Drive locomotion on base layer as normal
		float speed = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).magnitude;
		animator.SetFloat("Speed", speed);
    }
}
