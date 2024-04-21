using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
public class CradleManager : MonoBehaviour
{
    [Header("������Ʈ")]
    [SerializeField]
    private GameObject cradleState;    
    [SerializeField]
    private GameObject cradleGrowthRate;    
    [SerializeField]
    private GameObject[] elementSlider;

    [Header("��������Ʈ")]
    [SerializeField]
    private Sprite[] cradleSprite;
    [SerializeField]
    private Sprite[] cradleGrowthRateSprite;

    private int cradleLevel = 0;        

    // ���� �� ���� ����.
    // ��, ��, ��, ����.
    Queue<Tuple<int, DateTime>>[] elementQueue = { 
        new Queue<Tuple<int, DateTime>>(), 
        new Queue<Tuple<int, DateTime>>() , 
        new Queue<Tuple<int, DateTime>>() , 
        new Queue<Tuple<int, DateTime>>() 
    };
    public float[] elementAvarage = { 0, 0, 0, 0 };
    public int[] elementSum = { 0, 0, 0, 0 };
    TimeSpan span = TimeSpan.FromSeconds(10); // �ΰ��� �� 1���� ���� �ð� ��.

    // For Debug.
    [SerializeField]
    private GameObject DebuggingBtn;
    [SerializeField]
    private TextMeshProUGUI[] DebuggingText;
    void Start()
    {
        // For Debug.
        DebuggingBtn.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => AddSpiritToCradle("Fire", 10));
        DebuggingBtn.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => AddSpiritToCradle("Water", 10));
        DebuggingBtn.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => AddSpiritToCradle("Ground", 10));
        DebuggingBtn.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(() => AddSpiritToCradle("Air", 10));
        DebuggingBtn.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => AddSpiritToCradle("Fire", 50));
        DebuggingBtn.transform.GetChild(5).GetComponent<Button>().onClick.AddListener(() => AddSpiritToCradle("Water", 20));
        DebuggingBtn.transform.GetChild(6).GetComponent<Button>().onClick.AddListener(() => AddSpiritToCradle("Ground", 20));
        DebuggingBtn.transform.GetChild(7).GetComponent<Button>().onClick.AddListener(() => AddSpiritToCradle("Air", 20));
    }

    // Update is called once per frame    
    void Update()
    {
        RemoveExpiredElement();
        CalculateElementAvarage();
        UpdateCradleUI();

        // For Debug.
        SetDebuggingText();
    }

    // 4���� ���� ������ ���� ����.
    // ���ɿ� ���� ������ ����.            

    // ���ɿ� ������ �Լ�.
    public void ToNextCradle()
    {
        cradleLevel++;
        if (cradleLevel >= 8)
        {
            // ���� Ŭ���� ����.
        }        
    }

    // ������ ����� �ε����� ��� ȣ��Ǵ� �Լ� ����.
    public void AddSpiritToCradle(string name, int val)
    {
        switch (name)
        {
            case "Fire":                
                elementQueue[0].Enqueue(new Tuple<int, DateTime>(val, DateTime.Now));
                Debug.Log(DateTime.Now);
                elementSum[0] += val;                
                break;
            case "Water":
                elementQueue[1].Enqueue(new Tuple<int, DateTime>(val, DateTime.Now));
                elementSum[1] += val;                
                break;
            case "Ground":
                elementQueue[2].Enqueue(new Tuple<int, DateTime>(val, DateTime.Now));
                elementSum[2] += val;                
                break;
            case "Air":
                elementQueue[3].Enqueue(new Tuple<int, DateTime>(val, DateTime.Now));
                elementSum[3] += val;                
                break;
        }
    }

    public void RemoveExpiredElement()
    {        
        for (int i = 0; i < 4; i++)
        {            
            while (elementQueue[i].Count != 0)
            {
                // Temp. ���� �ð��� ���� ������ ������ ���� �ð��� 10�� �̳��� ���� �� ��� Break;
                if (DateTime.Now - elementQueue[i].Peek().Item2 < span)
                    break;
                Debug.Log("�ı� " + DateTime.Now);
                elementSum[i] -= elementQueue[i].Peek().Item1;
                Debug.Log(i + "���� " + elementQueue[i].Peek().Item1);
                elementQueue[i].Dequeue();
            }            
        }
    }

    public void CalculateElementAvarage()
    {
        for (int i = 0; i < 4; i++)
        {
            if (elementQueue[i].Count == 0)            
                elementAvarage[i] = 0f;            
            else
                elementAvarage[i] = elementSum[i] / elementQueue[i].Count;
        }
    }
    // �ϴ� UI ���� �Լ�.
    // ���� ���, �� ���Һ� ���� �ӵ� ������, ���� ���� �ӵ� ��ũ, ���ɿ��� ���� ������, ���ɿ� �̹���
    // ������ �ö󰡴� �������� �ڷ�ƾ �̿��ؼ� ����.
    public void UpdateCradleUI()
    {
        if (cradleLevel < 8)
            cradleState.GetComponent<Image>().sprite = cradleSprite[cradleLevel];

        ShowSpiritSlider(); // ���� �� �����̴�.
        CalculateCradleGrowthRate(); // ��� ���� �ӵ� ���.
    }

    // ���Һ� ���� �ӵ� ������ ǥ��
    public void ShowSpiritSlider()
    {
        float totalElementAverage = GetTotalElementAverage();
        for (int i = 0; i < 4; i++)
        {
            RectTransform result = elementSlider[i].transform.GetChild(0).GetComponent<RectTransform>();
            float val = elementAvarage[i] - totalElementAverage;
            
            float x_pos = val;
            float width = val > 0 ? val * 2 : val * (-2);
            result.sizeDelta = new Vector2(width, result.rect.height);
            result.anchoredPosition = new Vector2(x_pos, 0);
        }
    }

    // ���ɿ��� ���� �ӵ� ��� �� ���� �Լ�.
    public void CalculateCradleGrowthRate()
    {
        float totalElementAverage = GetTotalElementAverage();
        int result = 0;

        for (int i = 0; i < 4; i++)
        {
            if (totalElementAverage * 0.9f <= elementAvarage[i] && totalElementAverage * 1.1f >= elementAvarage[i])
            {
                result += 1;
                elementSlider[i].transform.GetChild(0).GetComponent<Image>().color = Color.green;
            }
            else if (totalElementAverage * 0.65f <= elementAvarage[i] && totalElementAverage * 1.35f >= elementAvarage[i])
            {
                result += 2;
                elementSlider[i].transform.GetChild(0).GetComponent<Image>().color = Color.yellow;
            }
            else
            {
                result += 3;
                elementSlider[i].transform.GetChild(0).GetComponent<Image>().color = Color.red;
            }
        }

        if (result <= 4)
        {        
            cradleGrowthRate.GetComponent<Image>().sprite = cradleGrowthRateSprite[0];
        }
        else if (result <= 6)
        {            
            cradleGrowthRate.GetComponent<Image>().sprite = cradleGrowthRateSprite[1];
        }
        else if (result <= 8)
        {         
            cradleGrowthRate.GetComponent<Image>().sprite = cradleGrowthRateSprite[2];
        }
        else
        {            
            cradleGrowthRate.GetComponent<Image>().sprite = cradleGrowthRateSprite[3];
        }
    }

    float GetTotalElementAverage()
    {
        int sum = 0, count = 0;
        for (int i = 0; i < 4; i++)
        {
            sum += elementSum[i];
            count += elementQueue[i].Count;
        }
        if (count == 0)
            return 0;
        return sum / count;
    }
    // ���� ��ȭ, ����ȭ ���� �Լ�.

    // ���� ���� �Լ�.    

    // For Debug.
    void SetDebuggingText()
    {
        DebuggingText[0].text = "Total : " + GetTotalElementAverage().ToString();
        DebuggingText[1].text = "Fire : " + elementAvarage[0].ToString();
        DebuggingText[2].text = "Water : " + elementAvarage[1].ToString();
        DebuggingText[3].text = "Ground : " + elementAvarage[2].ToString();
        DebuggingText[4].text = "Air : " + elementAvarage[3].ToString();
    }
}