using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BookEvent : MonoBehaviour
{
    [SerializeField]
    private GameObject bookPrefab;
    [SerializeField]
    private GameObject bookEventUI;

    private bool eventhasoccured;

    Node[,] nodes;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(eventhasoccured)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                // ���� �ð����� �ǵ��ƿ�
                Time.timeScale = 1f;
                bookEventUI.SetActive(false);
                
                // ������ ����Ʈ â ������ �Ѵ�.
            }
        }
    }

    // BookEvent Ȱ��ȭ ��ư
    public void BookEventTrigger()
    {
        nodes = TileDataManager.instance.GetNodes();

        int x = UnityEngine.Random.Range(2, 100);
        int y = UnityEngine.Random.Range(2, 100);
        if (TileDataManager.instance.GetTileType(x, y) == 3)
        {
            bool validLocation = true;

           for(int i = 0; i < 5; i++)
            {
                for(int j = 0; j < 5; j++)
                {
                    if (TileDataManager.instance.GetTileType(x + i, y + j) == 1)
                    {
                        validLocation = false;
                        break;
                    }
                   
                }
                if(!validLocation)
                {
                    Debug.Log("å�� ��ȯ�� �� ���� ��ġ�Դϴ�.");
                    BookEventTrigger();
                }
            }
            
           if(validLocation)
           {
               Instantiate(bookPrefab, new Vector3(x + 0.5f, y + 0.5f, 0), Quaternion.identity);
                return;
           }
        }
        else
        {
            BookEventTrigger();
        }    



    }


    // å ������ �� ����Ǵ� �޼���.
    public void BookMouseOn()
    {
        bookEventUI.SetActive(true);

        // 2. ī�޶� ������ ������ å�� �߽����� ��
        //playerCamera.transform.position = transform.position;
        // 3. ī�޶� �� �ƿ� ȿ�� 11 => 14�� ����
        GameObject.FindAnyObjectByType<Camera>().orthographicSize = 14f;
        // 4. ������ ���� Ÿ���� �� Ÿ�Ϸ� �ٲ�
        int ypos = (int)transform.position.y;
        int xpos = (int)transform.position.x;
        TileDataManager.instance.SetTileType(xpos, ypos, 0);

        // 5. 0.6�� ���Ŀ� ������ ���̾� ���� å�� ����
        StartCoroutine(ShowBookText());
        // 6. �Ͻ����� ����
        Time.timeScale = 0.01f;
        // 7. ������ �����ش�.

        // 8. esc ���� �ڷ� ���� �� �ְ� ��
        
        //if(Input.GetButton)
        // 9. ������ ����Ʈ â�� �߰� ��



    }

    IEnumerator ShowBookText()
    {
        yield return new WaitForSecondsRealtime (0.8f);
        SetActiveRecursively(bookEventUI, true);

        yield return new WaitForSecondsRealtime(7f);
        eventhasoccured = true;
    }

    private void SetActiveRecursively(GameObject obj, bool state)
    {
        obj.SetActive(state);
        foreach (Transform child in obj.transform)
        {
            child.gameObject.SetActive(state);
        }
    }

}
