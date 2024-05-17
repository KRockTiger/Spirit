using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// For Debug. 
// �׽�Ʈ �� Ŭ����. ���߿� �����ؾ� ��.
public class Task
{
    string name = null;    
    int Tap;
    int Step;
    int Index;    
    int StoneRequire = 10;
    int WoodRequire = 10;
    int EssenceRequire = 10;
    int WorkRequire = 10;

    bool isResearched = false;
    public Task(string _name, int _tap, int _step, int _index)
    {
        name = _name;
        Tap = _tap;
        Step = _step;
        Index = _index;
    }

    public bool GetIsResearched() {return isResearched;}
    public string GetName(){ return name;}
    public int GetTap() { return Tap; }
    public int GetStep() { return Step; }
    public int GetIndex() { return Index; }    
    public void SetIsResearched(bool _isResearched) { isResearched = _isResearched; }
}
public class ResearchManager : MonoBehaviour
{
    [Header("������Ʈ")]
    [SerializeField]
    private GameObject Resource;        
    [SerializeField]
    private GameObject Blurry;
    [SerializeField]
    private GameObject ResearchDetail;
    [SerializeField]
    private GameObject BuildingTree;    
    // ��ũ��Ʈ ����.
    private bool inProgress;


    // For Debug.    
    // Task[Tap, Step] = 
    private List<Task>[,] Tasks = new List<Task>[5, 4]{
        {new List<Task>(), new List<Task>(), new List<Task>(), new List<Task>() },
        {new List<Task>(), new List<Task>(), new List<Task>(), new List<Task>() },
        {new List<Task>(), new List<Task>(), new List<Task>(), new List<Task>() },
        {new List<Task>(), new List<Task>(), new List<Task>(), new List<Task>() },
        {new List<Task>(), new List<Task>(), new List<Task>(), new List<Task>() }};

    private Task currentTask;

    private void Start()
    {
        // For Debug.
        inProgress = false;
        Tasks[0,0].Add(new Task("11", 0, 0, 1));
        Tasks[0,1].Add(new Task("21", 0, 1, 1));
        Tasks[0,1].Add(new Task("22", 0, 1, 2));
        Tasks[0,2].Add(new Task("31", 0, 2, 1));
        Tasks[0,3].Add(new Task("41", 0, 3, 1));
        currentTask = null;
    }
    private void OnEnable()
    {
        SetInfo();
    }
    private void SetInfo()
    {
        // ������ ������ �� �� ������ ����.
        // 1. ��� UI�� ��� ����.
        // 2. ���� ��ũ��
        // 3. ���� �� ��ư enable;
        // + ������ ���� ������.

        // For Debug.
        inProgress = false;

    }

    private void SetBlurryScreen()
    {                    
    }

    private void SetResearchDetail(Task _task)
    {
        TextMeshProUGUI ResearchDetailName = ResearchDetail.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        Button StartBtn = ResearchDetail.transform.GetChild(4).GetComponent<Button>();
        
        ResearchDetailName.text = _task.GetName();        
        StartBtn.onClick.RemoveAllListeners();
        StartBtn.onClick.AddListener(() => OnClickResearchStartButton(_task));
    }

    public void OnClickTaskButton(string TaskID)
    {
        Task _task = GetTask(TaskID);
        if (_task == null || _task.GetIsResearched())
            return;
        
        SetResearchDetail(_task);
    }

    public void OnClickResearchStartButton(Task _task)
    {
        currentTask = _task;

        // �ϴ� ������ �ٷ� �Ϸ�ó�� �ǵ���.
        CompleteTask();        
    }

    public void CompleteTask()
    {
        currentTask.SetIsResearched(true);
        int tap = currentTask.GetTap();
        int step = currentTask.GetStep();
        int index = currentTask.GetIndex();        

        currentTask = null;
        Debug.Log("��");
    }

    Task GetTask(string TaskID)
    {
        switch (TaskID[0])
        {
            case '0':                
                return Tasks[0, TaskID[1] - '0' - 1][TaskID[2] - '0' - 1];
            default:
                return null;
        }        
    }
}