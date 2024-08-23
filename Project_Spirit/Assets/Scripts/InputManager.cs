using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class InputManager : MonoBehaviour
{
    [Header("�Է� Ű ����")]
    public GameObject tutorialObject;
    public GameObject guide;
    public GameObject pauseUI;
    public GameObject WinUI;
    public GameObject LoseUI;
    void Update()
    {
        // F11 Ű�� ������ �� => ���丮��
        if (Input.GetKeyDown(KeyCode.F1))
        {
          
            if (tutorialObject != null)
            {
             
                tutorialObject.SetActive(!tutorialObject.activeSelf);
            }
            else
            {
               
            }
        }

        // F11 Ű�� ������ �� => ���̵�
        if (Input.GetKeyDown(KeyCode.F2))
        {
            if (guide != null)
            {
                
                guide.SetActive(!guide.activeSelf);
            }
            else
            {
               
            }
        }

        // ����
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (pauseUI != null)
            {
                // ������Ʈ�� Ȱ�� ���¸� ���
                pauseUI.SetActive(!pauseUI.activeSelf);
            }
            else
            {

            }
        }




    }







    public void LoadMainScene()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
}
