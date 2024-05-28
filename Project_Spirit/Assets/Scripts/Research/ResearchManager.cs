using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResearchManager : MonoBehaviour
{
    [SerializeField]
    private GameObject Resource;
    [SerializeField]
    private GameObject[] Steps;

    // ��ũ��Ʈ ����.
    private bool inProgress;
    private Dictionary<string, bool> Research = new Dictionary<string, bool>();
    
    private void Start()
    {
        // For Debug.
        inProgress = false;

    }
    private void OnEnable()
    {
        SetInfo();
    }
    private void SetInfo()
    {
        // ������ ������ �� �� ������ ����.
        // 1. ��� UI�� ��� ����.
        // 2. ���� ��ũ��
        // 3. ���� �� ��ư enable;
        // + ������ ���� ������.


    }

    private void SetBlurryScreen()
    {                    
    }
}