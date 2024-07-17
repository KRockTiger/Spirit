using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterFallEvent : MonoBehaviour
{
    [SerializeField]
    private GameObject RainDropEventUI;
    [SerializeField]
    private GameObject NewsPaperEventUI;
   
    [SerializeField]
    bool newsPaperOpened = false;

    public void NewsPaperEventTrigger()
    {
        StartCoroutine(ShownewsText());
       
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

    private void FixedUpdate()
    {
        if (newsPaperOpened)
        {
            // �� ���� �����ð� �������� ���̱�
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                
                Debug.Log("Escape key pressed, hiding NewsPaperEventUI");

                Destroy(NewsPaperEventUI);
                Time.timeScale = 1f;
                newsPaperOpened =false;
                //SetActiveRecursively(NewsPaperEventUI, false);
                // �� �̺�Ʈ �߻�
                RainDropEventTrigger();
            }
        }
    }

        IEnumerator ShownewsText()
        {
            yield return new WaitForSeconds(0.7f);
            SetActiveRecursively(NewsPaperEventUI, true);

            yield return new WaitForSeconds(3f);
          
            newsPaperOpened = true;
        }

        private void SetActiveRecursively(GameObject obj, bool state)
        {
            if(obj != null)
            {
                obj.SetActive(state);
                foreach (Transform child in obj.transform)
                {
                    child.gameObject.SetActive(state);
                }

            }
        }
    } 

