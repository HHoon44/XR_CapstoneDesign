using System.Collections;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using XR_3MatchGame.Util;
using XR_3MatchGame_Object;

public class TestBlock : MonoBehaviour
{
    [Header("나의 좌표")]
    public int col;
    public int row;

    [Header("상대의 좌표")]
    public int targetCol;
    public int targetRow;

    public SpriteRenderer icon;

    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;
    private Vector2 tempPosition;

    private float swipeAngle;

    private TestBlock otherBlock;

    public void Initialize(int col, int row)
    {
        // 나의 좌표 설정
        this.col = col;
        this.row = row;

        // 상대의 좌표 설정
        targetCol = col;
        targetRow = row;
    }

    private void Update()
    {
        targetCol = col;
        targetRow = row;

        BlockSwipe();
    }

    private void BlockSwipe()
    {
        if (Mathf.Abs(targetCol - transform.localPosition.x) > .1f)
        {
            tempPosition = new Vector2(targetCol, transform.localPosition.y);
            transform.localPosition = Vector2.Lerp(transform.localPosition, tempPosition, .3f);
        }
        else
        {
            tempPosition = new Vector2(targetCol, transform.localPosition.y);
            transform.localPosition = tempPosition;
        }

        if (Mathf.Abs(targetRow - transform.localPosition.y) > .1f)
        {
            tempPosition = new Vector2(transform.localPosition.x, targetRow);
            transform.localPosition = Vector2.Lerp(transform.localPosition, tempPosition, .3f);
        }
        else
        {
            tempPosition = new Vector2(transform.localPosition.x, targetRow);
            transform.localPosition = tempPosition;
        }
    }

    private void OnMouseDown()
    {
        firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseUp()
    {
        finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        CalculateAngle();
    }

    private void CalculateAngle()
    {
        swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;

        BlockMove();
    }

    /// <summary>
    /// 블럭 이동 메서드
    /// </summary>
    private void BlockMove()
    {
        var blocks = TestGameManager.Instance.testBoard.blocks;

        // Top
        if ((swipeAngle > 45 && swipeAngle <= 135) && row < 3)
        {
            for (int i = 0; i < blocks.Count; i++)
            {
                if (blocks[i].col == col && blocks[i].row == row + 1)
                {

                    // 상대
                    otherBlock = blocks[i];
                    otherBlock.row -= 1;

                    // 나
                    row += 1;

                    // 매칭 검사
                    StartCoroutine(BlockCheck(SwipeDir.Top));
                    return;
                }
            }
        }
        // Bottom
        else if ((swipeAngle < -45 && swipeAngle >= -135) && row > -3)
        {
            for (int i = 0; i < blocks.Count; i++)
            {
                if (blocks[i].col == col && blocks[i].row == row - 1)
                {
                    // 상대
                    otherBlock = blocks[i];
                    otherBlock.row += 1;

                    // 나
                    row -= 1;

                    // 매칭 검사
                    StartCoroutine(BlockCheck(SwipeDir.Bottom));
                    return;
                }
            }
        }
        // Left
        else if ((swipeAngle > 135 || swipeAngle <= -135) && col > -3)
        {
            for (int i = 0; i < blocks.Count; i++)
            {
                if (blocks[i].col == col - 1 && blocks[i].row == row)
                {
                    // 상대
                    otherBlock = blocks[i];
                    otherBlock.col += 1;

                    // 나
                    col -= 1;

                    // 매칭 검사
                    StartCoroutine(BlockCheck(SwipeDir.Left));
                    return;
                }
            }
        }
        // Right
        else if ((swipeAngle > -45 && swipeAngle <= 45) && col < 3)
        {
            for (int i = 0; i < blocks.Count; i++)
            {
                if (blocks[i].col == col + 1 &&
                    blocks[i].row == row)
                {
                    // 상대
                    otherBlock = blocks[i];
                    otherBlock.col -= 1;

                    // 나
                    col += 1;

                    // 매칭 검사
                    StartCoroutine(BlockCheck(SwipeDir.Right));
                    return;
                }
            }
        }
    }

    /// <summary>
    /// 블럭 매칭을 체크하는 메서드
    /// </summary>
    /// <returns></returns>
    private IEnumerator BlockCheck(SwipeDir swipeDir)
    {
        var testBoard = TestGameManager.Instance.testBoard;

        testBoard.BlockUpdate();

        // 유저가 옮긴 블럭에 대한 로직
        switch (swipeDir)
        {
            case SwipeDir.Top:
                if (testBoard.BlockUpdate(this, null, swipeDir))
                {
                    // 블럭 매칭 시작
                    testBoard.isMatch = true;
                }
                else
                {
                    yield return new WaitForSeconds(.2f);

                    // OtherBlock의 매칭 여부 판단
                    if (testBoard.BlockUpdate(null, otherBlock, SwipeDir.None))
                    {
                        // 블럭 매칭 시작
                        testBoard.isMatch = true;
                    }
                    else
                    {
                        // 블럭 원위치
                        otherBlock.row += 1;
                        row -= 1;

                        yield return new WaitForSeconds(.4f);

                        // GM.SetGameState(GameState.Play);
                    }
                }
                break;

                //    case SwipeDir.Bottom:
                //        if (board.BlockCheck(this, null, swipeDir))
                //        {
                //            // 블럭 매칭 시작
                //            GM.isStart = true;
                //        }
                //        else
                //        {
                //            yield return new WaitForSeconds(.2f);

                //            // OtherBlock의 매칭 여부 판단
                //            if (board.BlockCheck(null, otherBlock, SwipeDir.None))
                //            {
                //                // 블럭 매칭 시작
                //                GM.isStart = true;
                //            }
                //            else
                //            {
                //                // 블럭 원위치
                //                otherBlock.row -= 1;
                //                row += 1;

                //                yield return new WaitForSeconds(.4f);

                //                GM.SetGameState(GameState.Play);
                //            }
                //        }
                //        break;

                //    case SwipeDir.Left:
                //        if (board.BlockCheck(this, null, swipeDir))
                //        {
                //            // 블럭 매칭 시작
                //            GM.isStart = true;
                //        }
                //        else
                //        {
                //            yield return new WaitForSeconds(.2f);

                //            // OtherBlock의 매칭 여부 판단
                //            if (board.BlockCheck(null, otherBlock, SwipeDir.None))
                //            {
                //                // 블럭 매칭 시작
                //                GM.isStart = true;
                //            }
                //            else
                //            {
                //                // 블럭 원위치
                //                otherBlock.col -= 1;
                //                col += 1;

                //                yield return new WaitForSeconds(.4f);

                //                GM.SetGameState(GameState.Play);
                //            }
                //        }
                //        break;

                //    case SwipeDir.Right:
                //        if (board.BlockCheck(this, null, swipeDir))
                //        {
                //            // 블럭 매칭 시작
                //            GM.isStart = true;
                //        }
                //        else
                //        {
                //            yield return new WaitForSeconds(.2f);

                //            // OtherBlock의 매칭 여부 판단
                //            if (board.BlockCheck(null, otherBlock, SwipeDir.None))
                //            {
                //                // 블럭 매칭 시작
                //                GM.isStart = true;
                //            }
                //            else
                //            {
                //                // 블럭 원위치
                //                otherBlock.col += 1;
                //                col -= 1;

                //                yield return new WaitForSeconds(.4f);

                //                GM.SetGameState(GameState.Play);
                //            }
                //        }
                //        break;
                //}

                //board.BlockUpdate();
        }

    }
}