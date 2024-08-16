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
    private GameObject StudyProgress;
    [SerializeField]
    private GameObject StudyComplete;
    [SerializeField]
    private GameObject[] Tree;
    [SerializeField]
    private GameObject[] Blurry;
    [SerializeField]
    private GameObject[] StepButton;
  
    public GameObject gainWorkUI;
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
            UpdateStudyProgress();
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

        Detail_StudyName.text = _study.StudyName;
        Detail_Explain.text = _study.StudyContent;
        Detail_StartButton.gameObject.SetActive(true);
        Detail_StartButton.onClick.RemoveAllListeners();
        Detail_StartButton.onClick.AddListener(() => OnClickResearchStartButton(_study));        

        StudyDetail.SetActive(true);
        StudyProgress.SetActive(false);        
        //if (_study.WoodRequire < (���� ����) || _study.StoneRequire < (���� ��))
        // StartBtn.interactable = false;               
    }

    void UpdateStudyProgress()
    {                
        Slider ProgressSlider = StudyProgress.transform.GetChild(3).GetComponent<Slider>();
        
        ProgressSlider.gameObject.SetActive(true);
        ProgressSlider.value = (float)currentWork / currentStudy.WorkRequirement;                

        StudyDetail.SetActive(false);
        StudyProgress.SetActive(true);
    }
    
    public void OnClickResearchStartButton(Study _study)
    {
        inProgress = true;
        currentStudy = _study;
        currentWork = 0;                

        UpdateStudyProgress();
    }

    public void CompleteStudy()
    {
        currentStudy.isComplete = true;

        ApplyStudyEffect();                
        ShowStudyComplete();

        currentClickedObj.GetComponent<Button>().interactable = false;        

        currentStudy = null;
        currentClickedObj = null;
        inProgress = false;        
    }

    public void ShowStudyComplete()
    {
        StudyComplete.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = currentStudy.StudyName;
        StudyComplete.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = currentStudy.StudyContent;

        StudyComplete.SetActive(true);
        StudyDetail.SetActive(false);
        StudyProgress.SetActive(false);
    }
    
    #region For Debug    
    // ������ �����ҿ��� ���� �� ȣ�� �� �Լ� ����.
    public void OnClickWork()
    {        
        currentWork += 100;
        UpdateStudyProgress();
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
    private GameObject[] StoneFactory_Button;
    [SerializeField]
    private GameObject[] WoodFactory_Button;
    [SerializeField]
    private GameObject[] StoneStorage_Button;
    [SerializeField]
    private GameObject[] WoodStorage_Button;
    [SerializeField]
    private GameObject KnightTraining;
    [SerializeField]
    private GameObject LeaderTraining;
    [SerializeField]
    private GameObject HealTraining;
    [SerializeField]
    private GameObject HealFoundation;
    [SerializeField]
    private GameObject WoodRoad;
    [SerializeField]
    private GameObject SpawnerUI;
    [SerializeField]
    private GameObject SpawnerSpiritUpgradeButton;
    [SerializeField]
    private GameObject SpawnerSpiritSpawnUpgradeButton;
    [SerializeField]
    private GameObject[] LowUp;
    [SerializeField]
    private GameObject[] highUp;


    [Header("ȿ�� ���� ��ũ��Ʈ")]
    [SerializeField]
    private SpiritSpawner[] spiritSpawner;    
    [SerializeField]
    private ResouceManager resourceManager;
    // �� ������ ȿ�� Ŭ����.
    public void ApplyStudyEffect()
    {
        switch(currentStudy.StudyID)
        {
            case 1001: // �ǹ� 1�ܰ� �ر�                
                Blurry[0].transform.GetChild(0).gameObject.SetActive(false);
                StepButton[0].transform.GetChild(1).GetComponent<Button>().interactable = true;
                break;
            case 1002: // �ǹ� 2�ܰ� �ر�
                Blurry[0].transform.GetChild(1).gameObject.SetActive(false);
                StepButton[0].transform.GetChild(2).GetComponent<Button>().interactable = true;
                break;
            case 1003: // �ǹ� 3�ܰ� �ر�
                Blurry[0].transform.GetChild(2).gameObject.SetActive(false);
                StepButton[0].transform.GetChild(3).GetComponent<Button>().interactable = true;
                break;
            case 1004: // �ǹ� 4�ܰ� �ر�
                Blurry[0].transform.GetChild(3).gameObject.SetActive(false);
                break;
            case 1011: // �� ����� 1 ���� ����.
                StoneFactory_Button[0].SetActive(true);
                Tree[0].transform.Find("Studies/Step2/StoneFactory2").GetComponent<Button>().interactable = true;
                break;
            case 1012: // ���� ����� 1 ���� ����.
                WoodFactory_Button[0].SetActive(true);
                Tree[0].transform.Find("Studies/Step2/WoodFactory2").GetComponent<Button>().interactable = true;
                break;
            case 1013: // ����� �� ����� ���� ����.
                StoneStorage_Button[0].SetActive(true);
                break;
            case 1014: // ����� ���� ����� ���� ����.
                WoodStorage_Button[0].SetActive(true);
                break;
            case 1021: // ��� �Ʒü�
                KnightTraining.SetActive(true);
                break;
            case 1022: // ��� �Ʒü�
                LeaderTraining.SetActive(true);
                break;
            case 1023: // �� ����� 2
                StoneFactory_Button[1].SetActive(true);
                break;
            case 1024: // ���� ����� 2
                WoodFactory_Button[1].SetActive(true);
                break;
            case 1025: // ���� ��
                WoodRoad.SetActive(true);
                break;

            case 1101: // ���� 1�ܰ� �ر�
                Blurry[1].transform.GetChild(0).gameObject.SetActive(false);
                StepButton[1].transform.GetChild(1).GetComponent<Button>().interactable = true;
                break;
            case 1102: // ���� 2�ܰ� �ر�
                Blurry[1].transform.GetChild(1).gameObject.SetActive(false);
                StepButton[1].transform.GetChild(2).GetComponent<Button>().interactable = true;
                break;
            case 1103: // ���� 3�ܰ� �ر�
                Blurry[1].transform.GetChild(2).gameObject.SetActive(false);
                StepButton[1].transform.GetChild(3).GetComponent<Button>().interactable = true;
                break;
            case 1104: // ���� 4�ܰ� �ر�
                Blurry[1].transform.GetChild(3).gameObject.SetActive(false);
                break;
            case 1111: // ���� �����ð� ���� 1 (�̱���)                
                for(int i = 0; i < 4; i++)
                {
                    spiritSpawner[i].spawnWeight = 1.2f;
                }
                SpawnerSpiritSpawnUpgradeButton.GetComponent<Button>().interactable = true;
                SpawnerSpiritSpawnUpgradeButton.GetComponent<Image>().sprite = SpawnerUI.GetComponent<SpawnerUI>().UpgradeSprite[1];
                break;
            case 1112: // ���� �̵� �ӵ� ��� 1
                SpiritManager.instance.spiritMoveSpeed = 1.03f;
                SpiritManager.instance.ChangeSpiritSpeed(1.03f);
                Tree[1].transform.Find("Studies/Step2/Speed2").GetComponent<Button>().interactable = true;
                break;
            case 1121: // ���� �ܰ� ���׷��̵� 1
                SpawnerSpiritUpgradeButton.GetComponent<Button>().interactable = true;
                break;
            case 1122: // ���� �̵� �ӵ� ��� 2
                SpiritManager.instance.spiritMoveSpeed = 1.06f;
                SpiritManager.instance.ChangeSpiritSpeed(1.06f);
                break;

            case 1201: // �ڿ� 1�ܰ� �ر�
                Blurry[2].transform.GetChild(0).gameObject.SetActive(false);
                StepButton[2].transform.GetChild(1).GetComponent<Button>().interactable = true;
                break;
            case 1202: // �ڿ� 2�ܰ� �ر�
                Blurry[2].transform.GetChild(1).gameObject.SetActive(false);
                StepButton[2].transform.GetChild(2).GetComponent<Button>().interactable = true;
                break;
            case 1203: // �ڿ� 3�ܰ� �ر�
                Blurry[2].transform.GetChild(2).gameObject.SetActive(false);
                StepButton[2].transform.GetChild(3).GetComponent<Button>().interactable = true;
                break;
            case 1204: // �ڿ� 4�ܰ� �ر�
                Blurry[2].transform.GetChild(3).gameObject.SetActive(false);
                break;
            case 1211: // �ڿ� �ڿ� ���귮 ���� 1
                resourceManager.IncreaseResourceWeight();
                Tree[2].transform.Find("Studies/Step2/IncreaseResource2").GetComponent<Button>().interactable = true;
                break;
            case 1212: // �ڿ� ä�� ȯ�� ���� 1
                SpiritManager.instance.resourceBuildingDamagePercent = 0.85f;
                Tree[2].transform.Find("Studies/Step2/ImproveResource2").GetComponent<Button>().interactable = true;
                break;
            case 1222: // �ڿ� �ڿ� ���귮 ���� 2
                resourceManager.IncreaseResourceWeight();                
                break;
            case 1223: // �ڿ� ä�� ȯ�� ���� 2
                SpiritManager.instance.resourceBuildingDamagePercent = 0.9f;
                break;
            case 2001:  // 2�ܰ� �������� ��� ����
                break;
            case 3001: // 3�ܰ� �������� ��� ����
                break;
            case 1301:  // ���ɿ� 1�ܰ� �ر�
                Blurry[3].transform.GetChild(0).gameObject.SetActive(false);
                StepButton[3].transform.GetChild(1).GetComponent<Button>().interactable = true;
                break;
            case 1302:  // ���ɿ� 2�ܰ� �ر�
                Blurry[3].transform.GetChild(1).gameObject.SetActive(false);
                StepButton[3].transform.GetChild(2).GetComponent<Button>().interactable = true;
                break;
            case 1303:  // ���ɿ� 3�ܰ� �ر�
                Blurry[3].transform.GetChild(2).gameObject.SetActive(false);
                StepButton[3].transform.GetChild(3).GetComponent<Button>().interactable = true;
                break;
            case 1304:  // ���ɿ� 4�ܰ� �ر�
                Blurry[3].transform.GetChild(3).gameObject.SetActive(false);
                break;
            case 1401:  // ���ɼ��� 1 �ر�
                Blurry[4].transform.GetChild(0).gameObject.SetActive(false);
                StepButton[4].transform.GetChild(1).GetComponent<Button>().interactable = true;
                break;
            case 1402:  // ���ɼ��� 2 �ر�
                Blurry[4].transform.GetChild(1).gameObject.SetActive(false);
                StepButton[4].transform.GetChild(2).GetComponent<Button>().interactable = true;
                break;
            case 1403:  // ���ɼ��� 3 �ر�
                Blurry[4].transform.GetChild(2).gameObject.SetActive(false);
                StepButton[4].transform.GetChild(3).GetComponent<Button>().interactable = true;
                break;
            case 1404:  // ���ɼ��� 4�ر�
                Blurry[4].transform.GetChild(3).gameObject.SetActive(false);
                break;
            case 1321:  // ���ɿ� ������ ���� 1
                for(int i = 0; i < 2; i++)
                { highUp[i].gameObject.GetComponent<Button>().interactable = false;}
                break;
            case 1322:  // ���ɿ� ���� ���� 1
                for (int i = 0; i < 2; i++)
                { LowUp[i].gameObject.GetComponent<Button>().interactable = false; }
                break;
            case 1331:  // ���ɿ� ������ ���� 2
                for (int i = 0; i < 2; i++)
                { highUp[i].gameObject.GetComponent<Button>().interactable = false; }
                break;
            case 1332:  // ���ɿ� ���� ���� 2
                for (int i = 0; i < 2; i++)
                { LowUp[i].gameObject.GetComponent<Button>().interactable = false; }
                break;
            case 1411:  // ���� ���� �ð� 1
                break;
            case 1421:  // ���� ���� �ð� 2
                break;
            case 1431:  // ���� ���� �ð� 3
                break;
            case 1031:  // �� �����- ����� 2��
                break;
            case 1032:  // ���� ����� ����� 2��
                break;
            case 1033:  // ���� �Ʒü� ���� 4��
                break;
            case 1034:  // ġ���� �Ʒü� �ر�
                HealTraining.SetActive(true);
                break;
            case 1131:  // ���� ���� �ð� ���� 2
                for (int i = 0; i < 4; i++)
                {
                    spiritSpawner[i].spawnWeight = 1.31f;
                }
                SpawnerSpiritSpawnUpgradeButton.GetComponent<Button>().interactable = true;
                SpawnerSpiritSpawnUpgradeButton.GetComponent<Image>().sprite = SpawnerUI.GetComponent<SpawnerUI>().UpgradeSprite[1];
                break;
            case 1132:  // ���� �̵� �ӵ� ��� 3
                SpiritManager.instance.spiritMoveSpeed = 1.14f;
                SpiritManager.instance.ChangeSpiritSpeed(1.14f);
                Tree[1].transform.Find("Studies/Step3/Speed3").GetComponent<Button>().interactable = true;
                break;
            case 1133:  // ���� �ܰ� ���׷��̵� 2
                SpawnerSpiritUpgradeButton.GetComponent<Button>().interactable = true;
                break;
            case 1231:  // �ڿ� �ڿ� ���귮 ���� 2
                resourceManager.IncreaseResourceWeightMax();
                break;
            case 1035:  // ������ �м�
                HealFoundation.SetActive(true);
                break;
        }
    }
}