using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

partial class ResearchManager : MonoBehaviour
{
    [Header("������Ʈ")]
    [SerializeField]
    private GameObject Research_UI;    
    [SerializeField]
    private GameObject StudyDetail;
    [SerializeField]
    private GameObject StudyComplete;    
    [SerializeField]
    private GameObject[] Tree;
    [SerializeField]
    private GameObject[] Blurry;
    [SerializeField]
    private GameObject[] StepButton;

    private GameObject currentClickedObj;
    private Study currentStudy;
    public int currentWork;
    private bool inProgress;

    private void Start()
    {
        inProgress = false;
        currentStudy = null;        
    }
    
    // ������ UI ����ִ� �Լ�.
    public void ShowResearchUI()
    {
        if (inProgress)
            UpdateStudyDetail();
        Research_UI.SetActive(true);
    }

    Study GetStudy(int StudyID)
    {
        if (!DatabaseManager.instance.Studies.ContainsKey(StudyID))
            return null;
        return DatabaseManager.instance.Studies[StudyID];
    }    

    public void OnClickStudyButton(int StudyID)
    {
        Study _study = GetStudy(StudyID);                
        if (_study.isComplete)
            return;
        
        currentClickedObj = EventSystem.current.currentSelectedGameObject;
        SetStudyDetail(_study);
        StudyDetail.SetActive(true);
    }

    void SetStudyDetail(Study _study)
    {        
        TextMeshProUGUI Detail_StudyName = StudyDetail.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI Detail_Explain = StudyDetail.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        Button Detail_StartButton = StudyDetail.transform.GetChild(4).GetComponent<Button>();
        Slider Detail_ProgressSlider = StudyDetail.transform.GetChild(5).GetComponent<Slider>();

        Detail_StudyName.text = _study.StudyName;
        Detail_Explain.text = _study.StudyContent;
        Detail_StartButton.gameObject.SetActive(true);
        Detail_StartButton.onClick.RemoveAllListeners();
        Detail_StartButton.onClick.AddListener(() => OnClickResearchStartButton(_study));
        Detail_ProgressSlider.gameObject.SetActive(false);

        //if (_study.WoodRequire < (���� ����) || _study.StoneRequire < (���� ��))
        // StartBtn.interactable = false;               
    }

    void UpdateStudyDetail()
    {
        TextMeshProUGUI Detail_StudyName = StudyDetail.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        Button Detail_StartButton = StudyDetail.transform.GetChild(4).GetComponent<Button>();
        Slider Detail_ProgressSlider = StudyDetail.transform.GetChild(5).GetComponent<Slider>();

        Detail_StudyName.text = currentStudy.StudyName;
        Detail_ProgressSlider.gameObject.SetActive(true);
        Detail_ProgressSlider.value = (float)currentWork / currentStudy.WorkRequirement;
        Detail_StartButton.gameObject.SetActive(false);

        StudyDetail.SetActive(true);
    }
    
    public void OnClickResearchStartButton(Study _study)
    {
        inProgress = true;
        currentStudy = _study;
        currentWork = 0;
        currentClickedObj.transform.Find("InProgressMark").gameObject.SetActive(true);        

        UpdateStudyDetail();
    }

    public void CompleteStudy()
    {
        currentStudy.isComplete = true;

        ApplyStudyEffect();        
        ActiveNextStepButton();
        ShowStudyComplete();

        currentClickedObj.GetComponent<Button>().interactable = false;
        currentClickedObj.transform.Find("InProgressMark").gameObject.SetActive(false);

        currentStudy = null;
        currentClickedObj = null;
        inProgress = false;        
    }

    public void ShowStudyComplete()
    {
        StudyComplete.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = currentStudy.StudyName;

        StudyComplete.SetActive(true);
        StudyDetail.SetActive(false);
    }
    
    #region For Debug
    // ������ �ܰ躰 ���� ����
    public void ActiveNextStepButton()
    {
        bool[,] check = new bool[5, 4] { { true, true, true, true },
        { true, true, true, true }, { true, true, true, true },
        { true, true, true, true }, { true, true, true, true }};

        foreach (Study study in DatabaseManager.instance.Studies.Values)
        {
            if (!study.isComplete)            
                check[study.CategoryofStudy, study.PhaseofStudy - 1] = false;
        }

        // �ش� �ܰ谡 ���� �� Ŭ���� �Ǿ� �ִ� ���. ���� �ܰ� ��ư Ȱ��ȭ.
        int tapCount = 3;
        for(int i = 0; i < tapCount; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (check[i, j] == true)
                    StepButton[i].transform.GetChild(j + 1).GetComponent<Button>().interactable = true;
            }
        }
    }
    // ������ �����ҿ��� ���� �� ȣ�� �� �Լ� ����.
    public void OnClickWork()
    {        
        currentWork += 2;
        UpdateStudyDetail();
        if (currentWork >= currentStudy.WorkRequirement)
        {
            currentWork = 0;            
            CompleteStudy();            
        }
    }
    #endregion
}

partial class ResearchManager
{
    [Header("ȿ�� ���� ������Ʈ")]
    [SerializeField]
    private GameObject RockCraft_Button;
    [SerializeField]
    private SpiritSpawner spiritSpawner;
    // �� ������ ȿ�� Ŭ����.
    public void ApplyStudyEffect()
    {
        switch(currentStudy.StudyID)
        {
            case 1001:
                Blurry[0].transform.GetChild(0).gameObject.SetActive(false);
                break;
            case 1002:
                Blurry[0].transform.GetChild(1).gameObject.SetActive(false);
                break;
            case 1003:
                Blurry[0].transform.GetChild(2).gameObject.SetActive(false);
                break;
            case 1004:
                Blurry[0].transform.GetChild(3).gameObject.SetActive(false);
                break;
            case 1011:
                // �� ���� ���� ����.
                RockCraft_Button.GetComponent<Button>().interactable = true;
                break;
            case 1101:
                Blurry[1].transform.GetChild(0).gameObject.SetActive(false);
                break;
            case 1102:
                Blurry[1].transform.GetChild(1).gameObject.SetActive(false);
                break;
            case 1103:
                Blurry[1].transform.GetChild(2).gameObject.SetActive(false);
                break;
            case 1104:
                Blurry[1].transform.GetChild(3).gameObject.SetActive(false);
                break;
            case 1201:
                Blurry[2].transform.GetChild(0).gameObject.SetActive(false);
                break;
            case 1202:
                Blurry[2].transform.GetChild(1).gameObject.SetActive(false);
                break;
            case 1203:
                Blurry[2].transform.GetChild(2).gameObject.SetActive(false);
                break;
            case 1204:
                Blurry[2].transform.GetChild(3).gameObject.SetActive(false);
                break;
        }
    }
}