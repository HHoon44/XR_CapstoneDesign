using Mono.Cecil.Cil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XR_3MatchGame.Util;
using XR_3MatchGame_Data;
using XR_3MatchGame_InGame;
using XR_3MatchGame_UI;
using XR_3MatchGame_Util;

namespace XR_3MatchGame_Object
{
    public class Board : MonoBehaviour
    {
        [Header("InGame Blocks")]
        public List<Block> blocks = new List<Block>();               // 인 게임 블럭 리스트
        public List<Block> downBlocks = new List<Block>();           // 아래 이동 블럭 리스트
        public List<Block> delBlocks = new List<Block>();            // 삭제 블럭 리스트

        [SerializeField]
        private GameObject uiEnd;

        private GameManager GM;
        private DataManager DM;

        public bool isReStart;

        private void Start()
        {
            GM = GameManager.Instance;
            DM = DataManager.Instance;
            GM.Initialize(this);

            StartCoroutine(SpawnBlock());
        }

        private void Update()
        {
            if (GM.GameState == GameState.Checking)
            {
                if (GM.isStart == true)
                {
                    GM.isStart = false;
                    StartCoroutine(BlockClear());
                }
            }

            // 게임 종료
            if (GM.GameState == GameState.End)
            {
                uiEnd.gameObject.SetActive(true);

                uiEnd.GetComponent<UIEnd>().Initialize();

                var pool = ObjectPoolManager.Instance.GetPool<Block>(PoolType.Block);

                for (int i = 0; i < blocks.Count; i++)
                {
                    pool.ReturnPoolableObject(blocks[i]);
                }

                blocks.Clear();

                gameObject.SetActive(false);
            }

            // 게임 재시작
            if (isReStart == true)
            {
                isReStart = false;

                StartCoroutine(SpawnBlock());
            }
        }

        public IEnumerator SpawnBlock()
        {
            GM.SetGameState(GameState.Checking);

            yield return new WaitForSeconds(.5f);

            var blockPool = ObjectPoolManager.Instance.GetPool<Block>(PoolType.Block);
            var size = GameManager.Instance.BoardSize;

            // 블럭 세팅 작업
            for (int row = 0; row < size.y; row++)
            {
                for (int col = 0; col < size.x; col++)
                {
                    var block = blockPool.GetPoolableObject(obj => obj.CanRecycle);
                    block.transform.position = new Vector3(col, row, 0);
                    block.Initialize(col, row);
                    block.gameObject.SetActive(true);

                    blocks.Add(block);
                }
            }

            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
                uiEnd.gameObject.SetActive(false);
            }

            BlockUpdate();
            StartCoroutine(BlockClear());
        }

