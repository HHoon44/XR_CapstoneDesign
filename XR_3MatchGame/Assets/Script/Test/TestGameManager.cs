using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XR_3MatchGame.Util;

public class TestGameManager : Singleton<TestGameManager>
{
    // 게임판
    public TestBoard testBoard;

    private void Start()
    {
        // 게임 프레임 60으로 고정
        Application.targetFrameRate = 60;
    }
}
