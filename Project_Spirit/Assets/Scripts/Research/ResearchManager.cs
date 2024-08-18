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
    private GameObject Research_prior;
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
    [SerializeField]
    private GameObject[] Flask_UI;
    [SerializeField]
    private GameObject currentClickedObj;
    [SerializeField]
    private bool inProgress;

    public GameObject gainWorkUI;
    public int currentWork;

    private GameObject StudyObject;
    private Study currentStudy;
    

    bool priorUI = false;
    int tabInt;
    int studyNum;
    private void Start()
    {
        inProgress = false;
        currentStudy = null;        
    }
    
    // ������ UI ����ִ� �Լ�.
    public void ShowResearchUI()
    {
        // ���� UI ��� Off.
        Research_prior.SetActive(false);

        if (inProgress)
            UpdateStudyProgress();
        Research_UI.SetActive(true);
    }
    public void ShowPriorUI()
    {
        Research_prior.SetActive(true);
        priorUI = true;

        // ���� �������� ��Ȳ
        if(inProgress)
        {
            Research_prior.transform.Find("Slider").gameObject.SetActive(true);
            Research_prior.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>($"pictogram 1/{currentStudy.StudyID}");
            Slider ProgressSlider = Research_prior.transform.GetChild(3).GetComponent<Slider>();

            ProgressSlider.gameObject.SetActive(true);
            ProgressSlider.value = (float)currentWork / currentStudy.WorkRequirement;
        }
        else
        {
            Research_prior.transform.Find("Slider").gameObject.SetActive(false);
            Research_prior.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>($"pictogram 1/button");
        }
    }

    private void Update()
    {
        if(priorUI)
        { if(Input.GetKeyDown(KeyCode.Escape))
            { Research_prior.SetActive(false);  priorUI = false; }
        }
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
        studyNum = StudyID;
        // �̹� �������̶��, �ٸ� ���� ���� �Ұ�, ��ư ��Ȱ��ȭ.
        if(inProgress)
        {
            StudyDetail.transform.GetChild(4).GetComponent<Button>().interactable = false;

            // ���� ���� �̹��� ���� ����

        }
        else
        {
            // ���� ���� �̹��� ���� ���� 
         
            // ���� ���� �ڿ� �Ҹ� ����
            StudyDetail.transform.GetChild(4).transform.Find("wood_icon/Text").GetComponent<TextMeshProUGUI>().text = _study.WoodRequirement.ToString();
            StudyDetail.transform.GetChild(4).transform.Find("rock_icon/Text").GetComponent<TextMeshProUGUI>().text = _study.StoneRequirement.ToString();
            StudyDetail.transform.GetChild(4).GetComponent<Button>().interactable = true;

            // �ڿ� ������ üũ
            if (resourceManager.GetComponent<ResouceManager>().Timber_reserves - _study.WoodRequirement < 0)
            {
                StudyDetail.transform.GetChild(4).GetComponent<Button>().interactable = false;
                
            }
            if (resourceManager.GetComponent<ResouceManager>().Rock_reserves - _study.StoneRequirement < 0)
            {
                StudyDetail.transform.GetChild(4).GetComponent<Button>().interactable = false;
               
            }
        }

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

        StudyObject = currentClickedObj;

        StudyDetail.SetActive(true);
        StudyProgress.SetActive(false);        
        //if (_study.WoodRequire < (���� ����) || _study.StoneRequire < (���� ��))
       
        // �������� ���� �ڿ� �Ҹ�
        if(resourceManager.GetComponent<ResouceManager>().Timber_reserves - _study.WoodRequirement < 0)
        {
            StudyDetail.transform.GetChild(4).GetComponent<Button>().interactable = false;
            return;
        }
        if(resourceManager.GetComponent<ResouceManager>().Rock_reserves - _study.StoneRequirement < 0)
        {
            StudyDetail.transform.GetChild(4).GetComponent<Button>().interactable = false;
            return ;
        }
        resourceManager.GetComponent<ResouceManager>().Timber_reserves -= _study.WoodRequirement;
        resourceManager.GetComponent<ResouceManager>().Rock_reserves -= _study.StoneRequirement;
                    
    }

    void UpdateStudyProgress()
    {                
        Slider ProgressSlider = StudyProgress.transform.GetChild(3).GetComponent<Slider>();
        
        ProgressSlider.gameObject.SetActive(true);
        ProgressSlider.value = (float)currentWork / currentStudy.WorkRequirement;                

        StudyDetail.SetActive(false);

        // �ش� â�� ���� ���� �ƴ� �ִϸ��̼��� ��µǾ����
        SetFlaskObj();
        // StudyProgress.SetActive(true);


    }
    
    public void OnClickResearchStartButton(Study _study)
    {
        inProgress = true;
        currentStudy = _study;
        currentWork = 0;                

        // ������ �ʿ��� �ڿ��� ������.

        UpdateStudyProgress();
    }

    public void CompleteStudy()
    {
        currentStudy.isComplete = true;

        // ���� �ִϸ��̼��� ������
        for(int i = 0; i < 5; i++)
        Flask_UI[i].SetActive(false);

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

    // ������ UI â���� ���� Ʈ���� ��ġ�ҋ�, ������ �ö�ũ ��� ������Ʈ �����ϴ� �Լ�.
    private void SetFlaskObj()
    {
        for(int j = 0; j < 5; j++)
            Flask_UI[j].SetActive(false);

        switch (tabInt)
        {
            case 0:
                RectTransform currentRect = StudyObject.GetComponent<RectTransform>();
                RectTransform rect1 = Flask_UI[tabInt].GetComponent<RectTransform>();
                Flask_UI[tabInt].SetActive(true);
                rect1.anchoredPosition = new Vector2(currentRect.anchoredPosition.x + 50, currentRect.anchoredPosition.y + 70);
                break;
            case 1:
                RectTransform currentRect2 = StudyObject.GetComponent<RectTransform>();
                RectTransform rect2 = Flask_UI[tabInt].GetComponent<RectTransform>();
                Flask_UI[tabInt].SetActive(true);
                rect2.anchoredPosition = new Vector2(currentRect2.anchoredPosition.x + 50, currentRect2.anchoredPosition.y + 70);
                break;
            case 2:
                RectTransform currentRect3 = StudyObject.GetComponent<RectTransform>();
                RectTransform rect3 = Flask_UI[tabInt].GetComponent<RectTransform>();
                Flask_UI[tabInt].SetActive(true);
                rect3.anchoredPosition = new Vector2(currentRect3.anchoredPosition.x + 50, currentRect3.anchoredPosition.y + 70);
                break;
            case 3:
                RectTransform currentRect4 = StudyObject.GetComponent<RectTransform>();
                RectTransform rect4 = Flask_UI[tabInt].GetComponent<RectTransform>();
                Flask_UI[tabInt].SetActive(true);
                rect4.anchoredPosition = new Vector2(currentRect4.anchoredPosition.x + 50, currentRect4.anchoredPosition.y + 70);
                break;
            case 4:
                RectTransform currentRect5 = StudyObject.GetComponent<RectTransform>();
                RectTransform rect5 = Flask_UI[tabInt].GetComponent<RectTransform>();
                Flask_UI[tabInt].SetActive(true);
                rect5.anchoredPosition = new Vector2(currentRect5.anchoredPosition.x + 50, currentRect5.anchoredPosition.y + 70);
                break;
        }
       
      
    }

    public void SetTab(int idx)
    {
        switch (idx)
        {
            case 0:
               tabInt = 0;
                break;
            case 1:
                tabInt = 1;
                break;
            case 2:
                tabInt = 2;
              
                break;
            case 3:
                tabInt = 3;
              
                break;
            case 4:
                tabInt = 4;
              
                break;
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