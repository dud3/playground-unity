using UnityEngine;
using UnityEngine.Animations.Rigging;

public class HandIKController : MonoBehaviour
{
	[SerializeField] private TwoBoneIKConstraint rightHandIK;
	[SerializeField] private Transform rightHandTarget; // RightHandTarget GameObject
    [SerializeField] private Transform worldTarget;     // What you want to grab/aim at

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    	rightHandTarget.position = worldTarget.position;
    	rightHandTarget.rotation = worldTarget.rotation;

    	// Blend weight in/out — same concept as before
        bool isActive = Input.GetMouseButton(1);
        float target = isActive ? 1f : 0f;
        rightHandIK.weight = Mathf.Lerp(rightHandIK.weight, target, Time.deltaTime * 8f);
    }
}
