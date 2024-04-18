using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ResouceManager : MonoBehaviour
{
    public List<List<KeyValuePair<Vector2Int, int>>> WoodAllpaired = new List<List<KeyValuePair<Vector2Int, int>>>();
    public List<List<KeyValuePair<Vector2Int, int>>> RockAllpaired = new List<List<KeyValuePair<Vector2Int, int>>>();
    public List<KeyValuePair<Vector2Int, int>> wpariedCoordinates = new List<KeyValuePair<Vector2Int, int>>();
    public List<KeyValuePair<Vector2Int, int>> rpariedCoordinates = new List<KeyValuePair<Vector2Int, int>>();
    GameObject[] RockSprite;
    GameObject[] WoodSprite;

    [HideInInspector]
    public List<GameObject> WoodObjects;
    [HideInInspector]
    public List<GameObject> RockObjects;
    [HideInInspector]
    public bool resourceDeployed = false;
    Node[,] nodes;
    float IncreasingTime = 1f;

    private void Update()
    {
        if (resourceDeployed)
        {
            IncreasingTime -= Time.deltaTime;
            if (IncreasingTime <= 0)
            {
                IncresementRockValue();
                IncreasingTime = 0.1f;
               
            }
        }
    }
    private void Start()
    {
        RockSprite = GetComponent<ResourceDeployment>().RockSprite;
        WoodSprite = GetComponent<ResourceDeployment>().WoodSprite;
    }
    void IncresementRockValue()
    {
        int randomlyselectednum = UnityEngine.Random.Range(0, 4);

        List<KeyValuePair<Vector2Int, int>> selectedPairedList = RockObjects[randomlyselectednum].GetComponent<ResourceBuilding>().resourceBuilding;
        Vector2Int minCoord = selectedPairedList[0].Key;
        int minvalue = selectedPairedList[0].Value;
        foreach (KeyValuePair<Vector2Int, int> pair in selectedPairedList)
        {
            int value = pair.Value;
            if (value < minvalue)
            {
                minvalue = value;
                minCoord = pair.Key;
            }

        }

        if (minvalue < 50)
        {
            //Debug.Log("is 50�� �ƴ�");
            // ������ �� �Ҵ�
            KeyValuePair<Vector2Int, int> updatedPair = new KeyValuePair<Vector2Int, int>(minCoord, minvalue + 1);
            for (int i = 0; i < selectedPairedList.Count; i++)
            {
                if (selectedPairedList[i].Key == minCoord)
                {
                    selectedPairedList.RemoveAt(i);
                    break;
                }
            }
            selectedPairedList.Add(updatedPair);
            RelocateTileasRock(minCoord, updatedPair.Value, RockObjects[randomlyselectednum]);
           
           
        }
        else if (minvalue == 50)
        {
           // Debug.Log("Over 50 resource has occured!");
            Vector2Int ValidLocation = FindNewTileisPossible(selectedPairedList);
            TileDataManager.instance.SetTileType(ValidLocation.x, ValidLocation.y, 6);
            nodes = TileDataManager.instance.GetNodes();
            nodes[ValidLocation.x, ValidLocation.y].resourceBuilding = RockObjects[randomlyselectednum].GetComponent<ResourceBuilding>();
            nodes[ValidLocation.x, ValidLocation.y].resourceBuilding.UpdateFieldStatus(1);
            nodes[ValidLocation.x, ValidLocation.y].SetNodeType(6);
            

            KeyValuePair<Vector2Int, int> newPair = new KeyValuePair<Vector2Int, int>(ValidLocation, 1);
            selectedPairedList.Add(newPair);
        }
    }

    void RelocateTileasRock(Vector2Int minCoord, int _updateValue, GameObject _setparents)
    {
        Vector3 col = new Vector3(minCoord.x + 0.5f, minCoord.y + 0.5f, 0);
        Collider[] colliders = Physics.OverlapSphere(col, 0.2f);
        foreach (Collider collider in colliders)
        {
            Destroy(collider.gameObject);
        }

        if (_updateValue > 0 && _updateValue < 10)
        {
            GameObject obj = Instantiate(RockSprite[0], col, Quaternion.identity);
            obj.transform.SetParent(_setparents.transform);
        }
        else if (_updateValue < 20)
        {
            GameObject obj = Instantiate(RockSprite[1], col, Quaternion.identity);
            obj.transform.SetParent(_setparents.transform);
        }
        else if (_updateValue < 30)
        {
            GameObject obj = Instantiate(RockSprite[2], col, Quaternion.identity);
            obj.transform.SetParent(_setparents.transform);
        }
        else if (_updateValue < 40)
        {
            GameObject obj = Instantiate(RockSprite[3], col, Quaternion.identity);
            obj.transform.SetParent(_setparents.transform);
        }
        else
        {
            GameObject obj = Instantiate(RockSprite[4], col, Quaternion.identity);
            obj.transform.SetParent(_setparents.transform);
        }
    }

    
    Vector2Int FindNewTileisPossible(List<KeyValuePair<Vector2Int, int>> findValue)
    {
        Vector2Int newLocation = Vector2Int.zero;
        foreach (KeyValuePair<Vector2Int, int> pair in findValue)
        {
            Vector2Int implicitPair = pair.Key;

            bool conditionMet = false;

            // �� ���⿡ ���� �õ�
            for (int direction = 0; direction < 4; direction++)
            {
                int randomDirection = direction; 

                // ���ǿ� ���� ������ �����մϴ�.
                switch (randomDirection)
                {
                    case 0:
                        implicitPair.y += 1;
                        break;
                    case 1:
                        implicitPair.y -= 1;
                        break;
                    case 2:
                        implicitPair.x += 1;
                        break;
                    case 3:
                        implicitPair.x -= 1;
                        break;
                }

                bool innercondition = false;
                // ������ �����ϴ��� Ȯ���մϴ�.
                for (int i = 1; i <= 8; i++)
                {
                    if (TileDataManager.instance.GetTileType(implicitPair.x, implicitPair.y) == i)
                    {
                        innercondition = true;
                        break;
                    }
                    else
                        conditionMet = true;
                }

                if(innercondition)
                {
                    break;
                }
                // ������ �����ϸ� ������ �����մϴ�.
                if (conditionMet)
                {
                    // ���ο� �� ã��!
                    newLocation = implicitPair;

                    if (ValidLocationisNearbyPath(newLocation))
                    {
                        newLocation = implicitPair;
                        //Debug.Log("���� ã�� Location : " + newLocation);
                        return newLocation;
                    }
                    else
                        continue;
                }

                // ������ �������� ������ ���� ������ �õ��մϴ�.
                implicitPair = pair.Key; // ���� ��ġ�� �ǵ������� ���� ������ �õ��մϴ�.
            }

            // ��� ������ �������� �� �����ϸ� ���� pair
            if (!conditionMet)
            {
                continue; // ���� pair�� �̵��մϴ�.
            }

        }

        return newLocation;
    }

    bool ValidLocationisNearbyPath(Vector2Int _validLocation)
    {
        Vector2Int checkingValid = _validLocation;
        for(int direction =  0; direction < 4; direction++)
        {
            int NSEW = direction;
            switch(NSEW)
            {
                case 0:
                    checkingValid.y += 1;
                    break;
                case 1:
                    checkingValid.y -= 1;
                    break;
                case 2: 
                    checkingValid.x += 1;
                    break;
                case 3:
                    checkingValid.x -= 1;
                    break;

            }

            if (TileDataManager.instance.GetTileType(checkingValid.x, checkingValid.y) == 3)
            {
                return false;
            }
            
        }
        return true;
    }

}
