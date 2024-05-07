using System.Collections;
using System.Collections.Generic;
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

    [HideInInspector]
    public Slider slider;
    [HideInInspector]
    public InputField textcoom;
    public GameObject controllingSpawn;
    private List<SpiritSpawnInfo> spawnInfoList = new List<SpiritSpawnInfo>();
    List<GameObject> spawnManage = new List<GameObject>();
    private void Start()
    {
        AddListOfSpawner();
        slider = GetComponentInChildren<Slider>();
        slider.onValueChanged.AddListener(updateText);
        foreach (Slider slider in allSliders)
        {
            slider.onValueChanged.AddListener(OnSliderValueChanged);
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
            if(controllingSpawn != null)
               controllingSpawn.GetComponent<SpiritSpawner>().slider.value = inputValue;
        }
    }

    #endregion

    #region UI ����
    void AddListOfSpawner()
    {
        spawnManage.Add(Sprit1);
        spawnManage.Add(Sprit2);
        spawnManage.Add(Sprit3);
    }
    public void ReceiveSpiritSpawnInfo(SpiritSpawnInfo spawnInfo)
    {
        spawnInfoList.RemoveAll(item => item.SpawnerName == spawnInfo.SpawnerName);
        spawnInfoList.Add(spawnInfo);
        ApplyNowSpawnInfo(spawnInfo);
        ApplyOtherSpawnInfo(spawnInfo);
        
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
        List<SpiritSpawnInfo> list = new List<SpiritSpawnInfo>();

        foreach (var item in spawnInfoList)
        {
            Debug.Log(spawnInfoList.Count);
            if (item.SpawnerName != newspawnInfo.SpawnerName)
            {
                list.Add(item);
            }

        }

       for(int i = 0; i < spawnManage.Count; i++)
       {
            spawnManage[i].GetComponent<Text>().text = list[i].SpawnerName + "�� ����";
           
       }
    }


  
    void OnClick(SpiritSpawnInfo spawnInfo)
    {
        // ��ġ ��ų UI ���� ���� setactive ȣ��
        otherSlider.SetActive(true);
        otherSlider.GetComponentInChildren<Slider>().value = spawnInfo.slider.value;
    }
    #endregion

    // UI ��ư�� ��ȣ�ۿ� case�� �ۼ� ����
    public void CloseTab()
    {
        this.gameObject.SetActive(false);
    }
}
