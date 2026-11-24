using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Fixed Camera Angle")]
    public float distance = 4f;
    public float height = 5f;
    public float lookHeight = 1f;
    public float followSpeed = 100f;

    private float fixedYaw;

    void Start()
    {
        fixedYaw = transform.eulerAngles.y;
    }

    void LateUpdate()
    {
        if (target == null)
            return;

        Quaternion fixedRotation = Quaternion.Euler(0f, fixedYaw, 0f);

        Vector3 offset = fixedRotation * new Vector3(
            0f,
            height,
            -distance
        );

        Vector3 desiredPosition = target.position + offset;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followSpeed * Time.deltaTime
        );

        // Sa kanya lang nakatingin
        transform.LookAt(
            target.position + Vector3.up * lookHeight
        );
    }
}