using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Node : MonoBehaviour
{
    public int nodeValue;   // Ÿ���� ��
    public bool isSignal = false;
    public bool isWalk = false;
    public int rotationStack;   // �÷��̾ �ش� ��带 ��� ȸ�� ���״���? 
    public int stack = 0;
    public Sprite nodeSprite;   // Ÿ�� ��������Ʈ
    // Tile 90, 180, 270��
    // (0.00000, 0.00000, 0.70711, 0.70711) , (0.00000, 0.00000, 1.00000, 0.00000), (0.00000, 0.00000, -0.70711, 0.70711)
    public Quaternion rotation;
    
    public Node(int _x, int _y)
    {
        x = _x;
        y = _y;
    }
    public int x;
    public int y;
  
}
