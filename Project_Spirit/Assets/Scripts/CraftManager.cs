using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using TMPro;
public partial class CraftManager : MonoBehaviour
{    
    [Header("���� ���")]
    public Grid grid; // �׸���.
    public GameObject craftGrid; // ���� ��� �� ���� ǥ��.
    public GameObject craftMenuUI; // �ϴ� ���� ���� UI
    public GameObject buildingParent; // ������ ������ �θ� ������Ʈ.

    [Header("Ÿ�� ��")]
    public Tilemap gameTilemap;
    public Tilemap gridTilemap;
    public Tile[] stateTile; 

    [Header("��Ÿ")]
    public LayerMask placement_LayerMask;

    private GameObject mouseIndicator;
    private List<Vector3Int> roadBufferList = new List<Vector3Int>();
    private Tile selectedRoad;
        
    public bool IsPointerOverUI()
    => EventSystem.current.IsPointerOverGameObject();

    private const int underLimit = 0;
    private const int overLimit = 103;

    // For Debug.
    public GameObject building_Prefab;
    enum CraftMode
    {
        None,
        Default,
        PlaceBuilding,
        PlaceRoad,
    }

    CraftMode craftMode;
    private void Start()
    {
        craftMode = CraftMode.None;
        mouseIndicator = null;

        // For Debug. 
        TileDataManager.instance.SetTileType(0, 0, 1);
        TileDataManager.instance.SetTileType(15, 3, 2);
    }

    void ChangeCraftMode(CraftMode mode)
    {
        craftMode = mode;
        switch (mode)
        {
            case CraftMode.None:
                break;
            case CraftMode.Default:                
                craftGrid.SetActive(true);
                craftMenuUI.SetActive(true);
                break;
            case CraftMode.PlaceBuilding:                
                craftMenuUI.SetActive(false);
                break;
            case CraftMode.PlaceRoad:
                craftMenuUI.SetActive(false);
                break;
        }
    }
    public void Update()
    {
        switch (craftMode)
        {
            case CraftMode.PlaceBuilding:                
                mouseIndicator.transform.position = grid.CellToWorld(grid.WorldToCell(TrackMousePosition()));

                // Ŭ�� �Է��� ������ �ش� ��ǥ�� ��ġ�ǵ���.
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    Debug.Log(mouseIndicator.transform.position);
                    mouseIndicator = null;
                    ChangeCraftMode(CraftMode.Default);                    
                }
                break;
            case CraftMode.PlaceRoad:
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    Vector3Int gridPos = grid.WorldToCell(TrackMousePosition());                     
                    if (!roadBufferList.Contains(gridPos))
                    {                        
                        // �ߺ� ��ġ���� üũ.
                        if (isOverlapRoad(gridPos))
                        {
                            Debug.Log("�ߺ� ��ġ!!");
                            gridTilemap.SetTile(gridPos, stateTile[2]);
                        }
                        else                        
                            gridTilemap.SetTile(gridPos, stateTile[1]);
                        
                        SetRoadTile(gridPos);
                        roadBufferList.Add(gridPos);
                        if (roadBufferList.Count == 1)
                            return;
                        
                        Vector3Int prevRoad = roadBufferList[roadBufferList.Count - 2];                        
                        if ((prevRoad.x != gridPos.x) && (prevRoad.y != gridPos.y))                                                    
                            SetRoadTile(new Vector3Int(prevRoad.x, gridPos.y, 0));
                    }                                                         
                }
                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    // ToDo. �� �Ǽ� ���� ���� ���� �ǽ�.                     
                    if (roadBufferList.Count == 0)
                        return;

                    Debug.Log("�� �Ǽ� : " + CanPlaceRoad());

                    resetGridTile();
                    roadBufferList.Clear();
                    ChangeCraftMode(CraftMode.Default);
                }
                break;
        }        
    }

    // Craft ��� ����.
    public void EnterCraftMode()
    {
        craftGrid.SetActive(true);
        craftMenuUI.SetActive(true);
    }

    private Vector3 TrackMousePosition()
    {
        // ���콺 ��ǥ�� ȭ�� �� ��ǥ�� ����.
        Vector3 mousePos = Input.mousePosition;                
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);        
        return worldPos;
    }    

    // �ǹ� ��ġ ����.

    // �ǹ� ���� ��ư�� ������ �Լ�.
    public void OnClickBuildingSelectButton(GameObject building)
    {                
        mouseIndicator = Instantiate(building, buildingParent.transform);

        // ������ �ǹ� ��ư �� �ٸ� ��ư ��� ó�� ������ ���� ��.
        ChangeCraftMode(CraftMode.PlaceBuilding);
    }

    // ������ �ǹ��� ������ ������ �¿��� �ҷ����� �Լ�.

    // �ǹ��� ���콺 ��ġ�� ���� �ٴϵ��� �ϴ� �Լ�.

    // �ǹ��� ��ġ�� �� ����� �Լ�.     

    // �� ��ġ ����.

    // �� ���� ��ư�� ������ �Լ�.
    public void OnClickRoadSelectButton(Tile tile)
    {
        selectedRoad = tile;
        
        ChangeCraftMode(CraftMode.PlaceRoad);
        roadBufferList.Clear();
        // ������ �ǹ� ��ư �� �ٸ� ��ư ��� ó�� ������ ���� ��.
        
    }

    void SetRoadTile(Vector3Int pos)
    {
        gameTilemap.SetTile(pos, selectedRoad);
        TileDataManager.instance.SetTileType(pos.x, pos.y, 3);
    }    
    
    bool CanPlaceRoad()
    {
        Vector2Int startPos = new Vector2Int(roadBufferList[0].x, roadBufferList[0].y);
        Vector2Int endPos = new Vector2Int(roadBufferList[^1].x, roadBufferList[^1].y);

        int[] dx = new int [4]{ 0, 0, -1, 1 };
        int[] dy = new int [4]{ 1, -1, 0, 0 };

        // ����� ���� üũ.
        for(int i = 0; i < 4; i++)
        {
            int nx = startPos.x + dx[i];
            int ny = startPos.y + dy[i];
            if (nx < underLimit || nx > overLimit || ny < underLimit || nx > overLimit)
                continue;

            int nextTileType = TileDataManager.instance.GetTileType(nx, ny);
            if (nextTileType == 1 || nextTileType == 2)
            {
                return true;
            }            
        }
        // ������ ���� üũ.
        for (int i = 0; i < 4; i++)
        {
            int nx = endPos.x + dx[i];
            int ny = endPos.y + dy[i];
            if (nx < underLimit || nx > overLimit || ny < underLimit || nx > overLimit)
                continue;

            int nextTileType = TileDataManager.instance.GetTileType(nx, ny);
            if (nextTileType == 1 || nextTileType == 2)
            {
                return true;
            }
        }
        return false;
    }

    bool isOverlapRoad(Vector3Int pos)
    {
        if (TileDataManager.instance.GetTileType(pos.x, pos.y) == 3)
            return true;
        return false;
    }

    void resetGridTile()
    {        
        for(int i = 0; i < 103; i++)
        {
            for(int j = 0; j < 103; j++)
            {
                gridTilemap.SetTile(new Vector3Int(i, j, 0), stateTile[0]);
            }
        }
    }
}