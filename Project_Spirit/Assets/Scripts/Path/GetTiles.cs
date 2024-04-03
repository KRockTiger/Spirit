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

    // Ÿ�ϸŴ��� Ŭ������ �ν��Ͻ� ����
    Node[,] nodes;

    private void Start()
    {   
        nodes = TileDataManager.instance.nodes;

        sizeX = topRight.x - bottomLeft.x + 1;
        sizeY = topRight.y - bottomLeft.y + 1;

        InstantiateTile();
        checkTileSprite();
    }

    public void InstantiateTile()
    {
        nodes = new Node[sizeX, sizeY];

        for(int i = 0; i < sizeX; i++)
        {
            for(int j = 0;  j < sizeY; j++)
            {
                nodes[i, j] = new Node();
                Debug.Log(nodes[i, j]);
            }
        }

    }
    // TileDataManager�� ��������Ʈ �� ����.
    public void checkTileSprite()
    {
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
                        //TileDataManager.instance.tileArray[i,j] = new TileDataManager(pos,tileSprite);
                        nodes[i,j].nodeSprite = tileSprite;
                        Debug.Log(nodes[i,j].nodeSprite);
                    }
                }
            }
        }
    }



}
