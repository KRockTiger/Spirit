using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterFallEvent : MonoBehaviour
{
    [SerializeField]
    private GameObject RainDropEventUI;
    [SerializeField]
    private GameObject NewsPaperEventUI;
    private bool rainEventOccured;

    public void NewsPaperEventTrigger()
    {
        NewsPaperEventUI.SetActive(true);
    }

    public void RainDropEventTrigger()
    {
        RainDropEventUI.SetActive(true);
        rainEventOccured = true;
    }

    private void Update()
    {
        if(rainEventOccured)
        {
            // �� ���� �����ð� �������� ���̱�


        }
    }
}
