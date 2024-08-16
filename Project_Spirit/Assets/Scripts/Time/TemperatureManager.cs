using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using TMPro;

public class TemperatureManager : MonoBehaviour
{
    TimeManager timeManager;
    public List<TemperatureData> temperatureDatas;
    public string fileName;

    [SerializeField]
    private GameObject SpiritParent;
    [SerializeField]
    private float worldTemperature;
    [SerializeField]
    private TextMeshProUGUI Temperature_text;
    [SerializeField]
    private string timeTostring;

    private void Awake()
    {
        LoadTempData(fileName);
    }

    private void Start()
    {
        timeManager = GetComponent<TimeManager>();
    }

    private void Update()
    {
        // ��� & ��¥ ���� ����ȭ.
        MatchDateWithTemperature();

        // ������ ������ ü�� ���� ����
        // WeatherAndSpiritRealtion();
    }
    // ��� XML ���� ������ �ε�.
    private void LoadTempData(string _fileName)
    {
        TextAsset xmlAsset = Resources.Load<TextAsset>("XML/" + _fileName);
        if (xmlAsset == null)
        {
            Debug.LogError("XML file not found: " + _fileName);
            return;
        }
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xmlAsset.text);

        XmlNodeList xmlNodeList = xmlDoc.SelectNodes("//TemperatureData");

        foreach (XmlNode xmlNode in xmlNodeList)
        {
            TemperatureData TempData = ScriptableObject.CreateInstance<TemperatureData>();
            TempData.Nowtime = "0"+xmlNode.SelectSingleNode("NowTime").InnerText;
            TempData.Temperature = float.Parse(xmlNode.SelectSingleNode("Temperature").InnerText);
            temperatureDatas.Add(TempData);
        }
    }

    void MatchDateWithTemperature()
    {
        timeTostring = timeManager.GetCurrentDateTimeString();

        foreach(TemperatureData TempData in temperatureDatas)
        {
            // ���� ����� �ÿ�
            if(TempData.Nowtime == timeTostring)
            {
                Temperature_text.text = "/ ��� " + TempData.Temperature.ToString() + "��";
                worldTemperature = TempData.Temperature;
            }
        }
    }

    public void WeatherAndSpiritRealtion()
    {
        // ����� 25�� �����Ͻ� 1�д� ü�� ���� ����
        // 0.041667 �� ü���� �����
        if(worldTemperature <= 25)
        {
            TakeDamageByWeather(SpiritParent);
        }
        // ����� 26�� �̻��� �� ������ 1�д� ü�� ���� ����
        // 0.041667 * ((������ -25)/ 10)
        else
        {
            TakeDamageOverByWeather(SpiritParent);
        }
    }

    void TakeDamageByWeather(GameObject spiritParent)
    {
      foreach(Transform child in spiritParent.transform)
        {
            Spirit spirit = child.GetComponent<Spirit>();
            if(spirit != null)
            {
                spirit.TakeDamage25ByWeather();
            }
        }
    }
    void TakeDamageOverByWeather(GameObject spiritParent)
    {
        foreach (Transform child in spiritParent.transform)
        {
            Spirit spirit = child.GetComponent<Spirit>();
            if (spirit != null)
            {
                spirit.TakeDamage25OverByWeather(worldTemperature);
            }
        }
    }
}


public class TemperatureData : ScriptableObject
{
    public string Nowtime;
    public float Temperature;

}