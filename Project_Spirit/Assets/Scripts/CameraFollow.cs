using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public Transform target;
    public float smoothSpeed = 3;
    public Vector2 offset;
    public float limitMinX, limitMaxX, limitMinY, limitMaxY;

    public float zoomSpeed = 2.0f;
    public float minZoom = 3.0f; // �ִ� Ȯ�� �� ī�޶� ������
    public float maxZoom = 17.0f; // �ִ� ��� �� ī�޶� ������

    float cameraHalfWidth, cameraHalfHeight;
    private Camera camera;
    private void Start()
    {
        cameraHalfWidth = Camera.main.aspect * Camera.main.orthographicSize;
        cameraHalfHeight = Camera.main.orthographicSize;
        camera = GetComponent<Camera>();
    }

    private void Update()
    {
        HandleZoom();
    }
    private void LateUpdate()
    {
        Vector3 desiredPosition = new Vector3(
            Mathf.Clamp(target.position.x + offset.x, limitMinX + cameraHalfWidth, limitMaxX - cameraHalfWidth),   // X
            Mathf.Clamp(target.position.y + offset.y, limitMinY + cameraHalfHeight, limitMaxY - cameraHalfHeight), // Y
            -10);                                                                                                  // Z
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * smoothSpeed);
    }

    void HandleZoom()
    {
        float scrollData;
    
        scrollData = Input.GetAxis("Mouse ScrollWheel"); // ���콺 �� �Է� ����
   
        float newSize = camera.orthographicSize - scrollData * zoomSpeed;
        camera.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom); // ī�޶� ����� �ּ�/�ִ�� ����
    }
}
