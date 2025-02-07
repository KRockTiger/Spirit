using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResouceCollider : MonoBehaviour
{
    GameObject uiObject;
    GameObject ParentObject;

    SpriteRenderer spriteRenderer;
    private void Start()
    {
        uiObject = GameObject.Find("[ResourceManager]");
        ParentObject = transform.parent.gameObject;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder =  103 - (int)transform.position.y;
    }

    private void Update()
    {
        // 마우스 위치를 매 프레임마다 갱신
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

    // UI 요소 위치 업데이트 메서드
    private void UpdateResourceBoxPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        uiObject.GetComponent<ResouceManager>().resourceShowbox.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y + 0.5f, mousePosition.z));
    }
}
