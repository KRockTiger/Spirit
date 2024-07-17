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
        // 기온 & 날짜 형식 동기화.
        MatchDateWithTemperature();
    }
    // 기온 XML 빌드 데이터 로드.
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
            // 같은 기온일 시에
            if(TempData.Nowtime == timeTostring)
            {
                Temperature_text.text = "/ 기온 " + TempData.Temperature.ToString() + "°";
            }
        }
    }

}


public class TemperatureData : ScriptableObject
{
    public string Nowtime;
    public float Temperature;

}