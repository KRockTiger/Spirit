using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class QuestManager : MonoBehaviour
{
    public static QuestManager instance = null;

    public GameObject QuestPrefab;
    public Transform QuestUI_Transform;

    // ����Ʈ ���� üũ
    public bool Research;
    public bool rain;
    public bool GainR;
    public bool Storage;
    public bool ResearchMode;
    public bool Spirit;

    // ����Ʈ Ŭ���� ����
    public int researchSettlement;  // ������ ��ġ
    public bool OverRain;       // ���� �Ⱓ
    public bool GainResource;   // �ڿ� ȹ��
    public bool StorageSettlement; // ����� �Ǽ�
    public bool ResearchMode2;  // ������ 2�ܰ� �ر�
    public bool SpiritKing; // ���ɿ��� �����


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }
    
    public void InstantiateQuest(int QuestID)
    {
        GameObject clone = Instantiate(QuestPrefab, QuestUI_Transform);
        QuestPrefab quest = clone.GetComponent<QuestPrefab>();
        quest.SetQuest(QuestID);

        UpdateQuestUI();
    }        
    
    public void UpdateQuestUI()
    {
        float y_pos = 0;
        for (int i = 0; i < QuestUI_Transform.childCount; i++)
        {
            RectTransform childRect = QuestUI_Transform.GetChild(i).GetComponent<RectTransform>();
            float offset = childRect.sizeDelta.y / 2;
            y_pos -= offset;
            childRect.anchoredPosition = new Vector2(0, y_pos);
            y_pos -= offset;
        }
    }
        
    // For Debug. ����, ���� ���� �𵨷� ����Ʈ ����� üũ�ϸ� �ɵ�.
    public void GainItem()
    {
        // targetNum�� �������� count�� ȹ���ߴ�.        
        Stack<int> clearIndex = new Stack<int>();
        for (int i = 0; i < QuestUI_Transform.childCount; i++)
        {
            QuestPrefab quest = QuestUI_Transform.GetChild(i).GetComponent<QuestPrefab>();
            if (quest.CheckClear())
                clearIndex.Push(i);
        }

        while (clearIndex.Count != 0)
        {
            int index = clearIndex.Pop();            
            QuestUI_Transform.transform.GetChild(index).GetComponent<QuestPrefab>().ClearQuest();
        }
        UpdateQuestUI();
    }    

    // For Debug    

    public void Update()
    {
        UpdateQuestUI();
    }
}
