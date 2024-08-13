using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaterFallEvent : MonoBehaviour
{
    [Header("��¥ �̺�Ʈ UI ����")]
    [SerializeField]
    private GameObject RainDropEventUI;
    [SerializeField]
    private GameObject RainDropimgUI;
    [SerializeField]
    private GameObject NewsPaperEventUI;
    [SerializeField]
    private GameObject NewsPaper;
    [SerializeField]
    private Sprite HotbookImage;

    [Header("���� �̺�Ʈ UI ����")]
   
    [SerializeField]
    private GameObject magicStatue;

    bool newsPaperOpened = false;
    bool HotnewsPaperOpened = false;
    public bool waterFallEvent = false;

    // ���� ���â
    public void NewsPaperEventTrigger()
    {
        StartCoroutine(ShownewsText());
       
    }

    // ���� ���â
    public void HotnewsEventTrigger()
    {
       NewsPaperEventUI.SetActive(true);
        NewsPaper.GetComponent<Image>().sprite = HotbookImage;
       
        StartCoroutine(ShowHotnewsText());
    }

    // ���� �̺�Ʈ ����
    public void RainDropEventTrigger()
    {
        RainDropEventUI.SetActive(true);
        waterFallEvent = true;

        foreach (Transform t in RainDropEventUI.transform)
        {
            if (t.gameObject.name == "RainDrop_img")
            {
                t.GetComponent<HotDay>().enabled = false;
            }
        }

        // �� ���� ����� �ι�� ����ϰ� �ϴ� �ż���

    }

    // ���� �̺�Ʈ ����
    public void RainDropEventEnd()
    {
        RainDropEventUI.SetActive(false);
        waterFallEvent = false;
        // �� ���� ����� ��¾� �����ϰ� ����ϰ� �ϴ� �ż���
    }    

    //���� vfX ���
    public void HotEventTrigger()
    {
        RainDropEventUI.SetActive(true);
       
        RainDropimgUI.GetComponent<RainDrop>().enabled = false;
        RainDropimgUI.GetComponent<HotDay>().enabled = true;
            
        
    }

    public void HotEventEventEnd()
    {
        RainDropEventUI.SetActive(false);
    }


    private void FixedUpdate()
    {
        if (newsPaperOpened)
        {
            // �� ���� �����ð� �������� ���̱�
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SoundManager.instance.UIButtonclick();
                Debug.Log("Escape key pressed, hiding NewsPaperEventUI");

                NewsPaperEventUI.SetActive(false);
                Time.timeScale = 1f;
                newsPaperOpened =false;
                // �� �̺�Ʈ �߻�
                RainDropEventTrigger();
            }
        }

        if (HotnewsPaperOpened)
        {
            // �� ���� �����ð� �������� ���̱�
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SoundManager.instance.UIButtonclick();
                Debug.Log("Escape key pressed, hiding NewsPaperEventUI");

                Destroy(NewsPaperEventUI);
                Time.timeScale = 1f;
                HotnewsPaperOpened = false;

                // ���� ��� �̺�Ʈ �߻�

                // ȸ�� â ����

                // ������ ���� ����
                magicStatue.SetActive(true);
            }
        }

    }

    // ���� �̺�Ʈ ���â
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

    IEnumerator ShowHotnewsText()
    {
        yield return new WaitForSeconds(0.7f);
        SetActiveRecursively(NewsPaperEventUI, true);

      

        yield return new WaitForSeconds(3f);

        HotnewsPaperOpened = true;
    }
} 

