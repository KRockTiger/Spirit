using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class ResourceBuilding : MonoBehaviour, IPointerClickHandler
{
    public int Resource_reserves;
    public int yOffset = 2;
    public List<KeyValuePair<Vector2Int, int>> resourceBuilding;
    public Tuple<Vector2Int, Vector2Int> connectedRoads;

    public GameObject[] RockObject;
    public GameObject[] WoodObject;
    public Camera mainCamera;

    [SerializeField]
    public bool CanUse = false;
    bool resourceWayTooltip = false;

    public List<GameObject> ResourcegameObjectList;
    ResouceManager resourceManager;
    SoundManager soundManager;
    Vector2Int firstKey;

    int decreasedamount = 0;
    enum ResourceType
    {
        None = 0,
        Rock = 1,
        Wood = 2,
        Elemetntal = 3
    }

    ResourceType resourceType = ResourceType.None;

    public void SetResourceType(int type)
    {
        resourceType = (ResourceType)type;
    }

    public int GetResourceType()
    {
        return (int)resourceType;
    }

    private void Start()
    {
        InitializeResourceManger(); // => �ڿ� ���� �ʱ�ȭ
        mainCamera = Camera.main;
        soundManager = GameObject.Find("AudioManager").GetComponent<SoundManager>();

    }
    private void Update()
    {
        if (resourceBuilding != null)
        {
            firstKey = default(Vector2Int);
            foreach (var pair in resourceBuilding)
            {
                firstKey = pair.Key;
                break;
            }
            foreach (KeyValuePair<Vector2Int, int> pair in resourceBuilding)
            {
                int pairX = pair.Key.x;
                int pairY = pair.Key.y;
                TileDataManager.instance.nodes[pairX, pairY].resourceBuilding = this;
            }
            Tuple<Vector2Int, Vector2Int> TwoRoads = isTwoRoadAttachedResource();
            if (TwoRoads != null) SetConnectedRoad(TwoRoads);
            CalculateTotalamountOfResoucre();

        }
        ResourcegameObjectList.RemoveAll(item => item == null);

        // �ڿ� ����, ����ǥ ������
        WarningContent();
    }

    #region �ڿ� �ѷ� ���.
    void CalculateTotalamountOfResoucre()
    {
        int total = 0;
        foreach (KeyValuePair<Vector2Int, int> pair in resourceBuilding)
        {
            int value = pair.Value;
            total += value;

        }
        Resource_reserves = total;
        if (Resource_reserves <= 0)
        {
            resourceType = ResourceType.None;
            foreach (KeyValuePair<Vector2Int, int> pair in resourceBuilding)
            {
                ResetTileType(pair.Key.x, pair.Key.y, 0);

            }
            //  gameObjectList.Clear();
            Destroy(this.gameObject);
        }
    }
    void ResetTileType(int x, int y, int typeNum)
    {
        TileDataManager.instance.SetTileType(x, y, typeNum);
        TileDataManager.instance.nodes[x, y].SetNodeType(typeNum);
        TileDataManager.instance.nodes[x, y].isWalk = false;
    }

    #endregion

    #region ���� �ڿ� �Ҹ� ����.
    public void GetDecreasement(int num)
    {
        DecreaseLeastColony(num);           // �ڿ� Ÿ�� �ʱ�ȭ
        CalculateTotalamountOfResoucre();   // �ڿ� �� ���� ���
        Addamount(num);                     // ResouceManager �� ���ϱ�
        //Debug.Log(Resource_reserves);
    }
    void DecreaseLeastColony(int num)
    {
        // Debug.Log(Resource_reserves);
        decreasedamount += num;
        if (decreasedamount % resourceBuilding.Count == 0)
        {
            int tempResourceamount = Resource_reserves / resourceBuilding.Count;

            foreach (KeyValuePair<Vector2Int, int> pair in resourceBuilding)
            {
                RelocateTile(pair.Key, tempResourceamount, TileType());
            }
            decreasedamount = 0;
        }

        int randomIndex = UnityEngine.Random.Range(0, resourceBuilding.Count);
        // �����ϴ� ���� 0�϶�, Ÿ�� ��� ���Ҵ�.
        if (resourceBuilding[randomIndex].Value <= 0)
        {
            randomIndex = UnityEngine.Random.Range(0, resourceBuilding.Count);
        }
        Vector2Int randPos = resourceBuilding[randomIndex].Key;
        int posses = resourceBuilding[randomIndex].Value;
        KeyValuePair<Vector2Int, int> updatedPair = new KeyValuePair<Vector2Int, int>(randPos, posses - num);
        resourceBuilding.RemoveAt(randomIndex);
        resourceBuilding.Add(updatedPair);


    }
    // Ÿ�� �絿��ȭ
    GameObject[] TileType()
    {
        if (resourceType == ResourceType.Rock)
            return RockObject;
        else
            return WoodObject;
    }
    void RelocateTile(Vector2Int minCoord, int _updateValue, GameObject[] _gameobject)
    {
        Vector3 col = new Vector3(minCoord.x + 0.5f, minCoord.y + 0.5f, 0);
        Collider[] colliders = Physics.OverlapSphere(col, 0.2f);
        foreach (Collider collider in colliders)
        {
            Destroy(collider.gameObject);
        }

        if (_updateValue < 10)
        {
            GameObject obj = Instantiate(_gameobject[0], col, Quaternion.identity);
            obj.transform.SetParent(transform);
        }
        else if (_updateValue < 20)
        {
            GameObject obj = Instantiate(_gameobject[1], col, Quaternion.identity);
            obj.transform.SetParent(transform);
        }
        else if (_updateValue < 30)
        {
            GameObject obj = Instantiate(_gameobject[2], col, Quaternion.identity);
            obj.transform.SetParent(transform);
        }
        else if (_updateValue < 40)
        {
            GameObject obj = Instantiate(_gameobject[3], col, Quaternion.identity);
            obj.transform.SetParent(transform);
        }
        else
        {
            GameObject obj = Instantiate(_gameobject[4], col, Quaternion.identity);
            obj.transform.SetParent(transform);
        }
    }

    #endregion

    #region �ڿ� - �� ����

    public Tuple<Vector2Int, Vector2Int> GetConnectedRoad()
    {
        if (connectedRoads == null)
        {
            Debug.Log("���ΰ� 2���� �ƴմϴ�.");
            return null;
        }
        return connectedRoads;
    }

    void SetConnectedRoad(Tuple<Vector2Int, Vector2Int> connectedRoads)
    {
        this.connectedRoads = connectedRoads;
    }

    public void UpdateFieldStatus(int _type)
    {
        Tuple<Vector2Int, Vector2Int> TwoRoads = isTwoRoadAttachedResource();
        SetConnectedRoad(TwoRoads);
        resourceType = (ResourceType)_type;
    }

    Tuple<Vector2Int, Vector2Int> isTwoRoadAttachedResource()
    {
        List<Vector2Int> result = new List<Vector2Int>();
        foreach (KeyValuePair<Vector2Int, int> pair in resourceBuilding)
        {
            Node[,] nodes = TileDataManager.instance.GetNodes();
            int pairX = pair.Key.x;
            int pairY = pair.Key.y;

            int[] dx = { 0, 0, -1, 1 };
            int[] dy = { 1, -1, 0, 0 };

            for (int i = 0; i < 4; i++)
            {
                int newX = pairX + dx[i];
                int newY = pairY + dy[i];

                if (!result.Contains(new Vector2Int(newX, newY)) && TileDataManager.instance.GetTileType(pairX + dx[i], pairY + dy[i]) == 3)
                {
                    result.Add(new Vector2Int(pairX + dx[i], pairY + dy[i]));
                    foreach (Vector2Int var in result)
                    {
                        // Debug.Log(var);
                        //Debug.Log("�� ������ : " + result.Count);
                    }
                }

            }
        }

        if (result.Count == 2)
        {
            CanUse = true;
            return new Tuple<Vector2Int, Vector2Int>(result[0], result[1]);
        }
        else
        {
            CanUse = false;
            return null;
        }
    }

    #endregion

    public bool CheckForCapacity()
    {
        if (connectedRoads == null) return false;
        if(!CanUse) return false;
        if (ResourcegameObjectList.Count >= 0 && ResourcegameObjectList.Count < 4)
        {
            return true;
        }
        else
            return false;
    }

    public void AddWorkingSprit(GameObject _gameObject)
    {
        if (!ResourcegameObjectList.Contains(_gameObject))
        {
            ResourcegameObjectList.Add(_gameObject);
            _gameObject.GetComponent<Spirit>().TakeDamageOfResourceBuilding();
            _gameObject.GetComponent<DetectMove>().TimeforWorking = 2f;
            soundManager.BuildingOnbound(0);
        }
    }
    public void DeleteWorkingSprit(GameObject _gameObject)
    {
        if (_gameObject == null)
        {
            Debug.LogWarning("Trying to remove a null game object from the list.");
            return;
        }

        if (ResourcegameObjectList.Contains(_gameObject))
        {
            ResourcegameObjectList.Remove(_gameObject);
            soundManager.BuildingOnbound(1);
        }
    }

    private void InitializeResourceManger()
    {
        // GameManager ������Ʈ ã��
        GameObject gameManager = GameObject.Find("GameManager");

        if (gameManager != null)
        {
            Transform resourceManagerTransform = gameManager.transform.Find("[ResourceManager]");

            if (resourceManagerTransform != null)
            {
                resourceManager = resourceManagerTransform.GetComponent<ResouceManager>();

            }
        }
    }

    // ���� �ڿ� ä��
    public void Addamount(int num)
    {
        if (resourceType == ResourceType.Rock)
        {
            resourceManager.AddRock(num);
        }
        else
            resourceManager.AddTimer(num);

        if(QuestManager.instance.GainR)
        {
            QuestManager.instance.GainResource = true;
            // �ڿ� ���� �� Ȯ��
            QuestManager.instance.GainItem();
        }
    }

    void WarningContent()
    { 
        if (!CanUse)
        {
            // �ش� UI ������Ʈ�� prefab���� �����Ǿ� MaxPairX,Y �� ��ġ�� �����ǰ� �ؾ���. ���� �ش� prefab�� ��������, �ȳ� ��ħ���� �߰� ���� �ʿ�.
            if (!resourceWayTooltip)
            {
                GameObject tooltipUI = GameObject.Find("CraftManager");
                Vector3 worldPos = new Vector3(firstKey.x, firstKey.y, 0);
               
                Transform tooltipTransform = FindChildByName(gameObject.transform, "Circle");
                Transform tooltipoffTransform = FindChildByName(gameObject.transform, "Detail");
                // UI�� ��ũ�� ��ǥ��� �̵�
                tooltipTransform.position = new Vector3(firstKey.x + 0.5f, firstKey.y + 3.5f, 0);
                tooltipoffTransform.position = new Vector3(firstKey.x + 0.5f, firstKey.y + 2.8f, 0);
                resourceWayTooltip = true;
            }
        }
        else
        {
            resourceWayTooltip = false;
            Transform tooltipTransform = FindNotChildByName(gameObject.transform, "Circle");
            Transform tooltip2Transform = FindNotChildByName(gameObject.transform, "Detail");
            // UI�� ��ũ�� ��ǥ��� �̵�
            tooltipTransform.position = new Vector3(firstKey.x+0.5f, firstKey.y + 3.5f, 0);
        }
    }

    Transform FindChildByName(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
            {
                child.gameObject.SetActive(true);
                return child;
            }

        }
        return null;
    }

    Transform FindNotChildByName(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
            {
                child.gameObject.SetActive(false);
                return child;
            }

        }
        return null;
    }

    void ToggleObject(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
            {
                child.gameObject.SetActive(!child.gameObject.activeSelf);
            }
        }
    }
    public void WarningButtonDown()
    {
        ToggleObject(gameObject.transform.parent, "Detail");
        Debug.Log(" warningbutton clicked");
    }

    // UI ��Ұ� �տ� �ִ��� Ȯ���� �޼���
    private bool IsUIElementHovered()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // UI ��Ұ� �ִ� ��� Ŭ���� ����
        if (IsUIElementHovered())
        {
            Debug.Log("Click blocked due to UI element.");
            return; // Ŭ�� �̺�Ʈ�� ����
        }

        // Ŭ���� ���� ���
        Debug.Log("2D object clicked.");
    }
}

