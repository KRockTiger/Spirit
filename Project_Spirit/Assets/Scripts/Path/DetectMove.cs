using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
    Signal signal = new Signal();

    // �ٶ󺸴� ���� ���� ��ǥ ��ȭ.  
    int[] frontX = { 0, -1, 0, 1 };  // Up , Left, Down, Right
    int[] frontY = { 1, 0, -1, 0 };

    int[] leftX = { -1, 0, 1, 0 }; // ��ȸ�� ���� ��ġ��
    int[] leftY = { 0, -1, 0, 1 }; // ��ȸ�� ���Ŀ� ���� �ٶ󺸴� ������ �ݽð�������� 90�� ȸ��

    int[] rightX = { 1, 0, -1, 0 };  // ��ȸ�� ���� ��ġ��
    int[] rightY = { 0, 1, 0, -1 };

    public float moveSpeed = 1f;
    int _dir = (int)Dir.Up;
    int nxtposX, nxtposY;
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
        CheckTile,
        Normal,
        Move,
        Factory,
        Loot,
        Academy,
        Sign
    }
    Detect detection = Detect.None;

    private void Start()
    {
        nodes = TileDataManager.instance.GetNodes();
        signal = GameObject.Find("[SignalManager]").GetComponent<Signal>();
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
                detection = Detect.CheckTile;
                break;
            case Detect.CheckTile:
                CheckTile();
                break;
            case Detect.Normal:
                NormalDirection();
                break;
            case Detect.Factory:
                break;
            case Detect.Loot:
                break;
            case Detect.Academy:
                break;
            case Detect.Move:
                MoveNext(CurposX, CurposY);
                break;

            case Detect.Sign:
                break;
        }
    }


    private void CheckTile()
    {   // ǥ�� 0
        nodes = TileDataManager.instance.GetNodes();
        string callSign = nodes[CurposX, CurposY].nodeSprite.name;
        int num = ExtractNumber(callSign);  // Sign type �Ǻ�.
        if(nodes[CurposX, CurposY].isSignal)
        {   
            signal.SetSignType(num, nodes[CurposX,CurposY].rotation, _dir); // signal �� sign Ÿ���� �����ϰ� dir ������ ����
            Debug.Log("üũŸ���� ������ ��� ���� ��Ȳ���̴�?");
            // ���� Dir = ��ȣ Dir�� ���ٸ�,
            if(_dir == signal.dir)
            {
                Debug.Log("ǥ���� ������ ������ ��ġ�ؿ�!");
                CurposX += signal.pair.Item1;
                CurposY += signal.pair.Item2;

                // ǥ���������� ������ ������ ���������
                _dir = signal.spiritDir;

                // ���� ������ // return
                detection = Detect.Move;
            }
            else
            {

                detection = Detect.Normal;
            }
        }
        else
        {
            detection = Detect.Normal;
        }

    }


    private void NormalDirection()
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
            if (nodes[leftx, lefty].isWalk)
            {
                // ���� �������� 90�� ȸ�� 
                _dir = (_dir + 1) % 4;

                CurposX += frontX[_dir];
                CurposY += frontY[_dir];
                detection = Detect.Move;
                return;
                
            }
        }
        if (frontx < TileDataManager.instance.sizeX && frontx >= 0 && fronty < TileDataManager.instance.sizeY && fronty >= 0)
        {
             // 2. ���� �ٶ󺸴� ������ �������� ������ �� �ִ��� Ȯ��.
            if (nodes[frontx, fronty].isWalk)
            {  
                CurposX += frontX[_dir];  
                 CurposY += frontY[_dir] ;
                 detection = Detect.Move;
                return;
              
            }
        }
        if (rightx < TileDataManager.instance.sizeX && rightx >= 0 && righty < TileDataManager.instance.sizeY && righty >= 0)
        {
            // 2. ���� �ٶ󺸴� ������ �������� ������ �� �ִ��� Ȯ��.
            if (nodes[rightx, righty].isWalk)
            {
                // ���� �����̶�� 90�� ȸ��
                _dir = (_dir - 1 + 4) % 4;

                 CurposX += frontX[_dir];  
                 CurposY += frontY[_dir]; 
                 detection = Detect.Move;
                return;
                
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
                detection = Detect.None;
            }
        }
    }

    static int ExtractNumber(string input)
    {
        // ����ǥ������ ����Ͽ� ���ڸ� ����
        Match match = Regex.Match(input, @"\d+");

        // ����� ���ڰ� �ִ��� Ȯ���ϰ� ��ȯ
        if (match.Success)
        {
            return int.Parse(match.Value);
        }
        else
        {
            // ���ڰ� ���� ��� ����ó�� �Ǵ� �⺻�� ��ȯ
             return 100;
        }
    }
}
