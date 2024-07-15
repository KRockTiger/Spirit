using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField]
    private float cameraSpeed;

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        // horizontal �� vertical �Է� ���� �����ɴϴ�.
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // ���� ��ġ�� �����ɴϴ�.
        Vector3 position = transform.position;

        // �Է� ���� ���� ��ġ�� ������Ʈ�մϴ�.
        position.x += horizontal * cameraSpeed ;
        position.y += vertical * cameraSpeed ;

        // ���ο� ��ġ�� �̵��մϴ�.
        transform.position = position;
    }









}
