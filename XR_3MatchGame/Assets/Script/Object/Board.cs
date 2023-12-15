using System.Collections;
using System.Data;
using UnityEngine;
using XR_3MatchGame.Util;
using XR_3MatchGame_Data;
using XR_3MatchGame_InGame;
using XR_3MatchGame_Resource;
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

        public Block[,] blocks = new Block[7, 7];

        public Block moveBlock;
        public Block boomBlock;

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

            GM.Board = this;

            // BGM 실행
            SoundManager.Instance.Initialize(SceneType.InGame);

            // 스폰 시작
            StartCoroutine(SpawnBlock());
        }

        private void Update()
        {
            if (GM.isMatch == true)
            {
                BlockSort();

                GM.isMatch = false;

                StartCoroutine(BlockMatch());
            }
        }

        public void SetState(GameState gameState)
        {
            switch (gameState)
            {
                case GameState.Move:
                    break;

                case GameState.Play:
                    break;

                case GameState.Checking:
                    break;

                case GameState.SKill:
                    break;

                case GameState.End:
                    break;

                case GameState.Boom:
                    // GameState -> Boom
                    GM.SetGameState(gameState);
                    StartCoroutine(BoomFun(boomBlock.col));
                    break;
            }
        }

        /// <summary>
        /// 보드에 블럭을 생성하는 메서드
        /// </summary>
        /// <returns></returns>
        public IEnumerator SpawnBlock()
        {
            GM.SetGameState(GameState.Move);

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
                    block.Initialize(-3 + col, 4 + row);

                    // 위에서 아래로 작업
                    var targetRow = (block.row = -3 + row);

                    if (Mathf.Abs(targetRow - block.transform.localPosition.y) > .1f)
                    {
                        Vector2 tempPosition = new Vector2(block.transform.localPosition.x, targetRow);
                        block.transform.localPosition = Vector2.Lerp(block.transform.localPosition, tempPosition, .05f);
                    }

                    blocks[row, col] = block;
                }

                yield return new WaitForSeconds(.3f);
            }

            GM.SetGameState(GameState.Checking);

            StartCoroutine(BlockMatch());
        }

        /// <summary>
        /// 블럭 매칭을 시작하는 코루틴
        /// </summary>
        /// <returns></returns>
        public IEnumerator BlockMatch()
        {
            // 4X4 먼저 체크
            StartCoroutine(MakeBoom());

            yield return new WaitForSeconds(.3f);

            var blockPool = ObjectPoolManager.Instance.GetPool<Block>(PoolType.Block);
            var uiElement = UIWindowManager.Instance.GetWindow<UIElement>();

            Block checkBlock = null;

            Block col_0 = null;
            Block col_1 = null;

            Block row_0 = null;
            Block row_1 = null;

            // 3X3 블록 파괴 - 예외 상황
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    // Col
                    if (blocks[row, col] != null)
                    {
                        checkBlock = blocks[row, col];

                        if (checkBlock.col != 2 && checkBlock.col != 3)
                        {
                            col_0 = blocks[row, col + 1];
                            col_1 = blocks[row, col + 2];

                            // 1번째 Col 특수 상황 모음집 
                            if ((checkBlock.row != 2 && checkBlock.row != 3) && (col_0 != null && col_1 != null))
                            {
                                /*
                                 * O
                                 * O
                                 * O O O
                                 */
                                row_0 = blocks[row + 1, col];
                                row_1 = blocks[row + 2, col];

                                if (row_0 != null && row_1 != null)
                                {
                                    if ((checkBlock.elementType == col_0.elementType && checkBlock.elementType == col_1.elementType) &&
                                        (checkBlock.elementType == row_0.elementType && checkBlock.elementType == row_1.elementType))
                                    {
                                        Debug.Log("특수 상황");

                                        // 점수 업데이트
                                        DM.SetScore(checkBlock.BlockScore);
                                        DM.SetScore(col_0.BlockScore);
                                        DM.SetScore(col_1.BlockScore);
                                        DM.SetScore(row_0.BlockScore);
                                        DM.SetScore(row_1.BlockScore);

                                        // 스킬 게이지 업데이트
                                        uiElement.SetGauge(checkBlock.ElementValue);
                                        uiElement.SetGauge(col_0.ElementValue);
                                        uiElement.SetGauge(col_1.ElementValue);
                                        uiElement.SetGauge(row_0.ElementValue);
                                        uiElement.SetGauge(row_1.ElementValue);

                                        // 저장소에서 비우기
                                        blocks[row, col] = null;
                                        blocks[row, col + 1] = null;
                                        blocks[row, col + 2] = null;

                                        blocks[row + 1, col] = null;
                                        blocks[row + 2, col] = null;

                                        // 풀에 반환
                                        blockPool.ReturnPoolableObject(checkBlock);
                                        blockPool.ReturnPoolableObject(col_0);
                                        blockPool.ReturnPoolableObject(col_1);
                                        blockPool.ReturnPoolableObject(row_0);
                                        blockPool.ReturnPoolableObject(row_1);
                                    }
                                    else
                                    {
                                        /*
                                         *   O 
                                         *   O 
                                         * O O O
                                         */
                                        row_0 = blocks[row + 1, col + 1];
                                        row_1 = blocks[row + 2, col + 1];

                                        if (row_0 != null && row_1 != null)
                                        {
                                            if ((checkBlock.elementType == col_0.elementType && checkBlock.elementType == col_1.elementType) &&
                                                (checkBlock.elementType == row_0.elementType && checkBlock.elementType == row_1.elementType))
                                            {
                                                // 점수 업데이트
                                                DM.SetScore(checkBlock.BlockScore);
                                                DM.SetScore(col_0.BlockScore);
                                                DM.SetScore(col_1.BlockScore);
                                                DM.SetScore(row_0.BlockScore);
                                                DM.SetScore(row_1.BlockScore);

                                                // 스킬 게이지 업데이트
                                                uiElement.SetGauge(checkBlock.ElementValue);
                                                uiElement.SetGauge(col_0.ElementValue);
                                                uiElement.SetGauge(col_1.ElementValue);
                                                uiElement.SetGauge(row_0.ElementValue);
                                                uiElement.SetGauge(row_1.ElementValue);

                                                // 저장소에서 비우기
                                                blocks[row, col] = null;
                                                blocks[row, col + 1] = null;
                                                blocks[row, col + 2] = null;

                                                blocks[row + 1, col + 1] = null;
                                                blocks[row + 2, col + 1] = null;

                                                // 풀에 반환
                                                blockPool.ReturnPoolableObject(checkBlock);
                                                blockPool.ReturnPoolableObject(col_0);
                                                blockPool.ReturnPoolableObject(col_1);
                                                blockPool.ReturnPoolableObject(row_0);
                                                blockPool.ReturnPoolableObject(row_1);
                                            }
                                            else
                                            {
                                                /*
                                                 *     O
                                                 *     O
                                                 * O O O
                                                 */
                                                row_0 = blocks[row + 1, col + 2];
                                                row_1 = blocks[row + 2, col + 2];

                                                if (row_0 != null && row_1 != null)
                                                {
                                                    if ((checkBlock.elementType == col_0.elementType && checkBlock.elementType == col_1.elementType) &&
                                                        (checkBlock.elementType == row_0.elementType && checkBlock.elementType == row_1.elementType))
                                                    {
                                                        // 점수 업데이트
                                                        DM.SetScore(checkBlock.BlockScore);
                                                        DM.SetScore(col_0.BlockScore);
                                                        DM.SetScore(col_1.BlockScore);
                                                        DM.SetScore(row_0.BlockScore);
                                                        DM.SetScore(row_1.BlockScore);

                                                        // 스킬 게이지 업데이트
                                                        uiElement.SetGauge(checkBlock.ElementValue);
                                                        uiElement.SetGauge(col_0.ElementValue);
                                                        uiElement.SetGauge(col_1.ElementValue);
                                                        uiElement.SetGauge(row_0.ElementValue);
                                                        uiElement.SetGauge(row_1.ElementValue);

                                                        // 저장소에서 비우기
                                                        blocks[row, col] = null;
                                                        blocks[row, col + 1] = null;
                                                        blocks[row, col + 2] = null;

                                                        blocks[row + 1, col + 2] = null;
                                                        blocks[row + 2, col + 2] = null;

                                                        // 풀에 반환
                                                        blockPool.ReturnPoolableObject(checkBlock);
                                                        blockPool.ReturnPoolableObject(col_0);
                                                        blockPool.ReturnPoolableObject(col_1);
                                                        blockPool.ReturnPoolableObject(row_0);
                                                        blockPool.ReturnPoolableObject(row_1);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            // 2번째 Col 특수 상황 모음집
                            if ((checkBlock.row != -2 && checkBlock.row != -3) && (col_0 != null && col_1 != null))
                            {
                                /*
                                 * O O O
                                 * O
                                 * O
                                 */
                                row_0 = blocks[row - 1, col];
                                row_1 = blocks[row - 2, col];

                                if (row_0 != null && row_1 != null)
                                {
                                    if ((checkBlock.elementType == col_0.elementType && checkBlock.elementType == col_1.elementType) &&
                                        (checkBlock.elementType == row_0.elementType && checkBlock.elementType == row_1.elementType))
                                    {
                                        // 점수 업데이트
                                        DM.SetScore(checkBlock.BlockScore);
                                        DM.SetScore(col_0.BlockScore);
                                        DM.SetScore(col_1.BlockScore);
                                        DM.SetScore(row_0.BlockScore);
                                        DM.SetScore(row_1.BlockScore);

                                        // 스킬 게이지 업데이트
                                        uiElement.SetGauge(checkBlock.ElementValue);
                                        uiElement.SetGauge(col_0.ElementValue);
                                        uiElement.SetGauge(col_1.ElementValue);
                                        uiElement.SetGauge(row_0.ElementValue);
                                        uiElement.SetGauge(row_1.ElementValue);

                                        // 저장소에서 비우기
                                        blocks[row, col] = null;
                                        blocks[row, col + 1] = null;
                                        blocks[row, col + 2] = null;

                                        blocks[row - 1, col] = null;
                                        blocks[row - 2, col] = null;

                                        // 풀에 반환
                                        blockPool.ReturnPoolableObject(checkBlock);
                                        blockPool.ReturnPoolableObject(col_0);
                                        blockPool.ReturnPoolableObject(col_1);
                                        blockPool.ReturnPoolableObject(row_0);
                                        blockPool.ReturnPoolableObject(row_1);
                                    }
                                    else
                                    {
                                        /*
                                         * O O O 
                                         *   O 
                                         *   O
                                         */
                                        row_0 = blocks[row - 1, col + 1];
                                        row_1 = blocks[row - 2, col + 1];

                                        if (row_0 != null && row_1 != null)
                                        {
                                            if ((checkBlock.elementType == col_0.elementType && checkBlock.elementType == col_1.elementType) &&
                                                (checkBlock.elementType == row_0.elementType && checkBlock.elementType == row_1.elementType))
                                            {
                                                // 점수 업데이트
                                                DM.SetScore(checkBlock.BlockScore);
                                                DM.SetScore(col_0.BlockScore);
                                                DM.SetScore(col_1.BlockScore);
                                                DM.SetScore(row_0.BlockScore);
                                                DM.SetScore(row_1.BlockScore);

                                                // 스킬 게이지 업데이트
                                                uiElement.SetGauge(checkBlock.ElementValue);
                                                uiElement.SetGauge(col_0.ElementValue);
                                                uiElement.SetGauge(col_1.ElementValue);
                                                uiElement.SetGauge(row_0.ElementValue);
                                                uiElement.SetGauge(row_1.ElementValue);

                                                // 저장소에서 비우기
                                                blocks[row, col] = null;
                                                blocks[row, col + 1] = null;
                                                blocks[row, col + 2] = null;

                                                blocks[row - 1, col + 1] = null;
                                                blocks[row - 2, col + 1] = null;

                                                // 풀에 반환
                                                blockPool.ReturnPoolableObject(checkBlock);
                                                blockPool.ReturnPoolableObject(col_0);
                                                blockPool.ReturnPoolableObject(col_1);
                                                blockPool.ReturnPoolableObject(row_0);
                                                blockPool.ReturnPoolableObject(row_1);
                                            }
                                            else
                                            {
                                                /*
                                                 * O O O
                                                 *     O
                                                 *     O
                                                 */
                                                row_0 = blocks[row - 1, col + 2];
                                                row_1 = blocks[row - 2, col + 2];

                                                if (row_0 != null && row_1 != null)
                                                {
                                                    if ((checkBlock.elementType == col_0.elementType && checkBlock.elementType == col_1.elementType) &&
                                                        (checkBlock.elementType == row_0.elementType && checkBlock.elementType == row_1.elementType))
                                                    {
                                                        // 점수 업데이트
                                                        DM.SetScore(checkBlock.BlockScore);
                                                        DM.SetScore(col_0.BlockScore);
                                                        DM.SetScore(col_1.BlockScore);
                                                        DM.SetScore(row_0.BlockScore);
                                                        DM.SetScore(row_1.BlockScore);

                                                        // 스킬 게이지 업데이트
                                                        uiElement.SetGauge(checkBlock.ElementValue);
                                                        uiElement.SetGauge(col_0.ElementValue);
                                                        uiElement.SetGauge(col_1.ElementValue);
                                                        uiElement.SetGauge(row_0.ElementValue);
                                                        uiElement.SetGauge(row_1.ElementValue);

                                                        // 저장소에서 비우기
                                                        blocks[row, col] = null;
                                                        blocks[row, col + 1] = null;
                                                        blocks[row, col + 2] = null;

                                                        blocks[row - 1, col + 2] = null;
                                                        blocks[row - 2, col + 2] = null;

                                                        // 풀에 반환
                                                        blockPool.ReturnPoolableObject(checkBlock);
                                                        blockPool.ReturnPoolableObject(col_0);
                                                        blockPool.ReturnPoolableObject(col_1);
                                                        blockPool.ReturnPoolableObject(row_0);
                                                        blockPool.ReturnPoolableObject(row_1);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // Row
                    if (blocks[row, col] != null)
                    {
                        checkBlock = blocks[row, col];

                        if (checkBlock.row != 2 && checkBlock.row != 3)
                        {
                            row_0 = blocks[row + 1, col];
                            row_1 = blocks[row + 2, col];

                            // 1번째 Row 특수 상황 모음집
                            if ((checkBlock.col != 2 && checkBlock.col != 3) && (row_0 != null && row_1 != null))
                            {
                                /*
                                 * O
                                 * O O O
                                 * O
                                 */
                                col_0 = blocks[row + 1, col + 1];
                                col_1 = blocks[row + 1, col + 2];

                                if (col_0 != null && col_1 != null)
                                {
                                    if ((checkBlock.elementType == col_0.elementType && checkBlock.elementType == col_1.elementType) &&
                                        (checkBlock.elementType == row_0.elementType && checkBlock.elementType == row_1.elementType))
                                    {
                                        // 점수 업데이트
                                        DM.SetScore(checkBlock.BlockScore);
                                        DM.SetScore(col_0.BlockScore);
                                        DM.SetScore(col_1.BlockScore);
                                        DM.SetScore(row_0.BlockScore);
                                        DM.SetScore(row_1.BlockScore);

                                        // 스킬 게이지 업데이트
                                        uiElement.SetGauge(checkBlock.ElementValue);
                                        uiElement.SetGauge(col_0.ElementValue);
                                        uiElement.SetGauge(col_1.ElementValue);
                                        uiElement.SetGauge(row_0.ElementValue);
                                        uiElement.SetGauge(row_1.ElementValue);

                                        // 저장소에서 비우기
                                        blocks[row, col] = null;
                                        blocks[row + 1, col] = null;
                                        blocks[row + 2, col] = null;

                                        blocks[row + 1, col + 1] = null;
                                        blocks[row + 1, col + 2] = null;

                                        // 풀에 반환
                                        blockPool.ReturnPoolableObject(checkBlock);
                                        blockPool.ReturnPoolableObject(col_0);
                                        blockPool.ReturnPoolableObject(col_1);
                                        blockPool.ReturnPoolableObject(row_0);
                                        blockPool.ReturnPoolableObject(row_1);
                                    }
                                }
                            }

                            // 2번째 Row 특수 상황 모음집
                            if ((checkBlock.col != -2 && checkBlock.col != -3) && (row_0 != null && row_1 != null))
                            {
                                /*
                                 *     O
                                 * O O O
                                 *     O
                                 */
                                col_0 = blocks[row + 1, col - 1];
                                col_1 = blocks[row + 1, col - 2];

                                if (col_0 != null && col_1 != null)
                                {
                                    if ((checkBlock.elementType == col_0.elementType && checkBlock.elementType == col_1.elementType) &&
                                        (checkBlock.elementType == row_0.elementType && checkBlock.elementType == row_1.elementType))
                                    {
                                        // 점수 업데이트
                                        DM.SetScore(checkBlock.BlockScore);
                                        DM.SetScore(col_0.BlockScore);
                                        DM.SetScore(col_1.BlockScore);
                                        DM.SetScore(row_0.BlockScore);
                                        DM.SetScore(row_1.BlockScore);

                                        // 스킬 게이지 업데이트
                                        uiElement.SetGauge(checkBlock.ElementValue);
                                        uiElement.SetGauge(col_0.ElementValue);
                                        uiElement.SetGauge(col_1.ElementValue);
                                        uiElement.SetGauge(row_0.ElementValue);
                                        uiElement.SetGauge(row_1.ElementValue);

                                        // 저장소에서 비우기
                                        blocks[row, col] = null;
                                        blocks[row + 1, col] = null;
                                        blocks[row + 2, col] = null;

                                        blocks[row + 1, col - 1] = null;
                                        blocks[row + 1, col - 2] = null;

                                        // 풀에 반환
                                        blockPool.ReturnPoolableObject(checkBlock);
                                        blockPool.ReturnPoolableObject(col_0);
                                        blockPool.ReturnPoolableObject(col_1);
                                        blockPool.ReturnPoolableObject(row_0);
                                        blockPool.ReturnPoolableObject(row_1);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // 3X3 블록 파괴 - 일반 상황
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (blocks[row, col] != null)
                    {
                        checkBlock = blocks[row, col];

                        if (checkBlock.col != 2 && checkBlock.col != 3)
                        {
                            col_0 = blocks[row, col + 1];
                            col_1 = blocks[row, col + 2];

                            // Col 일반 상황
                            if (checkBlock != null && col_0 != null && col_1 != null)
                            {
                                if (checkBlock.elementType == col_0.elementType &&
                                    checkBlock.elementType == col_1.elementType)
                                {
                                    // 점수 업데이트
                                    DM.SetScore(checkBlock.BlockScore);
                                    DM.SetScore(col_0.BlockScore);
                                    DM.SetScore(col_1.BlockScore);

                                    // 스킬 게이지 업데이트
                                    uiElement.SetGauge(checkBlock.ElementValue);
                                    uiElement.SetGauge(col_0.ElementValue);
                                    uiElement.SetGauge(col_1.ElementValue);

                                    // 저장소에서 비우기
                                    blocks[row, col] = null;
                                    blocks[row, col + 1] = null;
                                    blocks[row, col + 2] = null;

                                    // 풀에 반환
                                    blockPool.ReturnPoolableObject(checkBlock);
                                    blockPool.ReturnPoolableObject(col_0);
                                    blockPool.ReturnPoolableObject(col_1);
                                }
                            }
                        }

                        if (checkBlock.row != 2 && checkBlock.row != 3)
                        {
                            row_0 = blocks[row + 1, col];
                            row_1 = blocks[row + 2, col];

                            // Row 일반 상황
                            if (row_0 != null && row_1 != null)
                            {
                                if (checkBlock.elementType == row_0.elementType && checkBlock.elementType == row_1.elementType)
                                {

                                    // 점수 업데이트
                                    DM.SetScore(blocks[row, col].BlockScore);
                                    DM.SetScore(blocks[row + 1, col].BlockScore);
                                    DM.SetScore(blocks[row + 2, col].BlockScore);

                                    // 스킬 게이지 업데이트
                                    uiElement.SetGauge(blocks[row, col].ElementValue);
                                    uiElement.SetGauge(blocks[row + 1, col].ElementValue);
                                    uiElement.SetGauge(blocks[row + 2, col].ElementValue);

                                    blocks[row, col] = null;
                                    blocks[row + 1, col] = null;
                                    blocks[row + 2, col] = null;

                                    // 블럭 제거
                                    blockPool.ReturnPoolableObject(checkBlock);
                                    blockPool.ReturnPoolableObject(row_0);
                                    blockPool.ReturnPoolableObject(row_1);
                                }
                            }
                        }

                    }
                }
            }

            // 블럭 다운 카운트 계산
            int downCount = 0;

            for (int col = 0; col < width; col++)
            {
                for (int row = 0; row < height; row++)
                {
                    if (blocks[row, col] == null)
                    {
                        downCount++;
                    }
                    else
                    {
                        blocks[row, col].downCount = downCount;
                    }
                }

                downCount = 0;
            }

            // 블럭 내리기 작업
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (blocks[row, col] != null)
                    {
                        if (blocks[row, col].downCount > 0)
                        {
                            // 내릴 공간이 존재한다
                            var targetrow = (blocks[row, col].row -= blocks[row, col].downCount);

                            if (Mathf.Abs(targetrow - blocks[row, col].transform.localPosition.y) > .1f)
                            {
                                Vector2 tempposition = new Vector2(blocks[row, col].transform.localPosition.x, targetrow);
                                blocks[row, col].transform.localPosition = Vector2.Lerp(blocks[row, col].transform.localPosition, tempposition, .05f);
                            }

                            // 다운 카운트 초기화
                            blocks[row, col].downCount = 0;
                        }
                    }
                }
            }

            // 빈 공간 블럭 생성
            int newCol = -3;
            int newRow = 3;

            // 블럭 생성 및 이동
            for (int col = 0; col < width; col++)
            {
                for (int row = 0; row < height; row++)
                {
                    if (blocks[row, col] == null)
                    {
                        Block newBlock = blockPool.GetPoolableObject(obj => obj.CanRecycle);

                        // 부모 설정
                        newBlock.transform.SetParent(this.transform);

                        // 위치값 설정
                        newBlock.transform.localPosition = new Vector3(newCol, row + 4, 0);

                        // 회전값 설정
                        newBlock.transform.localRotation = Quaternion.identity;

                        // 사이즈값 설정
                        newBlock.transform.localScale = new Vector3(.19f, .19f, .19f);

                        // 활성화
                        newBlock.gameObject.SetActive(true);

                        // 블럭 초기화
                        newBlock.Initialize(newCol, row + 4);

                        // 블럭 저장
                        blocks[row, col] = newBlock;

                        var targetrow = (newBlock.row = newRow);

                        if (Mathf.Abs(targetrow - newBlock.transform.localPosition.y) > .1f)
                        {
                            Vector2 tempposition = new Vector2(newBlock.transform.localPosition.x, targetrow);
                            newBlock.transform.localPosition = Vector2.Lerp(newBlock.transform.localPosition, tempposition, .05f);
                        }

                        newRow--;
                    }
                }

                newCol++;
                newRow = 3;
            }

            BlockSort();

            if (MatchCheck())
            {
                // 블럭 매칭 재시작
                StartCoroutine(BlockMatch());
            }
            else
            {

                // GameState -> Play
                GM.SetGameState(GameState.Play);
            }
        }

        public IEnumerator MakeBoom()
        {
            var blockPool = ObjectPoolManager.Instance.GetPool<Block>(PoolType.Block);
            var uiElement = UIWindowManager.Instance.GetWindow<UIElement>();

            bool isMake = false;

            Block checkBlock = null;

            Block col_0 = null;
            Block col_1 = null;
            Block col_2 = null;

            Block row_0 = null;
            Block row_1 = null;
            Block row_2 = null;
            
            // 이동 블럭이 있는 경우 (아직 예외 상황 작성 안함)
            if (moveBlock)
            {
                for (int row = 0; row < height; row++)
                {
                    for (int col = 0; col < width; col++)
                    {
                        if (blocks[row, col] != null && GM.GameState != GameState.Move)
                        {
                            // MoveBlock
                            if (blocks[row, col] == moveBlock)
                            {
                                // Col
                                switch (moveBlock.col)
                                {
                                    case -3:
                                        // ★OOO
                                        col_0 = blocks[row, col + 1];
                                        col_1 = blocks[row, col + 2];
                                        col_2 = blocks[row, col + 3];

                                        if (moveBlock.elementType == col_0.elementType &&
                                            moveBlock.elementType == col_1.elementType &&
                                            moveBlock.elementType == col_2.elementType)
                                        {
                                            // 블럭 이동
                                            col_0.col = moveBlock.col;
                                            col_1.col = moveBlock.col;
                                            col_2.col = moveBlock.col;

                                            yield return new WaitForSeconds(.3f);

                                            // 블럭 제거
                                            blockPool.ReturnPoolableObject(col_0);
                                            blockPool.ReturnPoolableObject(col_1);
                                            blockPool.ReturnPoolableObject(col_2);

                                            // 점수 업데이트
                                            DM.SetScore(col_0.BlockScore);
                                            DM.SetScore(col_1.BlockScore);
                                            DM.SetScore(col_2.BlockScore);

                                            // 스킬 게이지 업데이트
                                            uiElement.SetGauge(col_0.ElementValue);
                                            uiElement.SetGauge(col_1.ElementValue);
                                            uiElement.SetGauge(col_2.ElementValue);

                                            blocks[row, col + 1] = null;
                                            blocks[row, col + 2] = null;
                                            blocks[row, col + 3] = null;

                                            // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                            moveBlock.elementType = ElementType.Balance;
                                            moveBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                            isMake = true;
                                        }
                                        break;

                                    case -2:
                                        // ★OOO
                                        col_0 = blocks[row, col + 1];
                                        col_1 = blocks[row, col + 2];
                                        col_2 = blocks[row, col + 3];

                                        if (moveBlock.elementType == col_0.elementType &&
                                            moveBlock.elementType == col_1.elementType &&
                                            moveBlock.elementType == col_2.elementType)
                                        {
                                            // 블럭 이동
                                            col_0.col = moveBlock.col;
                                            col_1.col = moveBlock.col;
                                            col_2.col = moveBlock.col;

                                            yield return new WaitForSeconds(.3f);

                                            // 블럭 제거
                                            blockPool.ReturnPoolableObject(col_0);
                                            blockPool.ReturnPoolableObject(col_1);
                                            blockPool.ReturnPoolableObject(col_2);

                                            // 점수 업데이트
                                            DM.SetScore(col_0.BlockScore);
                                            DM.SetScore(col_1.BlockScore);
                                            DM.SetScore(col_2.BlockScore);

                                            // 스킬 게이지 업데이트
                                            uiElement.SetGauge(col_0.ElementValue);
                                            uiElement.SetGauge(col_1.ElementValue);
                                            uiElement.SetGauge(col_2.ElementValue);

                                            blocks[row, col + 1] = null;
                                            blocks[row, col + 2] = null;
                                            blocks[row, col + 3] = null;

                                            // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                            moveBlock.elementType = ElementType.Balance;
                                            moveBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                            isMake = true;
                                        }
                                        else
                                        {
                                            // O★OO
                                            col_0 = blocks[row, col - 1];
                                            col_1 = blocks[row, col + 1];
                                            col_2 = blocks[row, col + 2];

                                            if (moveBlock.elementType == col_0.elementType &&
                                                moveBlock.elementType == col_1.elementType &&
                                                moveBlock.elementType == col_2.elementType)
                                            {
                                                // 블럭 이동
                                                col_0.col = moveBlock.col;
                                                col_1.col = moveBlock.col;
                                                col_2.col = moveBlock.col;

                                                yield return new WaitForSeconds(.3f);

                                                // 블럭 제거
                                                blockPool.ReturnPoolableObject(col_0);
                                                blockPool.ReturnPoolableObject(col_1);
                                                blockPool.ReturnPoolableObject(col_2);

                                                // 점수 업데이트
                                                DM.SetScore(col_0.BlockScore);
                                                DM.SetScore(col_1.BlockScore);
                                                DM.SetScore(col_2.BlockScore);

                                                // 스킬 게이지 업데이트
                                                uiElement.SetGauge(col_0.ElementValue);
                                                uiElement.SetGauge(col_1.ElementValue);
                                                uiElement.SetGauge(col_2.ElementValue);

                                                blocks[row, col - 1] = null;
                                                blocks[row, col + 1] = null;
                                                blocks[row, col + 2] = null;

                                                // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                                moveBlock.elementType = ElementType.Balance;
                                                moveBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                isMake = true;
                                            }
                                        }
                                        break;

                                    case -1:
                                    case 0:
                                        // ★OOO
                                        col_0 = blocks[row, col + 1];
                                        col_1 = blocks[row, col + 2];
                                        col_2 = blocks[row, col + 3];

                                        if (moveBlock.elementType == col_0.elementType &&
                                            moveBlock.elementType == col_1.elementType &&
                                            moveBlock.elementType == col_2.elementType)
                                        {
                                            // 블럭 이동
                                            col_0.col = moveBlock.col;
                                            col_1.col = moveBlock.col;
                                            col_2.col = moveBlock.col;

                                            yield return new WaitForSeconds(.3f);

                                            // 블럭 제거
                                            blockPool.ReturnPoolableObject(col_0);
                                            blockPool.ReturnPoolableObject(col_1);
                                            blockPool.ReturnPoolableObject(col_2);

                                            // 점수 업데이트
                                            DM.SetScore(col_0.BlockScore);
                                            DM.SetScore(col_1.BlockScore);
                                            DM.SetScore(col_2.BlockScore);

                                            // 스킬 게이지 업데이트
                                            uiElement.SetGauge(col_0.ElementValue);
                                            uiElement.SetGauge(col_1.ElementValue);
                                            uiElement.SetGauge(col_2.ElementValue);

                                            blocks[row, col + 1] = null;
                                            blocks[row, col + 2] = null;
                                            blocks[row, col + 3] = null;

                                            // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                            moveBlock.elementType = ElementType.Balance;
                                            moveBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                            isMake = true;
                                        }
                                        else
                                        {
                                            // O★OO
                                            col_0 = blocks[row, col - 1];
                                            col_1 = blocks[row, col + 1];
                                            col_2 = blocks[row, col + 2];

                                            if (moveBlock.elementType == col_0.elementType &&
                                                moveBlock.elementType == col_1.elementType &&
                                                moveBlock.elementType == col_2.elementType)
                                            {
                                                // 블럭 이동
                                                col_0.col = moveBlock.col;
                                                col_1.col = moveBlock.col;
                                                col_2.col = moveBlock.col;

                                                yield return new WaitForSeconds(.3f);

                                                // 블럭 제거
                                                blockPool.ReturnPoolableObject(col_0);
                                                blockPool.ReturnPoolableObject(col_1);
                                                blockPool.ReturnPoolableObject(col_2);

                                                // 점수 업데이트
                                                DM.SetScore(col_0.BlockScore);
                                                DM.SetScore(col_1.BlockScore);
                                                DM.SetScore(col_2.BlockScore);

                                                // 스킬 게이지 업데이트
                                                uiElement.SetGauge(col_0.ElementValue);
                                                uiElement.SetGauge(col_1.ElementValue);
                                                uiElement.SetGauge(col_2.ElementValue);

                                                blocks[row, col - 1] = null;
                                                blocks[row, col + 1] = null;
                                                blocks[row, col + 2] = null;

                                                // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                                moveBlock.elementType = ElementType.Balance;
                                                moveBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                isMake = true;
                                            }
                                            else
                                            {
                                                // OO★O
                                                col_0 = blocks[row, col - 1];
                                                col_1 = blocks[row, col - 2];
                                                col_2 = blocks[row, col + 1];

                                                if (moveBlock.elementType == col_0.elementType &&
                                                    moveBlock.elementType == col_1.elementType &&
                                                    moveBlock.elementType == col_2.elementType)
                                                {
                                                    // 블럭 이동
                                                    col_0.col = moveBlock.col;
                                                    col_1.col = moveBlock.col;
                                                    col_2.col = moveBlock.col;

                                                    yield return new WaitForSeconds(.3f);

                                                    // 블럭 제거
                                                    blockPool.ReturnPoolableObject(col_0);
                                                    blockPool.ReturnPoolableObject(col_1);
                                                    blockPool.ReturnPoolableObject(col_2);

                                                    // 점수 업데이트
                                                    DM.SetScore(col_0.BlockScore);
                                                    DM.SetScore(col_1.BlockScore);
                                                    DM.SetScore(col_2.BlockScore);

                                                    // 스킬 게이지 업데이트
                                                    uiElement.SetGauge(col_0.ElementValue);
                                                    uiElement.SetGauge(col_1.ElementValue);
                                                    uiElement.SetGauge(col_2.ElementValue);

                                                    blocks[row, col - 1] = null;
                                                    blocks[row, col - 2] = null;
                                                    blocks[row, col + 1] = null;

                                                    // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                                    moveBlock.elementType = ElementType.Balance;
                                                    moveBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                    isMake = true;
                                                }
                                            }
                                        }
                                        break;

                                    case 1:
                                        // OOO★
                                        col_0 = blocks[row, col - 1];
                                        col_1 = blocks[row, col - 2];
                                        col_2 = blocks[row, col - 3];

                                        if (moveBlock.elementType == col_0.elementType &&
                                            moveBlock.elementType == col_1.elementType &&
                                            moveBlock.elementType == col_2.elementType)
                                        {
                                            // 블럭 이동
                                            col_0.col = moveBlock.col;
                                            col_1.col = moveBlock.col;
                                            col_2.col = moveBlock.col;

                                            yield return new WaitForSeconds(.3f);

                                            // 블럭 제거
                                            blockPool.ReturnPoolableObject(col_0);
                                            blockPool.ReturnPoolableObject(col_1);
                                            blockPool.ReturnPoolableObject(col_2);

                                            // 점수 업데이트
                                            DM.SetScore(col_0.BlockScore);
                                            DM.SetScore(col_1.BlockScore);
                                            DM.SetScore(col_2.BlockScore);

                                            // 스킬 게이지 업데이트
                                            uiElement.SetGauge(col_0.ElementValue);
                                            uiElement.SetGauge(col_1.ElementValue);
                                            uiElement.SetGauge(col_2.ElementValue);

                                            blocks[row, col - 1] = null;
                                            blocks[row, col - 2] = null;
                                            blocks[row, col - 3] = null;

                                            // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                            moveBlock.elementType = ElementType.Balance;
                                            moveBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                            isMake = true;
                                        }
                                        else
                                        {
                                            // OO★O
                                            col_0 = blocks[row, col - 1];
                                            col_1 = blocks[row, col - 2];
                                            col_2 = blocks[row, col + 1];

                                            if (moveBlock.elementType == col_0.elementType &&
                                                moveBlock.elementType == col_1.elementType &&
                                                moveBlock.elementType == col_2.elementType)
                                            {
                                                // 블럭 이동
                                                col_0.col = moveBlock.col;
                                                col_1.col = moveBlock.col;
                                                col_2.col = moveBlock.col;

                                                yield return new WaitForSeconds(.3f);

                                                // 블럭 제거
                                                blockPool.ReturnPoolableObject(col_0);
                                                blockPool.ReturnPoolableObject(col_1);
                                                blockPool.ReturnPoolableObject(col_2);

                                                // 점수 업데이트
                                                DM.SetScore(col_0.BlockScore);
                                                DM.SetScore(col_1.BlockScore);
                                                DM.SetScore(col_2.BlockScore);

                                                // 스킬 게이지 업데이트
                                                uiElement.SetGauge(col_0.ElementValue);
                                                uiElement.SetGauge(col_1.ElementValue);
                                                uiElement.SetGauge(col_2.ElementValue);

                                                blocks[row, col - 1] = null;
                                                blocks[row, col - 2] = null;
                                                blocks[row, col + 1] = null;

                                                // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                                moveBlock.elementType = ElementType.Balance;
                                                moveBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                isMake = true;
                                            }
                                            else
                                            {
                                                // O★OO
                                                col_0 = blocks[row, col - 1];
                                                col_1 = blocks[row, col + 1];
                                                col_2 = blocks[row, col + 2];

                                                if (moveBlock.elementType == col_0.elementType &&
                                                    moveBlock.elementType == col_1.elementType &&
                                                    moveBlock.elementType == col_2.elementType)
                                                {
                                                    // 블럭 이동
                                                    col_0.col = moveBlock.col;
                                                    col_1.col = moveBlock.col;
                                                    col_2.col = moveBlock.col;

                                                    yield return new WaitForSeconds(.3f);

                                                    // 블럭 제거
                                                    blockPool.ReturnPoolableObject(col_0);
                                                    blockPool.ReturnPoolableObject(col_1);
                                                    blockPool.ReturnPoolableObject(col_2);

                                                    // 점수 업데이트
                                                    DM.SetScore(col_0.BlockScore);
                                                    DM.SetScore(col_1.BlockScore);
                                                    DM.SetScore(col_2.BlockScore);

                                                    // 스킬 게이지 업데이트
                                                    uiElement.SetGauge(col_0.ElementValue);
                                                    uiElement.SetGauge(col_1.ElementValue);
                                                    uiElement.SetGauge(col_2.ElementValue);

                                                    blocks[row, col - 1] = null;
                                                    blocks[row, col + 1] = null;
                                                    blocks[row, col + 2] = null;

                                                    // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                                    moveBlock.elementType = ElementType.Balance;
                                                    moveBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                    isMake = true;
                                                }
                                            }
                                        }
                                        break;

                                    case 2:
                                        // OOO★
                                        col_0 = blocks[row, col - 1];
                                        col_1 = blocks[row, col - 2];
                                        col_2 = blocks[row, col - 3];

                                        if (moveBlock.elementType == col_0.elementType &&
                                            moveBlock.elementType == col_1.elementType &&
                                            moveBlock.elementType == col_2.elementType)
                                        {
                                            // 블럭 이동
                                            col_0.col = moveBlock.col;
                                            col_1.col = moveBlock.col;
                                            col_2.col = moveBlock.col;

                                            yield return new WaitForSeconds(.3f);

                                            // 블럭 제거
                                            blockPool.ReturnPoolableObject(col_0);
                                            blockPool.ReturnPoolableObject(col_1);
                                            blockPool.ReturnPoolableObject(col_2);

                                            // 점수 업데이트
                                            DM.SetScore(col_0.BlockScore);
                                            DM.SetScore(col_1.BlockScore);
                                            DM.SetScore(col_2.BlockScore);

                                            // 스킬 게이지 업데이트
                                            uiElement.SetGauge(col_0.ElementValue);
                                            uiElement.SetGauge(col_1.ElementValue);
                                            uiElement.SetGauge(col_2.ElementValue);

                                            blocks[row, col - 1] = null;
                                            blocks[row, col - 2] = null;
                                            blocks[row, col - 3] = null;

                                            // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                            moveBlock.elementType = ElementType.Balance;
                                            moveBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                            isMake = true;
                                        }
                                        else
                                        {
                                            // OO★O
                                            col_0 = blocks[row, col - 1];
                                            col_1 = blocks[row, col - 2];
                                            col_2 = blocks[row, col + 1];

                                            if (moveBlock.elementType == col_0.elementType &&
                                                moveBlock.elementType == col_1.elementType &&
                                                moveBlock.elementType == col_2.elementType)
                                            {
                                                // 블럭 이동
                                                col_0.col = moveBlock.col;
                                                col_1.col = moveBlock.col;
                                                col_2.col = moveBlock.col;

                                                yield return new WaitForSeconds(.3f);

                                                // 블럭 제거
                                                blockPool.ReturnPoolableObject(col_0);
                                                blockPool.ReturnPoolableObject(col_1);
                                                blockPool.ReturnPoolableObject(col_2);

                                                // 점수 업데이트
                                                DM.SetScore(col_0.BlockScore);
                                                DM.SetScore(col_1.BlockScore);
                                                DM.SetScore(col_2.BlockScore);

                                                // 스킬 게이지 업데이트
                                                uiElement.SetGauge(col_0.ElementValue);
                                                uiElement.SetGauge(col_1.ElementValue);
                                                uiElement.SetGauge(col_2.ElementValue);

                                                blocks[row, col - 1] = null;
                                                blocks[row, col - 2] = null;
                                                blocks[row, col + 1] = null;

                                                // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                                moveBlock.elementType = ElementType.Balance;
                                                moveBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                isMake = true;
                                            }
                                        }
                                        break;

                                    case 3:
                                        // OOO★
                                        col_0 = blocks[row, col - 1];
                                        col_1 = blocks[row, col - 2];
                                        col_2 = blocks[row, col - 3];

                                        if (moveBlock.elementType == col_0.elementType &&
                                            moveBlock.elementType == col_1.elementType &&
                                            moveBlock.elementType == col_2.elementType)
                                        {
                                            // 블럭 이동
                                            col_0.col = moveBlock.col;
                                            col_1.col = moveBlock.col;
                                            col_2.col = moveBlock.col;

                                            yield return new WaitForSeconds(.3f);

                                            // 블럭 제거
                                            blockPool.ReturnPoolableObject(col_0);
                                            blockPool.ReturnPoolableObject(col_1);
                                            blockPool.ReturnPoolableObject(col_2);

                                            // 점수 업데이트
                                            DM.SetScore(col_0.BlockScore);
                                            DM.SetScore(col_1.BlockScore);
                                            DM.SetScore(col_2.BlockScore);

                                            // 스킬 게이지 업데이트
                                            uiElement.SetGauge(col_0.ElementValue);
                                            uiElement.SetGauge(col_1.ElementValue);
                                            uiElement.SetGauge(col_2.ElementValue);

                                            blocks[row, col - 1] = null;
                                            blocks[row, col - 2] = null;
                                            blocks[row, col - 3] = null;

                                            // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                            moveBlock.elementType = ElementType.Balance;
                                            moveBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                            isMake = true;
                                        }
                                        break;
                                }

                                // Row 
                                switch (moveBlock.row)
                                {
                                    case -3:
                                        // ★OOO
                                        col_0 = blocks[row + 1, col];
                                        col_1 = blocks[row + 2, col];
                                        col_2 = blocks[row + 3, col];

                                        if (moveBlock.elementType == col_0.elementType &&
                                            moveBlock.elementType == col_1.elementType &&
                                            moveBlock.elementType == col_2.elementType)
                                        {
                                            col_0.row = moveBlock.row;
                                            col_1.row = moveBlock.row;
                                            col_2.row = moveBlock.row;

                                            yield return new WaitForSeconds(.3f);

                                            blockPool.ReturnPoolableObject(col_0);
                                            blockPool.ReturnPoolableObject(col_1);
                                            blockPool.ReturnPoolableObject(col_2);

                                            // 점수를 더합니다!
                                            DM.SetScore(col_0.BlockScore);
                                            DM.SetScore(col_1.BlockScore);
                                            DM.SetScore(col_2.BlockScore);

                                            blocks[row + 1, col] = null;
                                            blocks[row + 2, col] = null;
                                            blocks[row + 3, col] = null;

                                            // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                            moveBlock.elementType = ElementType.Balance;
                                            moveBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                            isMake = true;
                                        }
                                        break;

                                    case -2:
                                        // ★OOO
                                        col_0 = blocks[row + 1, col];
                                        col_1 = blocks[row + 2, col];
                                        col_2 = blocks[row + 3, col];

                                        if (moveBlock.elementType == col_0.elementType &&
                                            moveBlock.elementType == col_1.elementType &&
                                            moveBlock.elementType == col_2.elementType)
                                        {
                                            col_0.row = moveBlock.row;
                                            col_1.row = moveBlock.row;
                                            col_2.row = moveBlock.row;

                                            yield return new WaitForSeconds(.3f);

                                            blockPool.ReturnPoolableObject(col_0);
                                            blockPool.ReturnPoolableObject(col_1);
                                            blockPool.ReturnPoolableObject(col_2);

                                            // 점수를 더합니다!
                                            DM.SetScore(col_0.BlockScore);
                                            DM.SetScore(col_1.BlockScore);
                                            DM.SetScore(col_2.BlockScore);

                                            blocks[row + 1, col] = null;
                                            blocks[row + 2, col] = null;
                                            blocks[row + 3, col] = null;

                                            // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                            moveBlock.elementType = ElementType.Balance;
                                            moveBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                            isMake = true;
                                        }
                                        else
                                        {
                                            // O★OO
                                            col_0 = blocks[row - 1, col];
                                            col_1 = blocks[row + 1, col];
                                            col_2 = blocks[row + 2, col];

                                            if (moveBlock.elementType == col_0.elementType &&
                                                moveBlock.elementType == col_1.elementType &&
                                                moveBlock.elementType == col_2.elementType)
                                            {
                                                col_0.row = moveBlock.row;
                                                col_1.row = moveBlock.row;
                                                col_2.row = moveBlock.row;

                                                yield return new WaitForSeconds(.3f);

                                                blockPool.ReturnPoolableObject(col_0);
                                                blockPool.ReturnPoolableObject(col_1);
                                                blockPool.ReturnPoolableObject(col_2);

                                                // 점수를 더합니다!
                                                DM.SetScore(col_0.BlockScore);
                                                DM.SetScore(col_1.BlockScore);
                                                DM.SetScore(col_2.BlockScore);

                                                blocks[row - 1, col] = null;
                                                blocks[row + 1, col] = null;
                                                blocks[row + 2, col] = null;

                                                // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                                moveBlock.elementType = ElementType.Balance;
                                                moveBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                isMake = true;
                                            }
                                        }
                                        break;

                                    case -1:
                                    case 0:
                                        // ★OOO
                                        col_0 = blocks[row + 1, col];
                                        col_1 = blocks[row + 2, col];
                                        col_2 = blocks[row + 3, col];

                                        if (moveBlock.elementType == col_0.elementType &&
                                            moveBlock.elementType == col_1.elementType &&
                                            moveBlock.elementType == col_2.elementType)
                                        {
                                            col_0.row = moveBlock.row;
                                            col_1.row = moveBlock.row;
                                            col_2.row = moveBlock.row;

                                            yield return new WaitForSeconds(.3f);

                                            blockPool.ReturnPoolableObject(col_0);
                                            blockPool.ReturnPoolableObject(col_1);
                                            blockPool.ReturnPoolableObject(col_2);

                                            // 점수를 더합니다!
                                            DM.SetScore(col_0.BlockScore);
                                            DM.SetScore(col_1.BlockScore);
                                            DM.SetScore(col_2.BlockScore);

                                            blocks[row + 1, col] = null;
                                            blocks[row + 2, col] = null;
                                            blocks[row + 3, col] = null;

                                            // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                            moveBlock.elementType = ElementType.Balance;
                                            moveBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                            isMake = true;
                                        }
                                        else
                                        {
                                            // O★OO
                                            col_0 = blocks[row - 1, col];
                                            col_1 = blocks[row + 1, col];
                                            col_2 = blocks[row + 2, col];

                                            if (moveBlock.elementType == col_0.elementType &&
                                                moveBlock.elementType == col_1.elementType &&
                                                moveBlock.elementType == col_2.elementType)
                                            {
                                                col_0.row = moveBlock.row;
                                                col_1.row = moveBlock.row;
                                                col_2.row = moveBlock.row;

                                                yield return new WaitForSeconds(.3f);

                                                blockPool.ReturnPoolableObject(col_0);
                                                blockPool.ReturnPoolableObject(col_1);
                                                blockPool.ReturnPoolableObject(col_2);

                                                // 점수를 더합니다!
                                                DM.SetScore(col_0.BlockScore);
                                                DM.SetScore(col_1.BlockScore);
                                                DM.SetScore(col_2.BlockScore);

                                                blocks[row - 1, col] = null;
                                                blocks[row + 1, col] = null;
                                                blocks[row + 2, col] = null;

                                                // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                                moveBlock.elementType = ElementType.Balance;
                                                moveBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                isMake = true;
                                            }
                                            else
                                            {
                                                // OO★O
                                                col_0 = blocks[row - 1, col];
                                                col_1 = blocks[row - 2, col];
                                                col_2 = blocks[row + 1, col];

                                                if (moveBlock.elementType == col_0.elementType &&
                                                    moveBlock.elementType == col_1.elementType &&
                                                    moveBlock.elementType == col_2.elementType)
                                                {
                                                    col_0.row = moveBlock.row;
                                                    col_1.row = moveBlock.row;
                                                    col_2.row = moveBlock.row;

                                                    yield return new WaitForSeconds(.3f);

                                                    blockPool.ReturnPoolableObject(col_0);
                                                    blockPool.ReturnPoolableObject(col_1);
                                                    blockPool.ReturnPoolableObject(col_2);

                                                    // 점수를 더합니다!
                                                    DM.SetScore(col_0.BlockScore);
                                                    DM.SetScore(col_1.BlockScore);
                                                    DM.SetScore(col_2.BlockScore);

                                                    blocks[row - 1, col] = null;
                                                    blocks[row - 2, col] = null;
                                                    blocks[row + 1, col] = null;

                                                    // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                                    moveBlock.elementType = ElementType.Balance;
                                                    moveBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                    isMake = true;
                                                }
                                            }
                                        }
                                        break;

                                    case 1:
                                        // OOO★
                                        col_0 = blocks[row - 1, col];
                                        col_1 = blocks[row - 2, col];
                                        col_2 = blocks[row - 3, col];

                                        if (moveBlock.elementType == col_0.elementType &&
                                            moveBlock.elementType == col_1.elementType &&
                                            moveBlock.elementType == col_2.elementType)
                                        {
                                            col_0.row = moveBlock.row;
                                            col_1.row = moveBlock.row;
                                            col_2.row = moveBlock.row;

                                            yield return new WaitForSeconds(.3f);

                                            blockPool.ReturnPoolableObject(col_0);
                                            blockPool.ReturnPoolableObject(col_1);
                                            blockPool.ReturnPoolableObject(col_2);

                                            // 점수를 더합니다!
                                            DM.SetScore(col_0.BlockScore);
                                            DM.SetScore(col_1.BlockScore);
                                            DM.SetScore(col_2.BlockScore);

                                            blocks[row - 1, col] = null;
                                            blocks[row - 2, col] = null;
                                            blocks[row - 3, col] = null;

                                            // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                            moveBlock.elementType = ElementType.Balance;
                                            moveBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                            isMake = true;
                                        }
                                        else
                                        {
                                            // OO★O
                                            col_0 = blocks[row + 1, col];
                                            col_1 = blocks[row - 1, col];
                                            col_2 = blocks[row - 2, col];

                                            if (moveBlock.elementType == col_0.elementType &&
                                                moveBlock.elementType == col_1.elementType &&
                                                moveBlock.elementType == col_2.elementType)
                                            {
                                                col_0.row = moveBlock.row;
                                                col_1.row = moveBlock.row;
                                                col_2.row = moveBlock.row;

                                                yield return new WaitForSeconds(.3f);

                                                blockPool.ReturnPoolableObject(col_0);
                                                blockPool.ReturnPoolableObject(col_1);
                                                blockPool.ReturnPoolableObject(col_2);

                                                // 점수를 더합니다!
                                                DM.SetScore(col_0.BlockScore);
                                                DM.SetScore(col_1.BlockScore);
                                                DM.SetScore(col_2.BlockScore);

                                                blocks[row + 1, col] = null;
                                                blocks[row - 1, col] = null;
                                                blocks[row - 2, col] = null;

                                                // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                                moveBlock.elementType = ElementType.Balance;
                                                moveBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                isMake = true;
                                            }
                                            else
                                            {
                                                // O★OO
                                                col_0 = blocks[row + 1, col];
                                                col_1 = blocks[row + 2, col];
                                                col_2 = blocks[row - 1, col];

                                                if (moveBlock.elementType == col_0.elementType &&
                                                    moveBlock.elementType == col_1.elementType &&
                                                    moveBlock.elementType == col_2.elementType)
                                                {
                                                    col_0.row = moveBlock.row;
                                                    col_1.row = moveBlock.row;
                                                    col_2.row = moveBlock.row;

                                                    yield return new WaitForSeconds(.3f);

                                                    blockPool.ReturnPoolableObject(col_0);
                                                    blockPool.ReturnPoolableObject(col_1);
                                                    blockPool.ReturnPoolableObject(col_2);

                                                    // 점수를 더합니다!
                                                    DM.SetScore(col_0.BlockScore);
                                                    DM.SetScore(col_1.BlockScore);
                                                    DM.SetScore(col_2.BlockScore);

                                                    blocks[row + 1, col] = null;
                                                    blocks[row + 2, col] = null;
                                                    blocks[row - 1, col] = null;

                                                    // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                                    moveBlock.elementType = ElementType.Balance;
                                                    moveBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                    isMake = true;
                                                }
                                            }
                                        }
                                        break;

                                    case 2:
                                        // OOO★
                                        col_0 = blocks[row - 1, col];
                                        col_1 = blocks[row - 2, col];
                                        col_2 = blocks[row - 3, col];

                                        if (moveBlock.elementType == col_0.elementType &&
                                            moveBlock.elementType == col_1.elementType &&
                                            moveBlock.elementType == col_2.elementType)
                                        {
                                            col_0.row = moveBlock.row;
                                            col_1.row = moveBlock.row;
                                            col_2.row = moveBlock.row;

                                            yield return new WaitForSeconds(.3f);

                                            blockPool.ReturnPoolableObject(col_0);
                                            blockPool.ReturnPoolableObject(col_1);
                                            blockPool.ReturnPoolableObject(col_2);

                                            // 점수를 더합니다!
                                            DM.SetScore(col_0.BlockScore);
                                            DM.SetScore(col_1.BlockScore);
                                            DM.SetScore(col_2.BlockScore);

                                            blocks[row - 1, col] = null;
                                            blocks[row - 2, col] = null;
                                            blocks[row - 3, col] = null;

                                            // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                            moveBlock.elementType = ElementType.Balance;
                                            moveBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                            isMake = true;
                                        }
                                        else
                                        {
                                            // OO★O
                                            col_0 = blocks[row + 1, col];
                                            col_1 = blocks[row - 1, col];
                                            col_2 = blocks[row - 2, col];

                                            if (moveBlock.elementType == col_0.elementType &&
                                                moveBlock.elementType == col_1.elementType &&
                                                moveBlock.elementType == col_2.elementType)
                                            {
                                                col_0.row = moveBlock.row;
                                                col_1.row = moveBlock.row;
                                                col_2.row = moveBlock.row;

                                                yield return new WaitForSeconds(.3f);

                                                blockPool.ReturnPoolableObject(col_0);
                                                blockPool.ReturnPoolableObject(col_1);
                                                blockPool.ReturnPoolableObject(col_2);

                                                // 점수를 더합니다!
                                                DM.SetScore(col_0.BlockScore);
                                                DM.SetScore(col_1.BlockScore);
                                                DM.SetScore(col_2.BlockScore);

                                                blocks[row + 1, col] = null;
                                                blocks[row - 1, col] = null;
                                                blocks[row - 2, col] = null;

                                                // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                                moveBlock.elementType = ElementType.Balance;
                                                moveBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                isMake = true;
                                            }
                                        }
                                        break;

                                    case 3:
                                        // OOO★
                                        col_0 = blocks[row - 1, col];
                                        col_1 = blocks[row - 2, col];
                                        col_2 = blocks[row - 3, col];

                                        if (moveBlock.elementType == col_0.elementType &&
                                            moveBlock.elementType == col_1.elementType &&
                                            moveBlock.elementType == col_2.elementType)
                                        {
                                            col_0.row = moveBlock.row;
                                            col_1.row = moveBlock.row;
                                            col_2.row = moveBlock.row;

                                            yield return new WaitForSeconds(.3f);

                                            blockPool.ReturnPoolableObject(col_0);
                                            blockPool.ReturnPoolableObject(col_1);
                                            blockPool.ReturnPoolableObject(col_2);

                                            // 점수를 더합니다!
                                            DM.SetScore(col_0.BlockScore);
                                            DM.SetScore(col_1.BlockScore);
                                            DM.SetScore(col_2.BlockScore);

                                            blocks[row - 1, col] = null;
                                            blocks[row - 2, col] = null;
                                            blocks[row - 3, col] = null;

                                            // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                            moveBlock.elementType = ElementType.Balance;
                                            moveBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                            isMake = true;
                                        }
                                        break;
                                }
                            }

                            Debug.Log(moveBlock.otherBlock);

                            /// 여기서 Null
                            if (moveBlock.otherBlock != null)
                            {
                                // OtherBlock
                                if (blocks[row, col] == moveBlock.otherBlock)
                                {
                                    // Col
                                    switch (moveBlock.otherBlock.col)
                                    {
                                        case -3:
                                            // ★OOO
                                            col_0 = blocks[row, col + 1];
                                            col_1 = blocks[row, col + 2];
                                            col_2 = blocks[row, col + 3];

                                            if (moveBlock.otherBlock.elementType == col_0.elementType &&
                                                moveBlock.otherBlock.elementType == col_1.elementType &&
                                                moveBlock.otherBlock.elementType == col_2.elementType)
                                            {
                                                col_0.col = moveBlock.otherBlock.col;
                                                col_1.col = moveBlock.otherBlock.col;
                                                col_2.col = moveBlock.otherBlock.col;

                                                yield return new WaitForSeconds(.3f);

                                                blockPool.ReturnPoolableObject(col_0);
                                                blockPool.ReturnPoolableObject(col_1);
                                                blockPool.ReturnPoolableObject(col_2);

                                                // 점수를 더합니다!
                                                DM.SetScore(col_0.BlockScore);
                                                DM.SetScore(col_1.BlockScore);
                                                DM.SetScore(col_2.BlockScore);

                                                blocks[row, col + 1] = null;
                                                blocks[row, col + 2] = null;
                                                blocks[row, col + 3] = null;

                                                // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                                moveBlock.otherBlock.elementType = ElementType.Balance;
                                                moveBlock.otherBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                isMake = true;
                                            }
                                            break;

                                        case -2:
                                            // ★OOO
                                            col_0 = blocks[row, col + 1];
                                            col_1 = blocks[row, col + 2];
                                            col_2 = blocks[row, col + 3];

                                            if (moveBlock.otherBlock.elementType == col_0.elementType &&
                                                moveBlock.otherBlock.elementType == col_1.elementType &&
                                                moveBlock.otherBlock.elementType == col_2.elementType)
                                            {
                                                col_0.col = moveBlock.otherBlock.col;
                                                col_1.col = moveBlock.otherBlock.col;
                                                col_2.col = moveBlock.otherBlock.col;

                                                yield return new WaitForSeconds(.3f);

                                                blockPool.ReturnPoolableObject(col_0);
                                                blockPool.ReturnPoolableObject(col_1);
                                                blockPool.ReturnPoolableObject(col_2);

                                                // 점수를 더합니다!
                                                DM.SetScore(col_0.BlockScore);
                                                DM.SetScore(col_1.BlockScore);
                                                DM.SetScore(col_2.BlockScore);

                                                blocks[row, col + 1] = null;
                                                blocks[row, col + 2] = null;
                                                blocks[row, col + 3] = null;

                                                // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                                moveBlock.otherBlock.elementType = ElementType.Balance;
                                                moveBlock.otherBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                isMake = true;
                                            }
                                            else
                                            {
                                                // O★OO
                                                col_0 = blocks[row, col - 1];
                                                col_1 = blocks[row, col + 1];
                                                col_2 = blocks[row, col + 2];

                                                if (moveBlock.otherBlock.elementType == col_0.elementType &&
                                                    moveBlock.otherBlock.elementType == col_1.elementType &&
                                                    moveBlock.otherBlock.elementType == col_2.elementType)
                                                {
                                                    col_0.col = moveBlock.otherBlock.col;
                                                    col_1.col = moveBlock.otherBlock.col;
                                                    col_2.col = moveBlock.otherBlock.col;

                                                    yield return new WaitForSeconds(.3f);

                                                    blockPool.ReturnPoolableObject(col_0);
                                                    blockPool.ReturnPoolableObject(col_1);
                                                    blockPool.ReturnPoolableObject(col_2);

                                                    // 점수를 더합니다!
                                                    DM.SetScore(col_0.BlockScore);
                                                    DM.SetScore(col_1.BlockScore);
                                                    DM.SetScore(col_2.BlockScore);

                                                    blocks[row, col - 1] = null;
                                                    blocks[row, col + 1] = null;
                                                    blocks[row, col + 2] = null;

                                                    // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                                    moveBlock.otherBlock.elementType = ElementType.Balance;
                                                    moveBlock.otherBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                    isMake = true;
                                                }
                                            }
                                            break;

                                        case -1:
                                        case 0:
                                            // ★OOO
                                            col_0 = blocks[row, col + 1];
                                            col_1 = blocks[row, col + 2];
                                            col_2 = blocks[row, col + 3];

                                            if (moveBlock.otherBlock.elementType == col_0.elementType &&
                                                moveBlock.otherBlock.elementType == col_1.elementType &&
                                                moveBlock.otherBlock.elementType == col_2.elementType)
                                            {
                                                col_0.col = moveBlock.otherBlock.col;
                                                col_1.col = moveBlock.otherBlock.col;
                                                col_2.col = moveBlock.otherBlock.col;

                                                yield return new WaitForSeconds(.3f);

                                                blockPool.ReturnPoolableObject(col_0);
                                                blockPool.ReturnPoolableObject(col_1);
                                                blockPool.ReturnPoolableObject(col_2);

                                                // 점수를 더합니다!
                                                DM.SetScore(col_0.BlockScore);
                                                DM.SetScore(col_1.BlockScore);
                                                DM.SetScore(col_2.BlockScore);

                                                blocks[row, col + 1] = null;
                                                blocks[row, col + 2] = null;
                                                blocks[row, col + 3] = null;

                                                // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                                moveBlock.otherBlock.elementType = ElementType.Balance;
                                                moveBlock.otherBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                isMake = true;
                                            }
                                            else
                                            {
                                                // O★OO
                                                col_0 = blocks[row, col - 1];
                                                col_1 = blocks[row, col + 1];
                                                col_2 = blocks[row, col + 2];

                                                if (moveBlock.otherBlock.elementType == col_0.elementType &&
                                                    moveBlock.otherBlock.elementType == col_1.elementType &&
                                                    moveBlock.otherBlock.elementType == col_2.elementType)
                                                {
                                                    col_0.col = moveBlock.otherBlock.col;
                                                    col_1.col = moveBlock.otherBlock.col;
                                                    col_2.col = moveBlock.otherBlock.col;

                                                    yield return new WaitForSeconds(.3f);

                                                    blockPool.ReturnPoolableObject(col_0);
                                                    blockPool.ReturnPoolableObject(col_1);
                                                    blockPool.ReturnPoolableObject(col_2);

                                                    // 점수를 더합니다!
                                                    DM.SetScore(col_0.BlockScore);
                                                    DM.SetScore(col_1.BlockScore);
                                                    DM.SetScore(col_2.BlockScore);

                                                    blocks[row, col - 1] = null;
                                                    blocks[row, col + 1] = null;
                                                    blocks[row, col + 2] = null;

                                                    // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                                    moveBlock.otherBlock.elementType = ElementType.Balance;
                                                    moveBlock.otherBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                    isMake = true;
                                                }
                                                else
                                                {
                                                    // OO★O
                                                    col_0 = blocks[row, col - 1];
                                                    col_1 = blocks[row, col - 2];
                                                    col_2 = blocks[row, col + 1];

                                                    if (moveBlock.otherBlock.elementType == col_0.elementType &&
                                                        moveBlock.otherBlock.elementType == col_1.elementType &&
                                                        moveBlock.otherBlock.elementType == col_2.elementType)
                                                    {
                                                        col_0.col = moveBlock.otherBlock.col;
                                                        col_1.col = moveBlock.otherBlock.col;
                                                        col_2.col = moveBlock.otherBlock.col;

                                                        yield return new WaitForSeconds(.3f);

                                                        blockPool.ReturnPoolableObject(col_0);
                                                        blockPool.ReturnPoolableObject(col_1);
                                                        blockPool.ReturnPoolableObject(col_2);

                                                        // 점수를 더합니다!
                                                        DM.SetScore(col_0.BlockScore);
                                                        DM.SetScore(col_1.BlockScore);
                                                        DM.SetScore(col_2.BlockScore);

                                                        blocks[row, col - 1] = null;
                                                        blocks[row, col - 2] = null;
                                                        blocks[row, col + 1] = null;

                                                        // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                                        moveBlock.otherBlock.elementType = ElementType.Balance;
                                                        moveBlock.otherBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                        isMake = true;
                                                    }
                                                }
                                            }
                                            break;

                                        case 1:
                                            // OOO★
                                            col_0 = blocks[row, col - 1];
                                            col_1 = blocks[row, col - 2];
                                            col_2 = blocks[row, col - 3];

                                            if (moveBlock.otherBlock.elementType == col_0.elementType &&
                                                moveBlock.otherBlock.elementType == col_1.elementType &&
                                                moveBlock.otherBlock.elementType == col_2.elementType)
                                            {
                                                col_0.col = moveBlock.otherBlock.col;
                                                col_1.col = moveBlock.otherBlock.col;
                                                col_2.col = moveBlock.otherBlock.col;

                                                yield return new WaitForSeconds(.3f);

                                                blockPool.ReturnPoolableObject(col_0);
                                                blockPool.ReturnPoolableObject(col_1);
                                                blockPool.ReturnPoolableObject(col_2);

                                                // 점수를 더합니다!
                                                DM.SetScore(col_0.BlockScore);
                                                DM.SetScore(col_1.BlockScore);
                                                DM.SetScore(col_2.BlockScore);

                                                blocks[row, col - 1] = null;
                                                blocks[row, col - 2] = null;
                                                blocks[row, col - 3] = null;

                                                // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                                moveBlock.otherBlock.elementType = ElementType.Balance;
                                                moveBlock.otherBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                isMake = true;
                                            }
                                            else
                                            {
                                                // OO★O
                                                col_0 = blocks[row, col - 1];
                                                col_1 = blocks[row, col - 2];
                                                col_2 = blocks[row, col + 1];

                                                if (moveBlock.otherBlock.elementType == col_0.elementType &&
                                                    moveBlock.otherBlock.elementType == col_1.elementType &&
                                                    moveBlock.otherBlock.elementType == col_2.elementType)
                                                {
                                                    col_0.col = moveBlock.otherBlock.col;
                                                    col_1.col = moveBlock.otherBlock.col;
                                                    col_2.col = moveBlock.otherBlock.col;

                                                    yield return new WaitForSeconds(.3f);

                                                    blockPool.ReturnPoolableObject(col_0);
                                                    blockPool.ReturnPoolableObject(col_1);
                                                    blockPool.ReturnPoolableObject(col_2);

                                                    // 점수를 더합니다!
                                                    DM.SetScore(col_0.BlockScore);
                                                    DM.SetScore(col_1.BlockScore);
                                                    DM.SetScore(col_2.BlockScore);

                                                    // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                                    moveBlock.otherBlock.elementType = ElementType.Balance;
                                                    moveBlock.otherBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                    isMake = true;
                                                }
                                                else
                                                {
                                                    // O★OO
                                                    col_0 = blocks[row, col - 1];
                                                    col_1 = blocks[row, col + 1];
                                                    col_2 = blocks[row, col + 2];

                                                    if (moveBlock.otherBlock.elementType == col_0.elementType &&
                                                        moveBlock.otherBlock.elementType == col_1.elementType &&
                                                        moveBlock.otherBlock.elementType == col_2.elementType)
                                                    {
                                                        col_0.col = moveBlock.otherBlock.col;
                                                        col_1.col = moveBlock.otherBlock.col;
                                                        col_2.col = moveBlock.otherBlock.col;

                                                        yield return new WaitForSeconds(.3f);

                                                        blockPool.ReturnPoolableObject(col_0);
                                                        blockPool.ReturnPoolableObject(col_1);
                                                        blockPool.ReturnPoolableObject(col_2);

                                                        // 점수를 더합니다!
                                                        DM.SetScore(col_0.BlockScore);
                                                        DM.SetScore(col_1.BlockScore);
                                                        DM.SetScore(col_2.BlockScore);

                                                        blocks[row, col - 1] = null;
                                                        blocks[row, col + 1] = null;
                                                        blocks[row, col + 2] = null;

                                                        // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                                        moveBlock.otherBlock.elementType = ElementType.Balance;
                                                        moveBlock.otherBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                        isMake = true;
                                                    }
                                                }
                                            }
                                            break;

                                        case 2:
                                            // OOO★
                                            col_0 = blocks[row, col - 1];
                                            col_1 = blocks[row, col - 2];
                                            col_2 = blocks[row, col - 3];

                                            if (moveBlock.otherBlock.elementType == col_0.elementType &&
                                                moveBlock.otherBlock.elementType == col_1.elementType &&
                                                moveBlock.otherBlock.elementType == col_2.elementType)
                                            {
                                                col_0.col = moveBlock.otherBlock.col;
                                                col_1.col = moveBlock.otherBlock.col;
                                                col_2.col = moveBlock.otherBlock.col;

                                                yield return new WaitForSeconds(.3f);

                                                blockPool.ReturnPoolableObject(col_0);
                                                blockPool.ReturnPoolableObject(col_1);
                                                blockPool.ReturnPoolableObject(col_2);

                                                // 점수를 더합니다!
                                                DM.SetScore(col_0.BlockScore);
                                                DM.SetScore(col_1.BlockScore);
                                                DM.SetScore(col_2.BlockScore);

                                                blocks[row, col - 1] = null;
                                                blocks[row, col - 2] = null;
                                                blocks[row, col - 3] = null;

                                                // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                                moveBlock.otherBlock.elementType = ElementType.Balance;
                                                moveBlock.otherBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                isMake = true;
                                            }
                                            else
                                            {
                                                // OO★O
                                                col_0 = blocks[row, col - 1];
                                                col_1 = blocks[row, col - 2];
                                                col_2 = blocks[row, col + 1];

                                                if (moveBlock.otherBlock.elementType == col_0.elementType &&
                                                    moveBlock.otherBlock.elementType == col_1.elementType &&
                                                    moveBlock.otherBlock.elementType == col_2.elementType)
                                                {
                                                    col_0.col = moveBlock.otherBlock.col;
                                                    col_1.col = moveBlock.otherBlock.col;
                                                    col_2.col = moveBlock.otherBlock.col;

                                                    yield return new WaitForSeconds(.3f);

                                                    blockPool.ReturnPoolableObject(col_0);
                                                    blockPool.ReturnPoolableObject(col_1);
                                                    blockPool.ReturnPoolableObject(col_2);

                                                    // 점수를 더합니다!
                                                    DM.SetScore(col_0.BlockScore);
                                                    DM.SetScore(col_1.BlockScore);
                                                    DM.SetScore(col_2.BlockScore);

                                                    blocks[row, col - 1] = null;
                                                    blocks[row, col - 2] = null;
                                                    blocks[row, col + 1] = null;

                                                    // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                                    moveBlock.otherBlock.elementType = ElementType.Balance;
                                                    moveBlock.otherBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                    isMake = true;
                                                }
                                            }
                                            break;

                                        case 3:
                                            // OOO★
                                            col_0 = blocks[row, col - 1];
                                            col_1 = blocks[row, col - 2];
                                            col_2 = blocks[row, col - 3];

                                            if (moveBlock.otherBlock.elementType == col_0.elementType &&
                                                moveBlock.otherBlock.elementType == col_1.elementType &&
                                                moveBlock.otherBlock.elementType == col_2.elementType)
                                            {
                                                col_0.col = moveBlock.otherBlock.col;
                                                col_1.col = moveBlock.otherBlock.col;
                                                col_2.col = moveBlock.otherBlock.col;

                                                yield return new WaitForSeconds(.3f);

                                                blockPool.ReturnPoolableObject(col_0);
                                                blockPool.ReturnPoolableObject(col_1);
                                                blockPool.ReturnPoolableObject(col_2);

                                                // 점수를 더합니다!
                                                DM.SetScore(col_0.BlockScore);
                                                DM.SetScore(col_1.BlockScore);
                                                DM.SetScore(col_2.BlockScore);

                                                blocks[row, col - 1] = null;
                                                blocks[row, col - 2] = null;
                                                blocks[row, col - 3] = null;

                                                // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                                moveBlock.otherBlock.elementType = ElementType.Balance;
                                                moveBlock.otherBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                isMake = true;
                                            }
                                            break;
                                    }

                                    // Row
                                    switch (moveBlock.otherBlock.row)
                                    {
                                        case -3:
                                            // ★OOO
                                            col_0 = blocks[row + 1, col];
                                            col_1 = blocks[row + 2, col];
                                            col_2 = blocks[row + 3, col];

                                            if (moveBlock.otherBlock.elementType == col_0.elementType &&
                                                moveBlock.otherBlock.elementType == col_1.elementType &&
                                                moveBlock.otherBlock.elementType == col_2.elementType)
                                            {
                                                col_0.row = moveBlock.otherBlock.row;
                                                col_1.row = moveBlock.otherBlock.row;
                                                col_2.row = moveBlock.otherBlock.row;

                                                yield return new WaitForSeconds(.3f);

                                                blockPool.ReturnPoolableObject(col_0);
                                                blockPool.ReturnPoolableObject(col_1);
                                                blockPool.ReturnPoolableObject(col_2);

                                                // 점수를 더합니다!
                                                DM.SetScore(col_0.BlockScore);
                                                DM.SetScore(col_1.BlockScore);
                                                DM.SetScore(col_2.BlockScore);

                                                blocks[row + 1, col] = null;
                                                blocks[row + 2, col] = null;
                                                blocks[row + 3, col] = null;

                                                // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                                moveBlock.otherBlock.elementType = ElementType.Balance;
                                                moveBlock.otherBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                isMake = true;
                                            }
                                            break;

                                        case -2:
                                            // ★OOO
                                            col_0 = blocks[row + 1, col];
                                            col_1 = blocks[row + 2, col];
                                            col_2 = blocks[row + 3, col];

                                            if (moveBlock.otherBlock.elementType == col_0.elementType &&
                                                moveBlock.otherBlock.elementType == col_1.elementType &&
                                                moveBlock.otherBlock.elementType == col_2.elementType)
                                            {
                                                col_0.row = moveBlock.otherBlock.row;
                                                col_1.row = moveBlock.otherBlock.row;
                                                col_2.row = moveBlock.otherBlock.row;

                                                yield return new WaitForSeconds(.3f);

                                                blockPool.ReturnPoolableObject(col_0);
                                                blockPool.ReturnPoolableObject(col_1);
                                                blockPool.ReturnPoolableObject(col_2);

                                                // 점수를 더합니다!
                                                DM.SetScore(col_0.BlockScore);
                                                DM.SetScore(col_1.BlockScore);
                                                DM.SetScore(col_2.BlockScore);

                                                blocks[row + 1, col] = null;
                                                blocks[row + 2, col] = null;
                                                blocks[row + 3, col] = null;

                                                // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                                moveBlock.otherBlock.elementType = ElementType.Balance;
                                                moveBlock.otherBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                isMake = true;
                                            }
                                            else
                                            {
                                                // O★OO
                                                col_0 = blocks[row - 1, col];
                                                col_1 = blocks[row + 1, col];
                                                col_2 = blocks[row + 2, col];

                                                if (moveBlock.otherBlock.elementType == col_0.elementType &&
                                                    moveBlock.otherBlock.elementType == col_1.elementType &&
                                                    moveBlock.otherBlock.elementType == col_2.elementType)
                                                {
                                                    col_0.row = moveBlock.otherBlock.row;
                                                    col_1.row = moveBlock.otherBlock.row;
                                                    col_2.row = moveBlock.otherBlock.row;

                                                    yield return new WaitForSeconds(.3f);

                                                    blockPool.ReturnPoolableObject(col_0);
                                                    blockPool.ReturnPoolableObject(col_1);
                                                    blockPool.ReturnPoolableObject(col_2);

                                                    // 점수를 더합니다!
                                                    DM.SetScore(col_0.BlockScore);
                                                    DM.SetScore(col_1.BlockScore);
                                                    DM.SetScore(col_2.BlockScore);

                                                    blocks[row - 1, col] = null;
                                                    blocks[row + 1, col] = null;
                                                    blocks[row + 2, col] = null;

                                                    // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                                    moveBlock.otherBlock.elementType = ElementType.Balance;
                                                    moveBlock.otherBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                    isMake = true;
                                                }
                                            }
                                            break;

                                        case -1:
                                        case 0:
                                            // ★OOO
                                            col_0 = blocks[row + 1, col];
                                            col_1 = blocks[row + 2, col];
                                            col_2 = blocks[row + 3, col];

                                            if (moveBlock.otherBlock.elementType == col_0.elementType &&
                                                moveBlock.otherBlock.elementType == col_1.elementType &&
                                                moveBlock.otherBlock.elementType == col_2.elementType)
                                            {
                                                col_0.row = moveBlock.otherBlock.row;
                                                col_1.row = moveBlock.otherBlock.row;
                                                col_2.row = moveBlock.otherBlock.row;

                                                yield return new WaitForSeconds(.3f);

                                                blockPool.ReturnPoolableObject(col_0);
                                                blockPool.ReturnPoolableObject(col_1);
                                                blockPool.ReturnPoolableObject(col_2);

                                                // 점수를 더합니다!
                                                DM.SetScore(col_0.BlockScore);
                                                DM.SetScore(col_1.BlockScore);
                                                DM.SetScore(col_2.BlockScore);

                                                blocks[row + 1, col] = null;
                                                blocks[row + 2, col] = null;
                                                blocks[row + 3, col] = null;

                                                // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                                moveBlock.otherBlock.elementType = ElementType.Balance;
                                                moveBlock.otherBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                isMake = true;
                                            }
                                            else
                                            {
                                                // O★OO
                                                col_0 = blocks[row - 1, col];
                                                col_1 = blocks[row + 1, col];
                                                col_2 = blocks[row + 2, col];

                                                if (moveBlock.otherBlock.elementType == col_0.elementType &&
                                                    moveBlock.otherBlock.elementType == col_1.elementType &&
                                                    moveBlock.otherBlock.elementType == col_2.elementType)
                                                {
                                                    col_0.row = moveBlock.row;
                                                    col_1.row = moveBlock.row;
                                                    col_2.row = moveBlock.row;

                                                    yield return new WaitForSeconds(.3f);

                                                    blockPool.ReturnPoolableObject(col_0);
                                                    blockPool.ReturnPoolableObject(col_1);
                                                    blockPool.ReturnPoolableObject(col_2);

                                                    // 점수를 더합니다!
                                                    DM.SetScore(col_0.BlockScore);
                                                    DM.SetScore(col_1.BlockScore);
                                                    DM.SetScore(col_2.BlockScore);

                                                    blocks[row - 1, col] = null;
                                                    blocks[row + 1, col] = null;
                                                    blocks[row + 2, col] = null;

                                                    // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                                    moveBlock.otherBlock.elementType = ElementType.Balance;
                                                    moveBlock.otherBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                    isMake = true;
                                                }
                                                else
                                                {
                                                    // OO★O
                                                    col_0 = blocks[row - 1, col];
                                                    col_1 = blocks[row - 2, col];
                                                    col_2 = blocks[row + 1, col];

                                                    if (moveBlock.otherBlock.elementType == col_0.elementType &&
                                                        moveBlock.otherBlock.elementType == col_1.elementType &&
                                                        moveBlock.otherBlock.elementType == col_2.elementType)
                                                    {
                                                        col_0.row = moveBlock.otherBlock.row;
                                                        col_1.row = moveBlock.otherBlock.row;
                                                        col_2.row = moveBlock.otherBlock.row;

                                                        yield return new WaitForSeconds(.3f);

                                                        blockPool.ReturnPoolableObject(col_0);
                                                        blockPool.ReturnPoolableObject(col_1);
                                                        blockPool.ReturnPoolableObject(col_2);

                                                        // 점수를 더합니다!
                                                        DM.SetScore(col_0.BlockScore);
                                                        DM.SetScore(col_1.BlockScore);
                                                        DM.SetScore(col_2.BlockScore);

                                                        blocks[row - 1, col] = null;
                                                        blocks[row - 2, col] = null;
                                                        blocks[row + 1, col] = null;

                                                        // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                                        moveBlock.otherBlock.elementType = ElementType.Balance;
                                                        moveBlock.otherBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                        isMake = true;
                                                    }
                                                }
                                            }
                                            break;

                                        case 1:
                                            // OOO★
                                            col_0 = blocks[row - 1, col];
                                            col_1 = blocks[row - 2, col];
                                            col_2 = blocks[row - 3, col];

                                            if (moveBlock.otherBlock.elementType == col_0.elementType &&
                                                moveBlock.otherBlock.elementType == col_1.elementType &&
                                                moveBlock.otherBlock.elementType == col_2.elementType)
                                            {
                                                col_0.row = moveBlock.otherBlock.row;
                                                col_1.row = moveBlock.otherBlock.row;
                                                col_2.row = moveBlock.otherBlock.row;

                                                yield return new WaitForSeconds(.3f);

                                                blockPool.ReturnPoolableObject(col_0);
                                                blockPool.ReturnPoolableObject(col_1);
                                                blockPool.ReturnPoolableObject(col_2);

                                                // 점수를 더합니다!
                                                DM.SetScore(col_0.BlockScore);
                                                DM.SetScore(col_1.BlockScore);
                                                DM.SetScore(col_2.BlockScore);

                                                blocks[row - 1, col] = null;
                                                blocks[row - 2, col] = null;
                                                blocks[row - 3, col] = null;

                                                // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                                moveBlock.otherBlock.elementType = ElementType.Balance;
                                                moveBlock.otherBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                isMake = true;
                                            }
                                            else
                                            {
                                                // OO★O
                                                col_0 = blocks[row + 1, col];
                                                col_1 = blocks[row - 1, col];
                                                col_2 = blocks[row - 2, col];

                                                if (moveBlock.otherBlock.elementType == col_0.elementType &&
                                                    moveBlock.otherBlock.elementType == col_1.elementType &&
                                                    moveBlock.otherBlock.elementType == col_2.elementType)
                                                {
                                                    col_0.row = moveBlock.otherBlock.row;
                                                    col_1.row = moveBlock.otherBlock.row;
                                                    col_2.row = moveBlock.otherBlock.row;

                                                    yield return new WaitForSeconds(.3f);

                                                    blockPool.ReturnPoolableObject(col_0);
                                                    blockPool.ReturnPoolableObject(col_1);
                                                    blockPool.ReturnPoolableObject(col_2);

                                                    // 점수를 더합니다!
                                                    DM.SetScore(col_0.BlockScore);
                                                    DM.SetScore(col_1.BlockScore);
                                                    DM.SetScore(col_2.BlockScore);

                                                    blocks[row + 1, col] = null;
                                                    blocks[row - 1, col] = null;
                                                    blocks[row - 2, col] = null;

                                                    // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                                    moveBlock.otherBlock.elementType = ElementType.Balance;
                                                    moveBlock.otherBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                    isMake = true;
                                                }
                                                else
                                                {
                                                    // O★OO
                                                    col_0 = blocks[row + 1, col];
                                                    col_1 = blocks[row + 2, col];
                                                    col_2 = blocks[row - 1, col];

                                                    if (moveBlock.otherBlock.elementType == col_0.elementType &&
                                                        moveBlock.otherBlock.elementType == col_1.elementType &&
                                                        moveBlock.otherBlock.elementType == col_2.elementType)
                                                    {
                                                        col_0.row = moveBlock.otherBlock.row;
                                                        col_1.row = moveBlock.otherBlock.row;
                                                        col_2.row = moveBlock.otherBlock.row;

                                                        yield return new WaitForSeconds(.3f);

                                                        blockPool.ReturnPoolableObject(col_0);
                                                        blockPool.ReturnPoolableObject(col_1);
                                                        blockPool.ReturnPoolableObject(col_2);

                                                        // 점수를 더합니다!
                                                        DM.SetScore(col_0.BlockScore);
                                                        DM.SetScore(col_1.BlockScore);
                                                        DM.SetScore(col_2.BlockScore);

                                                        blocks[row + 1, col] = null;
                                                        blocks[row + 2, col] = null;
                                                        blocks[row - 1, col] = null;

                                                        // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                                        moveBlock.otherBlock.elementType = ElementType.Balance;
                                                        moveBlock.otherBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                        isMake = true;
                                                    }
                                                }
                                            }
                                            break;

                                        case 2:
                                            // OOO★
                                            col_0 = blocks[row - 1, col];
                                            col_1 = blocks[row - 2, col];
                                            col_2 = blocks[row - 3, col];

                                            if (moveBlock.otherBlock.elementType == col_0.elementType &&
                                                moveBlock.otherBlock.elementType == col_1.elementType &&
                                                moveBlock.otherBlock.elementType == col_2.elementType)
                                            {
                                                col_0.row = moveBlock.otherBlock.row;
                                                col_1.row = moveBlock.otherBlock.row;
                                                col_2.row = moveBlock.otherBlock.row;

                                                yield return new WaitForSeconds(.3f);

                                                blockPool.ReturnPoolableObject(col_0);
                                                blockPool.ReturnPoolableObject(col_1);
                                                blockPool.ReturnPoolableObject(col_2);

                                                // 점수를 더합니다!
                                                DM.SetScore(col_0.BlockScore);
                                                DM.SetScore(col_1.BlockScore);
                                                DM.SetScore(col_2.BlockScore);

                                                blocks[row - 1, col] = null;
                                                blocks[row - 2, col] = null;
                                                blocks[row - 3, col] = null;

                                                // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                                moveBlock.otherBlock.elementType = ElementType.Balance;
                                                moveBlock.otherBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                isMake = true;
                                            }
                                            else
                                            {
                                                // OO★O
                                                col_0 = blocks[row + 1, col];
                                                col_1 = blocks[row - 1, col];
                                                col_2 = blocks[row - 2, col];

                                                if (moveBlock.otherBlock.elementType == col_0.elementType &&
                                                    moveBlock.otherBlock.elementType == col_1.elementType &&
                                                    moveBlock.otherBlock.elementType == col_2.elementType)
                                                {
                                                    col_0.row = moveBlock.otherBlock.row;
                                                    col_1.row = moveBlock.otherBlock.row;
                                                    col_2.row = moveBlock.otherBlock.row;

                                                    yield return new WaitForSeconds(.3f);

                                                    blockPool.ReturnPoolableObject(col_0);
                                                    blockPool.ReturnPoolableObject(col_1);
                                                    blockPool.ReturnPoolableObject(col_2);

                                                    // 점수를 더합니다!
                                                    DM.SetScore(col_0.BlockScore);
                                                    DM.SetScore(col_1.BlockScore);
                                                    DM.SetScore(col_2.BlockScore);

                                                    blocks[row + 1, col] = null;
                                                    blocks[row - 1, col] = null;
                                                    blocks[row - 2, col] = null;

                                                    // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                                    moveBlock.otherBlock.elementType = ElementType.Balance;
                                                    moveBlock.otherBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                    isMake = true;
                                                }
                                            }
                                            break;

                                        case 3:
                                            // OOO★
                                            col_0 = blocks[row - 1, col];
                                            col_1 = blocks[row - 2, col];
                                            col_2 = blocks[row - 3, col];

                                            if (moveBlock.otherBlock.elementType == col_0.elementType &&
                                                moveBlock.otherBlock.elementType == col_1.elementType &&
                                                moveBlock.otherBlock.elementType == col_2.elementType)
                                            {
                                                col_0.row = moveBlock.otherBlock.row;
                                                col_1.row = moveBlock.otherBlock.row;
                                                col_2.row = moveBlock.otherBlock.row;

                                                yield return new WaitForSeconds(.3f);

                                                blockPool.ReturnPoolableObject(col_0);
                                                blockPool.ReturnPoolableObject(col_1);
                                                blockPool.ReturnPoolableObject(col_2);

                                                // 점수를 더합니다!
                                                DM.SetScore(col_0.BlockScore);
                                                DM.SetScore(col_1.BlockScore);
                                                DM.SetScore(col_2.BlockScore);

                                                blocks[row - 1, col] = null;
                                                blocks[row - 2, col] = null;
                                                blocks[row - 3, col] = null;

                                                // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                                moveBlock.otherBlock.elementType = ElementType.Balance;
                                                moveBlock.otherBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                isMake = true;
                                            }
                                            break;
                                    }
                                }
                            }

                            // 빈 자리에 블럭 채우는 작업
                            if (isMake)
                            {
                                // 블럭 다운 카운트 계산
                                int downCount = 0;

                                for (int c_Col = 0; c_Col < width; c_Col++)
                                {
                                    for (int c_Row = 0; c_Row < height; c_Row++)
                                    {
                                        if (blocks[c_Row, c_Col] == null)
                                        {
                                            downCount++;
                                        }
                                        else
                                        {
                                            blocks[c_Row, c_Col].downCount = downCount;
                                        }
                                    }

                                    downCount = 0;
                                }

                                yield return new WaitForSeconds(.3f);

                                // 블럭 내리기 작업
                                for (int d_Row = 0; d_Row < height; d_Row++)
                                {
                                    for (int d_Col = 0; d_Col < width; d_Col++)
                                    {
                                        if (blocks[d_Row, d_Col] != null)
                                        {
                                            if (blocks[d_Row, d_Col].downCount > 0)
                                            {
                                                // 내릴 공간이 존재한다

                                                var targetrow = (blocks[d_Row, d_Col].row -= blocks[d_Row, d_Col].downCount);

                                                if (Mathf.Abs(targetrow - blocks[d_Row, d_Col].transform.localPosition.y) > .1f)
                                                {
                                                    Vector2 tempposition = new Vector2(blocks[d_Row, d_Col].transform.localPosition.x, targetrow);
                                                    blocks[d_Row, d_Col].transform.localPosition = Vector2.Lerp(blocks[d_Row, d_Col].transform.localPosition, tempposition, .05f);
                                                }

                                                // 다운 카운트 초기화
                                                blocks[d_Row, d_Col].downCount = 0;
                                            }
                                        }
                                    }
                                }

                                yield return new WaitForSeconds(.3f);

                                // 빈 공간 블럭 생성
                                int newCol = -3;
                                int newRow = 3;

                                for (int n_Col = 0; n_Col < width; n_Col++)
                                {
                                    for (int n_Row = 0; n_Row < height; n_Row++)
                                    {
                                        if (blocks[n_Row, n_Col] == null)
                                        {
                                            // 어? 여기 비어있는데? 탈모인데?
                                            // 그러면 머리 심어줘야자~
                                            Block newBlock = blockPool.GetPoolableObject(obj => obj.CanRecycle);

                                            // 부모 설정
                                            newBlock.transform.SetParent(this.transform);

                                            // 위치값 설정
                                            newBlock.transform.localPosition = new Vector3(newCol, 4 + n_Row, 0);

                                            // 회전값 설정
                                            newBlock.transform.localRotation = Quaternion.identity;

                                            // 사이즈값 설정
                                            newBlock.transform.localScale = new Vector3(.19f, .19f, .19f);

                                            // 활성화
                                            newBlock.gameObject.SetActive(true);

                                            // 블럭 초기화
                                            newBlock.Initialize(newCol, 4 + n_Row);

                                            // 블럭 저장
                                            blocks[n_Row, n_Col] = newBlock;

                                            var targetrow = (newBlock.row = newRow);

                                            if (Mathf.Abs(targetrow - newBlock.transform.localPosition.y) > .1f)
                                            {
                                                Vector2 tempposition = new Vector2(newBlock.transform.localPosition.x, targetrow);
                                                newBlock.transform.localPosition = Vector2.Lerp(newBlock.transform.localPosition, tempposition, .05f);
                                            }

                                            newRow--;
                                        }
                                    }

                                    newCol++;
                                    newRow = 3;
                                }

                                isMake = false;

                                BlockSort();
                            }
                        }
                    }
                }

                // 비워주기
                moveBlock = null;
            }
            // 이동 블럭이 없는 경우 (예외 상황 작성 중..)
            else
            {
                // 특수 상황 모음집
                for (int row = 0; row < height; row++)
                {
                    for (int col = 0; col < width; col++)
                    {
                        if (blocks[row, col] != null)
                        {
                            checkBlock = blocks[row, col];

                            if (checkBlock.col <= 0)
                            {
                                col_0 = blocks[row, col + 1];
                                col_1 = blocks[row, col + 2];
                                col_2 = blocks[row, col + 3];

                                // 4X4 특수 상황 모음집 1탄
                                if (checkBlock.row <= 0 && (col_0 != null && col_1 != null && col_2 != null))
                                {
                                    /*
                                     * O
                                     * O
                                     * O
                                     * O O O O
                                     */
                                    row_0 = blocks[row + 1, col];
                                    row_1 = blocks[row + 2, col];
                                    row_2 = blocks[row + 3, col];

                                    if (row_0 != null && row_1 != null & row_2 != null)
                                    {
                                        if ((checkBlock.elementType == col_0.elementType && checkBlock.elementType == col_1.elementType && checkBlock.elementType == col_2.elementType) &&
                                            (checkBlock.elementType == row_0.elementType && checkBlock.elementType == row_1.elementType && checkBlock.elementType == row_2.elementType))
                                        {
                                            // 블럭 이동
                                            col_0.col = checkBlock.col;
                                            col_1.col = checkBlock.col;
                                            col_2.col = checkBlock.col;

                                            row_0.row = checkBlock.row;
                                            row_1.row = checkBlock.row;
                                            row_2.row = checkBlock.row;

                                            yield return new WaitForSeconds(.3f);

                                            blockPool.ReturnPoolableObject(col_0);
                                            blockPool.ReturnPoolableObject(col_1);
                                            blockPool.ReturnPoolableObject(col_2);

                                            blockPool.ReturnPoolableObject(row_0);
                                            blockPool.ReturnPoolableObject(row_1);
                                            blockPool.ReturnPoolableObject(row_2);

                                            DM.SetScore(col_0.BlockScore);
                                            DM.SetScore(col_1.BlockScore);
                                            DM.SetScore(col_2.BlockScore);

                                            DM.SetScore(row_0.BlockScore);
                                            DM.SetScore(row_1.BlockScore);
                                            DM.SetScore(row_2.BlockScore);

                                            // 블럭 제거
                                            blocks[row, col + 1] = null;
                                            blocks[row, col + 2] = null;
                                            blocks[row, col + 3] = null;

                                            blocks[row + 1, col] = null;
                                            blocks[row + 2, col] = null;
                                            blocks[row + 3, col] = null;

                                            checkBlock.elementType = ElementType.Balance;
                                            checkBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                            isMake = true;
                                        }
                                        else
                                        {
                                            /*
                                             *   O
                                             *   O
                                             *   O
                                             * O O O O
                                             */
                                            checkBlock = blocks[row, col + 1];

                                            col_0 = blocks[row, col];

                                            row_0 = blocks[row + 1, col + 1];
                                            row_1 = blocks[row + 2, col + 1];
                                            row_2 = blocks[row + 3, col + 1];

                                            if (row_0 != null && row_1 != null & row_2 != null)
                                            {
                                                if ((checkBlock.elementType == col_0.elementType && checkBlock.elementType == col_1.elementType && checkBlock.elementType == col_2.elementType) &&
                                                    (checkBlock.elementType == row_0.elementType && checkBlock.elementType == row_1.elementType && checkBlock.elementType == row_2.elementType))
                                                {
                                                    // 블럭 이동
                                                    col_0.col = checkBlock.col;
                                                    col_1.col = checkBlock.col;
                                                    col_2.col = checkBlock.col;

                                                    row_0.row = checkBlock.row;
                                                    row_1.row = checkBlock.row;
                                                    row_2.row = checkBlock.row;

                                                    yield return new WaitForSeconds(.3f);

                                                    blockPool.ReturnPoolableObject(col_0);
                                                    blockPool.ReturnPoolableObject(col_1);
                                                    blockPool.ReturnPoolableObject(col_2);

                                                    blockPool.ReturnPoolableObject(row_0);
                                                    blockPool.ReturnPoolableObject(row_1);
                                                    blockPool.ReturnPoolableObject(row_2);

                                                    DM.SetScore(col_0.BlockScore);
                                                    DM.SetScore(col_1.BlockScore);
                                                    DM.SetScore(col_2.BlockScore);

                                                    DM.SetScore(row_0.BlockScore);
                                                    DM.SetScore(row_1.BlockScore);
                                                    DM.SetScore(row_2.BlockScore);

                                                    // 블럭 제거
                                                    blocks[row, col] = null;
                                                    blocks[row, col + 2] = null;
                                                    blocks[row, col + 3] = null;

                                                    blocks[row + 1, col + 1] = null;
                                                    blocks[row + 2, col + 1] = null;
                                                    blocks[row + 3, col + 1] = null;

                                                    checkBlock.elementType = ElementType.Balance;
                                                    checkBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                    isMake = true;
                                                }
                                                else
                                                {
                                                    /*   
                                                     *      O
                                                     *      O
                                                     *      O
                                                     *  O O O O
                                                     */
                                                    checkBlock = blocks[row, col + 2];

                                                    col_0 = blocks[row, col];
                                                    col_1 = blocks[row, col + 1];

                                                    row_0 = blocks[row + 1, col + 2];
                                                    row_1 = blocks[row + 2, col + 2];
                                                    row_2 = blocks[row + 3, col + 2];

                                                    if (row_0 != null && row_1 != null & row_2 != null)
                                                    {
                                                        if ((checkBlock.elementType == col_0.elementType && checkBlock.elementType == col_1.elementType && checkBlock.elementType == col_2.elementType) &&
                                                            (checkBlock.elementType == row_0.elementType && checkBlock.elementType == row_1.elementType && checkBlock.elementType == row_2.elementType))
                                                        {
                                                            col_0.col = checkBlock.col;
                                                            col_1.col = checkBlock.col;
                                                            col_2.col = checkBlock.col;

                                                            row_0.row = checkBlock.row;
                                                            row_1.row = checkBlock.row;
                                                            row_2.row = checkBlock.row;

                                                            yield return new WaitForSeconds(.3f);

                                                            blockPool.ReturnPoolableObject(col_0);
                                                            blockPool.ReturnPoolableObject(col_1);
                                                            blockPool.ReturnPoolableObject(col_2);

                                                            blockPool.ReturnPoolableObject(row_0);
                                                            blockPool.ReturnPoolableObject(row_1);
                                                            blockPool.ReturnPoolableObject(row_2);

                                                            DM.SetScore(col_0.BlockScore);
                                                            DM.SetScore(col_1.BlockScore);
                                                            DM.SetScore(col_2.BlockScore);

                                                            DM.SetScore(row_0.BlockScore);
                                                            DM.SetScore(row_1.BlockScore);
                                                            DM.SetScore(row_2.BlockScore);

                                                            blocks[row, col + 1] = null;
                                                            blocks[row, col + 2] = null;
                                                            blocks[row, col + 3] = null;

                                                            blocks[row + 1, col + 2] = null;
                                                            blocks[row + 2, col + 2] = null;
                                                            blocks[row + 3, col + 2] = null;

                                                            checkBlock.elementType = ElementType.Balance;
                                                            checkBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                            isMake = true;
                                                        }
                                                        else
                                                        {
                                                            /*
                                                             *        O
                                                             *        O
                                                             *        O
                                                             *  O O O O
                                                             */
                                                            checkBlock = blocks[row, col + 3];

                                                            col_0 = blocks[row, col];
                                                            col_1 = blocks[row, col + 1];
                                                            col_2 = blocks[row, col + 2];

                                                            row_0 = blocks[row + 1, col + 3];
                                                            row_1 = blocks[row + 2, col + 3];
                                                            row_2 = blocks[row + 3, col + 3];

                                                            if (row_0 != null && row_1 != null & row_2 != null)
                                                            {
                                                                if ((checkBlock.elementType == col_0.elementType && checkBlock.elementType == col_1.elementType && checkBlock.elementType == col_2.elementType) &&
                                                                    (checkBlock.elementType == row_0.elementType && checkBlock.elementType == row_1.elementType && checkBlock.elementType == row_2.elementType))
                                                                {
                                                                    col_0.col = checkBlock.col;
                                                                    col_1.col = checkBlock.col;
                                                                    col_2.col = checkBlock.col;

                                                                    row_0.row = checkBlock.row;
                                                                    row_1.row = checkBlock.row;
                                                                    row_2.row = checkBlock.row;

                                                                    yield return new WaitForSeconds(.3f);

                                                                    blockPool.ReturnPoolableObject(col_0);
                                                                    blockPool.ReturnPoolableObject(col_1);
                                                                    blockPool.ReturnPoolableObject(col_2);

                                                                    blockPool.ReturnPoolableObject(row_0);
                                                                    blockPool.ReturnPoolableObject(row_1);
                                                                    blockPool.ReturnPoolableObject(row_2);

                                                                    DM.SetScore(col_0.BlockScore);
                                                                    DM.SetScore(col_1.BlockScore);
                                                                    DM.SetScore(col_2.BlockScore);

                                                                    DM.SetScore(row_0.BlockScore);
                                                                    DM.SetScore(row_1.BlockScore);
                                                                    DM.SetScore(row_2.BlockScore);

                                                                    blocks[row, col] = null;
                                                                    blocks[row, col + 1] = null;
                                                                    blocks[row, col + 2] = null;

                                                                    blocks[row + 1, col + 3] = null;
                                                                    blocks[row + 2, col + 3] = null;
                                                                    blocks[row + 3, col + 3] = null;

                                                                    checkBlock.elementType = ElementType.Balance;
                                                                    checkBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                                    isMake = true;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                // 4X4 특수 상황 모음집 2탄
                                if (checkBlock.row >= 0 && (col_0 != null && col_1 != null && col_2 != null))
                                {
                                    /*
                                     * O O O O
                                     * O
                                     * O
                                     * O
                                     */
                                    row_0 = blocks[row - 1, col];
                                    row_1 = blocks[row - 2, col];
                                    row_2 = blocks[row - 3, col];

                                    if (row_0 != null && row_1 != null && row_2 != null)
                                    {
                                        if ((checkBlock.elementType == col_0.elementType && checkBlock.elementType == col_1.elementType && checkBlock.elementType == col_2.elementType) &&
                                            (checkBlock.elementType == row_0.elementType && checkBlock.elementType == row_1.elementType && checkBlock.elementType == row_2.elementType))
                                        {
                                            col_0.col = checkBlock.col;
                                            col_1.col = checkBlock.col;
                                            col_2.col = checkBlock.col;

                                            row_0.row = checkBlock.row;
                                            row_1.row = checkBlock.row;
                                            row_2.row = checkBlock.row;

                                            yield return new WaitForSeconds(.3f);

                                            blockPool.ReturnPoolableObject(col_0);
                                            blockPool.ReturnPoolableObject(col_1);
                                            blockPool.ReturnPoolableObject(col_2);

                                            blockPool.ReturnPoolableObject(row_0);
                                            blockPool.ReturnPoolableObject(row_1);
                                            blockPool.ReturnPoolableObject(row_2);

                                            DM.SetScore(col_0.BlockScore);
                                            DM.SetScore(col_1.BlockScore);
                                            DM.SetScore(col_2.BlockScore);

                                            DM.SetScore(row_0.BlockScore);
                                            DM.SetScore(row_1.BlockScore);
                                            DM.SetScore(row_2.BlockScore);

                                            blocks[row, col + 1] = null;
                                            blocks[row, col + 2] = null;
                                            blocks[row, col + 3] = null;

                                            blocks[row - 1, col] = null;
                                            blocks[row - 2, col] = null;
                                            blocks[row - 3, col] = null;

                                            checkBlock.elementType = ElementType.Balance;
                                            checkBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                            isMake = true;
                                        }
                                        else
                                        {
                                            /*
                                             * O O O O
                                             *   O
                                             *   O
                                             *   O
                                             */
                                            checkBlock = blocks[row, col + 1];

                                            col_0 = blocks[row, col];

                                            row_0 = blocks[row - 1, col + 1];
                                            row_1 = blocks[row - 2, col + 1];
                                            row_2 = blocks[row - 3, col + 1];

                                            if (row_0 != null && row_1 != null && row_2 != null)
                                            {
                                                if ((checkBlock.elementType == col_0.elementType && checkBlock.elementType == col_1.elementType && checkBlock.elementType == col_2.elementType) &&
                                                    (checkBlock.elementType == row_0.elementType && checkBlock.elementType == row_1.elementType && checkBlock.elementType == row_2.elementType))
                                                {
                                                    col_0.col = checkBlock.col;
                                                    col_1.col = checkBlock.col;
                                                    col_2.col = checkBlock.col;

                                                    row_0.row = checkBlock.row;
                                                    row_1.row = checkBlock.row;
                                                    row_2.row = checkBlock.row;

                                                    yield return new WaitForSeconds(.3f);

                                                    blockPool.ReturnPoolableObject(col_0);
                                                    blockPool.ReturnPoolableObject(col_1);
                                                    blockPool.ReturnPoolableObject(col_2);

                                                    blockPool.ReturnPoolableObject(row_0);
                                                    blockPool.ReturnPoolableObject(row_1);
                                                    blockPool.ReturnPoolableObject(row_2);

                                                    DM.SetScore(col_0.BlockScore);
                                                    DM.SetScore(col_1.BlockScore);
                                                    DM.SetScore(col_2.BlockScore);

                                                    DM.SetScore(row_0.BlockScore);
                                                    DM.SetScore(row_1.BlockScore);
                                                    DM.SetScore(row_2.BlockScore);

                                                    blocks[row, col] = null;
                                                    blocks[row, col + 2] = null;
                                                    blocks[row, col + 3] = null;

                                                    blocks[row - 1, col + 1] = null;
                                                    blocks[row - 2, col + 1] = null;
                                                    blocks[row - 3, col + 1] = null;

                                                    checkBlock.elementType = ElementType.Balance;
                                                    checkBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                    isMake = true;
                                                }
                                                else
                                                {
                                                    /*
                                                     * O O O O
                                                     *     O
                                                     *     O
                                                     *     O
                                                     */
                                                    checkBlock = blocks[row, col + 2];

                                                    col_0 = blocks[row, col];
                                                    col_1 = blocks[row, col + 1];

                                                    row_0 = blocks[row - 1, col + 2];
                                                    row_1 = blocks[row - 2, col + 2];
                                                    row_2 = blocks[row - 3, col + 2];

                                                    if (row_0 != null && row_1 != null && row_2 != null)
                                                    {
                                                        if ((checkBlock.elementType == col_0.elementType && checkBlock.elementType == col_1.elementType && checkBlock.elementType == col_2.elementType) &&
                                                            (checkBlock.elementType == row_0.elementType && checkBlock.elementType == row_1.elementType && checkBlock.elementType == row_2.elementType))
                                                        {
                                                            col_0.col = checkBlock.col;
                                                            col_1.col = checkBlock.col;
                                                            col_2.col = checkBlock.col;

                                                            row_0.row = checkBlock.row;
                                                            row_1.row = checkBlock.row;
                                                            row_2.row = checkBlock.row;

                                                            yield return new WaitForSeconds(.3f);

                                                            blockPool.ReturnPoolableObject(col_0);
                                                            blockPool.ReturnPoolableObject(col_1);
                                                            blockPool.ReturnPoolableObject(col_2);

                                                            blockPool.ReturnPoolableObject(row_0);
                                                            blockPool.ReturnPoolableObject(row_1);
                                                            blockPool.ReturnPoolableObject(row_2);

                                                            DM.SetScore(col_0.BlockScore);
                                                            DM.SetScore(col_1.BlockScore);
                                                            DM.SetScore(col_2.BlockScore);

                                                            DM.SetScore(row_0.BlockScore);
                                                            DM.SetScore(row_1.BlockScore);
                                                            DM.SetScore(row_2.BlockScore);

                                                            blocks[row, col] = null;
                                                            blocks[row, col + 1] = null;
                                                            blocks[row, col + 3] = null;

                                                            blocks[row - 1, col + 2] = null;
                                                            blocks[row - 2, col + 2] = null;
                                                            blocks[row - 3, col + 2] = null;

                                                            checkBlock.elementType = ElementType.Balance;
                                                            checkBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                            isMake = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        /*
                                                         * O O O O
                                                         *       O
                                                         *       O
                                                         *       O
                                                         */
                                                        checkBlock = blocks[row, col + 3];

                                                        col_0 = blocks[row, col];
                                                        col_1 = blocks[row, col + 1];
                                                        col_2 = blocks[row, col + 2];

                                                        row_0 = blocks[row - 1, col + 3];
                                                        row_1 = blocks[row - 2, col + 3];
                                                        row_2 = blocks[row - 3, col + 3];

                                                        if (row_0 != null && row_1 != null && row_2 != null)
                                                        {
                                                            if ((checkBlock.elementType == col_0.elementType && checkBlock.elementType == col_1.elementType && checkBlock.elementType == col_2.elementType) &&
                                                                (checkBlock.elementType == row_0.elementType && checkBlock.elementType == row_1.elementType && checkBlock.elementType == row_2.elementType))
                                                            {
                                                                col_0.col = blocks[row, col + 3].col;
                                                                col_1.col = blocks[row, col + 3].col;
                                                                col_2.col = blocks[row, col + 3].col;

                                                                row_0.row = blocks[row, col + 3].row;
                                                                row_1.row = blocks[row, col + 3].row;
                                                                row_2.row = blocks[row, col + 3].row;

                                                                yield return new WaitForSeconds(.3f);

                                                                blockPool.ReturnPoolableObject(col_0);
                                                                blockPool.ReturnPoolableObject(col_1);
                                                                blockPool.ReturnPoolableObject(col_2);

                                                                blockPool.ReturnPoolableObject(row_0);
                                                                blockPool.ReturnPoolableObject(row_1);
                                                                blockPool.ReturnPoolableObject(row_2);

                                                                DM.SetScore(col_0.BlockScore);
                                                                DM.SetScore(col_1.BlockScore);
                                                                DM.SetScore(col_2.BlockScore);

                                                                DM.SetScore(row_0.BlockScore);
                                                                DM.SetScore(row_1.BlockScore);
                                                                DM.SetScore(row_2.BlockScore);

                                                                blocks[row, col] = null;
                                                                blocks[row, col + 1] = null;
                                                                blocks[row, col + 2] = null;

                                                                blocks[row - 1, col + 3] = null;
                                                                blocks[row - 2, col + 3] = null;
                                                                blocks[row - 3, col + 3] = null;

                                                                checkBlock.elementType = ElementType.Balance;
                                                                checkBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                                isMake = true;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                }






                                /// 일반 상황인데 이것도 3X3에서 일반 상황 처리한것처럼 해야할듯
                                if (checkBlock.elementType == col_0.elementType &&
                                    checkBlock.elementType == col_1.elementType &&
                                    checkBlock.elementType == col_2.elementType)
                                {
                                    Debug.Log("Col <= 0");

                                    col_0.col = checkBlock.col;
                                    col_1.col = checkBlock.col;
                                    col_2.col = checkBlock.col;

                                    yield return new WaitForSeconds(.3f);

                                    // 풀에 리턴
                                    blockPool.ReturnPoolableObject(col_0);
                                    blockPool.ReturnPoolableObject(col_1);
                                    blockPool.ReturnPoolableObject(col_2);

                                    // 점수를 더합니다!
                                    DM.SetScore(col_0.BlockScore);
                                    DM.SetScore(col_1.BlockScore);
                                    DM.SetScore(col_2.BlockScore);

                                    // 블럭 저장소에서 제거
                                    blocks[row, col + 1] = null;
                                    blocks[row, col + 2] = null;
                                    blocks[row, col + 3] = null;

                                    // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                    checkBlock.elementType = ElementType.Balance;
                                    checkBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                    isMake = true;
                                }
                            }
                            else if (checkBlock.col >= 0)
                            {

                            }

                            else if (checkBlock.col >= 0)
                            {
                                col_0 = blocks[row, col - 1];
                                col_1 = blocks[row, col - 2];
                                col_2 = blocks[row, col - 3];

                                if (checkBlock.elementType == col_0.elementType &&
                                    checkBlock.elementType == col_1.elementType &&
                                    checkBlock.elementType == col_2.elementType)
                                {
                                    Debug.Log("Col >= 0");

                                    col_0.col = checkBlock.col;
                                    col_1.col = checkBlock.col;
                                    col_2.col = checkBlock.col;

                                    yield return new WaitForSeconds(.3f);

                                    // 풀에 리턴
                                    blockPool.ReturnPoolableObject(col_0);
                                    blockPool.ReturnPoolableObject(col_1);
                                    blockPool.ReturnPoolableObject(col_2);

                                    // 점수를 더합니다!
                                    DM.SetScore(col_0.BlockScore);
                                    DM.SetScore(col_1.BlockScore);
                                    DM.SetScore(col_2.BlockScore);

                                    // 블럭 저장소에서 제거
                                    blocks[row, col - 1] = null;
                                    blocks[row, col - 2] = null;
                                    blocks[row, col - 3] = null;

                                    // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                    checkBlock.elementType = ElementType.Balance;
                                    checkBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                    isMake = true;
                                }
                            }
                            else if (checkBlock.row <= 0)
                            {
                                col_0 = blocks[row + 1, col];
                                col_1 = blocks[row + 2, col];
                                col_2 = blocks[row + 3, col];

                                if (checkBlock.elementType == col_0.elementType &&
                                    checkBlock.elementType == col_1.elementType &&
                                    checkBlock.elementType == col_2.elementType)
                                {
                                    Debug.Log("Row <= 0");

                                    col_0.row = checkBlock.row;
                                    col_1.row = checkBlock.row;
                                    col_2.row = checkBlock.row;

                                    yield return new WaitForSeconds(.3f);

                                    // 풀에 리턴
                                    blockPool.ReturnPoolableObject(col_0);
                                    blockPool.ReturnPoolableObject(col_1);
                                    blockPool.ReturnPoolableObject(col_2);

                                    // 점수를 더합니다!
                                    DM.SetScore(col_0.BlockScore);
                                    DM.SetScore(col_1.BlockScore);
                                    DM.SetScore(col_2.BlockScore);

                                    // 블럭 저장소에서 제거
                                    blocks[row + 1, col] = null;
                                    blocks[row + 2, col] = null;
                                    blocks[row + 3, col] = null;

                                    // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                    checkBlock.elementType = ElementType.Balance;
                                    checkBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                    isMake = true;
                                }
                            }
                            else if (checkBlock.row >= 0)
                            {
                                col_0 = blocks[row - 1, col];
                                col_1 = blocks[row - 2, col];
                                col_2 = blocks[row - 3, col];

                                if (checkBlock.elementType == col_0.elementType &&
                                    checkBlock.elementType == col_1.elementType &&
                                    checkBlock.elementType == col_2.elementType)
                                {
                                    Debug.Log("Row >= 0");

                                    col_0.row = checkBlock.row;
                                    col_1.row = checkBlock.row;
                                    col_2.row = checkBlock.row;

                                    yield return new WaitForSeconds(.3f);

                                    // 풀에 리턴
                                    blockPool.ReturnPoolableObject(col_0);
                                    blockPool.ReturnPoolableObject(col_1);
                                    blockPool.ReturnPoolableObject(col_2);

                                    // 점수를 더합니다!
                                    DM.SetScore(col_0.BlockScore);
                                    DM.SetScore(col_1.BlockScore);
                                    DM.SetScore(col_2.BlockScore);

                                    // 블럭 저장소에서 제거
                                    blocks[row - 1, col] = null;
                                    blocks[row - 2, col] = null;
                                    blocks[row - 3, col] = null;

                                    // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                    checkBlock.elementType = ElementType.Balance;
                                    checkBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                    isMake = true;
                                }
                            }

                            if (isMake)
                            {
                                // 블럭 다운 카운트 계산
                                int downCount = 0;

                                for (int c_Col = 0; c_Col < width; c_Col++)
                                {
                                    for (int c_Row = 0; c_Row < height; c_Row++)
                                    {
                                        if (blocks[c_Row, c_Col] == null)
                                        {
                                            downCount++;
                                        }
                                        else
                                        {
                                            blocks[c_Row, c_Col].downCount = downCount;
                                        }
                                    }

                                    downCount = 0;
                                }

                                yield return new WaitForSeconds(.3f);

                                // 블럭 내리기 작업
                                for (int d_Row = 0; d_Row < height; d_Row++)
                                {
                                    for (int d_Col = 0; d_Col < width; d_Col++)
                                    {
                                        if (blocks[d_Row, d_Col] != null)
                                        {
                                            if (blocks[d_Row, d_Col].downCount > 0)
                                            {
                                                // 내릴 공간이 존재한다

                                                var targetrow = (blocks[d_Row, d_Col].row -= blocks[d_Row, d_Col].downCount);

                                                if (Mathf.Abs(targetrow - blocks[d_Row, d_Col].transform.localPosition.y) > .1f)
                                                {
                                                    Vector2 tempposition = new Vector2(blocks[d_Row, d_Col].transform.localPosition.x, targetrow);
                                                    blocks[d_Row, d_Col].transform.localPosition = Vector2.Lerp(blocks[d_Row, d_Col].transform.localPosition, tempposition, .05f);
                                                }

                                                // 다운 카운트 초기화
                                                blocks[d_Row, d_Col].downCount = 0;
                                            }
                                        }
                                    }
                                }

                                yield return new WaitForSeconds(.3f);

                                // 빈 공간 블럭 생성
                                int newCol = -3;
                                int newRow = 3;

                                for (int n_Col = 0; n_Col < width; n_Col++)
                                {
                                    for (int n_Row = 0; n_Row < height; n_Row++)
                                    {
                                        if (blocks[n_Row, n_Col] == null)
                                        {
                                            // 어? 여기 비어있는데? 탈모인데?
                                            // 그러면 머리 심어줘야자~
                                            Block newBlock = blockPool.GetPoolableObject(obj => obj.CanRecycle);

                                            // 부모 설정
                                            newBlock.transform.SetParent(this.transform);

                                            // 위치값 설정
                                            newBlock.transform.localPosition = new Vector3(newCol, 4 + n_Row, 0);

                                            // 회전값 설정
                                            newBlock.transform.localRotation = Quaternion.identity;

                                            // 사이즈값 설정
                                            newBlock.transform.localScale = new Vector3(.19f, .19f, .19f);

                                            // 활성화
                                            newBlock.gameObject.SetActive(true);

                                            // 블럭 초기화
                                            newBlock.Initialize(newCol, 4 + n_Row);

                                            // 블럭 저장
                                            blocks[n_Row, n_Col] = newBlock;

                                            var targetrow = (newBlock.row = newRow);

                                            if (Mathf.Abs(targetrow - newBlock.transform.localPosition.y) > .1f)
                                            {
                                                Vector2 tempposition = new Vector2(newBlock.transform.localPosition.x, targetrow);
                                                newBlock.transform.localPosition = Vector2.Lerp(newBlock.transform.localPosition, tempposition, .05f);
                                            }

                                            newRow--;
                                        }
                                    }

                                    newCol++;
                                    newRow = 3;
                                }

                                /// Test
                                yield return new WaitForSeconds(.3f);

                                isMake = false;

                                // 블록 정렬
                                BlockSort();
                            }
                        }



                    }
                }

                // 일반 상황 모음집

            }
        }

        /// <summary>
        /// 폭탄 기능을 가진 코루틴
        /// </summary>
        /// <returns></returns>
        private IEnumerator BoomFun(int boomCol)
        {
            /// 파티클 실행
            var blockPool = ObjectPoolManager.Instance.GetPool<Block>(PoolType.Block);

            UIElement uiElement = UIWindowManager.Instance.GetWindow<UIElement>();

            // Row
            for (int col = 0; col < width; col++)
            {
                for (int row = 0; row < height; row++)
                {
                    if (blocks[row, col] != null)
                    {
                        if (blocks[row, col].col == boomCol)
                        {
                            // 점수 업데이트
                            DataManager.Instance.SetScore(blocks[row, col].BlockScore);

                            // 스킬 게이지 업데이트
                            uiElement.SetGauge(blocks[row, col].ElementValue);

                            // 블럭 파괴
                            blockPool.ReturnPoolableObject(blocks[row, col]);

                            // 같은 Col에 있는 블럭 파괴
                            blocks[row, col] = null;
                        }
                    }
                }
            }

            // Col
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (blocks[row, col] != null)
                    {
                        if (blocks[row, col].row == -3)
                        {
                            // 점수 업데이트
                            DataManager.Instance.SetScore(blocks[row, col].BlockScore);

                            // 스킬 게이지 업데이트
                            uiElement.SetGauge(blocks[row, col].ElementValue);

                            // 블럭 파괴
                            blockPool.ReturnPoolableObject(blocks[row, col]);

                            // 같은 Col에 있는 블럭 파괴
                            blocks[row, col] = null;
                        }
                    }
                }
            }

            // 블럭 다운 카운트 계산
            int downCount = 0;

            for (int col = 0; col < width; col++)
            {
                for (int row = 0; row < height; row++)
                {
                    if (blocks[row, col] == null)
                    {
                        downCount++;
                    }
                    else
                    {
                        blocks[row, col].downCount = downCount;
                    }
                }

                downCount = 0;
            }

            // 블럭 내리기 작업
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (blocks[row, col] != null)
                    {
                        if (blocks[row, col].downCount > 0)
                        {
                            // 내릴 공간이 존재한다
                            var targetrow = (blocks[row, col].row -= blocks[row, col].downCount);

                            if (Mathf.Abs(targetrow - blocks[row, col].transform.localPosition.y) > .1f)
                            {
                                Vector2 tempposition = new Vector2(blocks[row, col].transform.localPosition.x, targetrow);
                                blocks[row, col].transform.localPosition = Vector2.Lerp(blocks[row, col].transform.localPosition, tempposition, .05f);
                            }

                            // 다운 카운트 초기화
                            blocks[row, col].downCount = 0;
                        }
                    }
                }
            }

            yield return new WaitForSeconds(.3f);

            int newCol = -3;
            int newRow = 3;

            // 블럭 생성 및 이동
            for (int col = 0; col < width; col++)
            {
                for (int row = 0; row < height; row++)
                {
                    if (blocks[row, col] == null)
                    {
                        Block newBlock = blockPool.GetPoolableObject(obj => obj.CanRecycle);

                        // 부모 설정
                        newBlock.transform.SetParent(this.transform);

                        // 위치값 설정
                        newBlock.transform.localPosition = new Vector3(newCol, row + 4, 0);

                        // 회전값 설정
                        newBlock.transform.localRotation = Quaternion.identity;

                        // 사이즈값 설정
                        newBlock.transform.localScale = new Vector3(.19f, .19f, .19f);

                        // 활성화
                        newBlock.gameObject.SetActive(true);

                        // 블럭 초기화
                        newBlock.Initialize(newCol, row + 4);

                        // 블럭 저장
                        blocks[row, col] = newBlock;

                        var targetrow = (newBlock.row = newRow);

                        if (Mathf.Abs(targetrow - newBlock.transform.localPosition.y) > .1f)
                        {
                            Vector2 tempposition = new Vector2(newBlock.transform.localPosition.x, targetrow);
                            newBlock.transform.localPosition = Vector2.Lerp(newBlock.transform.localPosition, tempposition, .05f);
                        }

                        newRow--;
                    }
                }

                newCol++;
                newRow = 3;
            }

            // 블록 정렬
            BlockSort();

            if (MatchCheck())
            {
                // 블럭 매칭 재시작
                StartCoroutine(BlockMatch());
            }
            else
            {

                // GameState -> Play
                GM.SetGameState(GameState.Play);
            }
        }

        /// <summary>
        /// 블럭 주위에 같은 블럭이 있는지 체크하는 메서드
        /// </summary>
        /// <returns></returns>
        public bool MatchCheck()
        {
            int matchCount = 0;

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (blocks[row, col] != null)
                    {
                        if (col != 5 && col != 6)
                        {
                            if (blocks[row, col].elementType == blocks[row, col + 1].elementType &&
                                blocks[row, col].elementType == blocks[row, col + 2].elementType)
                            {
                                matchCount++;
                            }
                        }
                    }

                    if (blocks[row, col] != null)
                    {
                        if (row != 5 && row != 6)
                        {
                            if (blocks[row, col].elementType == blocks[row + 1, col].elementType &&
                                blocks[row, col].elementType == blocks[row + 2, col].elementType)
                            {
                                matchCount++;
                            }
                        }
                    }
                }
            }

            if (matchCount > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 블럭을 정렬하는 메서드
        /// </summary>
        public void BlockSort()
        {
            Block[,] testArr = new Block[7, 7];

            int curCol = 0;
            int curRow = 0;

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    Block checkBlock = blocks[row, col];

                    if (checkBlock != null)
                    {
                        switch (checkBlock.col)
                        {
                            case -3:
                                curCol = 0;
                                break;

                            case -2:
                                curCol = 1;
                                break;

                            case -1:
                                curCol = 2;
                                break;

                            case -0:
                                curCol = 3;
                                break;

                            case 1:
                                curCol = 4;
                                break;

                            case 2:
                                curCol = 5;
                                break;

                            case 3:
                                curCol = 6;
                                break;
                        }

                        switch (checkBlock.row)
                        {
                            case -3:
                                curRow = 0;
                                break;

                            case -2:
                                curRow = 1;
                                break;

                            case -1:
                                curRow = 2;
                                break;

                            case -0:
                                curRow = 3;
                                break;

                            case 1:
                                curRow = 4;
                                break;

                            case 2:
                                curRow = 5;
                                break;

                            case 3:
                                curRow = 6;
                                break;
                        }

                    }

                    testArr[curRow, curCol] = checkBlock;
                }
            }

            // 정렬한 블럭을 저장
            blocks = testArr;
        }
    }
}