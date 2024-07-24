using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResouceCollider : MonoBehaviour
{
    GameObject uiObject;
    GameObject ParentObject;
    private void Start()
    {
        uiObject = GameObject.Find("[ResourceManager]");
        ParentObject = transform.parent.gameObject;
    }

    private void Update()
    {
        // ���콺 ��ġ�� �� �����Ӹ��� ����
        Vector3 mousePosition = Input.mousePosition;

    }
    private void OnMouseEnter()
    {
        Vector3 mousePosition;
        mousePosition = Input.mousePosition;
       // Debug.Log(uiObject.name);

        uiObject.GetComponent<ResouceManager>().resourceShowbox.SetActive(true);
        uiObject.GetComponent<ResouceManager>().resourceShowbox.transform.position = Input.mousePosition;
        
        uiObject.GetComponent<ResouceManager>().resourceShowbox.GetComponentInChildren<Text>().text = ParentObject.GetComponent<ResourceBuilding>().Resource_reserves.ToString();
    }
    private void OnMouseExit()
    {
        uiObject.GetComponent<ResouceManager>().resourceShowbox.SetActive(false);
    }

    // UI ��� ��ġ ������Ʈ �޼���
    private void UpdateResourceBoxPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        uiObject.GetComponent<ResouceManager>().resourceShowbox.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y + 0.5f, mousePosition.z));
    }
}
