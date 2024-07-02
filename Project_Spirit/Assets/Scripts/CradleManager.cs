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
    private GameObject cradleGage;
    
    [Header("��������Ʈ")]
    [SerializeField]
    private Sprite[] cradleSprite;
    [SerializeField]
    private Sprite[] cradleGrowthRateSprite;
    [SerializeField]
    private Sprite[] cradleGageSprite;

    // ���� ���� ����.
    Queue<Tuple<int, DateTime>>[] elementQueue = { 
        new Queue<Tuple<int, DateTime>>(), 
        new Queue<Tuple<int, DateTime>>(), 
        new Queue<Tuple<int, DateTime>>(), 
        new Queue<Tuple<int, DateTime>>() 
    };
    private float[] elementAverage = { 0, 0, 0, 0 };
    private int[] elementSum = { 0, 0, 0, 0 };
    TimeSpan span = TimeSpan.FromSeconds(10);

    // ��� ���� ����.
    private int Level = 0;
    private int GrowthPoint = 0;
    private int GrowthState = 0;
    private int[] GrowthValue = { 50, 25, 10, -40 };
    private float GrowthTime = 0f;
    private float GrowthCooldown = 10f;           
    
    void Start()
    {        
    }

    // Update is called once per frame    
    void Update()
    {
        RemoveExpiredElement();
        CalculateElementAverage();
        AddCradleGrowth();
        UpdateCradleUI();
    }

    // 4���� ���� ������ ���� ����.
    // ���ɿ� ���� ������ ����.            
    void AddCradleGrowth()
    {
        GrowthTime += Time.deltaTime;
        if (GrowthTime > GrowthCooldown)
        {
            GrowthPoint += GrowthValue[GrowthState];

            if (GrowthPoint > 100)
                ToNextCradle();
            else if (GrowthPoint < 100)
            {
                // ���� ����.
            }            
            
            SetCradleGrowthSlider();
            GrowthTime = 0f;
        }
    }

    void SetCradleGrowthSlider()
    {
        if (GrowthPoint < 0)
        {            
            cradleGage.GetComponent<Image>().fillAmount = GrowthPoint * 0.01f * (-1f);
            cradleGage.GetComponent<Image>().sprite = cradleGageSprite[1];
        }
        else
        {            
            cradleGage.GetComponent<Image>().fillAmount = GrowthPoint * 0.01f;
            cradleGage.GetComponent<Image>().sprite = cradleGageSprite[0];
        }
    }

    // ���ɿ� ������ �Լ�.
    public void ToNextCradle()
    {
        Level++;
        GrowthPoint = 0;
        if (Level < 8)
            cradle.transform.Find("CradleImage").GetComponent<Image>().sprite = cradleSprite[Level];
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
        float offset = 1.3f; // ��ü �����̴��� ���̿� ���� �ٲ�� ��. ��ü �����̴� ���� / 2 / 100 ���� ��.

        for (int i = 0; i < 4; i++)
        {
            RectTransform result = elementSlider[i].transform.GetChild(0).GetComponent<RectTransform>();

            float ratio = 0f; // ���������� ������ ����.
            float x_pos = 0f;
            float width = 0f;            

            ratio = Mathf.Log(Mathf.Abs(elementAverage[i] - totalElementAverage) / totalElementAverage * 1000) * 10;
            width = ratio * offset;
            if (elementAverage[i] - totalElementAverage > 0)
                x_pos = width / 2;
            else if (elementAverage[i] - totalElementAverage < 0)
                x_pos = -width / 2;
            else
                x_pos = 0;

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
        if (GetTotalElementAverage() == 0)
        {
            GrowthState = 3;
            cradleGrowthRate.GetComponent<Image>().sprite = cradleGrowthRateSprite[3];
            return;
        }

        if (val <= 4)
        {
            GrowthState = 0;
            cradleGrowthRate.GetComponent<Image>().sprite = cradleGrowthRateSprite[0];
        }
        else if (val <= 6)
        {
            GrowthState = 1;
            cradleGrowthRate.GetComponent<Image>().sprite = cradleGrowthRateSprite[1];
        }
        else if (val <= 8)
        {
            GrowthState = 2;
            cradleGrowthRate.GetComponent<Image>().sprite = cradleGrowthRateSprite[2];
        }
        else
        {
            GrowthState = 3;
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
    // ���� ���� �Լ�.
}