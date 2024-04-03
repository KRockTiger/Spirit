using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DetectMove : MonoBehaviour
{
    [SerializeField]
    Button button;
    [SerializeField]
    Sprite[] sprites;
    [SerializeField]
    Vector2Int bottomLeft, topRight, startPos, targetPos;
    [SerializeField]
    int CurposX = 2;
    [SerializeField]
    int CurposY = 1;
    
    Node[,] nodes;  // TileDataManager instance.

    // �ٶ󺸴� ���� ���� ��ǥ ��ȭ.  
    int[] frontX = { 0, -1, 0, 1 };  // Up , Left, Down, Right
    int[] frontY = { 1, 0, -1, 0 };

    int[] leftX = { -1, 0, 1, 0 }; // ��ȸ�� ���� ��ġ��
    int[] leftY = { 0, -1, 0, 1 }; // ��ȸ�� ���Ŀ� ���� �ٶ󺸴� ������ �ݽð�������� 90�� ȸ��

    int[] rightX = { 1, 0, -1, 0 };  // ��ȸ�� ���� ��ġ��
    int[] rightY = { 0, 1, 0, -1 };

    public float moveSpeed = 1f;
    int _dir = (int)Dir.Up;
    Vector2 currentPosition;

    enum Dir
    {
        Up = 0,
        Left = 1,
        Down = 2,
        Right = 3
    }

    enum Detect
    {
        None,
        Search,
        Move,
        Sign
    }
    Detect detection = Detect.Search;

    private void Start()
    {
        nodes = TileDataManager.instance.GetNodes();
        int startX = (int)startPos.x - (int)bottomLeft.x;
        int startY = (int)startPos.y - (int)bottomLeft.y;
    }
    private void Update()
    {
        currentPosition = new Vector2(transform.position.x, transform.position.y);
        //MoveNext(CurposX, CurposY);
        switch (detection)
        {
            case Detect.None:
                break;
            case Detect.Search:
                checkDirection();
                break;
            case Detect.Move:
                MoveNext(CurposX, CurposY);
                break;
            case Detect.Sign:
                break;
        }
    }
    public void checkDirection()
    {
        nodes = TileDataManager.instance.GetNodes();    // �ſ� �ſ� �߿�!! �ڵ� ����ȭ
                                                        //Debug.Log(TileDataManager.instance.nodes[1, 1].x);
                                                        //Debug.Log("�����ϰ� �ҷ�����.. " + nodes[1, 1].x);

        int leftx = CurposX + leftX[_dir];
        int lefty = CurposY + leftY[_dir];
        int frontx = CurposX + frontX[_dir];
        int fronty = CurposY + frontY[_dir];
        int rightx = CurposX + rightX[_dir];
        int righty = CurposY + rightY[_dir];

        if (leftx < TileDataManager.instance.sizeX && leftx >= 0 && lefty < TileDataManager.instance.sizeY && lefty >= 0)
        {
            //  1. ���� �ٶ󺸴� �������� �������� �������� �� �� �ִ��� Ȯ��.
            if (nodes[leftx, lefty].nodeSprite == GetdetectSprite(0))
            {
                Debug.Log("���ʿ� Ground�� �ֽ��ϴ�.");
                // ���� �������� 90�� ȸ�� 
                _dir = (_dir + 1) % 4;

                CurposX += frontX[_dir];
                CurposY += frontY[_dir];
                detection = Detect.Move;
                return;
                Debug.Log("���� ����� ������ �ȵǴµ�");
            }
        }
        if (frontx < TileDataManager.instance.sizeX && frontx >= 0 && fronty < TileDataManager.instance.sizeY && fronty >= 0)
        {
            Debug.Log("else if�� ����?");
            // 2. ���� �ٶ󺸴� ������ �������� ������ �� �ִ��� Ȯ��.
            if (nodes[frontx, fronty].nodeSprite == GetdetectSprite(0))
            {
                Debug.Log("���ʿ� Ground�� �ֽ��ϴ�.");
                 CurposX += frontX[_dir];  
                 CurposY += frontY[_dir] ;
                 detection = Detect.Move;
                return;
                Debug.Log("������ ����� ������ �ȵǴµ�");
            }
        }
        if (rightx < TileDataManager.instance.sizeX && rightx >= 0 && righty < TileDataManager.instance.sizeY && righty >= 0)
        {
            // 2. ���� �ٶ󺸴� ������ �������� ������ �� �ִ��� Ȯ��.
            if (nodes[rightx, righty].nodeSprite == GetdetectSprite(0))
            {
                Debug.Log("���ʿ� Ground�� �ֽ��ϴ�.");
                // ���� �����̶�� 90�� ȸ��
                _dir = (_dir - 1 + 4) % 4;

                 CurposX += frontX[_dir];  
                 CurposY += frontY[_dir]; 
                 detection = Detect.Move;
                return;
                Debug.Log("������ ����� ������ �ȵǴµ�");
            }
        }
        else
            Debug.Log("�ƹ��͵� �ȵ���!");
        return;
    }



    private Sprite GetdetectSprite(int _num)
    {   // Ground sprite�϶�.
        if (_num == 0)
            return sprites[0];
        else
            return null;
    }

    private void MoveNext(int _curposx, int _curposy)
    {
        Vector2 targetVector = new Vector2(_curposx, _curposy);
        Vector2 direction = (targetVector - currentPosition).normalized;

        if(direction.magnitude >= 0.001f)
        {
            Vector2 movement = direction.normalized * moveSpeed * Time.deltaTime;

            transform.Translate(movement);
        }
        else
        {
            transform.position = targetVector;
            if(direction.magnitude == 0)
            {
                detection = Detect.Search;
            }
        }
    }
}
