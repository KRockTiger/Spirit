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
    public Transform buildingParent; // ������ ������ �θ� ������Ʈ.
    public BuildingDatabaseSO database;

    [Header("Ÿ�� ��")]
    public Tilemap gameTilemap;
    public Tilemap gridTilemap;
    public Tile defaultTile, greenTile, orangeTile, redTile;

    private GameObject mouseIndicator;
    private int[,] copyArray = new int[103, 103];
    private List<Vector3Int> roadBufferList = new List<Vector3Int>();
    private List<Tuple<Vector2Int, Vector2Int>> buildingList = new List<Tuple<Vector2Int, Vector2Int>>(); // �ǹ� ����, ���ϴ� ��ǥ ����.    
    public bool IsPointerOverUI()
    => EventSystem.current.IsPointerOverGameObject();
    
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
                mouseIndicator.transform.position = grid.WorldToCell(ProcessingMousePosition());                
                if (Input.GetKeyDown(KeyCode.Mouse0))                 
                    PlaceBuilding();
                if (Input.GetKeyDown(KeyCode.Q))
                    RotateBuilding(false);
                if (Input.GetKeyDown(KeyCode.E))
                    RotateBuilding(true);                
                break;

            case CraftMode.PlaceRoad:
                Vector3Int mousePos = grid.WorldToCell(ProcessingMousePosition());
                if (Input.GetKey(KeyCode.Mouse0))                
                    PlaceRoadTileBuffer(mousePos);
                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    if (roadBufferList.Count == 0)                    
                        return;                    
                    PlaceRoadTile();
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

    private Vector3 ProcessingMousePosition()
    {
        // ���콺 ��ǥ�� ���� ��ǥ�� ����.
        Vector3 mousePos = Input.mousePosition;                
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);        
        return worldPos;
    }
    
    public void ResetGridTile()
    {        
        for(int i = 0; i < 103; i++)
        {
            for(int j = 0; j < 103; j++)
            {
                gridTilemap.SetTile(new Vector3Int(i, j, 0), defaultTile);
            }
        }
    }
}

partial class CraftManager
{
    public void OnClickBuildingSelectButton(GameObject building)
    {
        mouseIndicator = Instantiate(building, buildingParent);
        CopyTileArray();
        ChangeCraftMode(CraftMode.PlaceBuilding);
        // ������ �ǹ� ��ư �� �ٸ� ��ư ��� ó�� ������ ���� ��.        
    }

    // �ǹ��� ��ġ�� �� ����� �Լ�.     
    public void PlaceBuilding()
    {
        Vector2Int upperRight = new Vector2Int(Mathf.RoundToInt(mouseIndicator.transform.position.x), Mathf.RoundToInt(mouseIndicator.transform.position.y));
        var angles = mouseIndicator.transform.GetChild(0).rotation.eulerAngles;
        int x = 0, y = 0;
        if (angles.z % 180 == 0)
        {
            x = (int)mouseIndicator.transform.GetComponent<BoxCollider2D>().size.x;
            y = (int)mouseIndicator.transform.GetComponent<BoxCollider2D>().size.y;
        }
        else
        {
            x = (int)mouseIndicator.transform.GetComponent<BoxCollider2D>().size.y;
            y = (int)mouseIndicator.transform.GetComponent<BoxCollider2D>().size.x;
        }        
        Vector2Int bottomLeft = new Vector2Int(upperRight.x - x + 1, upperRight.y - y + 1);

        if (isOverTwoRoadsAttackedBuilding(new Tuple<Vector2Int, Vector2Int>(upperRight, bottomLeft))
            || isBuildingOvelapBuilding(upperRight, bottomLeft))
        {            
            Destroy(mouseIndicator);
            ChangeCraftMode(CraftMode.Default);
            return;
        }

        for (int i = (int)upperRight.y; i >= (int)bottomLeft.y; i--)
        {
            for (int j = (int)upperRight.x; j >= (int)bottomLeft.x; j--)
            {                
                copyArray[j, i] = 1;
            }
        }
        mouseIndicator = null;
        buildingList.Add(new Tuple<Vector2Int, Vector2Int>(upperRight, bottomLeft));
        PasteTileArray();
        ChangeCraftMode(CraftMode.Default);
    }

