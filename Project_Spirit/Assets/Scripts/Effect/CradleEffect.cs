using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CradleEffect : MonoBehaviour
{
    public Sprite[] sprites; // ��������Ʈ �迭
    public float frameRate = 0.1f; // ������ �ӵ�

    private SpriteRenderer image; // �̹��� ������Ʈ
    private float timer; // Ÿ�̸�
    private int currentFrame; // ���� ������

    void Start()
    {
        image = GetComponent<SpriteRenderer>();
        if (sprites.Length > 0)
        {
            image.sprite = sprites[0];
        }
    }

    void Update()
    {
       
    timer += Time.unscaledDeltaTime;

        if (timer >= frameRate)
        {
            timer -= frameRate;
            currentFrame = (currentFrame + 1) % sprites.Length; // �迭 ���� �����ϸ� ó������ ���ư�
            image.sprite = sprites[currentFrame];
        }
    }
    
}
