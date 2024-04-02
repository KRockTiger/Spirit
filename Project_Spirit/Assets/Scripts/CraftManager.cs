using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using TMPro;
public partial class CraftManager : MonoBehaviour
{
    // �ǹ� ���� ����.
    [SerializeField]
    private Grid grid; // �׸���.
    [SerializeField]
    private GameObject craftGrid; // ���� ��� �� ���� ǥ��.
    [SerializeField]
    private GameObject craftMenuUI; // �ϴ� ���� ���� UI
    [SerializeField]
    private GameObject buildingParent; // ������ ������ �θ� ������Ʈ.
    [SerializeField]
    private Tilemap gameTilemap;    

    private GameObject mouseIndicator;
    private List<Vector3Int> roadBufferList = new List<Vector3Int>();
    private Tile selectedRoad;
    
    // For Debug.
    public GameObject building_Prefab;
    public LayerMask placement_LayerMask;

    
    enum CraftMode
    {
        None,
        Craft,
        PlaceBuilding,
        PlaceRoad,
    }

    CraftMode craftMode;
    private void Start()
    {
        craftMode = CraftMode.None;
        mouseIndicator = null;
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
                    mouseIndicator = null;
                    craftMode = CraftMode.Craft;
                }
                break;
            case CraftMode.PlaceRoad:
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    Vector3Int gridPos = grid.WorldToCell(TrackMousePosition());                     
                    if (!roadBufferList.Contains(gridPos))
                    {                        
                        SetRoadTile(gridPos);
                        roadBufferList.Add(gridPos);
                        if (roadBufferList.Count == 1)
                            return;

                        // �밢�� �߰��� Ÿ�� ���� �ʵ���.
                        Vector3Int prevRoad = roadBufferList[roadBufferList.Count - 2];                        
                        if ((prevRoad.x != gridPos.x) && (prevRoad.y != gridPos.y))                                                    
                            SetRoadTile(new Vector3Int(prevRoad.x, gridPos.y, 0));
                    }                                                         
                }
                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    roadBufferList.Clear();

                    // ToDo. �� �Ǽ� ���� ���� ���� �ǽ�.                     
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
        // ���콺 ����ٴ� �ǹ� ���� �ҷ��ͼ� ���콺�� ����.
        mouseIndicator = Instantiate(building, buildingParent.transform); // Todo. transform�� ��ġ ���� �Ű����� �߰�.

        // ������ �ǹ� ��ư �� �ٸ� ��ư ��� ó�� ������ ���� ��.

        craftMode = CraftMode.PlaceBuilding;
    }

    // ������ �ǹ��� ������ ������ �¿��� �ҷ����� �Լ�.

    // �ǹ��� ���콺 ��ġ�� ���� �ٴϵ��� �ϴ� �Լ�.

    // �ǹ��� ��ġ�� �� ����� �Լ�.     

    // �� ��ġ ����.

    // �� ���� ��ư�� ������ �Լ�.
    public void OnClickRoadSelectButton(Tile tile)
    {
        selectedRoad = tile;

        craftMode = CraftMode.PlaceRoad;
        roadBufferList.Clear();
        // ������ �ǹ� ��ư �� �ٸ� ��ư ��� ó�� ������ ���� ��.
        
    }

    void SetRoadTile(Vector3Int pos)
    {
        gameTilemap.SetTile(pos, selectedRoad);
        TileDataManager.instance.SetTileType(pos.x, pos.y, 3);
    }    
    
    // ���콺�� ���� ����, �Ǽ��� �����ϰ� ���� ����� ���, ���� ��� ����.
    // �Ǽ� ���� ���� ���� ������ �ʿ�.    
}