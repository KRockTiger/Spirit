using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CirclemouseDown : MonoBehaviour
{
    public void OnMouseEnter()
    {
        ToggleOnObject(transform.parent, "Detail"); // ���콺�� UI ���� ���� �� ���� UI Ȱ��ȭ
        
    }

    public void OnMouseExit()
    {
        ToggleOffbject(transform.parent, "Detail"); // ���콺�� UI�� ��� �� ���� UI ��Ȱ��ȭ
    }

    void ToggleOnObject(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
            {
                child.gameObject.SetActive(true);
            }
        }
    }

    void ToggleOffbject(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
            {
                child.gameObject.SetActive(false);
            }
        }
    }
}