    bool isBuildingOvelapBuilding(Vector2Int upperRight, Vector2Int bottomLeft)
    {
        for (int i = upperRight.y; i >= bottomLeft.y; i--)
        {
            for (int j = upperRight.x; j >= bottomLeft.x; j--)
            {                
                if (TileDataManager.instance.GetTileType(j, i) == 1)
                {
                    Debug.Log("��ġ�� �ǹ� �ִ�");
                    return true;
                }
            }
        }
        return false;
    }   

    // �ǹ� ȸ�� �Լ�
    public void RotateBuilding(bool isRight)
    {
        var angles = mouseIndicator.transform.GetChild(0).rotation.eulerAngles;
        angles.z += isRight == true ? 90 : -90;

        if (angles.z >= 360)
            angles.z -= 360;
        else if (angles.z < 0)
            angles.z += 360;
        mouseIndicator.transform.GetChild(0).rotation = Quaternion.Euler(angles);

        float x = 0, y = 0;
        if (angles.z % 180 == 0)
        {
            x = mouseIndicator.transform.GetComponent<BoxCollider2D>().size.x;
            y = mouseIndicator.transform.GetComponent<BoxCollider2D>().size.y;
        }
        else
        {
            x = mouseIndicator.transform.GetComponent<BoxCollider2D>().size.y;
            y = mouseIndicator.transform.GetComponent<BoxCollider2D>().size.x;
        }
        Vector2 mouseIndicatorPos = new Vector2(-(x / 2 - 1), -(y / 2 - 1));
        mouseIndicator.transform.GetChild(0).localPosition = mouseIndicatorPos;
    }
}

partial class CraftManager
{
    private Tile selectedRoad;    
    public void OnClickRoadSelectButton(Tile tile)
    {
        selectedRoad = tile;
        CopyTileArray();
        ChangeCraftMode(CraftMode.PlaceRoad);

        // Todo. ������ �ǹ� ��ư �� �ٸ� ��ư ��� ó�� ������ ���� ��.
    }
    bool isRoadOverlapRoad(Vector3Int pos)
    {
        if (TileDataManager.instance.GetTileType(pos.x, pos.y) == 3)
            return true;
        return false;
    }
    bool isRoadOvelapBuilding(Vector3Int pos)
    {
        if (TileDataManager.instance.GetTileType(pos.x, pos.y) == 1)
            return true;
        return false;
    }
    bool canPlaceRoad()
    {
        foreach (Tuple<Vector2Int, Vector2Int> buildingPos in buildingList)
        {
            if (isOverTwoRoadsAttackedBuilding(buildingPos))
            {
                Debug.Log("�ǹ��� �� ���� �ʰ��ϴ� ���� ����Ǵ� ��찡 �ֽ��ϴ�.");
                return false;
            }
        }

        foreach (Vector3Int roadBuffer in roadBufferList)
        {
            if (TileDataManager.instance.GetTileType(roadBuffer.x, roadBuffer.y) == 1)
            {
                Debug.Log("�ǹ��� ���ΰ� ��Ĩ�ϴ�.");
                return false;
            }
        }
        Debug.Log("��ġ ����");
        return true;
    }
    public void PlaceRoadTileBuffer(Vector3Int pos)
    {        
        // ��ġ ������ Ÿ������ üũ.
        if (!roadBufferList.Contains(pos))
        {
            if (isRoadOverlapRoad(pos))
                gridTilemap.SetTile(pos, orangeTile);
            else if (isRoadOvelapBuilding(pos))
                gridTilemap.SetTile(pos, redTile);
            else
                gridTilemap.SetTile(pos, greenTile);

            gameTilemap.SetTile(pos, selectedRoad);
            if (TileDataManager.instance.isRange(pos.x, pos.y))
            {
                copyArray[pos.x, pos.y] = 3;
                Debug.Log(TileDataManager.instance.GetTileType(pos.x, pos.y));
                roadBufferList.Add(pos);
            }

            if (roadBufferList.Count == 1)
                return;

            // �밢������ �����ؼ� ���� �������� �� ����.
            Vector3Int prevRoad = roadBufferList[roadBufferList.Count - 2];
            if ((prevRoad.x != pos.x) && (prevRoad.y != pos.y))
            {
                Vector3Int complementaryPos = new Vector3Int(prevRoad.x, pos.y, 0);
                gameTilemap.SetTile(complementaryPos, selectedRoad);
                if (TileDataManager.instance.isRange(complementaryPos.x, complementaryPos.y))
                {
                    copyArray[complementaryPos.x, complementaryPos.y] = 3;
                    roadBufferList.Add(complementaryPos);
                }
            }
        }
    }
    public void PlaceRoadTile()
    {        
        if (canPlaceRoad())
        {            
            PasteTileArray();
        }
        else
        {
            foreach (Vector3Int roadBuffer in roadBufferList)
            {
                gameTilemap.SetTile(roadBuffer, null);
            }
        }
        ResetGridTile();
        roadBufferList.Clear();
        ChangeCraftMode(CraftMode.Default);
    }

