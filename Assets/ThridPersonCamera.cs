using System;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;          // the player

    public float distance = 5f;
    private float minDistance = 2f;
    private float maxDistance = 6f;

    public float height = 2f;

    public float rotationSpeed = 3f;
    public float zoomSpeed = 2f;
    public float smoothSpeed = 10f;

    private float currentYaw = 0f;    // horizontal orbit angle
    private float currentPitch = 15f; // vertical orbit angle
    public float minPitch = -10f;
    public float maxPitch = 60f;

    private float actualDistance;

    void Start()
    {
        actualDistance = distance;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Mouse input drives orbit
        currentYaw += Input.GetAxis("Mouse X") * rotationSpeed;
        currentPitch -= Input.GetAxis("Mouse Y") * rotationSpeed;
        currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);

        float prevDistance = distance;
        distance += Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
        distance = Mathf.Lerp(prevDistance, distance, Time.deltaTime * smoothSpeed);

        // Calculate desired camera position
        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0);
        Vector3 desiredPosition = target.position + rotation * new Vector3(0, height, -distance);

        RaycastHit hit;

        float targetDistance = distance;

        Vector3 targetPosition = target.position + target.transform.up * 2.5f;

        if (Physics.Raycast(targetPosition, transform.position - targetPosition, out hit, distance))
        {
            Debug.DrawRay(targetPosition, transform.position - targetPosition, Color.green);
            Debug.Log("Hit: " + hit.transform.gameObject.name);

            Debug.Log(hit.distance);
            Debug.Log(targetDistance);

            targetDistance = hit.distance * 0.9f;
            // actualDistance = Mathf.Lerp(actualDistance, targetDistance, Time.deltaTime * smoothSpeed);

            distance = targetDistance;
        } else
        {
            Debug.DrawRay(targetPosition, transform.position - targetPosition, Color.red);
        }

        // Smooth follow
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.LookAt(target.position + Vector3.up * height);
    }
}