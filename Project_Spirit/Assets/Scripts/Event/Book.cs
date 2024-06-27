using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Book : MonoBehaviour
{
    [SerializeField]
    private GameObject bookEventUI;
    private GameObject bookOpenimg;
    [SerializeField]
    private GameObject playerCamera;

    // Start is called before the first frame update
    void Start()
    {
        bookEventUI = GameObject.Find("BookEvent");
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMouseDown()
    {
       // 1. ������ ���̾ �򸰴�. v
       bookEventUI.SetActive(true);
       // 2. ī�޶� ������ ������ å�� �߽����� ��
       //playerCamera.transform.position = transform.position;
        // 3. ī�޶� �� �ƿ� ȿ�� 11 => 14�� ����
        GameObject.FindAnyObjectByType<Camera>().orthographicSize = 14f;
        // 4. ������ ���� Ÿ���� �� Ÿ�Ϸ� �ٲ�
        int ypos = (int)transform.position.y;
        int xpos = (int)transform.position.x;
        TileDataManager.instance.SetTileType(xpos, ypos, 0);

        // 5. �Ͻ����� ����
        Time.timeScale = 0f;
        // 6. 0.6�� ���Ŀ� ������ ���̾� ���� å�� ����
        StartCoroutine(ShowBookText());
        // 7. ������ �����ش�.
        // 8. esc ���� �ڷ� ���� �� �ְ� ��
        // 9. ������ ����Ʈ â�� �߰� ��
        
        

    }

    IEnumerator ShowBookText()
    {
        yield return new WaitForSeconds(0.8f);
        SetActiveRecursively(bookEventUI, true);

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
