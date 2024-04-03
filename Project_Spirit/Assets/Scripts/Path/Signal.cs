using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System;
using UnityEngine.Tilemaps;

public class Signal : MonoBehaviour
{
    public Dictionary<Vector3Int, Quaternion> tileRotations = new Dictionary<Vector3Int, Quaternion>(); // Ÿ���� ��ġ�� ȸ�� ������ ������ ��ųʸ�

    [SerializeField]
    Sprite[] signalSprite;
    [SerializeField]
    Tilemap tilemap;
    int sigNum = (int)SignalType.Forward;

    public void Start()
    {
      
    }
    // Sprite code => �ش��ϴ� �Լ� ȣ��
    public void GetSignCode(string _SpriteName)
    {
         string input = _SpriteName;

        int number = ExtractNumber(input);      
        signalType = (SignalType)number;
    }

    // Signal�� ������ �м��ϰ�, 
    enum SignalType
    {
        Forward = 0,
        Left= 1,
        Right = 2,
        LeftFoward = 3,
        FowardRight = 4,
        LeftRight = 5,
        All = 6,
        Stop = 7
    }
    SignalType signalType;
    
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
            throw new ArgumentException("�Էµ� ���ڿ��� ���ڰ� �����ϴ�.");
            // �Ǵ�
            // return 0;
        }
    }

    // �ش� ������Ʈ�� z������ �󸶳� ȸ�����ִ����� ���Ⱚ�� ������

    private void Update()
    {
        // ���콺 ���� ��ư�� Ŭ���ϸ�
        if (Input.GetMouseButtonDown(0))
        {
            // ���콺 ��ġ�� ���� ��ǥ�� ��ȯ
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // ���� ��ǥ�� Ÿ�� ��ǥ�� ��ȯ
            Vector3Int tilePos = tilemap.WorldToCell(mouseWorldPos);
            // Ÿ�� ��ǥ�� �ش��ϴ� Ÿ���� ������
            TileBase clickedTile = tilemap.GetTile(tilePos);

            if (clickedTile != null)
            {
                Debug.Log("Clicked tile information: " + clickedTile.name);

                // �ش� Ÿ���� ��ġ�� ���� ��ǥ�� ��ȯ�Ͽ� ���� ������Ʈ�� transform�� ������
                Vector3 clickedWorldPos = tilemap.GetCellCenterWorld(tilePos);
                // Ÿ���� ȸ�� ������ ������
                Quaternion tileRotation = tilemap.GetTransformMatrix(tilePos).rotation; ;
                tileRotations.TryGetValue(tilePos, out tileRotation);
                Debug.Log("Clicked tile rotation before: " + tileRotation.eulerAngles);

                // Ŭ���� Ÿ���� z�� ȸ���� 90�� ������Ŵ
                tileRotation *= Quaternion.Euler(0, 0, 90f);
                tileRotations[tilePos] = tileRotation; // ������Ʈ�� ȸ�� ���� ����
                Debug.Log("Clicked tile rotation after: " + tileRotation.eulerAngles);

                // Ÿ�ϸʿ��� �ش� Ÿ���� ȸ�� ���� ������Ʈ
                Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
                tilemap.SetTransformMatrix(tilePos, matrix);

            }
            else
            {
                Debug.Log("No tile found at clicked position.");
            }
        }
    }
}


