using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BookEvent : MonoBehaviour
{
    [SerializeField]
    private GameObject bookPrefab;
    [SerializeField]
    private GameObject bookEventUI;
    [SerializeField]
    private Sprite HotbookImage;

    private bool eventhasoccured;
    private bool Hothasoccured;
    public bool Hoteventhasoccured;
    bool bookSpawned = false;
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
                GameObject.FindAnyObjectByType<Camera>().orthographicSize = 11f;

                // ������ ����Ʈ â ������ �Ѵ�.
            }
        }

        if(Hothasoccured)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                Time.timeScale = 1f;
                bookEventUI.SetActive(false );
                GameObject.FindAnyObjectByType<Camera>().orthographicSize = 11f;
            }
        }
    }

    // BookEvent Ȱ��ȭ ��ư
    public void BookEventTrigger()
    {
        nodes = TileDataManager.instance.GetNodes();

        bool validLocation = true;

        if (bookSpawned) return;

        int x = UnityEngine.Random.Range(2, 100);
        int y = UnityEngine.Random.Range(2, 100);
        if (TileDataManager.instance.GetTileType(x, y) == 3)
        {

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
                    SoundManager.instance.BookDrop(0);
                }
            }
            
           if(validLocation)
           {
               Instantiate(bookPrefab, new Vector3(x + 0.5f, y + 0.5f, 0), Quaternion.identity);
                bookSpawned = true;
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
        SoundManager.instance.UIButtonclick();
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

    public void WeatherHotEvent()
    {
        bookEventUI.SetActive(true);
        Hoteventhasoccured = true;
        foreach(Transform transform in bookEventUI.transform)
        {
            if(transform.gameObject.name == "BookOpen_img")
            {
                transform.gameObject.GetComponent<Image>().sprite = HotbookImage;
            }
        }
        StartCoroutine(ShowWarnHotText());
        Time.timeScale = 0.01f;
    }


    IEnumerator ShowBookText()
    {
        yield return new WaitForSecondsRealtime (0.8f);
        SetActiveRecursively(bookEventUI, true);

        yield return new WaitForSecondsRealtime(7f);
       eventhasoccured = true;
    }

    IEnumerator ShowWarnHotText()
    {
        yield return new WaitForSecondsRealtime(0.8f);
        SetActiveRecursively(bookEventUI, true);

        yield return new WaitForSecondsRealtime(7f);
        Hothasoccured = true;
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
