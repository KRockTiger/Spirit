using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class BuildingDataManager : MonoBehaviour
{
    // ���๰�� �������� ���� �����Ѵ�.
    // ������ ���� ScriptableObject���� �����Ϸ��� ��ȹ ��.
    public static BuildingDataManager instance = null;

    [SerializeField]
    private GameObject BuildingParent;
    public List<Building> BuildingList = new List<Building>();
    [SerializeField]
    private string StructTableName;
    [SerializeField]
    private string StructUniqueTableName;

    // XML ������ scriptableObject ����Ʈ
    public List<BuildData> buildDataList;
    public List<StructUniqueData> structUniqueDataList;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        LoadBuildData(StructTableName);
        LoadStructUniqueData(StructUniqueTableName);
    }
    public void AddBuilding(Building building)
    {
        BuildingList.Add(building);
    }

    public List<Building> GetBuildingList()
    {
        return BuildingList;
    }

    public GameObject FindObjectContainsScript(Building building)
    {
        for (int i = 0; i < BuildingParent.transform.childCount; i++)
        {
            GameObject obj = BuildingParent.transform.GetChild(i).gameObject;
            if (obj.GetComponent<Building>() == building)
            {                
                return obj;
            }
        }
        return null;
    }    

    public void DestroyObjectByScript(Building building)
    {
        GameObject obj = FindObjectContainsScript(building);
        if (obj == null)
        {
            Debug.Log("�� ��ũ��Ʈ�� ������ ������Ʈ�� ã�� �� �����ϴ�.");
            return;
        }

        // ����Ʈ���� �������ְ� ������Ʈ �ı�.
        BuildingList.Remove(building);
        Destroy(obj);
    }

    // XML ���� ������ �ε�.
    private void LoadBuildData(string _fileName)
    {
        TextAsset xmlAsset = Resources.Load<TextAsset>("XML/"+_fileName);

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xmlAsset.text);

        XmlNodeList xmlNodeList = xmlDoc.SelectNodes("//text");

        foreach (XmlNode xmlNode in xmlNodeList)
        {
            BuildData buildData = ScriptableObject.CreateInstance<BuildData>();
            buildData.structureID = float.Parse(xmlNode.SelectSingleNode("StructureID").InnerText);
            buildData.structureName = xmlNode.SelectSingleNode("StructureName").InnerText;
            buildData.KindOfStructure = int.Parse(xmlNode.SelectSingleNode("KindOfStructure").InnerText);
            buildData.stoneRequirement = float.Parse(xmlNode.SelectSingleNode("StoneRequirement").InnerText);
            buildData.woodRequirement = float.Parse(xmlNode.SelectSingleNode("WoodRequirement").InnerText);
            buildData.essenceRequirement = float.Parse(xmlNode.SelectSingleNode("EssenceRequirement").InnerText);
            buildData.UniqueProperties = int.Parse(xmlNode.SelectSingleNode("UniqueProperties").InnerText);
            buildData.StructureEffect = int.Parse(xmlNode.SelectSingleNode("StructureEffect").InnerText);
            buildDataList.Add(buildData);
        }
    }

    // XML ���๰ ���� �Ӽ� ������ ���̺� �ε�
    private void LoadStructUniqueData(string _fileName)
    {
        TextAsset xmlAsset = Resources.Load<TextAsset>("XML/" + _fileName);

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xmlAsset.text);

        XmlNodeList xmlNodeList = xmlDoc.SelectNodes("//text");

        foreach (XmlNode xmlNode in xmlNodeList)
        {
            StructUniqueData buildUniqueData = ScriptableObject.CreateInstance<StructUniqueData>();
            buildUniqueData.UniqueProperties = int.Parse(xmlNode.SelectSingleNode("UniqueProperties").InnerText);
            buildUniqueData.WorkingTime = float.Parse(xmlNode.SelectSingleNode("WorkingTime").InnerText);
            buildUniqueData.Capacity = int.Parse(xmlNode.SelectSingleNode("Capacity").InnerText);
            buildUniqueData.HCostOfUse = float.Parse(xmlNode.SelectSingleNode("HCostofUse").InnerText);
            buildUniqueData.CostUseWood = float.Parse(xmlNode.SelectSingleNode("CostUseWood").InnerText);
            buildUniqueData.CostOfStone = float.Parse(xmlNode.SelectSingleNode("CostUseStone").InnerText);
            buildUniqueData.DemandingWork = int.Parse(xmlNode.SelectSingleNode("Demanding").InnerText);
            buildUniqueData.StructureCondition = int.Parse(xmlNode.SelectSingleNode("StructureCondition").InnerText);
          
            structUniqueDataList.Add(buildUniqueData);
        }
    }
}
