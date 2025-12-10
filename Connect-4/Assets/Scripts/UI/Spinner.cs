using UnityEngine;

// Spinner:
// - Rotates the object around a chosen axis every frame
public class Spinner : MonoBehaviour
{
    [Header("Rotation")]
    [Tooltip("Rotation speed in degrees per second")]
    [SerializeField] private float rotationSpeed = 90f;

    [Tooltip("Axis to rotate around")]
    [SerializeField] private Vector3 rotationAxis = Vector3.up;

    private void Update()
    {
        // rotating every frame
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime, Space.Self);
    }
}
