using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.WSA;
using XR_3MatchGame.Util;
using XR_3MatchGame_Data;
using XR_3MatchGame_InGame;
using XR_3MatchGame_UI;
using XR_3MatchGame_Util;

namespace XR_3MatchGame_Object
{
    public class Board : MonoBehaviour
    {
        // [row, col]
        // [가로, 세로]

        public int width = 7;
        public int height = 7;

        [Header("인 게임 블럭")]
        public Block[,] arrBlocks = new Block[7, 7];
        public List<Block> blocks = new List<Block>();               // 인 게임 블럭 리스트
        public List<Block> downBlocks = new List<Block>();           // 아래 이동 블럭 리스트
        public List<Block> delBlocks = new List<Block>();            // 삭제 블럭 리스트

        [Header("파괴 블럭")]
        public List<Block> colDel = new List<Block>();
        public List<Block> rowDel = new List<Block>();

        public List<Block> colDown = new List<Block>();
        public List<Block> rowDown = new List<Block>();

        public List<Block> newBlock = new List<Block>();

        #region Manager

        public InGameManager IGM;

        private GameManager GM;
        private DataManager DM;

        #endregion

        public bool isReStart;

        private void Start()
        {
            GM = GameManager.Instance;
            DM = DataManager.Instance;

            GM.Initialize(this);

            // BGM 실행
            SoundManager.Instance.Initialize(SceneType.InGame);

            StartCoroutine(SpawnBlock());
        }

        private void Update()
        {
            if (GM.GameState == GameState.Checking || GM.GameState == GameState.SKill)
            {
                if (GM.isStart == true)
                {
                    GM.isStart = false;
                    // StartCoroutine(BlockClear());
                }
            }
        }

        /// <summary>
        /// 보드에 블럭을 생성하는 메서드
        /// </summary>
        /// <returns></returns>
        public IEnumerator SpawnBlock()
        {
            // 게임 상태 Spawn
            GM.SetGameState(GameState.Spawn);

            var blockPool = ObjectPoolManager.Instance.GetPool<Block>(PoolType.Block);

            for (int row = 0; row < 7; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    Block block = blockPool.GetPoolableObject(obj => obj.CanRecycle);

                    // 부모 설정
                    block.transform.SetParent(this.transform);

                    // 위치값 설정
                    block.transform.localPosition = new Vector3(-3 + col, 4 + row, 0);

                    // 회전값 설정
                    block.transform.localRotation = Quaternion.identity;

                    // 사이즈값 설정
                    block.transform.localScale = new Vector3(.19f, .19f, .19f);

                    // 활성화
                    block.gameObject.SetActive(true);

                    // 블럭 초기화
                    block.Initialize(-3 + col, 4);

                    // 위에서 아래로 작업
                    var targetRow = (block.row = -3 + row);

                    if (Mathf.Abs(targetRow - block.transform.localPosition.y) > .1f)
                    {
                        Vector2 tempPosition = new Vector2(block.transform.localPosition.x, targetRow);
                        block.transform.localPosition = Vector2.Lerp(block.transform.localPosition, tempPosition, .05f);
                    }

                }

                yield return new WaitForSeconds(.3f);
            }

            StartCoroutine(Test());

            // 매칭후 게임 상태 Play
            GM.SetGameState(GameState.Play);
        }

