using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenBookSequence : MonoBehaviour
{
    public Sprite[] sprites; // ��������Ʈ �迭
    public float frameRate = 0.1f; // ������ �ӵ�

    private Image image; // �̹��� ������Ʈ
    private float timer; // Ÿ�̸�
    private int currentFrame; // ���� ������

    void Start()
    {
        image = GetComponent<Image>();
        if (sprites.Length > 0)
        {
            image.sprite = sprites[0];
        }
    }

    void Update()
    {
        if (currentFrame < sprites.Length - 1)
        {
            timer += Time.deltaTime;

            if (timer >= frameRate)
            {
                timer -= frameRate;
                currentFrame = Mathf.Min(currentFrame + 1, sprites.Length - 1);
                image.sprite = sprites[currentFrame];
            }
        }
    }
}
