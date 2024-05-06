using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimeController : MonoBehaviour
{
    // ���� �� �ð��� ���� �ð� ���� ����
    public float gameSecondsPerReal = 12f * 60f;

    public float gameTimer = 0f;
    // Update is called once per frame
    void Update()
    {
        gameTimer += Time.deltaTime * gameSecondsPerReal;
    }
}
