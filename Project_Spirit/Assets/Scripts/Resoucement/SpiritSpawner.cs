using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SpiritSpawner : MonoBehaviour
{
    [SerializeField]
    Slider[] allSliders;
    [SerializeField]
    GameObject[] allPrefabs;
    [SerializeField]
    GameObject SpawnUI;
    GameObject Spiritprefab;
    
    public bool Fire;
    public bool Water;
    public bool Ground;
    public bool Air;

    int[,] Area = new int[103,103];
    float[] Spawn = new float[] { 2f, 1.5f, 1f };   // 24, 18, 12
    float SpawnDuration = 0f;
    
    float gameTimer = 0f;
    float realTimeToGameTimeRatio = 720f;
    string SpawnerName;
    int spLv = 1;
    int spwLv = 2;

    Vector2 bottomLeft;
    Vector2 topRight;
    [HideInInspector]
    public Slider slider;
    Text textComp;

    enum Dir
    {
        Up = 0,
        Left = 1,
        Down = 2,
        Right = 3
    }

    private void Start()
    {
        SetSpawnerType();
        SpawnDuration = Spawn[0];
        SpawnUI.GetComponent<SpawnerUI>().ReceiveSpiritSpawnInfo(SetUIInfo());
    }

    private void Update()
    {
       // float spawnTime = slider.value * 1440f;
        // ���� 1�� => 12�� ���
        gameTimer += Time.deltaTime * realTimeToGameTimeRatio;

        if (slider != null)
        {
            float spawnTime = slider.value * 1440f;
            if (gameTimer >= realTimeToGameTimeRatio * SpawnDuration + spawnTime)
            {
                SpawnSpirit();
           
                gameTimer = 0f;
            }
        }
        else
        {
            if (gameTimer >= realTimeToGameTimeRatio * SpawnDuration)
            {
                SpawnSpirit();

                gameTimer = 0f;
            }
        }
        
    }
    void SpawnSpirit()
    {
        Vector2Int? OneRoad = isOneRoadAttached();
        
        if( OneRoad.HasValue)
        {
            Vector2Int road = OneRoad.Value;
            int row = road.x;
            int col = road.y;
            
            Vector3 newPosition = new Vector3(row, col, 0);
            GameObject SpiritObject = Instantiate(Spiritprefab, new Vector3(newPosition.x + 0.5f, newPosition.y + 0.5f, 0), Quaternion.identity);
            SpiritObject.GetComponent<DetectMove>().CurposX = row;
            SpiritObject.GetComponent<DetectMove>().CurposY = col;
            SpiritObject.GetComponent<DetectMove>()._dir = Redirection(row, col);
            
            
        }
        else
        {
           // Debug.Log("Oneroad is null");
        }
    }

    #region ���� ����� ����
    void SetSpawnerType()
    {
        if(Fire) { SpawnerName = "Fire"; }
        if (Water) { SpawnerName = "Water"; }
        if(Ground) { SpawnerName = "Ground"; }
        if(Air){ SpawnerName = "Air"; };
        SetRangeOfBuilding(SpawnerName);
    }

    void SetRangeOfBuilding(string name)
    {
        switch(name)
        {
            case "Fire":
                SetFireMap();
                break;
            case "Water":
                SetWaterMAp();
                break;
            case "Ground":
                SetGroundMap();
                break;
            case "Air":
                SetAirMap();
                break;
        }
    }

    void SetAirMap()
    {
        for(int i = 0; i < 6; i++)
        {
            for(int j = 50; j < 54; j++)
            {
                TileDataManager.instance.SetTileType(i, j, 1);
                Area[i, j] = 1;
                Spiritprefab = allPrefabs[3];
                //Debug.Log("���⿵��");
            }
        }
        bottomLeft = new Vector2(0, 50);
        topRight = new Vector2(6, 54);
    }

    void SetWaterMAp()
    {
        for (int i = 50; i < 56; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                TileDataManager.instance.SetTileType(i, j, 1);
                Area[i, j] = 1;
                Spiritprefab = allPrefabs[1];
                //Debug.Log("������");
            }
        }
        bottomLeft = new Vector2(50, 0);
        topRight = new Vector2(56, 4);
    }

    void SetGroundMap()
    {
        for (int i = 97; i <= 102; i++)
        {
            for (int j = 50; j < 54; j++)
            {
                TileDataManager.instance.SetTileType(i, j, 1);
                Area[i, j] = 1;
                Spiritprefab = allPrefabs[2];
                //Debug.Log("������");
            }
        }
        bottomLeft = new Vector2(97, 50);
        topRight = new Vector2(102, 54);
    }

    void SetFireMap()
    {
        for (int i = 49; i < 55; i++)
        {
            for (int j = 99; j <= 102; j++)
            {
                TileDataManager.instance.SetTileType(i, j, 1);
                Area[i, j] = 1;
                Spiritprefab = allPrefabs[0];
                //Debug.Log("�ҿ���");
            }
        }
        bottomLeft = new Vector2(49, 99);
        topRight = new Vector2(55, 102);
    }
    #endregion

    Vector2Int? isOneRoadAttached()
    {
        List<Vector2Int> resultList = new List<Vector2Int>();
        for (int i = (int)bottomLeft.x; i < topRight.x; i++)
        {
            for(int j = (int)bottomLeft.y; j <topRight.y; j++)
            {
                if (Area[i, j] == 1)
                {
                    int[] SpawndirX = { 0, 0, 1, -1 };
                    int[] SpawndirY = { 1, -1, 0, 0 };
                    for(int k = 0; k < 4; k++)
                    {
                        int newX = i + SpawndirX[k];
                        int newY = j + SpawndirY[k];
                    
                        // ���ο� �ε����� �迭 ���� ���� �ִ��� Ȯ���մϴ�.
                        if (newX >= 0 && newX < Area.GetLength(0) && newY >= 0 && newY < Area.GetLength(1))
                        {
                            // �����¿쿡 �ش��ϴ� �ε����� ���� 3���� Ȯ���ϰ�, �´ٸ� ����Ʈ�� �߰��մϴ�.
                            if (!resultList.Contains(new Vector2Int(newX,newY)) &&TileDataManager.instance.GetTileType(newX, newY) == 3)
                            {
                                Vector2Int newvec = new Vector2Int(newX,newY);
                                resultList.Add(newvec);
                            }
                        }
                    }
                }
            }
        }
       
        if (resultList.Count == 1)
            return resultList[0];
        else
            return null;
        
    }

    private int Redirection(int _row, int _col)
    {
        
        int[] dirX = { 0, 0, 1, -1 };
        int[] dirY = { 1, -1, 0, 0 };

        int resultDir = 0;
        for (int i = 0; i < 4; i++)
        {
            if (TileDataManager.instance.GetTileType(_row + dirX[i], _col + dirY[i]) == 1)
            {
                if (i == 0)
                {
                    resultDir = (int)Dir.Down;
                    //Debug.Log(resultDir);
                    break;
                }
                else if (i == 1)
                {
                    resultDir = (int)Dir.Up;
                    //Debug.Log(resultDir);
                    break;
                }
                else if (i == 2)
                {
                    resultDir = (int)Dir.Left;
                    //Debug.Log(resultDir);
                    break;
                }
                else
                {
                    resultDir = (int)Dir.Right;
                    //Debug.Log(resultDir);
                    break;
                }
            }
        }
            return resultDir;
    }

    private void OnMouseDown()
    {
        SpawnUI.SetActive(true);
        
        SpawnUI.GetComponent<SpawnerUI>().controllingSpawn = this.gameObject;
        // UI ���� ������ sync.
        SpawnUI.GetComponent<SpawnerUI>().ReceiveSpiritSpawnInfo(SetUIInfo());

    }

    SpiritSpawnInfo SetUIInfo()
    {
        // SpiritSpawnInfo �ν��Ͻ� ���� �� ���� ä���
        SpiritSpawnInfo spawnInfo = new SpiritSpawnInfo();
        spawnInfo.SpawnerName = SpawnerName;
        spawnInfo.spawnDuration = SpawnDuration;
        spawnInfo.SpiritLv = spLv;
        spawnInfo.SpwnLv = spwLv;
        spawnInfo.slider = slider;
        return spawnInfo;
    }
    
}

public class SpiritSpawnInfo
{
    public float spawnDuration;
    public string SpawnerName;
    public int SpwnLv;
    public int SpiritLv;
    public Slider slider;
}