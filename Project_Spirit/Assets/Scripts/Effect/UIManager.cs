using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("UI ����")]
    [SerializeField]
    private GameObject researchUI;
    [SerializeField]
    private GameObject CraftModeUI;
    [SerializeField]
    private GameObject SpiritSpawnerUI;
    [SerializeField]
    private GameObject CharacterUI;
    [SerializeField]
    private GameObject BuildingInfo;
    [SerializeField]
    private GameObject pauseUI;
    [SerializeField]
    private List<GameObject> UI;

    bool UI_init;

    // Update is called once per frame
    void Update()
    {
      if(researchUI.activeSelf || CraftModeUI.activeSelf || SpiritSpawnerUI.activeSelf)
        {
            CharacterUI.SetActive(false);
            foreach(Transform info in BuildingInfo.transform)
            {
                info.gameObject.SetActive(false);
            }
        }
            

      // �ʱ� �����϶� esc�� ������
      
      if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(pauseUI.activeSelf)
            {
                pauseUI.SetActive(false);
                return;
            }

            UI_init = true; // �⺻������ true�� �����ϰ� ���ǿ� ���� false�� ����

            for (int i = 0; i < UI.Count; i++)
            {
                if (i == 1 || i == 5 || i == 7 || i == 10)
                {
                    // i�� 1, 5, 7, 10, 11�� ��� ������Ʈ�� �����ִ��� Ȯ��
                    if (!UI[i].gameObject.activeSelf)
                    {
                        UI_init = false;
                        break;
                    }
                }
                else
                {
                    // ������ i�� ��� ������Ʈ�� �����ִ��� Ȯ��
                    if (UI[i].gameObject.activeSelf)
                    {
                        UI_init = false;
                        break;
                    }
                }
            }

            if (UI_init)
                pauseUI.SetActive(true);
            else
                Debug.Log("UI Ű�� ���ÿ�");
        }
    
    }


    public void ToMain()
    {
        // �Ͻ����� ȭ�� off
        pauseUI.SetActive(false);
        UI[23].SetActive(true);

    }

    public void ToDesktop()
    {
        // �Ͻ����� ȭ�� off
        pauseUI.SetActive(false);
        UI[24].SetActive(true);
    }

    public void RetainTopause()
    {
        // ����ϱ�
        pauseUI.SetActive(true);
        UI[23].SetActive(false);
        UI[24].SetActive(false);
    }
}
