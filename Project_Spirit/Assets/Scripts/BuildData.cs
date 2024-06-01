using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Build Item", menuName = "Build/Item", order = 1)]

public class BuildData : ScriptableObject
{
    public float structureID;
    public string structureName = "New Item";
    public int KindOfStructure = 0;
    public float stoneRequirement = 0;
    public float woodRequirement = 0;
    public float essenceRequirement = 0;
    public int UniqueProperties = 0;
    public int StructureEffect = 0;
    public float WorkingTime = 0;
    public int Capacity = 0;
    public float HcostOfUse = 0;

    //���� ���� �������ϴµ� ���� �̰��� �ҷ�����Ű���� �����̴�.
    public GameObject prefab;

    //���� �׸�Ÿ��
    public pathType pathtype;

    //�濡 ���� ����
    public string pathdescription;

    //���� Ÿ�� ���ϱ�
    public enum pathType
    {
        BabyRoom,
        OnTable,
        PlayGround

    }
}