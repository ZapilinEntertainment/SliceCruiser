using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class CameraController : MonoBehaviour
{
    [SerializeField] private Transform camTransform, orbiter, target;
    [SerializeField] private Vector3 cameraLocalPosition = new Vector3(0f, 5f, -15f);
    private float xrotationAcceleration = 0f, zoomAcceleration = 0f, zoomVal = 0f;
    private const float ROTATION_X_SPEED = 500f, ROTATION_X_ACCELERATION = 3f, ROTATION_Y_SPEED = -350f, MAX_FAR = 15f, ZOOM_ACCELERATION = 2f, ZOOM_SPEED = -100f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        camTransform.localPosition = Vector3.zero; zoomVal = 0f;
        orbiter.localPosition = cameraLocalPosition;
        transform.position = target.position;
        transform.rotation = Quaternion.LookRotation(target.forward, Vector3.up);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = target.position;

        float xmove = Input.GetAxis("Mouse X"), t = Time.deltaTime;
        if (xmove != 0f)
        {
            xrotationAcceleration += ROTATION_X_ACCELERATION * t;
            orbiter.RotateAround(transform.position, transform.up ,(xmove * (1f + xrotationAcceleration)) * ROTATION_X_SPEED * t);
        }
        else xrotationAcceleration = 0f;

        float ymove = Input.GetAxis("Mouse Y");
        if (ymove != 0f)
        {
            orbiter.RotateAround(transform.position, orbiter.right, ymove * ROTATION_Y_SPEED * t);
        }

        float zoom = Input.GetAxis("Mouse ScrollWheel");
        if (zoom != 0f)
        {
            zoomVal +=  ZOOM_SPEED * zoom * t * (1f + zoomAcceleration);
            zoomAcceleration += t * ZOOM_ACCELERATION;

            //zoomVal = Mathf.Clamp01(zoom); // работает некорректно
            if (zoomVal < 0f) zoomVal = 0f; else if (zoomVal > 1f) zoomVal = 1f;
            camTransform.localPosition = Vector3.back * MAX_FAR * zoomVal;
        }
        else zoomAcceleration = 0f;
    }
}
