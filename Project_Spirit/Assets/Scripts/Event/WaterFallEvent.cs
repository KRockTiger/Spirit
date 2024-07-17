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
      
        // 물 정령 생산소 두배로 출력하게 하는 매서드
    }

    public void RainDropEventEnd()
    {
        RainDropEventUI.SetActive(false);
      
        // 물 정령 생산소 출력양 복구하게 출력하게 하는 매서드
    }    

    private void FixedUpdate()
    {
        if (newsPaperOpened)
        {
            // 물 정령 스폰시간 절반으로 줄이기
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                
                Debug.Log("Escape key pressed, hiding NewsPaperEventUI");

                Destroy(NewsPaperEventUI);
                Time.timeScale = 1f;
                newsPaperOpened =false;
                //SetActiveRecursively(NewsPaperEventUI, false);
                // 비 이벤트 발생
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

