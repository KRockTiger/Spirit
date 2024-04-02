using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GetTiles : MonoBehaviour
{
    public Tilemap tilemap; // Ÿ�ϸ� ��ü �Ҵ�.
    public Sprite targetSprite; // ã���� �ϴ� ��������Ʈ

    public Vector2Int bottomLeft, topRight, startPos, targetPos;
    int sizeX, sizeY;

    TileDataManager [,] tileArray;


    private void Awake()
    {   
        sizeX = topRight.x - bottomLeft.x + 1;
        sizeY = topRight.y - bottomLeft.y + 1;

        checkTileSprite();
    }

    // TileDataManager�� ��������Ʈ �� ����.
    public void checkTileSprite()
    {
        //tileArray = new TileDataManager[sizeX, sizeY];

        for (int i = 0; i < sizeX; i++)
        {
            for(int j = 0; j < sizeY; j++)
            {
                // Ÿ�� ��ġ ����
                Vector3Int tilePosition = new Vector3Int(i , j);
            
                TileBase tile = tilemap.GetTile(tilePosition);
                
                if(tile != null)
                {
                    // ������ Ÿ���� ��������Ʈ�� Ȯ���Ѵ�.
                    Sprite tileSprite = (tile as Tile).sprite;

                    if(tileSprite == targetSprite) 
                    {
                        Vector2Int pos = new Vector2Int(i , j);
                        Sprite spr = tileSprite;
                       //TileDataManager.instance.tileArray[i,j] = new TileDataManager(pos,tileSprite);
                       
                    }
                }
            }
        }
    }



}
