using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public Transform target;
    public float distance = 10f;
    public float xSpeed = 120.0f;
    public float ySpeed = 120.0f;
    public float yMinLimit = 10f;
    public float yMaxLimit = 80f;
    public float distanceMin = 5f;
    public float distanceMax = 50f;

    private float x = 0.0f;
    private float y = 45.0f;

    void Start()
    {
        if (target)
        {
            Vector3 angles = transform.eulerAngles;
            x = angles.y;
            y = angles.x;
        }
        else
        {
            Debug.LogWarning("CameraOrbit: Не назначен target! Пожалуйста, сгенерируйте плейн или прикрепите объект в инспекторе.");
        }
    }

    void LateUpdate()
    {
        if (target && Input.GetMouseButton(1))
        {
            x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
            y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
            y = ClampAngle(y, yMinLimit, yMaxLimit);
        }

        distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5f, distanceMin, distanceMax);
        Quaternion rotation = Quaternion.Euler(y, x, 0);
        Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
        Vector3 position = rotation * negDistance + (target ? target.position : Vector3.zero);
        transform.rotation = rotation;
        transform.position = position;
    }

    static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}