        /// <summary>
        /// 매칭 블럭을 찾아 파괴하는 코루틴
        /// </summary>
        /// <returns></returns>
        public IEnumerator BlockMatch()
        {
            var blockPool = ObjectPoolManager.Instance.GetPool<Block>(PoolType.Block);
            if (blocks.Count > 0)
            {
                // 3X3
                // Col 블럭 찾기
                for (int i = 0; i < blocks.Count; i++)
                {
                    if ((blocks[i].leftBlock != null && blocks[i].rightBlock != null) &&
                        (blocks[i].leftBlock.elementType == blocks[i].elementType && blocks[i].rightBlock.elementType == blocks[i].elementType))
                    {
                        delBlocks.Add(blocks[i].leftBlock);
                        delBlocks.Add(blocks[i]);
                        delBlocks.Add(blocks[i].rightBlock);

                        // 삭제할 Col 블럭 저장
                        colDel.Add(blocks[i].leftBlock);
                        colDel.Add(blocks[i]);
                        colDel.Add(blocks[i].rightBlock);

                        // 삭제할 블럭
                        Block middleBlock = blocks[i];
                        Block leftBlock = blocks[i].leftBlock;
                        Block rightBlock = blocks[i].rightBlock;

                        blocks.Remove(middleBlock);
                        blocks.Remove(leftBlock);
                        blocks.Remove(rightBlock);


                        //for (int col = 0; col < 49; col++)
                        //{
                        //    // 왼쪽
                        //    if (arrBlocks[col] != null)
                        //    {
                        //        if ((arrBlocks[col].col == leftBlock.col && arrBlocks[col].row == leftBlock.row))
                        //        {
                        //            arrBlocks[col] = null;
                        //        }
                        //    }

                        //    // 가운데
                        //    if (arrBlocks[col] != null)
                        //    {
                        //        if ((arrBlocks[col].col == middleBlock.col && arrBlocks[col].row == middleBlock.row) && arrBlocks[col] != null)
                        //        {
                        //            arrBlocks[col] = null;
                        //        }
                        //    }

                        //    // 오른쪽
                        //    if (arrBlocks[col] != null)
                        //    {
                        //        if (arrBlocks[col].col == rightBlock.col && arrBlocks[col].row == rightBlock.row)
                        //        {
                        //            arrBlocks[col] = null;
                        //        }
                        //    }
                        //}
                    }
                }

                /*
                // Row 블럭 찾기
                for (int row = 0; row < blocks.Count; row++)
                {
                    if ((blocks[row].topBlock != null && blocks[row].bottomBlock != null) &&
                        (blocks[row].topBlock.elementType == blocks[row].elementType && blocks[row].bottomBlock.elementType == blocks[row].elementType))
                    {
                        delBlocks.Add(blocks[row].topBlock);
                        delBlocks.Add(blocks[row]);
                        delBlocks.Add(blocks[row].bottomBlock);

                        //  삭제할 Row 블럭 저장
                        rowDel.Add(blocks[row].topBlock);
                        rowDel.Add(blocks[row]);
                        rowDel.Add(blocks[row].bottomBlock);

                        // 삭제할 블럭
                        Block topBlock = blocks[row].topBlock;
                        Block bottomBlock = blocks[row].bottomBlock;

                        blocks.Remove(blocks[row]);
                        blocks.Remove(topBlock);
                        blocks.Remove(bottomBlock);
                    }
                }
                */

                // 블럭 파괴
                if (delBlocks.Count > 0)
                {
                    // 블럭 파괴 파티클 실행
                    for (int i = 0; i < delBlocks.Count; i++)
                    {
                        delBlocks[i].BlockParticle();
                    }

                    yield return new WaitForSeconds(.3f);

                    // 블럭 파괴
                    for (int i = 0; i < delBlocks.Count; i++)
                    {
                        blockPool.ReturnPoolableObject(delBlocks[i]);
                    }
                }

                /// Col과 Row에서 중복되는 블럭 처리 생각하기
                /// 일단 Col 부터 하기
                /// 여기서 부터 시작

                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 7; j++)
                    {
                    }
                }


                // 내릴 Col 블럭 찾기
                for (int i = 0; i < colDel.Count; i++)
                {
                    for (int j = 0; j < blocks.Count; j++)
                    {
                        if (blocks[j].col == colDel[i].col && blocks[j].row > colDel[i].row)
                        {
                            if (blocks[j].gameObject.activeSelf)
                            {
                                // 활성화된것만
                                colDown.Add(blocks[j]);
                            }
                        }
                    }
                }

                // Col 블럭을 내리는 작업
                if (colDown.Count > 0)
                {
                    for (int i = 0; i < colDown.Count; i++)
                    {
                        var targetrow = (colDown[i].row -= 1);

                        if (Mathf.Abs(targetrow - colDown[i].transform.localPosition.y) > .1f)
                        {
                            Vector2 tempposition = new Vector2(colDown[i].transform.localPosition.x, targetrow);
                            colDown[i].transform.localPosition = Vector2.Lerp(colDown[i].transform.localPosition, tempposition, .05f);
                        }
                    }
                }