    bool isOverTwoRoadsAttackedBuilding(Tuple<Vector2Int, Vector2Int> buildingPos)
    {
        int roadCount = 0;
        Vector2Int upperRight = buildingPos.Item1;
        Vector2Int bottomLeft = buildingPos.Item2;

        // ����
        if (copyArray[upperRight.x, upperRight.y + 1] == 3)
            roadCount++;
        if (copyArray[upperRight.x + 1, upperRight.y] == 3)
            roadCount++;

        // ���ϴ�
        if (copyArray[upperRight.x + 1, bottomLeft.y] == 3)
            roadCount++;
        if (copyArray[upperRight.x, bottomLeft.y - 1] == 3)
            roadCount++;

        // �»��
        if (copyArray[bottomLeft.x, upperRight.y + 1] == 3)
            roadCount++;
        if (copyArray[bottomLeft.x - 1, upperRight.y] == 3)
            roadCount++;

        // ���ϴ�
        if (copyArray[bottomLeft.x - 1, bottomLeft.y] == 3)
            roadCount++;
        if (copyArray[bottomLeft.x, bottomLeft.y - 1] == 3)
            roadCount++;

        // �𼭸�
        for (int i = upperRight.x - 1; i > bottomLeft.x; i--)
        {
            if (copyArray[i, upperRight.y + 1] == 3)
                roadCount++;
            if (copyArray[i, bottomLeft.y - 1] == 3)
                roadCount++;
        }

        for (int i = upperRight.y - 1; i > bottomLeft.y; i--)
        {
            if (copyArray[upperRight.x + 1, i] == 3)
                roadCount++;
            if (copyArray[bottomLeft.x - 1, i] == 3)
                roadCount++;
        }

        if (roadCount > 2)
        {
            Debug.Log(buildingPos);
            return true;
        }
        return false;
    }

    void CopyTileArray()
    {
        for(int i = 0; i < 103; i++)
        {
            for(int j = 0; j < 103; j++)
            {
                copyArray[j, i] = TileDataManager.instance.GetTileType(j, i);
            }
        }
    }

    void PasteTileArray()
    {
        for (int i = 0; i < 103; i++)
        {
            for (int j = 0; j < 103; j++)
            {
                TileDataManager.instance.SetTileType(j, i, copyArray[j, i]);
            }
        }
    }
}