using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingDataManager : MonoBehaviour
{
    // ���๰�� �������� ���� �����Ѵ�.
    // ������ ���� ScriptableObject���� �����Ϸ��� ��ȹ ��.
    public static BuildingDataManager instance = null;

    [SerializeField]
    private GameObject BuildingParent;
    public List<Building> BuildingList = new List<Building>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
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
}
