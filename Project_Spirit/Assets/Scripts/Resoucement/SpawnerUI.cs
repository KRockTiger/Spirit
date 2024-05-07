using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SpawnerUI : MonoBehaviour
{
    [Header("���� ����� UI ����")]
    [SerializeField]
    Slider[] allSliders;
    [SerializeField]
    GameObject Spawner_Name;
    [SerializeField]
    GameObject Sprit1;
    [SerializeField]
    GameObject Sprit2;
    [SerializeField]
    GameObject Sprit3;
    [SerializeField]
    GameObject SpawnLv;
    [SerializeField]
    GameObject SpiritLv;
    [SerializeField]
    GameObject otherSlider;
    [SerializeField]
    GameObject[] spirit123;
    [SerializeField]
    Transform parentSliderPos;

   
    [HideInInspector]
    public Button[] buttons;
    [HideInInspector]
    public Slider slider;
    [HideInInspector]
    public InputField textcoom;
    [HideInInspector]
    public GameObject[] Spawner;
    [HideInInspector]
    public Sprite[] HandleSprite;
    [HideInInspector]
    public GameObject MainSpawner;

    private List<SpiritSpawnInfo> spawnInfoList = new List<SpiritSpawnInfo>();
    private List<SpiritSpawnInfo> notspawnInfoList = new List<SpiritSpawnInfo>();
    bool isSlider1Active = false;
    bool isSlider2Active = false;
    bool isSlider3Active = false;
    
    List<GameObject> spawnManage = new List<GameObject>(); // ���� ���� ��ü ����Ʈ
    Canvas canvas;

    private void Start()
    {
        slider.onValueChanged.AddListener(updateText);
        foreach (Slider slider in allSliders)
        {
            slider.onValueChanged.AddListener(OnSliderValueChanged);
        }
        foreach(Button button in buttons)
        {
            button.onClick.AddListener(() => OnButtonClick(button));
        }
        textcoom.onEndEdit.AddListener(SetSliderValueFromInput);
    }

    #region �����̴� ����
    void updateText(float val)
    {
        textcoom.text = slider.value.ToString("F1");
    }

    // �ٸ� �����̴����� ���� �����ϴ� �޼���
    void OnSliderValueChanged(float value)
    {   
        foreach (Slider s in allSliders)
        {
            s.value = value;
            MainSpawner.GetComponent<SpiritSpawner>().sliderValue = value;
        } 
    }
    
    // ������� �Է°��� ���� �����̴��� ���� �����ϴ� �޼���
    void SetSliderValueFromInput(string input)
    {
        // ������� �Է°��� float���� ��ȯ�Ͽ� �����̴��� ������ ����
        float inputValue;
        if (float.TryParse(input, out inputValue))
        {
            slider.value = inputValue;
            Debug.Log(inputValue);
            if(MainSpawner != null)
            {
                Debug.Log(MainSpawner);
                MainSpawner.GetComponent<SpiritSpawner>().sliderValue = inputValue;
                Debug.Log(MainSpawner.GetComponent<SpiritSpawner>().sliderValue);
            }
        }
    }

    #endregion

    #region UI ����
   
    public void ReceiveDefaultSpiritSpawnInfo(SpiritSpawnInfo spawnInfo)
    {
        UpdateSpawnInfoList(spawnInfo);
        
    }

    public void ReceiveSpiritSpawnInfo(SpiritSpawnInfo spawnInfo, GameObject gameObject)
    {
        // ������Ʈ ���� ����� ����Ʈ
        UpdateSpawnInfoList(spawnInfo);
        MainSpawner = gameObject;
        slider.value = MainSpawner.GetComponent<SpiritSpawner>().sliderValue;
        ApplyNowSpawnInfo(spawnInfo);
        ApplyOtherSpawnInfo(spawnInfo);
    }

    void UpdateSpawnInfoList(SpiritSpawnInfo newSpawnInfo)
    {
        // ���ο� ������ ������ spawnerName�� ���� ��Ҹ� ã���ϴ�.
        SpiritSpawnInfo existingSpawnInfo = spawnInfoList.Find(item => item.SpawnerName == newSpawnInfo.SpawnerName);
        if (existingSpawnInfo != null)
        { 
          
            existingSpawnInfo.UpdateFrom(newSpawnInfo);
            
            Debug.Log(existingSpawnInfo.SpawnerName);
        }
        
        else
        {
            // �������� �ʴ� ��� ���ο� ��Ҹ� �߰��մϴ�.
            spawnInfoList.Add(newSpawnInfo);
        }
    }



    // Ŭ���� ���� ����� ���� �Է�.
    void ApplyNowSpawnInfo(SpiritSpawnInfo spawnInfo)
    {
        Spawner_Name.GetComponent<Text>().text = spawnInfo.SpawnerName;
        SpawnLv.GetComponentInChildren<Text>().text = spawnInfo.SpwnLv.ToString() + "�ܰ�";
        SpiritLv.GetComponentInChildren<Text>().text = spawnInfo.SpiritLv.ToString() + "�ܰ�";

    }

    // Ÿ ���� ����� �����ð� ����
    void ApplyOtherSpawnInfo(SpiritSpawnInfo newspawnInfo)
    {
        // ȣ���Ҷ����� ���Ӱ� ����
        notspawnInfoList.Clear();

        for(int i = 0; i < 4; i++)
        {
            if (spawnInfoList[i].SpawnerName != newspawnInfo.SpawnerName)
            {
                notspawnInfoList.Add(spawnInfoList[i]);
            }            
        }
        for(int i = 0; i < 3; i++)
        {
            spirit123[i].GetComponent<Text>().text = notspawnInfoList[i].SpawnerName + "�� ����";

        }
        
    }


  
    #endregion

    
    void OnButtonClick(Button clickedButton)
    {
        Debug.Log(clickedButton.name + " ��ư�� Ŭ���Ǿ����ϴ�.");

        switch(clickedButton.name)
        {
            case "0":
                ClickSpawner1(0);
                break;
            case "1":
                ClickSpawner2(1);
                break;
            case "2":
                ClickSpawner3(2);
                break;
        }
    }

    private void ClickSpawner1(int num) 
    {
        isSlider1Active = !isSlider1Active;
        // ��ġ ��ų UI ���� ���� setactive ȣ��
       // otherSlider.SetActive(isSlider1Active);
        if(isSlider1Active)
        {
            int selected = Output(notspawnInfoList[num].SpawnerName);

            GameObject temperalObj = Instantiate(otherSlider, parentSliderPos);
            Slider[] tempsliders = temperalObj.GetComponentsInChildren<Slider>();
            foreach(Slider slider in tempsliders)
            {
                slider.value = Spawner[selected].GetComponent<SpiritSpawner>().sliderValue;

            }
            // ������ ������Ʈ�� ���� ������Ʈ���� ��� �����ɴϴ�.
            ChangeHandleImageInChildren(temperalObj.transform, selected);
        }

    }

    private void ClickSpawner2(int num)
    {
        isSlider2Active = !isSlider2Active;
        // ��ġ ��ų UI ���� ���� setactive ȣ��
        //otherSlider.SetActive(isSlider2Active);
        if (isSlider2Active)
        {
            int selected = Output(notspawnInfoList[num].SpawnerName);

            GameObject temperalObj = Instantiate(otherSlider, parentSliderPos);
            Slider[] tempsliders = temperalObj.GetComponentsInChildren<Slider>();
            foreach (Slider slider in tempsliders)
            {
                slider.value = Spawner[selected].GetComponent<SpiritSpawner>().sliderValue;

            }
            // ������ ������Ʈ�� ���� ������Ʈ���� ��� �����ɴϴ�.
            ChangeHandleImageInChildren(temperalObj.transform, selected);
        }

    }

    private void ClickSpawner3(int num)
    {
        isSlider3Active = !isSlider3Active;
        // ��ġ ��ų UI ���� ���� setactive ȣ��
       // otherSlider.SetActive(isSlider3Active);
        if (isSlider3Active)
        {

            int selected = Output(notspawnInfoList[num].SpawnerName);

            GameObject temperalObj = Instantiate(otherSlider, parentSliderPos);
            Slider[] tempsliders = temperalObj.GetComponentsInChildren<Slider>();
            foreach (Slider slider in tempsliders)
            {
                slider.value = Spawner[selected].GetComponent<SpiritSpawner>().sliderValue;
            }

            // ������ ������Ʈ�� ���� ������Ʈ���� ��� �����ɴϴ�.
            ChangeHandleImageInChildren(temperalObj.transform, selected);


        }
        // �Ⱥ��̰� ��۷� ������

    }
    void ChangeHandleImageInChildren(Transform parent, int selected)
    {
        foreach (Transform child in parent)
        {
            // ���� ������Ʈ�� �̸��� "Handle"�̸� �̹����� �����մϴ�.
            if (child.name == "Handle")
            {
                Image handleImage = child.GetComponent<Image>();
                if (handleImage != null)
                {
                    handleImage.sprite = HandleSprite[selected];
                }
            }

            // ���� ������Ʈ�� �� �ٸ� ���� ������Ʈ�� ������ �ִ��� ��������� Ž���մϴ�.
            ChangeHandleImageInChildren(child, selected);
        }
    }


    int Output(string name)
    {
        int num = 0;
        for(int i = 0; i < 4; i++)
        {
            if (Spawner[i].GetComponent<SpiritSpawner>().SpawnerName == name)
            {
                num = i;
                break;
            }
           
        }
        return num;
    }

   

    // UI ��ư�� ��ȣ�ۿ� case�� �ۼ� ����
    public void CloseTab()
    {
        this.gameObject.SetActive(false);
    }
}
