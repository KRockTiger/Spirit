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
    SpriteRenderer spriteRenderer;
    
    public float CurposX = 2;
    public float CurposY = 1;
    // �ٶ󺸴� ���� ���� ��ǥ ��ȭ.  
    int[] frontX = { 0, -1, 0, 1 };  // Up , Left, Down, Right
    int[] frontY = { 1, 0, -1, 0 };

    int[] leftX = { -1, 0, 1, 0 }; 
    int[] leftY = { 0, -1, 0, 1 }; 

    int[] rightX = { 1, 0, -1, 0 };  
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
                Move((int)CurposX, (int)CurposY);
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
    {   
        nodes = TileDataManager.instance.GetNodes();

        if (nodes[(int)CurposX, (int)CurposY].isWalk)
        {
            // ��� ���ɿ��� Ȯ��.
            nodes[(int)CurposX, (int)CurposY].spiritElement = spiritElement;  
            string signName = nodes[(int)CurposX, (int)CurposY].nodeSprite.name;
            // Sign type �Ǻ�.
            signType = ExtractNumber(signName);  
            
            if (nodes[(int)CurposX, (int)CurposY].isSignal)
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
                                                        
        float leftx = CurposX + leftX[_dir];
        float lefty = CurposY + leftY[_dir];
        float frontx = CurposX + frontX[_dir];
        float fronty = CurposY + frontY[_dir];
        float rightx = CurposX + rightX[_dir];
        float righty = CurposY + rightY[_dir];

        if (leftx < TileDataManager.instance.sizeX && leftx >= 0 && lefty < TileDataManager.instance.sizeY && lefty >= 0)
        {
            //  1. ���� �ٶ󺸴� �������� �������� �������� �� �� �ִ��� Ȯ��.
            if (nodes[(int)leftx, (int)lefty].isWalk)
            {
                // ������ Ÿ�Ͽ� ���� ������ �ִٸ�.
                if (nodes[(int)leftx, (int)lefty].spiritElement == spiritElement) return;
                // ���� Ȥ�� �ڿ� ä���ϴ� ���̶��.. �� ���ִٰ� �ϸ�...
                
                nodes[(int)CurposX, (int)CurposY].spiritElement = 0;
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
            if (nodes[(int)frontx, (int)fronty].isWalk)
            {
                if (nodes[(int)frontx, (int)fronty].spiritElement == spiritElement) return;

                nodes[(int)CurposX, (int)CurposY].spiritElement = 0;
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
            if (nodes[(int)rightx, (int)righty].isWalk)
            {
                if (nodes[(int)rightx, (int)righty].spiritElement == spiritElement) return;
                nodes[(int)CurposX, (int)CurposY].spiritElement = 0;
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
            signal.SetSignType(_signType, nodes[(int)CurposX, (int)CurposY].rotation, _dir, (int)CurposX, (int)CurposY); // signal �� sign Ÿ���� �����ϰ� dir ������ ����
                                                                                          
        
        if (_dir == signal.dir)
        {
            CurposX += signal.pair.Item1;
            CurposY += signal.pair.Item2;

            // ǥ���������� ������ ������ ������.
            _dir = signal.spiritDir;

            // Ÿ�� üũ�� ���� �� ���ٸ�..
            if (!nodes[(int)CurposX, (int)CurposY].isWalk)
            { detection = Detect.None;}
            else
            {// ���� ĭ�� ���� Ȥ�� �ڿ� ä���ϴ� ���̶��..
                detection = Detect.Move;}
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
        Vector2 targetVector = new Vector2(_curposx + 0.5f, _curposy + 0.5f);
        Vector2 direction = (targetVector - (Vector2)transform.position).normalized;

        if(Vector2.Distance(targetVector, transform.position) <= 0.01f)
        {
            transform.position = targetVector;
            detection = Detect.None;
            return;
        }
        else
        {
            Vector2 movement = direction.normalized * moveSpeed * Time.smoothDeltaTime;
            transform.Translate(movement);
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
        Debug.Log("������ȣ ȣ��");
        detection = Detect.Basic_MoveMent;
        Debug.Log("detection�� ���´� " + detection);
    }
}
