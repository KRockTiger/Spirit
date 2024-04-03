using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DetectMove : MonoBehaviour
{
    Node[,] NodeArray;
    
    int[,] intArray, StartNode, CurNode;
    
   
    [SerializeField]
    Button button;
    [SerializeField]
    Sprite[] sprites;
    [SerializeField]
    Vector2Int bottomLeft, topRight, startPos, targetPos;
    
    enum Dir
    {
        Up = 0,
        Left = 1,
        Down = 2,
        Right = 3
    }

    enum Detect
    {
        None,
        Normal,
        Sign
    }
    Detect detection;

    private void Start()
    {
       
    }
    private void Update()
    {
        switch(detection)
        {
            case Detect.None:
                break;
            case Detect.Normal:
                break;
            case Detect.Sign:
                break;
        }
    }
    public void checkDirection()
    {
        Debug.Log(TileDataManager.instance.nodes[1, 1].x);
        int startX = (int)startPos.x - (int)bottomLeft.x;
        int startY = (int)startPos.y - (int)bottomLeft.y;
        
        Debug.Log(startX+" " +startY);

        // �ٶ󺸴� ���� �������� ��ǥ ��ȭ�� ��Ÿ��.  
        int[] frontX = { 0, -1, 0, 1 };  // Up , Left, Down, Right
        int[] frontY = { 1, 0, -1, 0 };

        int[] leftX = { -1, 0, 1, 0 }; // ��ȸ�� ���� ��ġ��
        int[] leftY = { 0, -1, 0, 1 }; // ��ȸ�� ���Ŀ� ���� �ٶ󺸴� ������ �ݽð�������� 90�� ȸ��

        int[] rightX = { 1, 0, -1, 0 };  // ��ȸ�� ���� ��ġ��
        int[] rightY = { 0, 1, 0, -1 };

        int _dir = (int)Dir.Up;

        int CurposX = 2;
        int CurposY = 1;

          Debug.Log(TileDataManager.instance.nodes[startX + leftX[_dir], startY + leftY[_dir]].x);  // x ��ǥ
          Debug.Log(TileDataManager.instance.nodes[startX + leftX[_dir], startY + leftY[_dir]].y);  // y ��ǥ
        //  1. ���� �ٶ󺸴� �������� �������� �������� �� �� �ִ��� Ȯ��.
         if (TileDataManager.instance.nodes[CurposX + leftX[_dir], CurposY + leftY[_dir]].nodeSprite == GetdetectSprite(0))
         {
            Debug.Log("���ʿ� Ground�� �ֽ��ϴ�.");
         }
        
        // 2. ���� �ٶ󺸴� ������ �������� ������ �� �ִ��� Ȯ��.
        if (TileDataManager.instance.nodes[(CurposX + frontX[_dir]), (CurposY + frontY[_dir])].nodeSprite == GetdetectSprite(0))
        {
           Debug.Log("���ʿ� Ground�� �ֽ��ϴ�.");
        }
        // else
        //    return;
    }

    private Sprite GetdetectSprite(int _num)
    {
        if (_num == 0)
            return sprites[0];
        else
            return null;
    }
}
