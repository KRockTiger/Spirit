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
            TempData.Nowtime = xmlNode.SelectSingleNode("NowTime").InnerText;
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
            }
        }
    }

}


public class TemperatureData : ScriptableObject
{
    public string Nowtime;
    public float Temperature;

}