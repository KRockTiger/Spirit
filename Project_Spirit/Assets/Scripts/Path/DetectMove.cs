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
    Text stopduration;
    [SerializeField]
    Vector2Int bottomLeft, topRight, startPos, targetPos;
    
    Node[,] nodes;  // TileDataManager instance.
    Signal signal = new Signal();
    Vector2 currentPosition;
    SpriteRenderer spriteRenderer;
    List<Tuple<Vector2Int, Vector2Int>> buildingList = new List<Tuple<Vector2Int, Vector2Int>>(); // �ǹ� ����, ���ϴ� ��ǥ ����.

    public int CurposX = 2;
    public int CurposY = 1;
    // �ٶ󺸴� ���� ���� ��ǥ ��ȭ.  
    int[] frontX = { 0, -1, 0, 1 };  // Up , Left, Down, Right
    int[] frontY = { 1, 0, -1, 0 };

    int[] leftX = { -1, 0, 1, 0 }; // ��ȸ�� ���� ��ġ��
    int[] leftY = { 0, -1, 0, 1 }; // ��ȸ�� ���Ŀ� ���� �ٶ󺸴� ������ �ݽð�������� 90�� ȸ��

    int[] rightX = { 1, 0, -1, 0 };  // ��ȸ�� ���� ��ġ��
    int[] rightY = { 0, 1, 0, -1 };

    public float moveSpeed = 1f;
    int _dir = (int)Dir.Up;
    int spiritID;
    int signType;
    public int spiritElement;
   
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
        Factory_MoveMent,
        Basic_MoveMent,
        Move,
        Stop,
        Factory,
        Loot,
        Academy,
        Mark_Check
    }
    Detect detection = Detect.None;

    private void Start()
    {
        nodes = TileDataManager.instance.GetNodes();
        signal = GameObject.Find("[SignalManager]").GetComponent<Signal>();
        spiritID = GetComponent<Spirit>().SpiritID;
        spiritElement = GetComponent<Spirit>().SpiritElement;
        spriteRenderer = GetComponent<SpriteRenderer>();
        int startX = (int)startPos.x - (int)bottomLeft.x;
        int startY = (int)startPos.y - (int)bottomLeft.y;
    }
    private void Update()
    {  
        switch (detection)
        {
            case Detect.None:
                detection = Detect.CheckTile;
                break;
            case Detect.CheckTile:
                CheckTile();
                break;
            case Detect.Factory_MoveMent:
                FactoryMove();
                break;
            case Detect.Basic_MoveMent:
                BaseMove();
                break;
            case Detect.Factory:
                FactoryWork();
                break;
            case Detect.Loot:
                break;
            case Detect.Academy:
                break;
            case Detect.Move:
                Move(CurposX, CurposY);
                break;
            case Detect.Mark_Check:
                MarkCheck(signType);
                break;
            case Detect.Stop:
                StopMove();
                break;
        }
    }


    private void CheckTile()
    {   // ǥ�� 0
        nodes = TileDataManager.instance.GetNodes();

        // ���� ���� �� �ִ� Ÿ���϶�.
        if (nodes[CurposX, CurposY].isWalk)
        {
            nodes[CurposX, CurposY].spiritElement = spiritElement;  // ��忡 ���ɿ��� üũ
            string signName = nodes[CurposX, CurposY].nodeSprite.name;
            signType = ExtractNumber(signName);  // Sign type �Ǻ�.
            
            if (nodes[CurposX, CurposY].isSignal)
            {
                detection = Detect.Mark_Check;
                
            }
            
            else
            {
                detection = Detect.Basic_MoveMent;
            }


        }
    }


    private void BaseMove()
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
                // ������ Ÿ�Ͽ� ���� ������ �ִٸ�.
                if (nodes[leftx, lefty].spiritElement == spiritElement) return;
                // ���� Ȥ�� �ڿ� ä���ϴ� ���̶��.. �� ���ִٰ� �ϸ�...
                
                nodes[CurposX, CurposY].spiritElement = 0;
                // ���� �������� 90�� ȸ�� 
                _dir = (_dir + 1) % 4;

                CurposX += frontX[_dir];
                CurposY += frontY[_dir];

                // if(nodes[curposx,curposy].isFactory) detection = Detect.EnterFactory
                detection = Detect.Move;
                return;
            }
        }
        if (frontx < TileDataManager.instance.sizeX && frontx >= 0 && fronty < TileDataManager.instance.sizeY && fronty >= 0)
        {
             // 2. ���� �ٶ󺸴� ������ �������� ������ �� �ִ��� Ȯ��.
            if (nodes[frontx, fronty].isWalk)
            {
                if (nodes[frontx, fronty].spiritElement == spiritElement) return;

                nodes[CurposX, CurposY].spiritElement = 0;
                CurposX += frontX[_dir];  
                 CurposY += frontY[_dir] ;

                // if(nodes[curposx,curposy].isFactory) detection = Detect.EnterFactory
                detection = Detect.Move;
                return;
            }
        }
        if (rightx < TileDataManager.instance.sizeX && rightx >= 0 && righty < TileDataManager.instance.sizeY && righty >= 0)
        {
            // 2. ���� �ٶ󺸴� ������ �������� ������ �� �ִ��� Ȯ��.
            if (nodes[rightx, righty].isWalk)
            {
                if (nodes[rightx, righty].spiritElement == spiritElement) return;
                nodes[CurposX, CurposY].spiritElement = 0;
                // ���� �����̶�� 90�� ȸ��
                _dir = (_dir - 1 + 4) % 4;

                 CurposX += frontX[_dir];  
                 CurposY += frontY[_dir];

                // if(nodes[curposx,curposy].isFactory) detection = Detect.EnterFactory => �̵��Ҷ� �����ϴ� �ִϸ��̼� ������ �Լ�
                detection = Detect.Move;
                return;
                
            }
        }
        else
            Debug.Log("�ƹ��͵� �ȵ���!");
        return;
    }

    private void StopMove()
    {
    }
    private void MarkCheck(int _signType)
    {
        if (_signType == 7)
        {  
            float stopDuration = float.Parse(stopduration.text);
            StartCoroutine(StopSign(stopDuration));

            return;
        }
        
        else
            signal.SetSignType(_signType, nodes[CurposX, CurposY].rotation, _dir, CurposX, CurposY); // signal �� sign Ÿ���� �����ϰ� dir ������ ����
                                                                                           // ���� Dir = ��ȣ Dir�� ���ٸ�,
        
        if (_dir == signal.dir)
        {

            CurposX += signal.pair.Item1;
            CurposY += signal.pair.Item2;

            // ǥ���������� ������ ������ ������.
            _dir = signal.spiritDir;

            // Ÿ�� üũ�� ���� �� ���ٸ�..
            if (!nodes[CurposX, CurposY].isWalk)
            {

                detection = Detect.None;
            }
            else
            {
                // ���� ĭ�� ���� Ȥ�� �ڿ� ä���ϴ� ���̶��..
                detection = Detect.Move;
            }
        }
        else
        {

            detection = Detect.Basic_MoveMent;
        }
    }

    private void FactoryMove()
    {
        
    }

    private void Move(int _curposx, int _curposy)
    {
        if(detection != Detect.Move) { return; }
        Vector2 targetVector = new Vector2(_curposx, _curposy);
        Vector2 direction = (targetVector - (Vector2)transform.position).normalized;

        if(Vector2.Distance(targetVector, transform.position) <= 0.01f)
        {
            transform.position = targetVector;
            detection = Detect.None;
            return;
            //if(direction.magnitude == 0)
        }
        else
        {
            Vector2 movement = direction.normalized * moveSpeed * Time.smoothDeltaTime;
             
            transform.Translate(movement);
            //{
            //}
        }
    }

    private void FactoryWork()
    {
        // ���ϴ� �ð���ŭ spriterenderer = null
        float workatFactory = 10f;
        while(workatFactory <= 0)
        {
            spriteRenderer.enabled = false;
            workatFactory -= Time.deltaTime;
        }
        spriteRenderer.enabled = true;
        workatFactory = 10f;
        // �̵� ���� ����
        detection = Detect.None;
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

    IEnumerator StopSign(float _time)
    {
        yield return new WaitForSeconds(_time);
        Debug.Log("���� ȣ��Ǿ�����");
        detection = Detect.Basic_MoveMent;
        Debug.Log("detection�� ���´� " + detection);
    }
}
