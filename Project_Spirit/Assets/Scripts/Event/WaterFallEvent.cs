using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterFallEvent : MonoBehaviour
{
    [SerializeField]
    private GameObject RainDropEventUI;
    [SerializeField]
    private GameObject NewsPaperEventUI;
   
    bool newsPaperOpened = false;

    public void NewsPaperEventTrigger()
    {
        StartCoroutine(ShownewsText());
        Time.timeScale = 0.01f;
    }

 
    public void RainDropEventTrigger()
    {
        RainDropEventUI.SetActive(true);
      
        // �� ���� ����� �ι�� ����ϰ� �ϴ� �ż���
    }

    public void RainDropEventEnd()
    {
        RainDropEventUI.SetActive(false);
      
        // �� ���� ����� ��¾� �����ϰ� ����ϰ� �ϴ� �ż���
    }    

    private void Update()
    {
        if (newsPaperOpened)
        {
            // �� ���� �����ð� �������� ���̱�
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                NewsPaperEventUI.SetActive(false);
                Time.timeScale = 1f;

                // �� �̺�Ʈ �߻�
                RainDropEventTrigger();
            }
        }
    }

        IEnumerator ShownewsText()
        {
            yield return new WaitForSecondsRealtime(0.7f);
            SetActiveRecursively(NewsPaperEventUI, true);

            yield return new WaitForSecondsRealtime(3f);
            newsPaperOpened = true;
        }

        private void SetActiveRecursively(GameObject obj, bool state)
        {
            obj.SetActive(state);
            foreach (Transform child in obj.transform)
            {
                child.gameObject.SetActive(state);
            }
        }
    } 