                // 빈 곳 탐색
                int num = 0;

                //for (int row = -3; row < 4; row++)
                //{
                //    for (int col = -3; col < 4; col++)
                //    {
                //        if (arrBlocks[num] == null)
                //        {
                //            // 블럭이 존재하지 않으므로 블럭 생성
                //            var block = blockPool.GetPoolableObject(obj => obj.CanRecycle);

                //            // 부모 설정
                //            block.transform.SetParent(this.transform);

                //            /// 여기서 TargetRow를 설정해주는게 좋을거 같은데

                //            // 위치값 설정
                //            block.transform.localPosition = new Vector3(col, row, 0);

                //            // 회전값 설정
                //            block.transform.localRotation = Quaternion.identity;

                //            // 사이즈값 설정
                //            block.transform.localScale = new Vector3(.19f, .19f, .19f);

                //            // 블럭 초기화
                //            block.Initialize(col, row);

                //            newBlock.Add(block);

                //            arrBlocks[num] = block;

                //            // 활성화는 나중에 내릴 때 할거임
                //        }

                //        num++;
                //    }
                //}

                /*
                // 내릴 Row 블럭 찾기
                for (int row = 0; row < rowDel.Count; row++)
                {
                    for (int col = 0; col < blocks.Count; col++)
                    {
                        if (blocks[col].col == rowDel[row].col && blocks[col].row > rowDel[row].row)
                        {
                            if (rowDown.Count > 0 && col > 0)
                            {
                                // 중복체크
                                for (int c = 0; c < rowDown.Count; c++)
                                {
                                    if (rowDown[c].row != blocks[col].row)
                                    {
                                        // 활성화 상태만 저장
                                        if (blocks[col].gameObject.activeSelf)
                                        {
                                            rowDown.Add(blocks[col]);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                */


                // 빈자리 채우기
                for (int i = 0; i < newBlock.Count; i++)
                {
                    int myRow = 0;

                    // 위치값 설정
                    newBlock[i].transform.localPosition = new Vector3(newBlock[i].col, 4, 0);
                    newBlock[i].row = 4;

                    newBlock[i].gameObject.SetActive(true);

                    // 위에서 아래로 작업
                    var targetRow = (newBlock[i].row = myRow);

                    if (Mathf.Abs(targetRow - newBlock[i].transform.localPosition.y) > .1f)
                    {
                        Vector2 tempPosition = new Vector2(newBlock[i].transform.localPosition.x, targetRow);
                        newBlock[i].transform.localPosition = Vector2.Lerp(newBlock[i].transform.localPosition, tempPosition, .05f);
                    }
                }

