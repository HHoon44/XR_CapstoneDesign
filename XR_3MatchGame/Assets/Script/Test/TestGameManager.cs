using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XR_3MatchGame.Util;

public class TestGameManager : Singleton<TestGameManager>
{
    // ������
    public TestBoard testBoard;

    private void Start()
    {
        // ���� ������ 60���� ����
        Application.targetFrameRate = 60;
    }
}
