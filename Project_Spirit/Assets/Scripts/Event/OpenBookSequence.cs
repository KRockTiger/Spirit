using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenBookSequence : MonoBehaviour
{
    public Sprite[] sprites; // ��������Ʈ �迭
    public Sprite[] Hotsprites; // ��������Ʈ �迭
    public float frameRate = 0.1f; // ������ �ӵ�
    public float rainDropInterval = 1.5f; // BookDrop �޼��� ȣ�� ����
    public GameObject eventmanager;
    private Image image; // �̹��� ������Ʈ
    private float timer; // Ÿ�̸�
    private int currentFrame; // ���� ������

    private bool isBookDropPlay;
    private float rainDropTimer; // BookDrop ȣ�� Ÿ�̸�

    void Start()
    {
        if (!eventmanager.GetComponent<BookEvent>().Hoteventhasoccured)
        {
            image = GetComponent<Image>();
            if (sprites.Length > 0)
            {
                image.sprite = sprites[0];
            }
        }
        else
        {
            image = GetComponent<Image>();
                image.sprite = Hotsprites[0];
        }

        
    }

    void Update()
    {
        if(!eventmanager.GetComponent<BookEvent>().Hoteventhasoccured)
        {

            if (currentFrame < sprites.Length - 1)
            {
                timer += Time.unscaledDeltaTime;

                if (timer >= frameRate)
                {
                    timer -= frameRate;
                    currentFrame = Mathf.Min(currentFrame + 1, sprites.Length - 1);
                    image.sprite = sprites[currentFrame];
                }
            }

            rainDropTimer += Time.unscaledDeltaTime;
            if (!isBookDropPlay && rainDropTimer >= rainDropInterval)
            {
                int count = Random.Range(3, 9);
                StartCoroutine(PlayRainDrop(count));
                rainDropTimer = 0f; // Ÿ�̸� ����
            }



        }
    }

    private IEnumerator PlayRainDrop(int count)
    {
        Debug.Log("��ȣ��");
        isBookDropPlay = true;
        AudioClip clip = SoundManager.instance.EventSFX[count];
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume *= 0.3f;
        source.Play();
        yield return new WaitForSeconds(clip.length);
        Destroy(source);
        isBookDropPlay = false;
    }
}
