using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DetectMove : MonoBehaviour
{
    [Header("정령 위치 세팅")]
    Text stopduration;
    Vector2Int bottomLeft, topRight;
    
    Node[,] nodes;  // TileDataManager instance.
    Signal signal;
    MeshRenderer meshRenderer;
    Building TempBuilding;
    ResourceBuilding TempResoucebuilding;

    public float CurposX;
    public float CurposY; 
    // 바라보는 방향 기준 좌표 변화.  
    int[] frontX = { 0, -1, 0, 1 };  // Up , Left, Down, Right
    int[] frontY = { 1, 0, -1, 0 };

    int[] leftX = { -1, 0, 1, 0 }; 
    int[] leftY = { 0, -1, 0, 1 }; 

    int[] rightX = { 1, 0, -1, 0 };  
    int[] rightY = { 0, 1, 0, -1 };

    [Header ("정령 동적 세팅")]
    public float moveSpeed = 1f;
    public int LootAmount = 15;
    public float TimeforWorking = 3f;
    public int spiritElement;
    public int _dir;
    public int spiritID;
    int signType;
    int tempx, tempy;
    [SerializeField]
    int saveX, saveY;
    [SerializeField]
    bool isFactory = false;
    [SerializeField]
    bool isLoot = false;
    bool isPause = false;
    
    CapsuleCollider capsuleCollider;
    SpiritAnim spiritAni;
    GameObject CradleManager;
    // 정령 텔포 이동전 좌표.
    Vector2 accessPoint;

    enum Dir
    {
        Up = 0,
        Left = 1,
        Down = 2,
        Right = 3
    }
   
    public enum Detect
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
        Mark_Check,
        FactoryOrLootOut,
        FactoryOrLootEnter,
        Cradle,
        Wait,
        Dead,
        Ascension
    }
    [SerializeField]
    Detect detection = Detect.None;

    public Detect GetDetection()
    {
        return detection;
    }

    public void SetDetection(Detect detect)
    {
        detection = detect;
    }
    public int GetDirection()
    {
        return _dir;
    }

    public int GetSpiritID()
    {
        return spiritID;
    }
    private void Start()
    {  
        nodes = TileDataManager.instance.GetNodes();
        signal = GameObject.Find("[SignalManager]").GetComponent<Signal>();
        spiritID = GetComponent<Spirit>().SpiritID;
        spiritElement = GetComponent<Spirit>().SpiritElement;
        spiritAni = GetComponent<SpiritAnim>();
        meshRenderer = GetComponent<MeshRenderer>();
        capsuleCollider = this.GetComponent<CapsuleCollider>();
        CradleManager = GameObject.Find("CradleManager");

        // 배속에 따른 스피드에 맞게 생성
        moveSpeed = GameObject.Find("TimeNTemperatureManager").GetComponent<TimeManager>().timeSpeed;
        // 연구 적용 스피드
        moveSpeed *= GameObject.Find("SpiritManager").GetComponent<SpiritManager>().spiritMoveSpeed;
    }
    private void Update()
    {  
        spiritID = GetComponent<Spirit>().GetSpiritID();
        SortSkeletonLayer();

        switch (detection)
        {
            case Detect.None:
                detection = Detect.CheckTile;
                break;
            case Detect.CheckTile:
                CheckTile();
                break;
            case Detect.Factory_MoveMent:
                FactoryWork(); // 공장에서 움직임 시작
                break;
            case Detect.Basic_MoveMent:
                BaseMove();
                break;
            case Detect.Factory:
                FactoryWork(); // 일하는 값
                break;
            case Detect.Loot:
                LootWork();
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
            case Detect.FactoryOrLootOut:
                if (!isFactory && !isLoot)
                {
                    detection = Detect.Move;
                    break;
                }
                else
                {
                    LootOrFactoryAnimationMove(saveX, saveY);
                    return;// 나온 후의 움직임
                }
                
            case Detect.FactoryOrLootEnter:
                FactoryOrLootEnter((int)CurposX, (int)CurposY);
                break;
            case Detect.Cradle:
                break;
            case Detect.Wait:
                WaitUntilPush();
                break;
            case Detect.Dead:
                break;
            case Detect.Ascension:
                AscensionToSky();
                break;

        }
    }


    private void CheckTile()
    {   
        nodes = TileDataManager.instance.GetNodes();

        if (isFactory)
        {
            SuddenlyFactoryDisapper();
            return;
        }
        if (isLoot)
        {
            SuddenlyLootDisapper();
            return;
        }
        if (isPause) return;
        // 돌 혹은 나무 자원           
        if (TileDataManager.instance.GetTileType((int)CurposX, (int)CurposY) == 6 || (TileDataManager.instance.GetTileType((int)CurposX, (int)CurposY) == 7))
        {
            detection = Detect.Loot; return; 
        }
        // 표식
        if (TileDataManager.instance.GetTileType((int)CurposX, (int)CurposY) == 5)
        {
            nodes[(int)CurposX, (int)CurposY].spiritElement = spiritElement;
            string signName = nodes[(int)CurposX, (int)CurposY].nodeTile.name;

            signType = ExtractNumber(signName);
           // Debug.Log(" 표식의 숫자는 " + signType);
            detection = Detect.Mark_Check;
            return;
        }
        // 정령 요람
        if(TileDataManager.instance.GetTileType((int)CurposX, (int)CurposY) == 2)
        {
            Debug.Log("정령 세계로 이바지 합니다.");
            GetComponent<Spirit>().DevoteToCradle();
            return;
        }

        if (TileDataManager.instance.GetTileType((int)CurposX, (int)CurposY) == 3)
        {
            if(nodes[(int)CurposX, (int)CurposY].isBuild)
            {
                // 건물 이용시에는 반환
                if (isFactory) return;
                detection = Detect.Factory_MoveMent; return;
            }
           
            else
            {
                nodes[(int)CurposX, (int)CurposY].spiritElement = spiritElement;
                
                detection = Detect.Basic_MoveMent;
                return;
               
            }

        }
        // 길 타일이 아니라면 소멸되게 하기
        else
        {
            detection = Detect.Ascension;
          
            return;
        }
    }

    private void MarkCheck(int _signType)
    {

        nodes = TileDataManager.instance.GetNodes();

        // 표식에 올라가서도 땅 차지해야함
        //nodes[(int)CurposX, (int)CurposY].spiritElement = spiritElement;

        if (_signType == 7)
        {
            if (!isPause)
            {
            float stopDuration = float.Parse(stopduration.text);
            isPause = true;
            StartCoroutine(StopSign(stopDuration));
            detection = Detect.Basic_MoveMent;
            }
            
        }
        else
            signal.SetSignType(_signType, nodes[(int)CurposX, (int)CurposY].nodeTile, _dir, (int)CurposX, (int)CurposY); // signal 에 sign 타입을 지정하고 dir 방향을 받음
                                                                                          
        
        if (_dir == signal.dir)
        {
            float disX = CurposX;
            float disY = CurposY;
            float tempsignX = CurposX + signal.pair.Item1;
            float tempsigny = CurposY + signal.pair.Item2;

            if (nodes[(int)tempsignX, (int)tempsigny].spiritElement == spiritElement)
                return;

            CurposX += signal.pair.Item1;
            CurposY += signal.pair.Item2;

            // 표지방향으로 정령방향 전환.
            _dir = signal.spiritDir;

            // 타일 체크후 걸을 수 없다면..
            if (TileDataManager.instance.GetTileType((int)CurposX, (int)CurposY) != 3 && TileDataManager.instance.GetTileType((int)CurposX, (int)CurposY) != 5)

            { detection = Detect.None;}
            // 걸을 수 있다면..
            else if(TileDataManager.instance.GetTileType((int)CurposX, (int)CurposY) == 3 || TileDataManager.instance.GetTileType((int)CurposX, (int)CurposY) == 5)
            {
                nodes[(int)disX, (int)disY].spiritElement = 0;
                detection = Detect.Move;
            }
            else
            {// 다음 칸이 공장 혹은 자원 채집하는 곳이라면..

                nodes[(int)disX, (int)disY].spiritElement = 0;

                detection = Detect.Move;
                return;
            }
        }
        else
        {
            detection = Detect.Basic_MoveMent;
        }
    }
    #region 기본 움직임
    private void BaseMove()
    {
        nodes = TileDataManager.instance.GetNodes();    // 매우 매우 중요!! 코드 간결화
        if (TileDataManager.instance.GetTileType((int)CurposX, (int)CurposY) != 3)
        {
            detection = Detect.Ascension;
        }
           
        float leftx = CurposX + leftX[_dir];
        float lefty = CurposY + leftY[_dir];
        float frontx = CurposX + frontX[_dir];
        float fronty = CurposY + frontY[_dir];
        float rightx = CurposX + rightX[_dir];
        float righty = CurposY + rightY[_dir];

        if (leftx < TileDataManager.instance.sizeX && leftx >= 0 && lefty < TileDataManager.instance.sizeY && lefty >= 0)
        {
            //  1. 현재 바라보는 방향으로 기준으로 왼쪽으로 갈 수 있는지 확인.
           if(TileDataManager.instance.GetTileType((int)leftx, (int)lefty) == 3 || TileDataManager.instance.GetTileType((int)leftx, (int)lefty) == 6 || TileDataManager.instance.GetTileType((int)leftx, (int)lefty) == 7 || TileDataManager.instance.GetTileType((int)leftx, (int)lefty) == 5 || TileDataManager.instance.GetTileType((int)leftx, (int)lefty) == 2)
            {
                // 가야할 타일에 같은 정령이 있다면.
                if (nodes[(int)leftx, (int)lefty].spiritElement == spiritElement)
                {
                    detection = Detect.None;
                    return;
                }

                if (nodes[(int)leftx, (int)lefty].building != null)
                {
                    if (nodes[(int)leftx, (int)lefty].building.AskPermissionOfUse(this.gameObject) == false)
                        return;
                   
                }
                else if (nodes[(int)leftx, (int)lefty].resourceBuilding != null)
                {
                    if (!nodes[(int)leftx, (int)lefty].resourceBuilding.CheckForCapacity())
                     return;
                    
                   
                }
                // 좌표 이동전
                accessPoint = new Vector2(CurposX, CurposY);

                nodes[(int)CurposX, (int)CurposY].spiritElement = 0;
                // 왼쪽 방향으로 90도 회전 
                _dir = (_dir + 1) % 4;

                CurposX += frontX[_dir];
                CurposY += frontY[_dir];
                detection = Detect.Move;
                return;
            }
        }
         if (frontx < TileDataManager.instance.sizeX && frontx >= 0 && fronty < TileDataManager.instance.sizeY && fronty >= 0)
        {
            // 2. 현재 바라보는 방향을 기준으로 전진할 수 있는지 확인.
            if (TileDataManager.instance.GetTileType((int)frontx, (int)fronty) == 3 || TileDataManager.instance.GetTileType((int)frontx, (int)fronty) == 6 || TileDataManager.instance.GetTileType((int)frontx, (int)fronty) == 7 || TileDataManager.instance.GetTileType((int)frontx, (int)fronty) == 5 || TileDataManager.instance.GetTileType((int)frontx, (int)fronty) == 2)
            {
                if (nodes[(int)frontx, (int)fronty].spiritElement == spiritElement)
                {   detection = Detect.None; 
                    return;
                }
                if (nodes[(int)frontx, (int)fronty].building != null)
                {
                    if (nodes[(int)frontx, (int)fronty].building.AskPermissionOfUse(this.gameObject) == false)
                        return;

                }
                else if (nodes[(int)frontx, (int)fronty].resourceBuilding != null)
                {
                    if (!nodes[(int)frontx, (int)fronty].resourceBuilding.CheckForCapacity())
                        return;
                }
                // 좌표 이동전
                accessPoint = new Vector2(CurposX, CurposY);

                nodes[(int)CurposX, (int)CurposY].spiritElement = 0;
                CurposX += frontX[_dir];  
                CurposY += frontY[_dir] ;
                detection = Detect.Move;
                return;
            }
        }
         if (rightx < TileDataManager.instance.sizeX && rightx >= 0 && righty < TileDataManager.instance.sizeY && righty >= 0)
        {
            // 2. 현재 바라보는 방향을 기준으로 우측할 수 있는지 확인.
            if (TileDataManager.instance.GetTileType((int)rightx, (int)righty) == 3 || (TileDataManager.instance.GetTileType((int)rightx, (int)righty) == 6 || (TileDataManager.instance.GetTileType((int)rightx, (int)righty) == 7) || (TileDataManager.instance.GetTileType((int)rightx, (int)righty) == 5 || TileDataManager.instance.GetTileType((int)rightx, (int)righty) == 2)))
            {
                if (nodes[(int)rightx, (int)righty].spiritElement == spiritElement)
                {
                    detection = Detect.None;
                    return;
                }
                if (nodes[(int)rightx, (int)righty].building != null)
                {
                    if (nodes[(int)rightx, (int)righty].building.AskPermissionOfUse(this.gameObject) == false)
                        return;

                }
                else if (nodes[(int)rightx, (int)righty].resourceBuilding != null)
                {
                    if (!nodes[(int)rightx, (int)righty].resourceBuilding.CheckForCapacity()) return;

                }
                // 좌표 이동전
                accessPoint = new Vector2(CurposX, CurposY);
                nodes[(int)CurposX, (int)CurposY].spiritElement = 0;
                _dir = (_dir - 1 + 4) % 4;

                 CurposX += frontX[_dir];  
                 CurposY += frontY[_dir];
                 detection = Detect.Move;
                return;
            }
        }
        else
            Debug.Log("아무것도 안들어갔음!");
       
    }
    private void Move(int _curposx, int _curposy)
    {
        //if(detection != Detect.Move) { return; }

        // 건물 진입 애니메이션 작동하게 해야함
        if (nodes[_curposx, _curposy].building != null || nodes[_curposx, _curposy].resourceBuilding != null)
        {   // ** 날짜 및 시간 구현에 speed 값 조정처리 필요
            //  Debug.Log("건물 진입 전 상태");
           // Debug.Log("다시 재진입함.");
            detection = Detect.FactoryOrLootEnter;
            return;
        }

        // 정령 움직임 구현
        Vector2 targetVector = new Vector2(_curposx + 0.5f, _curposy + 0.5f);
        Vector2 direction = (targetVector - (Vector2)transform.position).normalized;

        if(Vector2.Distance(targetVector, transform.position) <= 0.05f)
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

    private void FactoryOrLootEnter(int _curposx, int _curposy)
    {
       // Debug.Log("건물 진입점에서 대기중");
        // 정령 움직임 구현
        Vector2 targetVector = new Vector2(_curposx + 0.5f , _curposy + 0.5f);
        Vector2 direction = (targetVector - (Vector2)transform.position).normalized;

        if (Vector2.Distance(targetVector, transform.position) <= 0.1f)
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
    #endregion

    #region 공장 움직임
    private void FactoryWork()
    {
        nodes = TileDataManager.instance.GetNodes();

        StartCoroutine(Buildpattern());
        detection = Detect.None;
        return;
    }
    IEnumerator Buildpattern()
    {
        FindFactoryPoint();
       
        yield return new WaitForSeconds(TimeforWorking);

        // 출구에 정령이 있다면, 휴식으로 전환
        if(nodes[(int)saveX, (int)saveY].spiritElement != 0)
        {
            detection = Detect.Wait;
            //Debug.Log("출구에 정령이 있습니다.");
        }
        else
        {
            TempBuilding.GetComponent<Building>().DeleteWorkingSprit(this.gameObject);
            meshRenderer.enabled = true;
            detection = Detect.FactoryOrLootOut;
        }

    }
    private void FindFactoryPoint()
    {
        if (!isFactory)
        {
            TempBuilding = nodes[(int)CurposX, (int)CurposY].building;
            // 정령이 치러야할 댓가
            // 노동 시간, 체력, 이용 비용 
             nodes[(int)CurposX, (int)CurposY].building.BuildingExpense(this.gameObject);
             nodes[(int)CurposX, (int)CurposY].building.AddWorkingSprit(this.gameObject);
            
            Vector2 sP = nodes[(int)CurposX, (int)CurposY].building.connectedRoads.Item1;
            Vector2 nP = nodes[(int)CurposX, (int)CurposY].building.connectedRoads.Item2;
            Vector2 transformPosition = new Vector2(transform.position.x, transform.position.y);
            Vector2 target = Vector2.zero;
            float distanceToA = Vector2.Distance(accessPoint, sP);
            float distanceToB = Vector2.Distance(accessPoint, nP);
            if (distanceToA < distanceToB)
            {
                isFactory = true;
                target = nP;
                CurposX = target.x; CurposY = target.y;
                tempx = (int)CurposX; tempy = (int)CurposY;
                RedirectionafterFactory(tempx, tempy);
            }
            else
            {
                isFactory = true;
                target = sP;
                CurposX = target.x; CurposY = target.y;
                tempx = (int)CurposX; tempy = (int)CurposY;
                RedirectionafterFactory(tempx, tempy);
            }
            capsuleCollider.enabled = false;
            transform.position = new Vector2(tempx + 0.5f, tempy + 0.5f);
            
            meshRenderer.enabled = false;
            saveX = (int)CurposX; saveY = (int)CurposY;
            CurposX = tempx; CurposY = tempy;
        }
        
    }
    private void RedirectionafterFactory(float _curposX, float _curposY)
    {
        int[] FactorydirX = { 0, 0, 1, -1 };
        int[] FactorydirY = { 1, -1, 0, 0 };

        for(int i = 0; i < 4; i++)
        {
            if (nodes[(int)_curposX + FactorydirX[i], (int)_curposY + FactorydirY[i]].GetNodeType() == 1)
            {
                if (i == 0)
                {
                    _dir = (int)Dir.Down;
                    tempy += 1;
                    break;
                }
                else if (i == 1)
                {
                    _dir = (int)Dir.Up;
                    tempy -= 1;
                    break;
                }
                else if (i == 2)
                {
                    _dir = (int)Dir.Left;
                    tempx += 1;
                    break;
                }
                else
                {
                    _dir = (int)Dir.Right;
                    tempx -= 1;
                    break;
                }
            }
        }
    }
    #endregion

    #region 자원 움직임
    void LootWork()
    {
        nodes = TileDataManager.instance.GetNodes();

        StartCoroutine(LootPattern());
        detection = Detect.None;
        return;
    }
    IEnumerator LootPattern()
    {
        FindLootPoint();
       
        yield return new WaitForSeconds(TimeforWorking);

        // 출구에 정령이 있다면, 휴식으로 전환
        if (nodes[(int)saveX, (int)saveY].spiritElement != 0)
        {
            detection = Detect.Wait;
            //Debug.Log("출구에 정령이 있습니다.");
        }
        else
        {  
            TempResoucebuilding.GetComponent<ResourceBuilding>().DeleteWorkingSprit(this.gameObject);
            meshRenderer.enabled = true;
            detection = Detect.FactoryOrLootOut;
        }
        
    }

    void FindLootPoint()
    {
        if(!isLoot)
        {
            TempResoucebuilding = nodes[(int)CurposX, (int)CurposY].resourceBuilding;
            TempResoucebuilding.AddWorkingSprit(this.gameObject);
            TempResoucebuilding.GetDecreasement(LootAmount);

            // null 값일떄는 Loot를 취소시키고 마지막 save 위치로 이동시켜야함
            if(!nodes[(int)CurposX, (int)CurposY].resourceBuilding.CanUse) return;

            Vector2 sP = nodes[(int)CurposX, (int)CurposY].resourceBuilding.connectedRoads.Item1;
            Vector2 nP = nodes[(int)CurposX, (int)CurposY].resourceBuilding.connectedRoads.Item2;
            Vector2 transformPosition = new Vector2(transform.position.x, transform.position.y);
            Vector2 target = Vector2.zero;
            float distanceToA = Vector2.Distance(accessPoint, sP);
            float distanceToB = Vector2.Distance(accessPoint, nP);
            if (distanceToA < distanceToB)
            {
                isLoot = true;
                target = nP;
                CurposX = target.x; CurposY = target.y;
                tempx = (int)CurposX; tempy = (int)CurposY;
                RedirectionafterLoot(tempx, tempy);
            }
            else
            {
                isLoot = true;
                target = sP;
                CurposX = target.x; CurposY = target.y;
                tempx = (int)CurposX; tempy = (int)CurposY; 
                RedirectionafterLoot(tempx, tempy);
            }
            capsuleCollider.enabled = false;
            transform.position = new Vector2(tempx + 0.5f, tempy + 0.5f);
            
            meshRenderer.enabled = false;
            saveX = (int)CurposX; saveY = (int)CurposY;
            CurposX = tempx; CurposY = tempy;
           
        }
    
    }
    
    // 공장 및 자원 사용 이후 위치값 수정
    void RedirectionafterLoot(float _curposX, float _curposY)
    {
        int[] FactorydirX = { 0, 0, 1, -1 };
        int[] FactorydirY = { 1, -1, 0, 0 };

        for (int i = 0; i < 4; i++)
        {
            if (nodes[(int)_curposX + FactorydirX[i], (int)_curposY + FactorydirY[i]].GetNodeType() == 6 || nodes[(int)_curposX + FactorydirX[i], (int)_curposY + FactorydirY[i]].GetNodeType() == 7)
            {
                if (i == 0)
                {
                    _dir = (int)Dir.Down;
                    tempy += 1;
                    break;
                }
                else if (i == 1)
                {
                    _dir = (int)Dir.Up;
                    tempy -= 1;
                    break;
                }
                else if (i == 2)
                {
                    _dir = (int)Dir.Left;
                    tempx += 1;
                    break;
                }
                else
                {
                    _dir = (int)Dir.Right;
                    tempx -= 1;
                    break;
                }
            }

        }
    }

    #endregion

    //  공장 혹은 자원 나올때 실행하는 애니메이션.
    void LootOrFactoryAnimationMove(int _curposx, int _curposy)
    {
        if (!isFactory && !isLoot) return;

        Vector2 targetVector = new Vector2(_curposx + 0.5f, _curposy + 0.5f);
       
        Vector2 direction = (targetVector - (Vector2)transform.position).normalized;

        if (Vector2.Distance(targetVector, transform.position) <= 0.1f)
        {
            transform.position = targetVector;
            CurposX = _curposx; CurposY = _curposy;
            isLoot = false;
            isFactory = false;
            capsuleCollider.enabled = true;
            detection = Detect.None;
            return;
        }
        else
        {
            Vector2 movement = direction.normalized * moveSpeed * Time.smoothDeltaTime;
            transform.Translate(movement);
        }
    }

    // 표지판 판정 기준
    static int ExtractNumber(string input)
    {
        // 정규표현식을 사용하여 숫자만 추출
        Match match = Regex.Match(input, @"\d+");

        // 추출된 숫자가 있는지 확인하고 반환
        if (match.Success)
        {
            return int.Parse(match.Value);
        }
        else
        {
            // 숫자가 없는 경우 예외처리 또는 기본값 반환
             return 100;
        }
    }

    private void StopMove()
    {
    }
    IEnumerator StopSign(float _time)
   {
        yield return new WaitForSeconds(_time);
        isPause = false;
        detection = Detect.Basic_MoveMent;
   }

  
    // 자원 혹은 공장이 파괴되었을때.
    private void SuddenlyFactoryDisapper()
    {
        if (nodes[(int)CurposX, (int)CurposY].building == null)
        {
            isFactory = false;
            meshRenderer.enabled = true;
            CurposX = saveX; CurposY = saveY;
            detection = Detect.None;
        }
    }

    private void SuddenlyLootDisapper()
    {
        if (nodes[(int)CurposX, (int)CurposY].resourceBuilding == null)
        {
            isLoot = false;
            meshRenderer.enabled = true;
            CurposX = saveX; CurposY = saveY;
            detection = Detect.None;
        }
    }

    // 출구에서 나온 정령이 대기하는 방법
    private void WaitUntilPush()
    {
        if (nodes[(int)saveX, (int)saveY].spiritElement == 0)
        {
            TempBuilding.GetComponent<Building>().DeleteWorkingSprit(this.gameObject);
            meshRenderer.enabled = true;
            detection = Detect.FactoryOrLootOut;

        }
    }
    // Layer 정해주는 로직
    void SortSkeletonLayer()
    {
        GetComponent<MeshRenderer>().sortingOrder = 103 - (int)CurposY;
    }

    // 하늘로 승천
    void AscensionToSky()
    {
        nodes[(int)CurposX, (int)CurposY].spiritElement = 0;
        GetComponent<Spirit>().InitializeUIInfo();
        Destroy(gameObject, 1.3f);
    }

}
