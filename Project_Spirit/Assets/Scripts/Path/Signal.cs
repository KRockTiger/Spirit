using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System;
using UnityEngine.Tilemaps;

public class Signal : MonoBehaviour
{
    //public Dictionary<Vector3Int, Quaternion> tileRotations = new Dictionary<Vector3Int, Quaternion>(); // Ÿ���� ��ġ�� ȸ�� ������ ������ ��ųʸ�

    [SerializeField]
    Sprite[] signalSprite;
    [SerializeField]
    Tilemap tilemap;
    [SerializeField]
    Text stopduration;

    Node[,] nodes;
    int[] frontdirX = { 0, -1, 0, 1 };
    int[] frontdirY = { 1, 0, -1, 0 };

    int[] leftdirX = { -1, 0, 1, 0 }; 
    int[] leftdirY = { 0, -1, 0, 1 };

    int[] rightdirX = { 1, 0, -1, 0 }; 
    int[] rightdirY = { 0, 1, 0, -1 };
    [HideInInspector]
    public int dir;
    [HideInInspector]
    public int spiritDir;
    public (int,int) pair;

    int curposX;
    int curposy;
    enum Dir
    {
        Up = 0,
        Left = 1,
        Down = 2,
        Right = 3
    }

    enum SignalType
    {
        None = 0,
        Forward = 1,
        Left= 2,
        Right = 3,
        LeftFoward = 4,
        FowardRight = 5,
        LeftRight = 6,
        All = 7,
        Stop = 8
    }
    SignalType signalType;
    
    public void SetDirectionOfSpirit(int _dir)
    {
        switch(signalType)
        {
            case SignalType.None:
                break;
            case SignalType.Forward:
                Forward(_dir);
                break;
            case SignalType.Left:
                Left(_dir);
                break;
            case SignalType.Right:
                Right(_dir);
                break;
            case SignalType.LeftFoward:
                LeftFoward(_dir);
                break;
            case SignalType.FowardRight:
                FowardRight(_dir);
                break;
            case SignalType.LeftRight:
                LeftRight(_dir);
                break;
            case SignalType.All:
                All(_dir);
                break;
            case SignalType.Stop:
                Stop();
                break;
        }
    }

    #region ��������Ʈ ǥ�� �з�
    // Sprite code => �ش��ϴ� �Լ� ȣ��
    public void SetSignType(int _num, Quaternion _quaternion, int _dir, int _curposX, int _curposY)
    {
        nodes = TileDataManager.instance.GetNodes();
        int number = _num;
        Quaternion rot = _quaternion;
        curposX = _curposX;
        curposy = _curposY;
        signalType = (SignalType)number + 1;    // Type + 1 ������ enum ����.
        dir = CheckRotation(rot);
        spiritDir = _dir;
        SetDirectionOfSpirit(dir);
    }
    #endregion
    #region ǥ�Ĺ��� ����
    void Forward(int _dir)
    {  
        //Debug.Log("�������⿡�� ȸ������ ��� ? : " + _dir);
       
        int frontx = frontdirX[_dir];
        int fronty = frontdirY[_dir];
        // �絵�� ����
        pair = (frontx, fronty);
        signalType = SignalType.None;
    }
    void Left(int _dir)
    {
        //Debug.Log("���� ǥ������ : " + _dir);
        int leftx = leftdirX[_dir];
        int lefty = leftdirY[_dir];

        pair = (leftx, lefty);
        spiritDir = (spiritDir + 1) % 4;
        signalType = SignalType.None;
    }
    void Right(int _dir)
    {
        //Debug.Log("������ ǥ������ : " + _dir);
        int rightx = rightdirX[_dir];
        int righty = rightdirY[_dir];

        pair = (rightx, righty);
        spiritDir = (spiritDir - 1 + 4) % 4;
        signalType = SignalType.None;
    }

    void LeftFoward(int _dir)
    {
        if (nodes[curposX,curposy].stack == 0)
        {
            Left(_dir);
            nodes[curposX, curposy].stack += 1;
        }
        else
        {
            Forward(_dir);
            nodes[curposX,curposy].stack = 0;
        }
        
        signalType = SignalType.None;
    }

    void FowardRight(int _dir)
    {
        if (nodes[curposX, curposy].stack == 0)
        {
            Forward(_dir);
            nodes[curposX, curposy].stack += 1;
        }
        else
        {
            Right(_dir);
            nodes[curposX, curposy].stack = 0;
        }

        signalType = SignalType.None;
    }

    void LeftRight(int _dir)
    {
        if (nodes[curposX, curposy].stack == 0)
        {
            Left(_dir);
            nodes[curposX, curposy].stack += 1;
        }
        else
        {
            Right(_dir);
            nodes[curposX, curposy].stack = 0;
        }

        signalType = SignalType.None;
    }

    void All(int _dir)
    {
        if (nodes[curposX, curposy].stack == 0)
        {
            Left(_dir);
            nodes[curposX, curposy].stack += 1;
        }
        else if (nodes[curposX, curposy].stack == 1)
        {
            Forward(_dir);
            nodes[curposX, curposy].stack += 1; 
        }
        else
        {
            Right(_dir);
            nodes[curposX,curposy].stack = 0;
        }

        signalType = SignalType.None;
    }

    void Stop()
    {
        StartCoroutine(StopPattern(3f));
        signalType = SignalType.None;
    }
    IEnumerator StopPattern(float _time)
    {
        yield return new WaitForSeconds(_time);

        signalType = SignalType.None;
    }
    #endregion
    #region ǥ��ȸ���� üũ
    int CheckRotation(Quaternion _quaternion)
    {   // Quaternion�� ���Ϸ� ������ ��ȯ
        Vector3 eulerRotation = _quaternion.eulerAngles;
        // ȸ������ 0 ~ 360 ������ ����
        float adjustedRotation = eulerRotation.z % 360;
        // 0���� 360 ������ ȸ������ ���� int ���� ���
        int intValue = Mathf.FloorToInt(adjustedRotation / 90);

        // ���� 4�� �ʰ��ϴ� ��� ó��
        if (intValue > 4)
        {
            intValue = 0; // 360�� ȸ���� 0���� �����Ƿ� 1�� ó��
        }

        return intValue;
    }
    #endregion
}