                // 무조건 리스트 클리어 작업 해줘야 한다
            }
        }

        public IEnumerator Test()
        {
            yield return null;

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if ((col != 2 || col != 3) && arrBlocks[row, col] != null)
                    {
                        Block block_0 = arrBlocks[row, col];
                        Block block_1 = arrBlocks[row, col - 1];
                        Block block_2 = arrBlocks[row, col - 2];

                        if (block_0.elementType == block_1.elementType && block_1.elementType == block_2.elementType)
                        {
                            arrBlocks[row, col] = null;
                            arrBlocks[row, col - 1] = null;
                            arrBlocks[row, col - 2] = null;
                        }
                    }
                }
            }



        }

        /// 블럭 클리어 및 블럭 생성을 담당하는 메서드
        /// </summary>
        /// <returns></returns>
        public IEnumerator BlockClear()
        {

            var blockPool = ObjectPoolManager.Instance.GetPool<Block>(PoolType.Block);
            var size = (GM.BoardSize.x * GM.BoardSize.y);
            var uiElement = UIWindowManager.Instance.GetWindow<UIElement>();

            // 3X3 작업
            for (int i = 0; i < blocks.Count; i++)
            {
                // Col
                if (blocks[i].leftBlock != null && blocks[i].rightBlock && blocks[i].leftBlock.elementType == blocks[i].elementType &&
                    blocks[i].rightBlock.elementType == blocks[i].elementType)
                {
                    var col_L = blocks[i].leftBlock.col;
                    var col_M = blocks[i].col;
                    var col_R = blocks[i].rightBlock.col;
                    var row_M = blocks[i].row;

                    colDel.Add(blocks[i]);
                    colDel.Add(blocks[i].leftBlock);
                    colDel.Add(blocks[i].rightBlock);


                    // 파티클 실행
                    // curBlock.BlockParticle();
                    // curBlock.leftBlock.BlockParticle();
                    // curBlock.rightBlock.BlockParticle();

                    yield return new WaitForSeconds(.5f);

                    // 풀 반환 및 점수 업데이트
                    for (int j = 0; j < colDel.Count; j++)
                    {
                        blockPool.ReturnPoolableObject(colDel[j]);

                        //uiElement.SetGauge(delBlocks[row].ElementValue);

                        blocks.Remove(colDel[j]);

                        //if (GM.isPlus)
                        //{
                        //    DM.SetScore(delBlocks[row].BlockScore * 2);
                        //}
                        //else
                        //{
                        //    DM.SetScore(delBlocks[row].BlockScore);
                        //}
                    }

                    // 맨 위에 있는 블럭인지 확인
                    if (row_M != 3)
                    {
                        // 삭제 블럭 위에 존재하는 블럭 탐색
                        for (int j = 0; j < blocks.Count; j++)
                        {
                            if ((blocks[j].col == col_L || blocks[j].col == col_M || blocks[j].col == col_R) &&
                                blocks[j].row > row_M)
                            {
                                // 내릴 블럭 저장
                                downBlocks.Add(blocks[j]);
                            }
                        }
                    }

                    yield return new WaitForSeconds(.4f);

                    // 블럭을 내리는 작업
                    for (int j = 0; j < downBlocks.Count; j++)
                    {
                        var targetRow = (downBlocks[j].row -= 1);

                        if (Mathf.Abs(targetRow - downBlocks[j].transform.position.y) > .1f)
                        {
                            Vector2 tempPosition = new Vector2(downBlocks[j].transform.position.x, targetRow);
                            downBlocks[j].transform.position = Vector2.Lerp(downBlocks[j].transform.position, tempPosition, .05f);
                        }
                    }

                    //// 비어있는 칸의 개수
                    //var emptyCount = size - blocks.Count;
                    //var col_NewNum = col_L;
                    //var row_NewNum = downBlocks.Count > 0 ? downBlocks[downBlocks.Count - 1].row + 1 : row_M;

                    //yield return new WaitForSeconds(.4f);

                    //// 빈 공간에 블럭 생성 작업
                    //for (int row = 0; row < emptyCount; row++)
                    //{
                    //    if (col_NewNum <= col_R && row_NewNum < GM.BoardSize.y)
                    //    {
                    //        var newBlock = blockPool.GetPoolableObject(obj => obj.CanRecycle);
                    //        newBlock.transform.position = new Vector3(col_NewNum, row_NewNum, 0);
                    //        newBlock.gameObject.SetActive(true);
                    //        newBlock.Initialize(col_NewNum, row_NewNum);

                    //        blocks.Add(newBlock);

                    //        col_NewNum++;
                    //    }

                    //    if (col_NewNum > col_R)
                    //    {
                    //        // 다음 줄을 채우기 위한 작업
                    //        col_NewNum = col_L;
                    //        row_NewNum++;
                    //    }
                    //}

                    colDel.Clear();
                    downBlocks.Clear();
                }

                // Top, Bottom
                //if (curBlock.topBlock != null && curBlock.bottomBlock != null)
                //{
                //    if (curBlock.topBlock.elementType == curBlock.elementType && curBlock.bottomBlock.elementType == curBlock.elementType)
                //    {
                //        yield return new WaitForSeconds(.2f);

                //        delBlocks.Clear();
                //        downBlocks.Clear();

                //        delBlocks.Add(curBlock.topBlock);
                //        delBlocks.Add(curBlock.bottomBlock);
                //        delBlocks.Add(curBlock);

                //        파티클 실행
                //        curBlock.BlockParticle();
                //        curBlock.topBlock.BlockParticle();
                //        curBlock.bottomBlock.BlockParticle();

                //        yield return new WaitForSeconds(.5f);

                //        for (int row = 0; row < delBlocks.Count; row++)
                //        {
                //            blockPool.ReturnPoolableObject(delBlocks[row]);
                //            uiElement.SetGauge(delBlocks[row].ElementValue);
                //            blocks.Remove(delBlocks[row]);

                //            if (GM.isPlus)
                //            {
                //                DM.SetScore(delBlocks[row].BlockScore * 2);
                //            }
                //            else
                //            {
                //                DM.SetScore(delBlocks[row].BlockScore);
                //            }
                //        }

                //        var col_B = curBlock.col;
                //        var row_B = curBlock.topBlock.row;

                //        downBlocks.Clear();

                //        맨 위 블럭인지 확인
                //        if (row_B != (GM.BoardSize.y - 1))
                //        {
                //            내릴 블럭 탐색
                //            for (int row = 0; row < blocks.Count; row++)
                //            {
                //                if ((col_B == blocks[row].col) && (row_B < blocks[row].row))
                //                {
                //                    downBlocks.Add(blocks[row]);
                //                }
                //            }
                //        }

                //        yield return new WaitForSeconds(.4f);

                //        블럭 내리는 작업
                //        for (int row = 0; row < downBlocks.Count; row++)
                //        {
                //            var targetRow = (downBlocks[row].row -= 3);

                //            if (Mathf.Abs(targetRow - downBlocks[row].transform.position.y) > .1f)
                //            {
                //                Vector2 tempPosition = new Vector2(downBlocks[row].transform.position.x, targetRow);
                //                downBlocks[row].transform.position = Vector2.Lerp(downBlocks[row].transform.position, tempPosition, .05f);
                //            }
                //        }

                //        비어있는 칸 개수
                //       var emptyBlockCount = size - blocks.Count;

                //        var n_Row = downBlocks.Count > 0 ? downBlocks[downBlocks.Count - 1].row + 1 : row_B - 2;

                //        yield return new WaitForSeconds(.4f);

                //        for (int row = 0; row < emptyBlockCount; row++)
                //        {
                //            if (n_Row < GM.BoardSize.y)
                //            {
                //                var newBlock = blockPool.GetPoolableObject(obj => obj.CanRecycle);
                //                newBlock.transform.position = new Vector3(col_B, n_Row, 0);
                //                newBlock.gameObject.SetActive(true);
                //                newBlock.Initialize(col_B, n_Row);

                //                blocks.Add(newBlock);

                //                n_Row++;
                //            }
                //        }

                //        delBlocks.Clear();
                //        downBlocks.Clear();
                //        BlockUpdate();

                //        / TestBoard
                //        row = 0;
                //    }
            }

            // 다시 한번더 체크
            if (BlockCheck())
            {
                GM.isStart = true;

                if (GM.GameState != GameState.SKill)
                {
                    GM.SetGameState(GameState.Checking);
                }
            }
            else
            {
                // 불 스킬을 사용중일땐 안들어가도록
                if (GM.GameState == GameState.SKill && GM.ElementType == ElementType.Fire)
                {
                    Debug.Log("안됩니다.");
                }
                else
                {
                    GM.SetGameState(GameState.Play);
                }
            }
        }

        /// <summary>
        /// 블럭 주위에 같은 블럭이 있는지 체크하는 메서드
        /// </summary>
        /// <param name="checkBlock"></param>
        /// <param name="otherBlock"></param>
        /// <param name="swipeDir"></param>
        /// <returns></returns>
        public bool BlockCheck(Block checkBlock = null, Block otherBlock = null, SwipeDir swipeDir = SwipeDir.None)
        {
            if (checkBlock != null || otherBlock != null)
            {
                // 같은 블럭 개수
                var count_T = 0;
                var count_B = 0;
                var count_M = 0;
                var count_L = 0;
                var count_R = 0;
                var count_M2 = 0;

                switch (swipeDir)
                {
                    case SwipeDir.None:
                        // OtherBlock 매칭 블럭 탐색 작업
                        for (int i = 0; i < blocks.Count; i++)
                        {
                            // Top
                            if ((otherBlock.row + 1 == blocks[i].row || otherBlock.row + 2 == blocks[i].row) &&
                                otherBlock.col == blocks[i].col)
                            {
                                if (otherBlock.elementType == blocks[i].elementType)
                                {
                                    count_T++;
                                }
                            }

                            // Col Middle
                            if ((otherBlock.col + 1 == blocks[i].col || otherBlock.col - 1 == blocks[i].col) &&
                                otherBlock.row == blocks[i].row)
                            {
                                if (otherBlock.elementType == blocks[i].elementType)
                                {
                                    count_M++;
                                }
                            }

                            // Row Middle
                            if ((otherBlock.row + 1 == blocks[i].row || otherBlock.row - 1 == blocks[i].row) &&
                                otherBlock.col == blocks[i].col)
                            {
                                if (otherBlock.elementType == blocks[i].elementType)
                                {
                                    count_M2++;
                                }
                            }

                            // Bottom
                            if ((otherBlock.row - 1 == blocks[i].row || otherBlock.row - 2 == blocks[i].row) &&
                                otherBlock.col == blocks[i].col)
                            {
                                if (otherBlock.elementType == blocks[i].elementType)
                                {
                                    count_B++;
                                }
                            }

                            // Left
                            if ((otherBlock.col - 1 == blocks[i].col || otherBlock.col - 2 == blocks[i].col) &&
                                otherBlock.row == blocks[i].row)
                            {
                                if (otherBlock.elementType == blocks[i].elementType)
                                {
                                    count_L++;
                                }
                            }

                            // Right
                            if ((otherBlock.col + 1 == blocks[i].col || otherBlock.col + 2 == blocks[i].col) &&
                                otherBlock.row == blocks[i].row)
                            {
                                if (otherBlock.elementType == blocks[i].elementType)
                                {
                                    count_R++;
                                }
                            }
                        }
                        break;

                    case SwipeDir.Top:
                        for (int i = 0; i < blocks.Count; i++)
                        {
                            // Top
                            if ((checkBlock.row + 1 == blocks[i].row || checkBlock.row + 2 == blocks[i].row) &&
                                checkBlock.col == blocks[i].col)
                            {
                                if (checkBlock.elementType == blocks[i].elementType)
                                {
                                    count_T++;
                                }
                            }

                            // Middle
                            if ((checkBlock.col - 1 == blocks[i].col || checkBlock.col + 1 == blocks[i].col) &&
                                checkBlock.row == blocks[i].row)
                            {
                                if (checkBlock.elementType == blocks[i].elementType)
                                {
                                    count_M++;
                                }
                            }

                            // Left
                            if ((checkBlock.col - 1 == blocks[i].col || checkBlock.col - 2 == blocks[i].col) &&
                                checkBlock.row == blocks[i].row)
                            {
                                if (checkBlock.elementType == blocks[i].elementType)
                                {
                                    count_L++;
                                }
                            }

                            // Right
                            if ((checkBlock.col + 1 == blocks[i].col || checkBlock.col + 2 == blocks[i].col) &&
                                checkBlock.row == blocks[i].row)
                            {
                                if (checkBlock.elementType == blocks[i].elementType)
                                {
                                    count_R++;
                                }
                            }
                        }
                        break;

                    case SwipeDir.Bottom:
                        for (int i = 0; i < blocks.Count; i++)
                        {
                            // Bottom
                            if ((checkBlock.row - 1 == blocks[i].row || checkBlock.row - 2 == blocks[i].row) &&
                                checkBlock.col == blocks[i].col)
                            {
                                if (checkBlock.elementType == blocks[i].elementType)
                                {
                                    count_B++;
                                }
                            }

                            // Middle
                            if ((checkBlock.col - 1 == blocks[i].col || checkBlock.col + 1 == blocks[i].col) &&
                                checkBlock.row == blocks[i].row)
                            {
                                if (checkBlock.elementType == blocks[i].elementType)
                                {
                                    count_M++;
                                }
                            }

                            // Left
                            if ((checkBlock.col - 1 == blocks[i].col || checkBlock.col - 2 == blocks[i].col) &&
                                checkBlock.row == blocks[i].row)
                            {
                                if (checkBlock.elementType == blocks[i].elementType)
                                {
                                    count_L++;
                                }
                            }

                            //Right
                            if ((checkBlock.col + 1 == blocks[i].col || checkBlock.col + 2 == blocks[i].col) &&
                                checkBlock.row == blocks[i].row)
                            {
                                if (checkBlock.elementType == blocks[i].elementType)
                                {
                                    count_R++;
                                }
                            }
                        }
                        break;

                    case SwipeDir.Left:
                        for (int i = 0; i < blocks.Count; i++)
                        {
                            // Top
                            if ((checkBlock.row + 1 == blocks[i].row || checkBlock.row + 2 == blocks[i].row) &&
                                checkBlock.col == blocks[i].col)
                            {
                                if (checkBlock.elementType == blocks[i].elementType)
                                {
                                    count_T++;
                                }
                            }

                            // Bottom
                            if ((checkBlock.row - 1 == blocks[i].row || checkBlock.row - 2 == blocks[i].row) &&
                                checkBlock.col == blocks[i].col)
                            {
                                if (checkBlock.elementType == blocks[i].elementType)
                                {
                                    count_B++;
                                }
                            }

                            // Middle
                            if ((checkBlock.row - 1 == blocks[i].row || checkBlock.row + 1 == blocks[i].row) &&
                                checkBlock.col == blocks[i].col)
                            {
                                if (checkBlock.elementType == blocks[i].elementType)
                                {
                                    count_M++;
                                }
                            }

                            // Left
                            if ((checkBlock.col - 1 == blocks[i].col || checkBlock.col - 2 == blocks[i].col) &&
                                checkBlock.row == blocks[i].row)
                            {
                                if (checkBlock.elementType == blocks[i].elementType)
                                {
                                    count_L++;
                                }
                            }
                        }
                        break;

                    case SwipeDir.Right:
                        for (int i = 0; i < blocks.Count; i++)
                        {
                            // Top
                            if ((checkBlock.row + 1 == blocks[i].row || checkBlock.row + 2 == blocks[i].row) &&
                                checkBlock.col == blocks[i].col)
                            {
                                if (blocks[i].elementType == checkBlock.elementType)
                                {
                                    count_T++;
                                }
                            }

                            // Bottom
                            if ((checkBlock.row - 1 == blocks[i].row || checkBlock.row - 2 == blocks[i].row) &&
                                checkBlock.col == blocks[i].col)
                            {
                                if (blocks[i].elementType == checkBlock.elementType)
                                {
                                    count_B++;
                                }
                            }

                            // Middle
                            if ((checkBlock.row - 1 == blocks[i].row || checkBlock.row + 1 == blocks[i].row) &&
                                checkBlock.col == blocks[i].col)
                            {
                                if (blocks[i].elementType == checkBlock.elementType)
                                {
                                    count_M++;
                                }
                            }

                            // Right
                            if ((checkBlock.col + 1 == blocks[i].col || checkBlock.col + 2 == blocks[i].col) &&
                                checkBlock.row == blocks[i].row)
                            {
                                if (blocks[i].elementType == checkBlock.elementType)
                                {
                                    count_R++;
                                }
                            }
                        }
                        break;
                }

                // 매칭 되는 블럭 존재
                if (count_T >= 2 || count_B >= 2 || count_M >= 2 || count_L >= 2 || count_R >= 2 || count_M2 >= 2)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                for (int i = 0; i < blocks.Count; i++)
                {
                    // Col
                    if ((blocks[i].leftBlock != null && blocks[i].rightBlock != null) &&
                        (blocks[i].elementType == blocks[i].leftBlock.elementType && blocks[i].elementType == blocks[i].rightBlock.elementType))
                    {
                        return true;
                    }

                    // Row
                    if ((blocks[i].topBlock != null && blocks[i].bottomBlock != null) &&
                        (blocks[i].elementType == blocks[i].topBlock.elementType && blocks[i].elementType == blocks[i].bottomBlock.elementType))
                    {
                        return true;
                    }
                }

                return false;
            }
        }
    }
}