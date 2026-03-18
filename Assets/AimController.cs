using UnityEngine;

public class AimController : MonoBehaviour
{
	[SerializeField] private Rig aimRig;
	[SerializeField] private Transform aimTarget;
	[SerializeField] private Transform cameraTransform;

	private float currentWeight = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        bool isAiming = Input.GetMouseButton(1);

        // Smoothly blend rig weight in/out
        float targetWeight = isAiming ? 1f : 0f;
        currentWeight = Mathf.Lerp(currentWeight, targetWeight, Time.deltaTime * 8f);
        aimRig.weight = currentWeight;

         // Move aim target to where camera is looking
        if (isAiming)
        {
            aimTarget.position = cameraTransform.position + cameraTransform.forward * 10f;
        }
    }
}
