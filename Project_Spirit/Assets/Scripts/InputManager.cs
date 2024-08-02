using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    public GameObject tutorialObject;
    public GameObject guide;

    void Update()
    {
        // F11 Ű�� ������ ��
        if (Input.GetKeyDown(KeyCode.F1))
        {
            // targetObject�� null�� �ƴ��� Ȯ��
            if (tutorialObject != null)
            {
                // ������Ʈ�� Ȱ�� ���¸� ���
                tutorialObject.SetActive(!tutorialObject.activeSelf);
            }
            else
            {
               
            }
        }

        // F11 Ű�� ������ ��
        if (Input.GetKeyDown(KeyCode.F2))
        {
            // targetObject�� null�� �ƴ��� Ȯ��
            if (guide != null)
            {
                // ������Ʈ�� Ȱ�� ���¸� ���
                guide.SetActive(!guide.activeSelf);
            }
            else
            {
               
            }
        }
    }


}
