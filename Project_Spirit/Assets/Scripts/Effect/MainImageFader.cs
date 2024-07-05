using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainImageFader : MonoBehaviour
{
    public Image image1;
    public Image image2;
    public float crossfadeDuration = 2.0f;
    public float displayDuration = 10.0f;

    private void Start()
    {
        // ó�� ������ �� �ϳ��� �̹����� ���̵��� ����
        image1.color = new Color(image1.color.r, image1.color.g, image1.color.b, 1f);
        image2.color = new Color(image2.color.r, image2.color.g, image2.color.b, 0f);

        // ���� ȿ�� ����
        StartCoroutine(CrossfadeImages());
    }

    private IEnumerator CrossfadeImages()
    {
        while (true)
        {
            // �̹��� 1���� �̹��� 2�� ���̵�
            yield return StartCoroutine(Fade(image1, image2));

            // 10�ʰ� ���
            yield return new WaitForSeconds(displayDuration);

            // �̹��� 2���� �̹��� 1�� ���̵�
            yield return StartCoroutine(Fade(image2, image1));

            // 10�ʰ� ���
            yield return new WaitForSeconds(displayDuration);
        }
    }

    private IEnumerator Fade(Image from, Image to)
    {
        float elapsedTime = 0f;

        while (elapsedTime < crossfadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / crossfadeDuration);

            from.color = new Color(from.color.r, from.color.g, from.color.b, 1f - alpha);
            to.color = new Color(to.color.r, to.color.g, to.color.b, alpha);

            yield return null;
        }

        from.color = new Color(from.color.r, from.color.g, from.color.b, 0f);
        to.color = new Color(to.color.r, to.color.g, to.color.b, 1f);
    }
}
