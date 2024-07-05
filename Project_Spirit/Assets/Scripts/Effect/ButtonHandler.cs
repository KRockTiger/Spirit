using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 originalScale;
    private Vector3 originalPosition;
    public Vector3 hoverScale = new Vector3(1.2f, 1.2f, 1.2f); // Ŀ���� ���� ũ��
    public float hoverYOffset = 10f;

    private void Start()
    {
        originalScale = transform.localScale; // ���� ũ�⸦ �����մϴ�.
        originalPosition = transform.localPosition;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = hoverScale; // Ŀ���� ��ư ���� �÷����� �� ũ�⸦ �����մϴ�.
        transform.localPosition = new Vector3(originalPosition.x, originalPosition.y + hoverYOffset, originalPosition.z);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = originalScale; // Ŀ���� ��ư���� ����� �� ���� ũ��� �ǵ����ϴ�.
        transform.localPosition = originalPosition;
    }
}