        /// <summary>
        /// 블럭의 주위의 블럭들에 대한 데이터를 업데이트하는 메서드
        /// </summary>
        public void BlockUpdate()
        {
            // Top = 6, Bottom = 0
            // Left = 0, Right = 6
            for (int i = 0; i < blocks.Count; i++)
            {
                for (int j = 0; j < blocks.Count; j++)
                {
                    // Top
                    if (blocks[i].row == 6)
                    {
                        blocks[i].topBlock = null;
                        blocks[i].Top_T = ElementType.None;
                    }
                    else
                    {
                        if (blocks[i].row + 1 == blocks[j].row && blocks[i].col == blocks[j].col)
                        {
                            blocks[i].topBlock = blocks[j];

                            // Test
                            blocks[i].Top_T = blocks[j].elementType;
                        }
                    }

                    // Bottom
                    if (blocks[i].row == 0)
                    {
                        blocks[i].bottomBlock = null;
                        blocks[i].Bottom_T = ElementType.None;
                    }
                    else
                    {
                        if (blocks[i].row - 1 == blocks[j].row && blocks[i].col == blocks[j].col)
                        {
                            blocks[i].bottomBlock = blocks[j];

                            // Test
                            blocks[i].Bottom_T = blocks[j].elementType;
                        }
                    }

                    // Left
                    if (blocks[i].col == 0)
                    {
                        // 현재 블럭은 Col = 0에 존재하는 블럭
                        blocks[i].leftBlock = null;
                        blocks[i].Left_T = ElementType.None;
                    }
                    else
                    {
                        if (blocks[i].col - 1 == blocks[j].col && blocks[i].row == blocks[j].row)
                        {
                            blocks[i].leftBlock = blocks[j];

                            // Test
                            blocks[i].Left_T = blocks[j].elementType;
                        }
                    }

                    // Right
                    if (blocks[i].col == 6)
                    {
                        // 현재 블럭은 Col = 6에 존재하는 블럭
                        blocks[i].rightBlock = null;
                        blocks[i].Right_T = ElementType.None;
                    }
                    else
                    {
                        if (blocks[i].col + 1 == blocks[j].col && blocks[i].row == blocks[j].row)
                        {
                            blocks[i].rightBlock = blocks[j];

                            // Test
                            blocks[i].Right_T = blocks[j].elementType;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 블럭 클리어 및 블럭 생성을 담당하는 메서드
        /// </summary>
        /// <returns></returns>
        public IEnumerator BlockClear()
        {
            Block curBlock = null;
            var blockPool = ObjectPoolManager.Instance.GetPool<Block>(PoolType.Block);
            var size = (GM.BoardSize.x * GM.BoardSize.y);
            var uiElement = UIWindowManager.Instance.GetWindow<UIElement>();

            for (int i = 0; i < blocks.Count; i++)
            {
                curBlock = blocks[i];

                // Left, Right
                if (curBlock.leftBlock != null && curBlock.rightBlock != null)
                {
                    if (curBlock.leftBlock.elementType == curBlock.elementType && curBlock.rightBlock.elementType == curBlock.elementType)
                    {
                        yield return new WaitForSeconds(.2f);

                        // 삭제할 블럭들 삭제 저장소에 저장
                        delBlocks.Add(curBlock);
                        delBlocks.Add(curBlock.leftBlock);
                        delBlocks.Add(curBlock.rightBlock);

                        var col_L = curBlock.leftBlock.col;
                        var col_M = curBlock.col;
                        var col_R = curBlock.rightBlock.col;
                        var row_M = curBlock.row;

                        // 풀 반환 및 점수 업데이트
                        for (int j = 0; j < delBlocks.Count; j++)
                        {
                            blockPool.ReturnPoolableObject(delBlocks[j]);
                            DM.SetScore(delBlocks[j].BlockScore);
                            uiElement.SetSkillAmount(delBlocks[j].ElementValue);
                            blocks.Remove(delBlocks[j]);
                        }

                        // 맨 위에 있는 블럭인지 확인
                        if (row_M != (GM.BoardSize.y - 1))
                        {
                            downBlocks.Clear();

                            // 삭제 블럭 위에 존재하는 블럭 탐색
                            for (int j = 0; j < blocks.Count; j++)
                            {
                                if ((blocks[j].col == col_L || blocks[j].col == col_M || blocks[j].col == col_R) && blocks[j].row > row_M)
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
                            var targetRow = downBlocks[j].row -= 1;

                            if (Mathf.Abs(targetRow - downBlocks[j].transform.position.y) > .1f)
                            {
                                Vector2 tempPosition = new Vector2(downBlocks[j].transform.position.x, targetRow);
                                downBlocks[j].transform.position = Vector2.Lerp(downBlocks[j].transform.position, tempPosition, .05f);
                            }
                        }

                        // 비어있는 칸의 개수
                        var emptyCount = size - blocks.Count;
                        var col_NewNum = col_L;
                        var row_NewNum = downBlocks.Count > 0 ? downBlocks[downBlocks.Count - 1].row + 1 : row_M;

                        yield return new WaitForSeconds(.4f);

                        // 빈 공간에 블럭 생성 작업
                        for (int j = 0; j < emptyCount; j++)
                        {
                            if (col_NewNum <= col_R && row_NewNum < GM.BoardSize.y)
                            {
                                var newBlock = blockPool.GetPoolableObject(obj => obj.CanRecycle);
                                newBlock.transform.position = new Vector3(col_NewNum, row_NewNum, 0);
                                newBlock.gameObject.SetActive(true);
                                newBlock.Initialize(col_NewNum, row_NewNum);

                                blocks.Add(newBlock);

                                col_NewNum++;
                            }

                            if (col_NewNum > col_R)
                            {
                                // 다음 줄을 채우기 위한 작업
                                col_NewNum = col_L;
                                row_NewNum++;
                            }
                        }

                        delBlocks.Clear();
                        downBlocks.Clear();
                        BlockUpdate();
                    }
                }

                // Top, Bottom
                if (curBlock.topBlock != null && curBlock.bottomBlock != null)
                {
                    if (curBlock.topBlock.elementType == curBlock.elementType && curBlock.bottomBlock.elementType == curBlock.elementType)
                    {
                        yield return new WaitForSeconds(.2f);

                        delBlocks.Add(curBlock.topBlock);
                        delBlocks.Add(curBlock.bottomBlock);
                        delBlocks.Add(curBlock);

                        for (int j = 0; j < delBlocks.Count; j++)
                        {
                            blockPool.ReturnPoolableObject(delBlocks[j]);
                            DM.SetScore(delBlocks[j].BlockScore);
                            uiElement.SetSkillAmount(delBlocks[j].ElementValue);
                            blocks.Remove(delBlocks[j]);
                        }

                        var col_B = curBlock.col;
                        var row_B = curBlock.topBlock.row;

                        downBlocks.Clear();

                        // 맨 위 블럭인지 확인
                        if (row_B != (GM.BoardSize.y - 1))
                        {
                            // 내릴 블럭 탐색
                            for (int j = 0; j < blocks.Count; j++)
                            {
                                if ((col_B == blocks[j].col) && (row_B < blocks[j].row))
                                {
                                    downBlocks.Add(blocks[j]);
                                }
                            }
                        }

                        yield return new WaitForSeconds(.4f);

                        // 블럭 내리는 작업
                        for (int j = 0; j < downBlocks.Count; j++)
                        {
                            var targetRow = downBlocks[j].row -= 3;

                            if (Mathf.Abs(targetRow - downBlocks[j].transform.position.y) > .1f)
                            {
                                Vector2 tempPosition = new Vector2(downBlocks[j].transform.position.x, targetRow);
                                downBlocks[j].transform.position = Vector2.Lerp(downBlocks[j].transform.position, tempPosition, .05f);
                            }
                        }

                        // 비어있는 칸 개수
                        var emptyBlockCount = size - blocks.Count;

                        var n_Row = downBlocks.Count > 0 ? downBlocks[downBlocks.Count - 1].row + 1 : row_B - 2;

                        yield return new WaitForSeconds(.4f);

                        for (int j = 0; j < emptyBlockCount; j++)
                        {
                            if (n_Row < GM.BoardSize.y)
                            {
                                var newBlock = blockPool.GetPoolableObject(obj => obj.CanRecycle);
                                newBlock.transform.position = new Vector3(col_B, n_Row, 0);
                                newBlock.gameObject.SetActive(true);
                                newBlock.Initialize(col_B, n_Row);

                                blocks.Add(newBlock);

                                n_Row++;
                            }
                        }

                        delBlocks.Clear();
                        downBlocks.Clear();
                        BlockUpdate();
                    }
                }
            }

            yield return new WaitForSeconds(.4f);

            if (BlockCheck())
            {
                GM.isStart = true;
                GM.SetGameState(GameState.Checking);
            }
            else
            {
                GM.SetGameState(GameState.Play);
            }
        }

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
                            if ((otherBlock.row + 1 == blocks[i].row || otherBlock.row + 2 == blocks[i].row) && otherBlock.col == blocks[i].col)
                            {
                                if (otherBlock.elementType == blocks[i].elementType)
                                {
                                    count_T++;
                                }
                            }

                            // Horizontal Middle
                            if ((otherBlock.col + 1 == blocks[i].col || otherBlock.col - 1 == blocks[i].col) && otherBlock.row == blocks[i].row)
                            {
                                if (otherBlock.elementType == blocks[i].elementType)
                                {
                                    count_M++;
                                }
                            }

                            // Vertical Middle
                            // Horizontal에서 매칭되는 블럭이 없으므로 재사용
                            if ((otherBlock.row + 1 == blocks[i].row || otherBlock.row - 1 == blocks[i].row) && otherBlock.col == blocks[i].col)
                            {
                                if (otherBlock.elementType == blocks[i].elementType)
                                {
                                    count_M2++;
                                }
                            }

                            // Bottom
                            if ((otherBlock.row - 1 == blocks[i].row || otherBlock.row - 2 == blocks[i].row) && otherBlock.col == blocks[i].col)
                            {
                                if (otherBlock.elementType == blocks[i].elementType)
                                {
                                    count_B++;
                                }
                            }

                            // Left
                            if ((otherBlock.col - 1 == blocks[i].col || otherBlock.col - 2 == blocks[i].col) && otherBlock.row == blocks[i].row)
                            {
                                if (otherBlock.elementType == blocks[i].elementType)
                                {
                                    count_L++;
                                }
                            }

                            // Right
                            if ((otherBlock.col + 1 == blocks[i].col || otherBlock.col + 2 == blocks[i].col) && otherBlock.row == blocks[i].row)
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
                            if ((checkBlock.row + 1 == blocks[i].row || checkBlock.row + 2 == blocks[i].row) && checkBlock.col == blocks[i].col)
                            {
                                if (checkBlock.elementType == blocks[i].elementType)
                                {
                                    count_T++;
                                }
                            }

                            // Middle
                            if ((checkBlock.col - 1 == blocks[i].col || checkBlock.col + 1 == blocks[i].col) && checkBlock.row == blocks[i].row)
                            {
                                if (checkBlock.elementType == blocks[i].elementType)
                                {
                                    count_M++;
                                }
                            }

                            // Left
                            if ((checkBlock.col - 1 == blocks[i].col || checkBlock.col - 2 == blocks[i].col) && checkBlock.row == blocks[i].row)
                            {
                                if (checkBlock.elementType == blocks[i].elementType)
                                {
                                    count_L++;
                                }
                            }

                            // Right
                            if ((checkBlock.col + 1 == blocks[i].col || checkBlock.col + 2 == blocks[i].col) && checkBlock.row == blocks[i].row)
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
                            if ((checkBlock.row - 1 == blocks[i].row || checkBlock.row - 2 == blocks[i].row) && checkBlock.col == blocks[i].col)
                            {
                                if (checkBlock.elementType == blocks[i].elementType)
                                {
                                    count_B++;
                                }
                            }

                            // Middle
                            if ((checkBlock.col - 1 == blocks[i].col || checkBlock.col + 1 == blocks[i].col) && checkBlock.row == blocks[i].row)
                            {
                                if (checkBlock.elementType == blocks[i].elementType)
                                {
                                    count_M++;
                                }
                            }

                            // Left
                            if ((checkBlock.col - 1 == blocks[i].col || checkBlock.col - 2 == blocks[i].col) && checkBlock.row == blocks[i].row)
                            {
                                if (checkBlock.elementType == blocks[i].elementType)
                                {
                                    count_L++;
                                }
                            }

                            //Right
                            if ((checkBlock.col + 1 == blocks[i].col || checkBlock.col + 2 == blocks[i].col) && checkBlock.row == blocks[i].row)
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
                            if ((checkBlock.row + 1 == blocks[i].row || checkBlock.row + 2 == blocks[i].row) && checkBlock.col == blocks[i].col)
                            {
                                if (checkBlock.elementType == blocks[i].elementType)
                                {
                                    count_T++;
                                }
                            }

                            // Bottom
                            if ((checkBlock.row - 1 == blocks[i].row || checkBlock.row - 2 == blocks[i].row) && checkBlock.col == blocks[i].col)
                            {
                                if (checkBlock.elementType == blocks[i].elementType)
                                {
                                    count_B++;
                                }
                            }

                            // Middle
                            if ((checkBlock.row - 1 == blocks[i].row || checkBlock.row + 1 == blocks[i].row) && checkBlock.col == blocks[i].col)
                            {
                                if (checkBlock.elementType == blocks[i].elementType)
                                {
                                    count_M++;
                                }
                            }

                            // Left
                            if ((checkBlock.col - 1 == blocks[i].col || checkBlock.col - 2 == blocks[i].col) && checkBlock.row == blocks[i].row)
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
                            if ((checkBlock.row + 1 == blocks[i].row || checkBlock.row + 2 == blocks[i].row) && checkBlock.col == blocks[i].col)
                            {
                                if (blocks[i].elementType == checkBlock.elementType)
                                {
                                    count_T++;
                                }
                            }

                            // Bottom
                            if ((checkBlock.row - 1 == blocks[i].row || checkBlock.row - 2 == blocks[i].row) && checkBlock.col == blocks[i].col)
                            {
                                if (blocks[i].elementType == checkBlock.elementType)
                                {
                                    count_B++;
                                }
                            }

                            // Middle
                            if ((checkBlock.row - 1 == blocks[i].row || checkBlock.row + 1 == blocks[i].row) && checkBlock.col == blocks[i].col)
                            {
                                if (blocks[i].elementType == checkBlock.elementType)
                                {
                                    count_M++;
                                }
                            }

                            // Right
                            if ((checkBlock.col + 1 == blocks[i].col || checkBlock.col + 2 == blocks[i].col) && checkBlock.row == blocks[i].row)
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
                    if (blocks[i].leftBlock != null && blocks[i].rightBlock != null)
                    {
                        if (blocks[i].elementType == blocks[i].leftBlock.elementType && blocks[i].elementType == blocks[i].rightBlock.elementType)
                        {
                            return true;
                        }
                    }

                    if (blocks[i].topBlock != null && blocks[i].bottomBlock != null)
                    {
                        if (blocks[i].elementType == blocks[i].topBlock.elementType &&
                            blocks[i].elementType == blocks[i].bottomBlock.elementType)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }
    }
}