using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public int nodeValue;   // Ÿ���� ��
    public Sprite nodeSprite;   // Ÿ�� ��������Ʈ
    public Node(int _x, int _y)
    {
        x = _x;
        y = _y;
    }
    public int x;
    public int y;
  
}
