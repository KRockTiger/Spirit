using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class BuildingDataManager : MonoBehaviour
{
    // ���๰�� �������� ���� �����Ѵ�.
    // ������ ���� ScriptableObject���� �����Ϸ��� ��ȹ ��.
    public static BuildingDataManager instance = null;

    public List<Building> BuildingList = new List<Building>();    
    [SerializeField]
    private GameObject BuildingParent;    
    [SerializeField]
    private string StructTableName;
    [SerializeField]
    private string StructUniqueTableName;

    public GameObject buildinginfo_UI;
    public GameObject characterinfo_UI;
    public GameObject BuildingSlot;
    // XML ������ scriptableObject ����Ʈ
    public List<BuildData> buildDataList;
    public List<StructUniqueData> structUniqueDataList;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            LoadBuildData(StructTableName);
            LoadStructUniqueData(StructUniqueTableName);
        }

        else
            Destroy(this);
    }

    private void Start()
    {
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

        XmlNodeList xmlNodeList = xmlDoc.SelectNodes("//StructData");

        foreach (XmlNode xmlNode in xmlNodeList)
        {
            BuildData buildData = ScriptableObject.CreateInstance<BuildData>();
            buildData.structureID = float.Parse(xmlNode.SelectSingleNode("StructureID").InnerText);
            buildData.structureName = xmlNode.SelectSingleNode("StructureName").InnerText;
            buildData.KindOfStructure = int.Parse(xmlNode.SelectSingleNode("KindOfStructure").InnerText);
            buildData.SturctureIndex = int.Parse(xmlNode.SelectSingleNode("StructureIndex").InnerText);
            buildData.stoneRequirement = float.Parse(xmlNode.SelectSingleNode("StoneRequirement").InnerText);
            buildData.woodRequirement = float.Parse(xmlNode.SelectSingleNode("WoodRequirement").InnerText);
            buildData.essenceRequirement = float.Parse(xmlNode.SelectSingleNode("EssenceRequirement").InnerText);
            buildData.StructureDescription = xmlNode.SelectSingleNode("StructureDescription").InnerText;
            buildData.UniqueProperties = int.Parse(xmlNode.SelectSingleNode("UniqueProperties").InnerText);
            //buildData.StructureEffect = int.Parse(xmlNode.SelectSingleNode("StructureEffect").InnerText);
            buildData.baseState = xmlNode.SelectSingleNode("BasicStructure").InnerText;
            buildData.ConstructionAmount = int.Parse(xmlNode.SelectSingleNode("ConstructionAmount").InnerText);
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
            buildUniqueData.UniqueProperties = int.Parse(xmlNode.SelectSingleNode("UniquePropertiesID").InnerText);
            buildUniqueData.WorkingTime = float.Parse(xmlNode.SelectSingleNode("WorkingTime").InnerText);
            buildUniqueData.Capacity = int.Parse(xmlNode.SelectSingleNode("Capacity").InnerText);
            buildUniqueData.HCostOfUse = float.Parse(xmlNode.SelectSingleNode("HCostOfUse").InnerText);
            buildUniqueData.CostUseWood = float.Parse(xmlNode.SelectSingleNode("CostOfUseWood").InnerText);
            buildUniqueData.CostOfStone = float.Parse(xmlNode.SelectSingleNode("CostOfUseStone").InnerText);
            buildUniqueData.DemandingWork = int.Parse(xmlNode.SelectSingleNode("DemandingWork").InnerText);
            buildUniqueData.StructureCondition = int.Parse(xmlNode.SelectSingleNode("StructureCondition").InnerText);
          
            structUniqueDataList.Add(buildUniqueData);
        }
    }

    public void ChangeBuildingWorkTime(float time)
    {
        for (int i = 0; i < BuildingSlot.transform.childCount; i++)
        {
            BuildingSlot.transform.GetChild(i).GetComponent<Building>().WorkingTime *= time;
        }
    }
}
