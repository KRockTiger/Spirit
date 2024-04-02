using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileDataManager : MonoBehaviour
{
    public static TileDataManager instance = null;        
    public int[,] tileArray = new int[103, 103];

    enum TileType
    {
        None = 0,
        Building = 1, // �ǹ�
        Cradle = 2, // ���
        Road = 3, // ����
        Resource = 4, // �ڿ�
        Mark = 5, // ǥ��
    }
    
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public void SetTileType(int x, int y, int type)
    {
        tileArray[x, y] = type;
    }

    public int GetTileType(int x, int y)
    {
        return tileArray[x, y];
    }    
}
