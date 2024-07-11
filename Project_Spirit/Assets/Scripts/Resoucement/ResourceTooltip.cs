using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ResourceTooltip : MonoBehaviour
{
    // ���� UI
    Transform subUI;
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (subUI != null)
        {
            ToggleOnObject(transform, "Detail"); // ���콺�� UI ���� ���� �� ���� UI Ȱ��ȭ
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (subUI != null)
        {
            ToggleOffbject(transform, "Detail"); // ���콺�� UI�� ��� �� ���� UI ��Ȱ��ȭ
        }
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
