using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Study : MonoBehaviour
{
    // CSV���� �о� �� ����.
    public int StudyID;
    public string StudyName;
    public bool KindofStudy;
    public int CategoryofStudy;
    public string StudyContent;
    public int PhaseofStudy;
    public int StoneRequirement;
    public int WoodRequirement;
    public int EssenceRequirement;
    public int WorkRequirement;
    public int StudyEffect;
    public int PriorResearch;

    // ���� �����ϸ鼭 ���ϴ� ���� ������
    public bool isComplete;

    private void Start()
    {
        isComplete = false;
    }
}
