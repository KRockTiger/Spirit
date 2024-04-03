using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DetectMove : MonoBehaviour
{
    TileDataManager[,] tileArray;
    TileDataManager StartNode, TargetNode, CurNode;
    List<TileDataManager> OpenList, ClosedList;

    public Sprite[] sprites;
    public Vector2Int bottomLeft, topRight, startPos, targetPos;
    int sizeX = 20, sizeY = 20;


    enum Dir
    {
        Up = 0,
        Left = 1,
        Down = 2,
        Right = 3
    }

    private void Start()
    {
        tileArray = new TileDataManager[sizeX, sizeY];

        // tileArray�� �� ��Ҹ� �ʱ�ȭ�մϴ�.
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
               // tileArray[i, j] = new TileDataManager(i,j,null); // TileDataManager�� �⺻ �����ڸ� ȣ���Ͽ� ��ü�� �ʱ�ȭ�մϴ�.
            }
        }

        //check();
    }
    public void check()
    {   
        sizeX = topRight.x - bottomLeft.x + 1;
        sizeY = topRight.y - bottomLeft.y + 1;

        StartNode = tileArray[startPos.x - bottomLeft.x, startPos.y - bottomLeft.y];
        TargetNode = tileArray[targetPos.x - bottomLeft.x, targetPos.y - bottomLeft.y];

        OpenList = new List<TileDataManager> { StartNode };
        ClosedList = new List<TileDataManager>();
        CurNode = OpenList[0];

        // �ٶ󺸴� ���� �������� ��ǥ ��ȭ�� ��Ÿ��.  
        int[] frontX = { 0, -1, 0, 1 };  // Up , Left, Down, Right
        int[] frontY = { 1, 0, -1, 0 };

        int[] leftX = { -1, 0, 1, 0 }; // ��ȸ�� ���� ��ġ��
        int[] leftY = { 0, -1, 0, 1 }; // ��ȸ�� ���Ŀ� ���� �ٶ󺸴� ������ �ݽð�������� 90�� ȸ��

        int[] rightX = { 1, 0, -1, 0 };  // ��ȸ�� ���� ��ġ��
        int[] rightY = { 0, 1, 0, -1 };

        int _dir = (int)Dir.Up;

        Debug.Log(tileArray[CurNode.x + leftX[_dir], CurNode.y + leftY[_dir]]);
        Debug.Log(tileArray[CurNode.x + leftX[_dir], CurNode.y + leftY[_dir]].tileSprite);
        // 1. ���� �ٶ󺸴� �������� �������� �������� �� �� �ִ��� Ȯ��.
        if (tileArray[CurNode.x + leftX[_dir], CurNode.y + leftY[_dir]].GetTileSprit() == GetdetectTileSprite(0))
        {
            Debug.Log("���ʿ� Ground�� �ֽ��ϴ�.");
        }
        // 2. ���� �ٶ󺸴� ������ �������� ������ �� �ִ��� Ȯ��.
        if (tileArray[(CurNode.x + frontX[_dir]), (CurNode.y + frontY[_dir])].GetTileSprit() == GetdetectTileSprite(0))
        {
            Debug.Log("���ʿ� Ground�� �ֽ��ϴ�.");
        }
        else
            return;
    }

    private Sprite GetdetectTileSprite(int _num)
    {
        if(_num == 0)
        {
            Sprite sprite = sprites[0];
            return sprite;
        }
        else
        {
            return null;
        }

    }
}
