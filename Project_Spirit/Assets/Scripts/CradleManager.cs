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
    private GameObject cradle;    
    [SerializeField]
    private GameObject cradleGrowthRate;    
    [SerializeField]
    private GameObject[] elementSlider;
    [SerializeField]
    private GameObject cradleSlider;
    
    [Header("��������Ʈ")]
    [SerializeField]
    private Sprite[] cradleSprite;
    [SerializeField]
    private Sprite[] cradleGrowthRateSprite;
    
    // ���� ���� ����.
    Queue<Tuple<int, DateTime>>[] elementQueue = { 
        new Queue<Tuple<int, DateTime>>(), 
        new Queue<Tuple<int, DateTime>>(), 
        new Queue<Tuple<int, DateTime>>(), 
        new Queue<Tuple<int, DateTime>>() 
    };
    public float[] elementAverage = { 0, 0, 0, 0 };
    public int[] elementSum = { 0, 0, 0, 0 };
    TimeSpan span = TimeSpan.FromSeconds(10);

    // ��� ���� ����.
    private int cradleLevel = 0;
    private int cradleGrowth = 0;
    private int cradleGrowthState = 0;
    private int[] cradleGrowthValue = { 50, 25, 10, -40 };
    private float cradleGrowthTime = 0f;
    private float cradleGrowthCooldown = 10f;

    // For Debug.
    [SerializeField]
    private GameObject DebuggingBtn;
    [SerializeField]
    private TextMeshProUGUI[] DebuggingText;
    void Start()
    {
        // For Debug.
        DebuggingBtn.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => AddElement("Fire", 10));
        DebuggingBtn.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => AddElement("Water", 10));
        DebuggingBtn.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => AddElement("Ground", 10));
        DebuggingBtn.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(() => AddElement("Air", 10));
        DebuggingBtn.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => AddElement("Fire", 30));
        DebuggingBtn.transform.GetChild(5).GetComponent<Button>().onClick.AddListener(() => AddElement("Water", 20));
        DebuggingBtn.transform.GetChild(6).GetComponent<Button>().onClick.AddListener(() => AddElement("Ground", 15));
        DebuggingBtn.transform.GetChild(7).GetComponent<Button>().onClick.AddListener(() => AddElement("Air", 40));
        //
    }

    // Update is called once per frame    
    void Update()
    {
        RemoveExpiredElement();
        CalculateElementAverage();
        AddCradleGrowth();

        UpdateCradleUI();        
    
        // For Debug.
        SetDebuggingText();
        //
    }

    // 4���� ���� ������ ���� ����.
    // ���ɿ� ���� ������ ����.            
    void AddCradleGrowth()
    {
        cradleGrowthTime += Time.deltaTime;
        if (cradleGrowthTime > cradleGrowthCooldown)
        {
            cradleGrowth += cradleGrowthValue[cradleGrowthState];

            if (cradleGrowth > 100)
                ToNextCradle();
            else if (cradleGrowth < 100)
            {
                // ���� ����.
            }            
            
            SetCradleGrowthSlider();
            cradleGrowthTime = 0f;
        }
    }

    void SetCradleGrowthSlider()
    {
        if (cradleGrowth < 0)
        {
            cradleSlider.GetComponent<Slider>().value = cradleGrowth * (-1);
            cradleSlider.transform.Find("Fill Area/Fill").GetComponent<Image>().color = Color.red;
        }
        else
        {
            cradleSlider.GetComponent<Slider>().value = cradleGrowth;
            cradleSlider.transform.Find("Fill Area/Fill").GetComponent<Image>().color = Color.green;
        }
    }

    // ���ɿ� ������ �Լ�.
    public void ToNextCradle()
    {
        cradleLevel++;
        cradleGrowth = 0;
        if (cradleLevel < 8)
            cradle.GetComponent<Image>().sprite = cradleSprite[cradleLevel];
        else
        {
            // ���� Ŭ���� ����.
        }
    }

    // ������ ����� �ε����� ��� ȣ��Ǵ� �Լ� ����.
    public void AddElement(string name, int val)
    {
        switch (name)
        {
            case "Fire":                
                elementQueue[0].Enqueue(new Tuple<int, DateTime>(val, DateTime.Now));                
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
                
                elementSum[i] -= elementQueue[i].Peek().Item1;                
                elementQueue[i].Dequeue();
            }            
        }
    }

    public void CalculateElementAverage()
    {
        for (int i = 0; i < 4; i++)
        {
            if (elementQueue[i].Count == 0)            
                elementAverage[i] = 0f;            
            else
                elementAverage[i] = elementSum[i] / elementQueue[i].Count;
        }
    }
    // �ϴ� UI ���� �Լ�.
    // ���� ���, �� ���Һ� ���� �ӵ� ������, ���� ���� �ӵ� ��ũ, ���ɿ��� ���� ������, ���ɿ� �̹���
    // ������ �ö󰡴� �������� �ڷ�ƾ �̿��ؼ� ����.
    public void UpdateCradleUI()
    {
        // ��� ���� ���� ����.
        SetCradleGrowthState(GetCradleGrowthRate());
        
        // ���� �⿩ �����̴� ����.
        SetElementSliderColor();
        SetElementSliderSize();        
    }

    // ���Һ� ���� �ӵ� ������ ǥ��
    public void SetElementSliderSize()
    {
        float totalElementAverage = GetTotalElementAverage();
        float offset = 2f; // ��ü �����̴��� ���̿� ���� �ٲ�� ��. ��ü �����̴� ���� / 2 / 100 ���� ��.
        for (int i = 0; i < 4; i++)
        {
            RectTransform result = elementSlider[i].transform.GetChild(0).GetComponent<RectTransform>();

            float ratio; // ���������� ������ ����.
            float x_pos;
            float width;
            if (elementAverage[i] - totalElementAverage > 0)
            {
                ratio = Mathf.Log((elementAverage[i] - totalElementAverage) / totalElementAverage * 100) * 10;
                x_pos = ratio;                
            }
            else if (elementAverage[i] - totalElementAverage < 0)
            {
                ratio = Mathf.Log((totalElementAverage - elementAverage[i]) / totalElementAverage * 100) * 10;
                x_pos = -ratio;
            }
            else                            
                x_pos = 0;           
            width = x_pos > 0 ? x_pos * offset : x_pos * offset * (-1);

            result.sizeDelta = new Vector2(width, result.rect.height);
            result.anchoredPosition = new Vector2(x_pos, 0);
        }
    }

    // ���ɿ��� ���� �ӵ� ��� �� ���� �Լ�.
    public int GetCradleGrowthRate()
    {
        float totalElementAverage = GetTotalElementAverage();
        int result = 0;
        for (int i = 0; i < 4; i++)
        {
            if (totalElementAverage * 0.9f <= elementAverage[i] && totalElementAverage * 1.1f >= elementAverage[i])            
                result += 1;                            
            else if (totalElementAverage * 0.65f <= elementAverage[i] && totalElementAverage * 1.35f >= elementAverage[i])            
                result += 2;                
            else            
                result += 3;                
        }        
        return result;
    }

    void SetElementSliderColor()
    {
        float totalElementAverage = GetTotalElementAverage();        
        for (int i = 0; i < 4; i++)
        {
            if (totalElementAverage * 0.9f <= elementAverage[i] && totalElementAverage * 1.1f >= elementAverage[i])            
                elementSlider[i].transform.GetChild(0).GetComponent<Image>().color = Color.green;            
            else if (totalElementAverage * 0.65f <= elementAverage[i] && totalElementAverage * 1.35f >= elementAverage[i])            
                elementSlider[i].transform.GetChild(0).GetComponent<Image>().color = Color.yellow;            
            else            
                elementSlider[i].transform.GetChild(0).GetComponent<Image>().color = Color.red;            
        }
    }
    void SetCradleGrowthState(int val)
    {
        if (val <= 4)
        {
            cradleGrowthState = 0;
            cradleGrowthRate.GetComponent<Image>().sprite = cradleGrowthRateSprite[0];
        }
        else if (val <= 6)
        {
            cradleGrowthState = 1;
            cradleGrowthRate.GetComponent<Image>().sprite = cradleGrowthRateSprite[1];
        }
        else if (val <= 8)
        {
            cradleGrowthState = 2;
            cradleGrowthRate.GetComponent<Image>().sprite = cradleGrowthRateSprite[2];
        }
        else
        {
            cradleGrowthState = 3;
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
        DebuggingText[1].text = "Fire : " + elementAverage[0].ToString();
        DebuggingText[2].text = "Water : " + elementAverage[1].ToString();
        DebuggingText[3].text = "Ground : " + elementAverage[2].ToString();
        DebuggingText[4].text = "Air : " + elementAverage[3].ToString();
    }
}