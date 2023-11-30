using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using XR_3MatchGame.Util;
using XR_3MatchGame_Object;

public class TestBoard : MonoBehaviour
{
    public List<Sprite> sprites = new List<Sprite>();

    [Header("Block Obj")]
    // Block 프리팹
    public GameObject block;

    // Block을 담을 리스트
    public List<TestBlock> blocks = new List<TestBlock>();

    // 매칭 여부
    public bool isMatch;

    private void Start()
    {
        TestGameManager.Instance.testBoard = this;

        SetBoard();
    }

    /// <summary>
    /// 보드에 블럭을 세팅하는 메서드
    /// </summary>
    public void SetBoard()
    {
        // Block Row
        for (int row = -3; row < 4; row++)
        {
            // Block Col
            for (int col = -3; col < 4; col++)
            {
                var newBlock = Instantiate(block);

                // 부모 설정
                newBlock.transform.SetParent(this.transform);

                // 위치 값 설정
                newBlock.transform.localPosition = new Vector3(col, row, 0);

                // 회전 값 설정
                newBlock.transform.localRotation = Quaternion.identity;

                // 사이즈 값 설정
                newBlock.transform.localScale = new Vector3(.19f, .19f, .19f);

                // 활성화
                newBlock.SetActive(true);

                // Block 초기화
                newBlock.GetComponent<TestBlock>().Initialize(col, row);

                var num = Random.Range(0, 6);

                newBlock.GetComponent<TestBlock>().icon.sprite = sprites[num];

                // Block 저장
                blocks.Add(newBlock.GetComponent<TestBlock>());
            }
        }
    }

    public bool BlockUpdate(TestBlock checkBlock = null, TestBlock otherBlock = null, SwipeDir swipeDir = SwipeDir.None)
    {
        return true;
    }
}