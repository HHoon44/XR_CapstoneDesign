using System.Collections;
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
            if (GM.GameState == GameState.Checking || GM.GameState == GameState.SKill)
            {
                if (GM.isMatch == true)
                {
                    BlockSort();

                    GM.isMatch = false;

                    StartCoroutine(BlockMatch());
                }
            }
        }

        /// <summary>
        /// 보드에 블럭을 생성하는 메서드
        /// </summary>
        /// <returns></returns>
        public IEnumerator SpawnBlock()
        {
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

            // 3X3 블럭 파괴
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (blocks[row, col] != null)
                    {
                        // Col
                        if (col != 5 && col != 6)
                        {
                            Block checkBlock = blocks[row, col];
                            Block col_0 = blocks[row, col + 1];
                            Block col_1 = blocks[row, col + 2];

                            if (col_0 != null && col_1 != null)
                            {
                                if (checkBlock.elementType == col_0.elementType && checkBlock.elementType == col_1.elementType)
                                {
                                    // 점수 업데이트
                                    DM.SetScore(blocks[row, col].BlockScore);
                                    DM.SetScore(blocks[row, col + 1].BlockScore);
                                    DM.SetScore(blocks[row, col + 2].BlockScore);

                                    // 스킬 게이지 업데이트
                                    uiElement.SetGauge(blocks[row, col].ElementValue);
                                    uiElement.SetGauge(blocks[row, col + 1].ElementValue);
                                    uiElement.SetGauge(blocks[row, col + 2].ElementValue);

                                    blocks[row, col] = null;
                                    blocks[row, col + 1] = null;
                                    blocks[row, col + 2] = null;

                                    // 블럭 제거
                                    blockPool.ReturnPoolableObject(checkBlock);
                                    blockPool.ReturnPoolableObject(col_0);
                                    blockPool.ReturnPoolableObject(col_1);
                                }
                            }
                        }
                    }

                    if (blocks[row, col] != null)
                    {
                        // Row
                        if (row != 5 && row != 6)
                        {
                            Block checkBlock = blocks[row, col];
                            Block row_0 = blocks[row + 1, col];
                            Block row_1 = blocks[row + 2, col];

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

            yield return new WaitForSeconds(.3f);

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

            // 빈 공간 블럭 생성
            int newCol = -3;
            int newRow = 3;

            // GameState -> Move
            GM.SetGameState(GameState.Move);

            // 블럭 생성 및 이동
            for (int col = 0; col < width; col++)
            {
                for (int row = 0; row < height; row++)
                {
                    if (blocks[row, col] == null)
                    {
                        // 어? 여기 비어있는데? 탈모인데?
                        // 그러면 머리 심어줘야자~
                        Block newBlock = blockPool.GetPoolableObject(obj => obj.CanRecycle);

                        // 부모 설정
                        newBlock.transform.SetParent(this.transform);

                        // 위치값 설정
                        newBlock.transform.localPosition = new Vector3(newCol, 4 + row, 0);

                        // 회전값 설정
                        newBlock.transform.localRotation = Quaternion.identity;

                        // 사이즈값 설정
                        newBlock.transform.localScale = new Vector3(.19f, .19f, .19f);

                        // 활성화
                        newBlock.gameObject.SetActive(true);

                        // 블럭 초기화
                        newBlock.Initialize(newCol, 4 + row);

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

            // GameState -> Checking
            GM.SetGameState(GameState.Checking);

            BlockSort();

            if (MatchCheck())
            {
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

            var isMake = false;

            Block block_0 = null;
            Block block_1 = null;
            Block block_2 = null;

            // 이동 블럭이 있는 경우
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
                                switch (blocks[row, col].col)
                                {
                                    case -3:
                                        // XOOO
                                        block_0 = blocks[row, col + 1];
                                        block_1 = blocks[row, col + 2];
                                        block_2 = blocks[row, col + 3];

                                        if (moveBlock.elementType == block_0.elementType &&
                                            moveBlock.elementType == block_1.elementType &&
                                            moveBlock.elementType == block_2.elementType)
                                        {
                                            // 블럭 이동
                                            block_0.col = moveBlock.col;
                                            block_1.col = moveBlock.col;
                                            block_2.col = moveBlock.col;

                                            yield return new WaitForSeconds(.3f);

                                            // 블럭 제거
                                            blockPool.ReturnPoolableObject(block_0);
                                            blockPool.ReturnPoolableObject(block_1);
                                            blockPool.ReturnPoolableObject(block_2);

                                            // 점수 업데이트
                                            DM.SetScore(block_0.BlockScore);
                                            DM.SetScore(block_1.BlockScore);
                                            DM.SetScore(block_2.BlockScore);

                                            // 스킬 게이지 업데이트
                                            uiElement.SetGauge(block_0.ElementValue);
                                            uiElement.SetGauge(block_1.ElementValue);
                                            uiElement.SetGauge(block_2.ElementValue);

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
                                        // XOOO
                                        block_0 = blocks[row, col + 1];
                                        block_1 = blocks[row, col + 2];
                                        block_2 = blocks[row, col + 3];

                                        if (moveBlock.elementType == block_0.elementType &&
                                            moveBlock.elementType == block_1.elementType &&
                                            moveBlock.elementType == block_2.elementType)
                                        {
                                            // 블럭 이동
                                            block_0.col = moveBlock.col;
                                            block_1.col = moveBlock.col;
                                            block_2.col = moveBlock.col;

                                            yield return new WaitForSeconds(.3f);

                                            // 블럭 제거
                                            blockPool.ReturnPoolableObject(block_0);
                                            blockPool.ReturnPoolableObject(block_1);
                                            blockPool.ReturnPoolableObject(block_2);

                                            // 점수 업데이트
                                            DM.SetScore(block_0.BlockScore);
                                            DM.SetScore(block_1.BlockScore);
                                            DM.SetScore(block_2.BlockScore);

                                            // 스킬 게이지 업데이트
                                            uiElement.SetGauge(block_0.ElementValue);
                                            uiElement.SetGauge(block_1.ElementValue);
                                            uiElement.SetGauge(block_2.ElementValue);

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
                                            // OXOO
                                            block_0 = blocks[row, col - 1];
                                            block_1 = blocks[row, col + 1];
                                            block_2 = blocks[row, col + 2];

                                            if (moveBlock.elementType == block_0.elementType &&
                                                moveBlock.elementType == block_1.elementType &&
                                                moveBlock.elementType == block_2.elementType)
                                            {
                                                // 블럭 이동
                                                block_0.col = moveBlock.col;
                                                block_1.col = moveBlock.col;
                                                block_2.col = moveBlock.col;

                                                yield return new WaitForSeconds(.3f);

                                                // 블럭 제거
                                                blockPool.ReturnPoolableObject(block_0);
                                                blockPool.ReturnPoolableObject(block_1);
                                                blockPool.ReturnPoolableObject(block_2);

                                                // 점수 업데이트
                                                DM.SetScore(block_0.BlockScore);
                                                DM.SetScore(block_1.BlockScore);
                                                DM.SetScore(block_2.BlockScore);

                                                // 스킬 게이지 업데이트
                                                uiElement.SetGauge(block_0.ElementValue);
                                                uiElement.SetGauge(block_1.ElementValue);
                                                uiElement.SetGauge(block_2.ElementValue);

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
                                        // XOOO
                                        block_0 = blocks[row, col + 1];
                                        block_1 = blocks[row, col + 2];
                                        block_2 = blocks[row, col + 3];

                                        if (moveBlock.elementType == block_0.elementType &&
                                            moveBlock.elementType == block_1.elementType &&
                                            moveBlock.elementType == block_2.elementType)
                                        {
                                            // 블럭 이동
                                            block_0.col = moveBlock.col;
                                            block_1.col = moveBlock.col;
                                            block_2.col = moveBlock.col;

                                            yield return new WaitForSeconds(.3f);

                                            // 블럭 제거
                                            blockPool.ReturnPoolableObject(block_0);
                                            blockPool.ReturnPoolableObject(block_1);
                                            blockPool.ReturnPoolableObject(block_2);

                                            // 점수 업데이트
                                            DM.SetScore(block_0.BlockScore);
                                            DM.SetScore(block_1.BlockScore);
                                            DM.SetScore(block_2.BlockScore);

                                            // 스킬 게이지 업데이트
                                            uiElement.SetGauge(block_0.ElementValue);
                                            uiElement.SetGauge(block_1.ElementValue);
                                            uiElement.SetGauge(block_2.ElementValue);

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
                                            // OXOO
                                            block_0 = blocks[row, col - 1];
                                            block_1 = blocks[row, col + 1];
                                            block_2 = blocks[row, col + 2];

                                            if (moveBlock.elementType == block_0.elementType &&
                                                moveBlock.elementType == block_1.elementType &&
                                                moveBlock.elementType == block_2.elementType)
                                            {
                                                // 블럭 이동
                                                block_0.col = moveBlock.col;
                                                block_1.col = moveBlock.col;
                                                block_2.col = moveBlock.col;

                                                yield return new WaitForSeconds(.3f);

                                                // 블럭 제거
                                                blockPool.ReturnPoolableObject(block_0);
                                                blockPool.ReturnPoolableObject(block_1);
                                                blockPool.ReturnPoolableObject(block_2);

                                                // 점수 업데이트
                                                DM.SetScore(block_0.BlockScore);
                                                DM.SetScore(block_1.BlockScore);
                                                DM.SetScore(block_2.BlockScore);

                                                // 스킬 게이지 업데이트
                                                uiElement.SetGauge(block_0.ElementValue);
                                                uiElement.SetGauge(block_1.ElementValue);
                                                uiElement.SetGauge(block_2.ElementValue);

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
                                                // OOXO
                                                block_0 = blocks[row, col - 1];
                                                block_1 = blocks[row, col - 2];
                                                block_2 = blocks[row, col + 1];

                                                if (moveBlock.elementType == block_0.elementType &&
                                                    moveBlock.elementType == block_1.elementType &&
                                                    moveBlock.elementType == block_2.elementType)
                                                {
                                                    // 블럭 이동
                                                    block_0.col = moveBlock.col;
                                                    block_1.col = moveBlock.col;
                                                    block_2.col = moveBlock.col;

                                                    yield return new WaitForSeconds(.3f);

                                                    // 블럭 제거
                                                    blockPool.ReturnPoolableObject(block_0);
                                                    blockPool.ReturnPoolableObject(block_1);
                                                    blockPool.ReturnPoolableObject(block_2);

                                                    // 점수 업데이트
                                                    DM.SetScore(block_0.BlockScore);
                                                    DM.SetScore(block_1.BlockScore);
                                                    DM.SetScore(block_2.BlockScore);

                                                    // 스킬 게이지 업데이트
                                                    uiElement.SetGauge(block_0.ElementValue);
                                                    uiElement.SetGauge(block_1.ElementValue);
                                                    uiElement.SetGauge(block_2.ElementValue);

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
                                        // OOOX
                                        block_0 = blocks[row, col - 1];
                                        block_1 = blocks[row, col - 2];
                                        block_2 = blocks[row, col - 3];

                                        if (moveBlock.elementType == block_0.elementType &&
                                            moveBlock.elementType == block_1.elementType &&
                                            moveBlock.elementType == block_2.elementType)
                                        {
                                            // 블럭 이동
                                            block_0.col = moveBlock.col;
                                            block_1.col = moveBlock.col;
                                            block_2.col = moveBlock.col;

                                            yield return new WaitForSeconds(.3f);

                                            // 블럭 제거
                                            blockPool.ReturnPoolableObject(block_0);
                                            blockPool.ReturnPoolableObject(block_1);
                                            blockPool.ReturnPoolableObject(block_2);

                                            // 점수 업데이트
                                            DM.SetScore(block_0.BlockScore);
                                            DM.SetScore(block_1.BlockScore);
                                            DM.SetScore(block_2.BlockScore);

                                            // 스킬 게이지 업데이트
                                            uiElement.SetGauge(block_0.ElementValue);
                                            uiElement.SetGauge(block_1.ElementValue);
                                            uiElement.SetGauge(block_2.ElementValue);

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
                                            // OOXO
                                            block_0 = blocks[row, col - 1];
                                            block_1 = blocks[row, col - 2];
                                            block_2 = blocks[row, col + 1];

                                            if (moveBlock.elementType == block_0.elementType &&
                                                moveBlock.elementType == block_1.elementType &&
                                                moveBlock.elementType == block_2.elementType)
                                            {
                                                // 블럭 이동
                                                block_0.col = moveBlock.col;
                                                block_1.col = moveBlock.col;
                                                block_2.col = moveBlock.col;

                                                yield return new WaitForSeconds(.3f);

                                                // 블럭 제거
                                                blockPool.ReturnPoolableObject(block_0);
                                                blockPool.ReturnPoolableObject(block_1);
                                                blockPool.ReturnPoolableObject(block_2);

                                                // 점수 업데이트
                                                DM.SetScore(block_0.BlockScore);
                                                DM.SetScore(block_1.BlockScore);
                                                DM.SetScore(block_2.BlockScore);

                                                // 스킬 게이지 업데이트
                                                uiElement.SetGauge(block_0.ElementValue);
                                                uiElement.SetGauge(block_1.ElementValue);
                                                uiElement.SetGauge(block_2.ElementValue);

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
                                                // OXOO
                                                block_0 = blocks[row, col - 1];
                                                block_1 = blocks[row, col + 1];
                                                block_2 = blocks[row, col + 2];

                                                if (moveBlock.elementType == block_0.elementType &&
                                                    moveBlock.elementType == block_1.elementType &&
                                                    moveBlock.elementType == block_2.elementType)
                                                {
                                                    // 블럭 이동
                                                    block_0.col = moveBlock.col;
                                                    block_1.col = moveBlock.col;
                                                    block_2.col = moveBlock.col;

                                                    yield return new WaitForSeconds(.3f);

                                                    // 블럭 제거
                                                    blockPool.ReturnPoolableObject(block_0);
                                                    blockPool.ReturnPoolableObject(block_1);
                                                    blockPool.ReturnPoolableObject(block_2);

                                                    // 점수 업데이트
                                                    DM.SetScore(block_0.BlockScore);
                                                    DM.SetScore(block_1.BlockScore);
                                                    DM.SetScore(block_2.BlockScore);

                                                    // 스킬 게이지 업데이트
                                                    uiElement.SetGauge(block_0.ElementValue);
                                                    uiElement.SetGauge(block_1.ElementValue);
                                                    uiElement.SetGauge(block_2.ElementValue);

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
                                        // OOOX
                                        block_0 = blocks[row, col - 1];
                                        block_1 = blocks[row, col - 2];
                                        block_2 = blocks[row, col - 3];

                                        if (moveBlock.elementType == block_0.elementType &&
                                            moveBlock.elementType == block_1.elementType &&
                                            moveBlock.elementType == block_2.elementType)
                                        {
                                            // 블럭 이동
                                            block_0.col = moveBlock.col;
                                            block_1.col = moveBlock.col;
                                            block_2.col = moveBlock.col;

                                            yield return new WaitForSeconds(.3f);

                                            // 블럭 제거
                                            blockPool.ReturnPoolableObject(block_0);
                                            blockPool.ReturnPoolableObject(block_1);
                                            blockPool.ReturnPoolableObject(block_2);

                                            // 점수 업데이트
                                            DM.SetScore(block_0.BlockScore);
                                            DM.SetScore(block_1.BlockScore);
                                            DM.SetScore(block_2.BlockScore);

                                            // 스킬 게이지 업데이트
                                            uiElement.SetGauge(block_0.ElementValue);
                                            uiElement.SetGauge(block_1.ElementValue);
                                            uiElement.SetGauge(block_2.ElementValue);

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
                                            // OOXO
                                            block_0 = blocks[row, col - 1];
                                            block_1 = blocks[row, col - 2];
                                            block_2 = blocks[row, col + 1];

                                            if (moveBlock.elementType == block_0.elementType &&
                                                moveBlock.elementType == block_1.elementType &&
                                                moveBlock.elementType == block_2.elementType)
                                            {
                                                // 블럭 이동
                                                block_0.col = moveBlock.col;
                                                block_1.col = moveBlock.col;
                                                block_2.col = moveBlock.col;

                                                yield return new WaitForSeconds(.3f);

                                                // 블럭 제거
                                                blockPool.ReturnPoolableObject(block_0);
                                                blockPool.ReturnPoolableObject(block_1);
                                                blockPool.ReturnPoolableObject(block_2);

                                                // 점수 업데이트
                                                DM.SetScore(block_0.BlockScore);
                                                DM.SetScore(block_1.BlockScore);
                                                DM.SetScore(block_2.BlockScore);

                                                // 스킬 게이지 업데이트
                                                uiElement.SetGauge(block_0.ElementValue);
                                                uiElement.SetGauge(block_1.ElementValue);
                                                uiElement.SetGauge(block_2.ElementValue);

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
                                        // OOOX
                                        block_0 = blocks[row, col - 1];
                                        block_1 = blocks[row, col - 2];
                                        block_2 = blocks[row, col - 3];

                                        if (moveBlock.elementType == block_0.elementType &&
                                            moveBlock.elementType == block_1.elementType &&
                                            moveBlock.elementType == block_2.elementType)
                                        {
                                            // 블럭 이동
                                            block_0.col = moveBlock.col;
                                            block_1.col = moveBlock.col;
                                            block_2.col = moveBlock.col;

                                            yield return new WaitForSeconds(.3f);

                                            // 블럭 제거
                                            blockPool.ReturnPoolableObject(block_0);
                                            blockPool.ReturnPoolableObject(block_1);
                                            blockPool.ReturnPoolableObject(block_2);

                                            // 점수 업데이트
                                            DM.SetScore(block_0.BlockScore);
                                            DM.SetScore(block_1.BlockScore);
                                            DM.SetScore(block_2.BlockScore);

                                            // 스킬 게이지 업데이트
                                            uiElement.SetGauge(block_0.ElementValue);
                                            uiElement.SetGauge(block_1.ElementValue);
                                            uiElement.SetGauge(block_2.ElementValue);

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
                                switch (blocks[row, col].row)
                                {
                                    case -3:
                                        // XOOO
                                        block_0 = blocks[row + 1, col];
                                        block_1 = blocks[row + 2, col];
                                        block_2 = blocks[row + 3, col];

                                        if (moveBlock.elementType == block_0.elementType &&
                                            moveBlock.elementType == block_1.elementType &&
                                            moveBlock.elementType == block_2.elementType)
                                        {
                                            block_0.row = moveBlock.row;
                                            block_1.row = moveBlock.row;
                                            block_2.row = moveBlock.row;

                                            yield return new WaitForSeconds(.3f);

                                            blockPool.ReturnPoolableObject(block_0);
                                            blockPool.ReturnPoolableObject(block_1);
                                            blockPool.ReturnPoolableObject(block_2);

                                            // 점수를 더합니다!
                                            DM.SetScore(block_0.BlockScore);
                                            DM.SetScore(block_1.BlockScore);
                                            DM.SetScore(block_2.BlockScore);

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
                                        // XOOO
                                        block_0 = blocks[row + 1, col];
                                        block_1 = blocks[row + 2, col];
                                        block_2 = blocks[row + 3, col];

                                        if (moveBlock.elementType == block_0.elementType &&
                                            moveBlock.elementType == block_1.elementType &&
                                            moveBlock.elementType == block_2.elementType)
                                        {
                                            block_0.row = moveBlock.row;
                                            block_1.row = moveBlock.row;
                                            block_2.row = moveBlock.row;

                                            yield return new WaitForSeconds(.3f);

                                            blockPool.ReturnPoolableObject(block_0);
                                            blockPool.ReturnPoolableObject(block_1);
                                            blockPool.ReturnPoolableObject(block_2);

                                            // 점수를 더합니다!
                                            DM.SetScore(block_0.BlockScore);
                                            DM.SetScore(block_1.BlockScore);
                                            DM.SetScore(block_2.BlockScore);

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
                                            // OXOO
                                            block_0 = blocks[row - 1, col];
                                            block_1 = blocks[row + 1, col];
                                            block_2 = blocks[row + 2, col];

                                            if (moveBlock.elementType == block_0.elementType &&
                                                moveBlock.elementType == block_1.elementType &&
                                                moveBlock.elementType == block_2.elementType)
                                            {
                                                block_0.row = moveBlock.row;
                                                block_1.row = moveBlock.row;
                                                block_2.row = moveBlock.row;

                                                yield return new WaitForSeconds(.3f);

                                                blockPool.ReturnPoolableObject(block_0);
                                                blockPool.ReturnPoolableObject(block_1);
                                                blockPool.ReturnPoolableObject(block_2);

                                                // 점수를 더합니다!
                                                DM.SetScore(block_0.BlockScore);
                                                DM.SetScore(block_1.BlockScore);
                                                DM.SetScore(block_2.BlockScore);

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
                                        // XOOO
                                        block_0 = blocks[row + 1, col];
                                        block_1 = blocks[row + 2, col];
                                        block_2 = blocks[row + 3, col];

                                        if (moveBlock.elementType == block_0.elementType &&
                                            moveBlock.elementType == block_1.elementType &&
                                            moveBlock.elementType == block_2.elementType)
                                        {
                                            block_0.row = moveBlock.row;
                                            block_1.row = moveBlock.row;
                                            block_2.row = moveBlock.row;

                                            yield return new WaitForSeconds(.3f);

                                            blockPool.ReturnPoolableObject(block_0);
                                            blockPool.ReturnPoolableObject(block_1);
                                            blockPool.ReturnPoolableObject(block_2);

                                            // 점수를 더합니다!
                                            DM.SetScore(block_0.BlockScore);
                                            DM.SetScore(block_1.BlockScore);
                                            DM.SetScore(block_2.BlockScore);

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
                                            // OXOO
                                            block_0 = blocks[row - 1, col];
                                            block_1 = blocks[row + 1, col];
                                            block_2 = blocks[row + 2, col];

                                            if (moveBlock.elementType == block_0.elementType &&
                                                moveBlock.elementType == block_1.elementType &&
                                                moveBlock.elementType == block_2.elementType)
                                            {
                                                block_0.row = moveBlock.row;
                                                block_1.row = moveBlock.row;
                                                block_2.row = moveBlock.row;

                                                yield return new WaitForSeconds(.3f);

                                                blockPool.ReturnPoolableObject(block_0);
                                                blockPool.ReturnPoolableObject(block_1);
                                                blockPool.ReturnPoolableObject(block_2);

                                                // 점수를 더합니다!
                                                DM.SetScore(block_0.BlockScore);
                                                DM.SetScore(block_1.BlockScore);
                                                DM.SetScore(block_2.BlockScore);

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
                                                // OOXO
                                                block_0 = blocks[row - 1, col];
                                                block_1 = blocks[row - 2, col];
                                                block_2 = blocks[row + 1, col];

                                                if (moveBlock.elementType == block_0.elementType &&
                                                    moveBlock.elementType == block_1.elementType &&
                                                    moveBlock.elementType == block_2.elementType)
                                                {
                                                    block_0.row = moveBlock.row;
                                                    block_1.row = moveBlock.row;
                                                    block_2.row = moveBlock.row;

                                                    yield return new WaitForSeconds(.3f);

                                                    blockPool.ReturnPoolableObject(block_0);
                                                    blockPool.ReturnPoolableObject(block_1);
                                                    blockPool.ReturnPoolableObject(block_2);

                                                    // 점수를 더합니다!
                                                    DM.SetScore(block_0.BlockScore);
                                                    DM.SetScore(block_1.BlockScore);
                                                    DM.SetScore(block_2.BlockScore);

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
                                        // OOOX
                                        block_0 = blocks[row - 1, col];
                                        block_1 = blocks[row - 2, col];
                                        block_2 = blocks[row - 3, col];

                                        if (moveBlock.elementType == block_0.elementType &&
                                            moveBlock.elementType == block_1.elementType &&
                                            moveBlock.elementType == block_2.elementType)
                                        {
                                            block_0.row = moveBlock.row;
                                            block_1.row = moveBlock.row;
                                            block_2.row = moveBlock.row;

                                            yield return new WaitForSeconds(.3f);

                                            blockPool.ReturnPoolableObject(block_0);
                                            blockPool.ReturnPoolableObject(block_1);
                                            blockPool.ReturnPoolableObject(block_2);

                                            // 점수를 더합니다!
                                            DM.SetScore(block_0.BlockScore);
                                            DM.SetScore(block_1.BlockScore);
                                            DM.SetScore(block_2.BlockScore);

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
                                            // OOXO
                                            block_0 = blocks[row + 1, col];
                                            block_1 = blocks[row - 1, col];
                                            block_2 = blocks[row - 2, col];

                                            if (moveBlock.elementType == block_0.elementType &&
                                                moveBlock.elementType == block_1.elementType &&
                                                moveBlock.elementType == block_2.elementType)
                                            {
                                                block_0.row = moveBlock.row;
                                                block_1.row = moveBlock.row;
                                                block_2.row = moveBlock.row;

                                                yield return new WaitForSeconds(.3f);

                                                blockPool.ReturnPoolableObject(block_0);
                                                blockPool.ReturnPoolableObject(block_1);
                                                blockPool.ReturnPoolableObject(block_2);

                                                // 점수를 더합니다!
                                                DM.SetScore(block_0.BlockScore);
                                                DM.SetScore(block_1.BlockScore);
                                                DM.SetScore(block_2.BlockScore);

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
                                                // OXOO
                                                block_0 = blocks[row + 1, col];
                                                block_1 = blocks[row + 2, col];
                                                block_2 = blocks[row - 1, col];

                                                if (moveBlock.elementType == block_0.elementType &&
                                                    moveBlock.elementType == block_1.elementType &&
                                                    moveBlock.elementType == block_2.elementType)
                                                {
                                                    block_0.row = moveBlock.row;
                                                    block_1.row = moveBlock.row;
                                                    block_2.row = moveBlock.row;

                                                    yield return new WaitForSeconds(.3f);

                                                    blockPool.ReturnPoolableObject(block_0);
                                                    blockPool.ReturnPoolableObject(block_1);
                                                    blockPool.ReturnPoolableObject(block_2);

                                                    // 점수를 더합니다!
                                                    DM.SetScore(block_0.BlockScore);
                                                    DM.SetScore(block_1.BlockScore);
                                                    DM.SetScore(block_2.BlockScore);

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
                                        // OOOX
                                        block_0 = blocks[row - 1, col];
                                        block_1 = blocks[row - 2, col];
                                        block_2 = blocks[row - 3, col];

                                        if (moveBlock.elementType == block_0.elementType &&
                                            moveBlock.elementType == block_1.elementType &&
                                            moveBlock.elementType == block_2.elementType)
                                        {
                                            block_0.row = moveBlock.row;
                                            block_1.row = moveBlock.row;
                                            block_2.row = moveBlock.row;

                                            yield return new WaitForSeconds(.3f);

                                            blockPool.ReturnPoolableObject(block_0);
                                            blockPool.ReturnPoolableObject(block_1);
                                            blockPool.ReturnPoolableObject(block_2);

                                            // 점수를 더합니다!
                                            DM.SetScore(block_0.BlockScore);
                                            DM.SetScore(block_1.BlockScore);
                                            DM.SetScore(block_2.BlockScore);

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
                                            // OOXO
                                            block_0 = blocks[row + 1, col];
                                            block_1 = blocks[row - 1, col];
                                            block_2 = blocks[row - 2, col];

                                            if (moveBlock.elementType == block_0.elementType &&
                                                moveBlock.elementType == block_1.elementType &&
                                                moveBlock.elementType == block_2.elementType)
                                            {
                                                block_0.row = moveBlock.row;
                                                block_1.row = moveBlock.row;
                                                block_2.row = moveBlock.row;

                                                yield return new WaitForSeconds(.3f);

                                                blockPool.ReturnPoolableObject(block_0);
                                                blockPool.ReturnPoolableObject(block_1);
                                                blockPool.ReturnPoolableObject(block_2);

                                                // 점수를 더합니다!
                                                DM.SetScore(block_0.BlockScore);
                                                DM.SetScore(block_1.BlockScore);
                                                DM.SetScore(block_2.BlockScore);

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
                                        // OOOX
                                        block_0 = blocks[row - 1, col];
                                        block_1 = blocks[row - 2, col];
                                        block_2 = blocks[row - 3, col];

                                        if (moveBlock.elementType == block_0.elementType &&
                                            moveBlock.elementType == block_1.elementType &&
                                            moveBlock.elementType == block_2.elementType)
                                        {
                                            block_0.row = moveBlock.row;
                                            block_1.row = moveBlock.row;
                                            block_2.row = moveBlock.row;

                                            yield return new WaitForSeconds(.3f);

                                            blockPool.ReturnPoolableObject(block_0);
                                            blockPool.ReturnPoolableObject(block_1);
                                            blockPool.ReturnPoolableObject(block_2);

                                            // 점수를 더합니다!
                                            DM.SetScore(block_0.BlockScore);
                                            DM.SetScore(block_1.BlockScore);
                                            DM.SetScore(block_2.BlockScore);

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

                            // OtherBlock
                            if (blocks[row, col] == moveBlock.otherBlock)
                            {
                                // Col
                                switch (blocks[row, col].col)
                                {
                                    case -3:
                                        // XOOO
                                        block_0 = blocks[row, col + 1];
                                        block_1 = blocks[row, col + 2];
                                        block_2 = blocks[row, col + 3];

                                        if (moveBlock.otherBlock.elementType == block_0.elementType &&
                                            moveBlock.otherBlock.elementType == block_1.elementType &&
                                            moveBlock.otherBlock.elementType == block_2.elementType)
                                        {
                                            block_0.col = moveBlock.otherBlock.col;
                                            block_1.col = moveBlock.otherBlock.col;
                                            block_2.col = moveBlock.otherBlock.col;

                                            yield return new WaitForSeconds(.3f);

                                            blockPool.ReturnPoolableObject(block_0);
                                            blockPool.ReturnPoolableObject(block_1);
                                            blockPool.ReturnPoolableObject(block_2);

                                            // 점수를 더합니다!
                                            DM.SetScore(block_0.BlockScore);
                                            DM.SetScore(block_1.BlockScore);
                                            DM.SetScore(block_2.BlockScore);

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
                                        // XOOO
                                        block_0 = blocks[row, col + 1];
                                        block_1 = blocks[row, col + 2];
                                        block_2 = blocks[row, col + 3];

                                        if (moveBlock.otherBlock.elementType == block_0.elementType &&
                                            moveBlock.otherBlock.elementType == block_1.elementType &&
                                            moveBlock.otherBlock.elementType == block_2.elementType)
                                        {
                                            block_0.col = moveBlock.otherBlock.col;
                                            block_1.col = moveBlock.otherBlock.col;
                                            block_2.col = moveBlock.otherBlock.col;

                                            yield return new WaitForSeconds(.3f);

                                            blockPool.ReturnPoolableObject(block_0);
                                            blockPool.ReturnPoolableObject(block_1);
                                            blockPool.ReturnPoolableObject(block_2);

                                            // 점수를 더합니다!
                                            DM.SetScore(block_0.BlockScore);
                                            DM.SetScore(block_1.BlockScore);
                                            DM.SetScore(block_2.BlockScore);

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
                                            // OXOO
                                            block_0 = blocks[row, col - 1];
                                            block_1 = blocks[row, col + 1];
                                            block_2 = blocks[row, col + 2];

                                            if (moveBlock.otherBlock.elementType == block_0.elementType &&
                                                moveBlock.otherBlock.elementType == block_1.elementType &&
                                                moveBlock.otherBlock.elementType == block_2.elementType)
                                            {
                                                block_0.col = moveBlock.otherBlock.col;
                                                block_1.col = moveBlock.otherBlock.col;
                                                block_2.col = moveBlock.otherBlock.col;

                                                yield return new WaitForSeconds(.3f);

                                                blockPool.ReturnPoolableObject(block_0);
                                                blockPool.ReturnPoolableObject(block_1);
                                                blockPool.ReturnPoolableObject(block_2);

                                                // 점수를 더합니다!
                                                DM.SetScore(block_0.BlockScore);
                                                DM.SetScore(block_1.BlockScore);
                                                DM.SetScore(block_2.BlockScore);

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
                                        // XOOO
                                        block_0 = blocks[row, col + 1];
                                        block_1 = blocks[row, col + 2];
                                        block_2 = blocks[row, col + 3];

                                        if (moveBlock.otherBlock.elementType == block_0.elementType &&
                                            moveBlock.otherBlock.elementType == block_1.elementType &&
                                            moveBlock.otherBlock.elementType == block_2.elementType)
                                        {
                                            block_0.col = moveBlock.otherBlock.col;
                                            block_1.col = moveBlock.otherBlock.col;
                                            block_2.col = moveBlock.otherBlock.col;

                                            yield return new WaitForSeconds(.3f);

                                            blockPool.ReturnPoolableObject(block_0);
                                            blockPool.ReturnPoolableObject(block_1);
                                            blockPool.ReturnPoolableObject(block_2);

                                            // 점수를 더합니다!
                                            DM.SetScore(block_0.BlockScore);
                                            DM.SetScore(block_1.BlockScore);
                                            DM.SetScore(block_2.BlockScore);

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
                                            // OXOO
                                            block_0 = blocks[row, col - 1];
                                            block_1 = blocks[row, col + 1];
                                            block_2 = blocks[row, col + 2];

                                            if (moveBlock.otherBlock.elementType == block_0.elementType &&
                                                moveBlock.otherBlock.elementType == block_1.elementType &&
                                                moveBlock.otherBlock.elementType == block_2.elementType)
                                            {
                                                block_0.col = moveBlock.otherBlock.col;
                                                block_1.col = moveBlock.otherBlock.col;
                                                block_2.col = moveBlock.otherBlock.col;

                                                yield return new WaitForSeconds(.3f);

                                                blockPool.ReturnPoolableObject(block_0);
                                                blockPool.ReturnPoolableObject(block_1);
                                                blockPool.ReturnPoolableObject(block_2);

                                                // 점수를 더합니다!
                                                DM.SetScore(block_0.BlockScore);
                                                DM.SetScore(block_1.BlockScore);
                                                DM.SetScore(block_2.BlockScore);

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
                                                // OOXO
                                                block_0 = blocks[row, col - 1];
                                                block_1 = blocks[row, col - 2];
                                                block_2 = blocks[row, col + 1];

                                                if (moveBlock.otherBlock.elementType == block_0.elementType &&
                                                    moveBlock.otherBlock.elementType == block_1.elementType &&
                                                    moveBlock.otherBlock.elementType == block_2.elementType)
                                                {
                                                    block_0.col = moveBlock.otherBlock.col;
                                                    block_1.col = moveBlock.otherBlock.col;
                                                    block_2.col = moveBlock.otherBlock.col;

                                                    yield return new WaitForSeconds(.3f);

                                                    blockPool.ReturnPoolableObject(block_0);
                                                    blockPool.ReturnPoolableObject(block_1);
                                                    blockPool.ReturnPoolableObject(block_2);

                                                    // 점수를 더합니다!
                                                    DM.SetScore(block_0.BlockScore);
                                                    DM.SetScore(block_1.BlockScore);
                                                    DM.SetScore(block_2.BlockScore);

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

                                    case 0:
                                        // XOOO
                                        block_0 = blocks[row, col + 1];
                                        block_1 = blocks[row, col + 2];
                                        block_2 = blocks[row, col + 3];

                                        if (moveBlock.otherBlock.elementType == block_0.elementType &&
                                            moveBlock.otherBlock.elementType == block_1.elementType &&
                                            moveBlock.otherBlock.elementType == block_2.elementType)
                                        {
                                            block_0.col = moveBlock.otherBlock.col;
                                            block_1.col = moveBlock.otherBlock.col;
                                            block_2.col = moveBlock.otherBlock.col;

                                            yield return new WaitForSeconds(.3f);

                                            blockPool.ReturnPoolableObject(block_0);
                                            blockPool.ReturnPoolableObject(block_1);
                                            blockPool.ReturnPoolableObject(block_2);

                                            // 점수를 더합니다!
                                            DM.SetScore(block_0.BlockScore);
                                            DM.SetScore(block_1.BlockScore);
                                            DM.SetScore(block_2.BlockScore);

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
                                            // OXOO
                                            block_0 = blocks[row, col - 1];
                                            block_1 = blocks[row, col + 1];
                                            block_2 = blocks[row, col + 2];

                                            if (moveBlock.otherBlock.elementType == block_0.elementType &&
                                                moveBlock.otherBlock.elementType == block_1.elementType &&
                                                moveBlock.otherBlock.elementType == block_2.elementType)
                                            {
                                                block_0.col = moveBlock.otherBlock.col;
                                                block_1.col = moveBlock.otherBlock.col;
                                                block_2.col = moveBlock.otherBlock.col;

                                                yield return new WaitForSeconds(.3f);

                                                blockPool.ReturnPoolableObject(block_0);
                                                blockPool.ReturnPoolableObject(block_1);
                                                blockPool.ReturnPoolableObject(block_2);

                                                // 점수를 더합니다!
                                                DM.SetScore(block_0.BlockScore);
                                                DM.SetScore(block_1.BlockScore);
                                                DM.SetScore(block_2.BlockScore);

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
                                                // OOXO
                                                block_0 = blocks[row, col - 1];
                                                block_1 = blocks[row, col - 2];
                                                block_2 = blocks[row, col + 1];

                                                if (moveBlock.otherBlock.elementType == block_0.elementType &&
                                                    moveBlock.otherBlock.elementType == block_1.elementType &&
                                                    moveBlock.otherBlock.elementType == block_2.elementType)
                                                {
                                                    block_0.col = moveBlock.otherBlock.col;
                                                    block_1.col = moveBlock.otherBlock.col;
                                                    block_2.col = moveBlock.otherBlock.col;

                                                    yield return new WaitForSeconds(.3f);

                                                    blockPool.ReturnPoolableObject(block_0);
                                                    blockPool.ReturnPoolableObject(block_1);
                                                    blockPool.ReturnPoolableObject(block_2);

                                                    // 점수를 더합니다!
                                                    DM.SetScore(block_0.BlockScore);
                                                    DM.SetScore(block_1.BlockScore);
                                                    DM.SetScore(block_2.BlockScore);

                                                    blocks[row, col - 1] = null;
                                                    blocks[row, col - 2] = null;
                                                    blocks[row, col + 1] = null;

                                                    // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                                    moveBlock.otherBlock.elementType = ElementType.Balance;
                                                    moveBlock.otherBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                    isMake = true;
                                                }
                                                else
                                                {
                                                    // OOOX
                                                    block_0 = blocks[row, col - 1];
                                                    block_1 = blocks[row, col - 2];
                                                    block_2 = blocks[row, col - 3];

                                                    if (moveBlock.otherBlock.elementType == block_0.elementType &&
                                                        moveBlock.otherBlock.elementType == block_1.elementType &&
                                                        moveBlock.otherBlock.elementType == block_2.elementType)
                                                    {
                                                        block_0.col = moveBlock.otherBlock.col;
                                                        block_1.col = moveBlock.otherBlock.col;
                                                        block_2.col = moveBlock.otherBlock.col;

                                                        yield return new WaitForSeconds(.3f);

                                                        blockPool.ReturnPoolableObject(block_0);
                                                        blockPool.ReturnPoolableObject(block_1);
                                                        blockPool.ReturnPoolableObject(block_2);

                                                        // 점수를 더합니다!
                                                        DM.SetScore(block_0.BlockScore);
                                                        DM.SetScore(block_1.BlockScore);
                                                        DM.SetScore(block_2.BlockScore);

                                                        blocks[row, col - 1] = null;
                                                        blocks[row, col - 2] = null;
                                                        blocks[row, col - 3] = null;

                                                        // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                                        moveBlock.otherBlock.elementType = ElementType.Balance;
                                                        moveBlock.otherBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                        isMake = true;
                                                    }
                                                }
                                            }
                                        }
                                        break;

                                    case 1:
                                        // OOOX
                                        block_0 = blocks[row, col - 1];
                                        block_1 = blocks[row, col - 2];
                                        block_2 = blocks[row, col - 3];

                                        if (moveBlock.otherBlock.elementType == block_0.elementType &&
                                            moveBlock.otherBlock.elementType == block_1.elementType &&
                                            moveBlock.otherBlock.elementType == block_2.elementType)
                                        {
                                            block_0.col = moveBlock.otherBlock.col;
                                            block_1.col = moveBlock.otherBlock.col;
                                            block_2.col = moveBlock.otherBlock.col;

                                            yield return new WaitForSeconds(.3f);

                                            blockPool.ReturnPoolableObject(block_0);
                                            blockPool.ReturnPoolableObject(block_1);
                                            blockPool.ReturnPoolableObject(block_2);

                                            // 점수를 더합니다!
                                            DM.SetScore(block_0.BlockScore);
                                            DM.SetScore(block_1.BlockScore);
                                            DM.SetScore(block_2.BlockScore);

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
                                            // OOXO
                                            block_0 = blocks[row, col - 1];
                                            block_1 = blocks[row, col - 2];
                                            block_2 = blocks[row, col + 1];

                                            if (moveBlock.otherBlock.elementType == block_0.elementType &&
                                                moveBlock.otherBlock.elementType == block_1.elementType &&
                                                moveBlock.otherBlock.elementType == block_2.elementType)
                                            {
                                                block_0.col = moveBlock.otherBlock.col;
                                                block_1.col = moveBlock.otherBlock.col;
                                                block_2.col = moveBlock.otherBlock.col;

                                                yield return new WaitForSeconds(.3f);

                                                blockPool.ReturnPoolableObject(block_0);
                                                blockPool.ReturnPoolableObject(block_1);
                                                blockPool.ReturnPoolableObject(block_2);

                                                // 점수를 더합니다!
                                                DM.SetScore(block_0.BlockScore);
                                                DM.SetScore(block_1.BlockScore);
                                                DM.SetScore(block_2.BlockScore);

                                                // 기준이 되는 블럭을 폭탄으로 바꿔줘야지!!
                                                moveBlock.otherBlock.elementType = ElementType.Balance;
                                                moveBlock.otherBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                isMake = true;
                                            }
                                            else
                                            {
                                                // OXOO
                                                block_0 = blocks[row, col - 1];
                                                block_1 = blocks[row, col + 1];
                                                block_2 = blocks[row, col + 2];

                                                if (moveBlock.otherBlock.elementType == block_0.elementType &&
                                                    moveBlock.otherBlock.elementType == block_1.elementType &&
                                                    moveBlock.otherBlock.elementType == block_2.elementType)
                                                {
                                                    block_0.col = moveBlock.otherBlock.col;
                                                    block_1.col = moveBlock.otherBlock.col;
                                                    block_2.col = moveBlock.otherBlock.col;

                                                    yield return new WaitForSeconds(.3f);

                                                    blockPool.ReturnPoolableObject(block_0);
                                                    blockPool.ReturnPoolableObject(block_1);
                                                    blockPool.ReturnPoolableObject(block_2);

                                                    // 점수를 더합니다!
                                                    DM.SetScore(block_0.BlockScore);
                                                    DM.SetScore(block_1.BlockScore);
                                                    DM.SetScore(block_2.BlockScore);

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
                                        // OOOX
                                        block_0 = blocks[row, col - 1];
                                        block_1 = blocks[row, col - 2];
                                        block_2 = blocks[row, col - 3];

                                        if (moveBlock.otherBlock.elementType == block_0.elementType &&
                                            moveBlock.otherBlock.elementType == block_1.elementType &&
                                            moveBlock.otherBlock.elementType == block_2.elementType)
                                        {
                                            block_0.col = moveBlock.otherBlock.col;
                                            block_1.col = moveBlock.otherBlock.col;
                                            block_2.col = moveBlock.otherBlock.col;

                                            yield return new WaitForSeconds(.3f);

                                            blockPool.ReturnPoolableObject(block_0);
                                            blockPool.ReturnPoolableObject(block_1);
                                            blockPool.ReturnPoolableObject(block_2);

                                            // 점수를 더합니다!
                                            DM.SetScore(block_0.BlockScore);
                                            DM.SetScore(block_1.BlockScore);
                                            DM.SetScore(block_2.BlockScore);

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
                                            // OOXO
                                            block_0 = blocks[row, col - 1];
                                            block_1 = blocks[row, col - 2];
                                            block_2 = blocks[row, col + 1];

                                            if (moveBlock.otherBlock.elementType == block_0.elementType &&
                                                moveBlock.otherBlock.elementType == block_1.elementType &&
                                                moveBlock.otherBlock.elementType == block_2.elementType)
                                            {
                                                block_0.col = moveBlock.otherBlock.col;
                                                block_1.col = moveBlock.otherBlock.col;
                                                block_2.col = moveBlock.otherBlock.col;

                                                yield return new WaitForSeconds(.3f);

                                                blockPool.ReturnPoolableObject(block_0);
                                                blockPool.ReturnPoolableObject(block_1);
                                                blockPool.ReturnPoolableObject(block_2);

                                                // 점수를 더합니다!
                                                DM.SetScore(block_0.BlockScore);
                                                DM.SetScore(block_1.BlockScore);
                                                DM.SetScore(block_2.BlockScore);

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
                                        // OOOX
                                        block_0 = blocks[row, col - 1];
                                        block_1 = blocks[row, col - 2];
                                        block_2 = blocks[row, col - 3];

                                        if (moveBlock.otherBlock.elementType == block_0.elementType &&
                                            moveBlock.otherBlock.elementType == block_1.elementType &&
                                            moveBlock.otherBlock.elementType == block_2.elementType)
                                        {
                                            block_0.col = moveBlock.otherBlock.col;
                                            block_1.col = moveBlock.otherBlock.col;
                                            block_2.col = moveBlock.otherBlock.col;

                                            yield return new WaitForSeconds(.3f);

                                            blockPool.ReturnPoolableObject(block_0);
                                            blockPool.ReturnPoolableObject(block_1);
                                            blockPool.ReturnPoolableObject(block_2);

                                            // 점수를 더합니다!
                                            DM.SetScore(block_0.BlockScore);
                                            DM.SetScore(block_1.BlockScore);
                                            DM.SetScore(block_2.BlockScore);

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
                                switch (blocks[row, col].row)
                                {
                                    case -3:
                                        // XOOO
                                        block_0 = blocks[row + 1, col];
                                        block_1 = blocks[row + 2, col];
                                        block_2 = blocks[row + 3, col];

                                        if (moveBlock.otherBlock.elementType == block_0.elementType &&
                                            moveBlock.otherBlock.elementType == block_1.elementType &&
                                            moveBlock.otherBlock.elementType == block_2.elementType)
                                        {
                                            block_0.row = moveBlock.otherBlock.row;
                                            block_1.row = moveBlock.otherBlock.row;
                                            block_2.row = moveBlock.otherBlock.row;

                                            yield return new WaitForSeconds(.3f);

                                            blockPool.ReturnPoolableObject(block_0);
                                            blockPool.ReturnPoolableObject(block_1);
                                            blockPool.ReturnPoolableObject(block_2);

                                            // 점수를 더합니다!
                                            DM.SetScore(block_0.BlockScore);
                                            DM.SetScore(block_1.BlockScore);
                                            DM.SetScore(block_2.BlockScore);

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
                                        // XOOO
                                        block_0 = blocks[row + 1, col];
                                        block_1 = blocks[row + 2, col];
                                        block_2 = blocks[row + 3, col];

                                        if (moveBlock.otherBlock.elementType == block_0.elementType &&
                                            moveBlock.otherBlock.elementType == block_1.elementType &&
                                            moveBlock.otherBlock.elementType == block_2.elementType)
                                        {
                                            block_0.row = moveBlock.otherBlock.row;
                                            block_1.row = moveBlock.otherBlock.row;
                                            block_2.row = moveBlock.otherBlock.row;

                                            yield return new WaitForSeconds(.3f);

                                            blockPool.ReturnPoolableObject(block_0);
                                            blockPool.ReturnPoolableObject(block_1);
                                            blockPool.ReturnPoolableObject(block_2);

                                            // 점수를 더합니다!
                                            DM.SetScore(block_0.BlockScore);
                                            DM.SetScore(block_1.BlockScore);
                                            DM.SetScore(block_2.BlockScore);

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
                                            // OXOO
                                            block_0 = blocks[row - 1, col];
                                            block_1 = blocks[row + 1, col];
                                            block_2 = blocks[row + 2, col];

                                            if (moveBlock.otherBlock.elementType == block_0.elementType &&
                                                moveBlock.otherBlock.elementType == block_1.elementType &&
                                                moveBlock.otherBlock.elementType == block_2.elementType)
                                            {
                                                block_0.row = moveBlock.otherBlock.row;
                                                block_1.row = moveBlock.otherBlock.row;
                                                block_2.row = moveBlock.otherBlock.row;

                                                yield return new WaitForSeconds(.3f);

                                                blockPool.ReturnPoolableObject(block_0);
                                                blockPool.ReturnPoolableObject(block_1);
                                                blockPool.ReturnPoolableObject(block_2);

                                                // 점수를 더합니다!
                                                DM.SetScore(block_0.BlockScore);
                                                DM.SetScore(block_1.BlockScore);
                                                DM.SetScore(block_2.BlockScore);

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
                                        // XOOO
                                        block_0 = blocks[row + 1, col];
                                        block_1 = blocks[row + 2, col];
                                        block_2 = blocks[row + 3, col];

                                        if (moveBlock.otherBlock.elementType == block_0.elementType &&
                                            moveBlock.otherBlock.elementType == block_1.elementType &&
                                            moveBlock.otherBlock.elementType == block_2.elementType)
                                        {
                                            block_0.row = moveBlock.otherBlock.row;
                                            block_1.row = moveBlock.otherBlock.row;
                                            block_2.row = moveBlock.otherBlock.row;

                                            yield return new WaitForSeconds(.3f);

                                            blockPool.ReturnPoolableObject(block_0);
                                            blockPool.ReturnPoolableObject(block_1);
                                            blockPool.ReturnPoolableObject(block_2);

                                            // 점수를 더합니다!
                                            DM.SetScore(block_0.BlockScore);
                                            DM.SetScore(block_1.BlockScore);
                                            DM.SetScore(block_2.BlockScore);

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
                                            // OXOO
                                            block_0 = blocks[row - 1, col];
                                            block_1 = blocks[row + 1, col];
                                            block_2 = blocks[row + 2, col];

                                            if (moveBlock.otherBlock.elementType == block_0.elementType &&
                                                moveBlock.otherBlock.elementType == block_1.elementType &&
                                                moveBlock.otherBlock.elementType == block_2.elementType)
                                            {
                                                block_0.row = moveBlock.row;
                                                block_1.row = moveBlock.row;
                                                block_2.row = moveBlock.row;

                                                yield return new WaitForSeconds(.3f);

                                                blockPool.ReturnPoolableObject(block_0);
                                                blockPool.ReturnPoolableObject(block_1);
                                                blockPool.ReturnPoolableObject(block_2);

                                                // 점수를 더합니다!
                                                DM.SetScore(block_0.BlockScore);
                                                DM.SetScore(block_1.BlockScore);
                                                DM.SetScore(block_2.BlockScore);

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
                                                // OOXO
                                                block_0 = blocks[row - 1, col];
                                                block_1 = blocks[row - 2, col];
                                                block_2 = blocks[row + 1, col];

                                                if (moveBlock.otherBlock.elementType == block_0.elementType &&
                                                    moveBlock.otherBlock.elementType == block_1.elementType &&
                                                    moveBlock.otherBlock.elementType == block_2.elementType)
                                                {
                                                    block_0.row = moveBlock.otherBlock.row;
                                                    block_1.row = moveBlock.otherBlock.row;
                                                    block_2.row = moveBlock.otherBlock.row;

                                                    yield return new WaitForSeconds(.3f);

                                                    blockPool.ReturnPoolableObject(block_0);
                                                    blockPool.ReturnPoolableObject(block_1);
                                                    blockPool.ReturnPoolableObject(block_2);

                                                    // 점수를 더합니다!
                                                    DM.SetScore(block_0.BlockScore);
                                                    DM.SetScore(block_1.BlockScore);
                                                    DM.SetScore(block_2.BlockScore);

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
                                        // OOOX
                                        block_0 = blocks[row - 1, col];
                                        block_1 = blocks[row - 2, col];
                                        block_2 = blocks[row - 3, col];

                                        if (moveBlock.otherBlock.elementType == block_0.elementType &&
                                            moveBlock.otherBlock.elementType == block_1.elementType &&
                                            moveBlock.otherBlock.elementType == block_2.elementType)
                                        {
                                            block_0.row = moveBlock.otherBlock.row;
                                            block_1.row = moveBlock.otherBlock.row;
                                            block_2.row = moveBlock.otherBlock.row;

                                            yield return new WaitForSeconds(.3f);

                                            blockPool.ReturnPoolableObject(block_0);
                                            blockPool.ReturnPoolableObject(block_1);
                                            blockPool.ReturnPoolableObject(block_2);

                                            // 점수를 더합니다!
                                            DM.SetScore(block_0.BlockScore);
                                            DM.SetScore(block_1.BlockScore);
                                            DM.SetScore(block_2.BlockScore);

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
                                            // OOXO
                                            block_0 = blocks[row + 1, col];
                                            block_1 = blocks[row - 1, col];
                                            block_2 = blocks[row - 2, col];

                                            if (moveBlock.otherBlock.elementType == block_0.elementType &&
                                                moveBlock.otherBlock.elementType == block_1.elementType &&
                                                moveBlock.otherBlock.elementType == block_2.elementType)
                                            {
                                                block_0.row = moveBlock.otherBlock.row;
                                                block_1.row = moveBlock.otherBlock.row;
                                                block_2.row = moveBlock.otherBlock.row;

                                                yield return new WaitForSeconds(.3f);

                                                blockPool.ReturnPoolableObject(block_0);
                                                blockPool.ReturnPoolableObject(block_1);
                                                blockPool.ReturnPoolableObject(block_2);

                                                // 점수를 더합니다!
                                                DM.SetScore(block_0.BlockScore);
                                                DM.SetScore(block_1.BlockScore);
                                                DM.SetScore(block_2.BlockScore);

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
                                                // OXOO
                                                block_0 = blocks[row + 1, col];
                                                block_1 = blocks[row + 2, col];
                                                block_2 = blocks[row - 1, col];

                                                if (moveBlock.otherBlock.elementType == block_0.elementType &&
                                                    moveBlock.otherBlock.elementType == block_1.elementType &&
                                                    moveBlock.otherBlock.elementType == block_2.elementType)
                                                {
                                                    block_0.row = moveBlock.otherBlock.row;
                                                    block_1.row = moveBlock.otherBlock.row;
                                                    block_2.row = moveBlock.otherBlock.row;

                                                    yield return new WaitForSeconds(.3f);

                                                    blockPool.ReturnPoolableObject(block_0);
                                                    blockPool.ReturnPoolableObject(block_1);
                                                    blockPool.ReturnPoolableObject(block_2);

                                                    // 점수를 더합니다!
                                                    DM.SetScore(block_0.BlockScore);
                                                    DM.SetScore(block_1.BlockScore);
                                                    DM.SetScore(block_2.BlockScore);

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
                                        // OOOX
                                        block_0 = blocks[row - 1, col];
                                        block_1 = blocks[row - 2, col];
                                        block_2 = blocks[row - 3, col];

                                        if (moveBlock.otherBlock.elementType == block_0.elementType &&
                                            moveBlock.otherBlock.elementType == block_1.elementType &&
                                            moveBlock.otherBlock.elementType == block_2.elementType)
                                        {
                                            block_0.row = moveBlock.otherBlock.row;
                                            block_1.row = moveBlock.otherBlock.row;
                                            block_2.row = moveBlock.otherBlock.row;

                                            yield return new WaitForSeconds(.3f);

                                            blockPool.ReturnPoolableObject(block_0);
                                            blockPool.ReturnPoolableObject(block_1);
                                            blockPool.ReturnPoolableObject(block_2);

                                            // 점수를 더합니다!
                                            DM.SetScore(block_0.BlockScore);
                                            DM.SetScore(block_1.BlockScore);
                                            DM.SetScore(block_2.BlockScore);

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
                                            // OOXO
                                            block_0 = blocks[row + 1, col];
                                            block_1 = blocks[row - 1, col];
                                            block_2 = blocks[row - 2, col];

                                            if (moveBlock.otherBlock.elementType == block_0.elementType &&
                                                moveBlock.otherBlock.elementType == block_1.elementType &&
                                                moveBlock.otherBlock.elementType == block_2.elementType)
                                            {
                                                block_0.row = moveBlock.otherBlock.row;
                                                block_1.row = moveBlock.otherBlock.row;
                                                block_2.row = moveBlock.otherBlock.row;

                                                yield return new WaitForSeconds(.3f);

                                                blockPool.ReturnPoolableObject(block_0);
                                                blockPool.ReturnPoolableObject(block_1);
                                                blockPool.ReturnPoolableObject(block_2);

                                                // 점수를 더합니다!
                                                DM.SetScore(block_0.BlockScore);
                                                DM.SetScore(block_1.BlockScore);
                                                DM.SetScore(block_2.BlockScore);

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
                                        // OOOX
                                        block_0 = blocks[row - 1, col];
                                        block_1 = blocks[row - 2, col];
                                        block_2 = blocks[row - 3, col];

                                        if (moveBlock.otherBlock.elementType == block_0.elementType &&
                                            moveBlock.otherBlock.elementType == block_1.elementType &&
                                            moveBlock.otherBlock.elementType == block_2.elementType)
                                        {
                                            block_0.row = moveBlock.otherBlock.row;
                                            block_1.row = moveBlock.otherBlock.row;
                                            block_2.row = moveBlock.otherBlock.row;

                                            yield return new WaitForSeconds(.3f);

                                            blockPool.ReturnPoolableObject(block_0);
                                            blockPool.ReturnPoolableObject(block_1);
                                            blockPool.ReturnPoolableObject(block_2);

                                            // 점수를 더합니다!
                                            DM.SetScore(block_0.BlockScore);
                                            DM.SetScore(block_1.BlockScore);
                                            DM.SetScore(block_2.BlockScore);

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

                                // GameState -> Move
                                GM.SetGameState(GameState.Move);

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

                                // GameState -> Checking
                                GM.SetGameState(GameState.Checking);

                                isMake = false;

                                BlockSort();
                            }
                        }
                    }
                }

                // 비워주기
                moveBlock = null;
            }
            // 이동 블럭이 없는 경우
            else
            {
                for (int row = 0; row < height; row++)
                {
                    for (int col = 0; col < width; col++)
                    {
                        if (blocks[row, col] != null && GM.GameState != GameState.Move)
                        {
                            Block checkBlock = blocks[row, col];

                            // Col이 음수 또는 0
                            if (checkBlock.col <= 0)
                            {
                                block_0 = blocks[row, col + 1];
                                block_1 = blocks[row, col + 2];
                                block_2 = blocks[row, col + 3];

                                if (checkBlock.elementType == block_0.elementType &&
                                    checkBlock.elementType == block_1.elementType &&
                                    checkBlock.elementType == block_2.elementType)
                                {
                                    block_0.col = checkBlock.col;
                                    block_1.col = checkBlock.col;
                                    block_2.col = checkBlock.col;

                                    yield return new WaitForSeconds(.3f);

                                    // 풀에 리턴
                                    blockPool.ReturnPoolableObject(block_0);
                                    blockPool.ReturnPoolableObject(block_1);
                                    blockPool.ReturnPoolableObject(block_2);

                                    // 점수를 더합니다!
                                    DM.SetScore(block_0.BlockScore);
                                    DM.SetScore(block_1.BlockScore);
                                    DM.SetScore(block_2.BlockScore);

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
                            // Col이 양수
                            else if (checkBlock.col > 0)
                            {
                                block_0 = blocks[row, col - 1];
                                block_1 = blocks[row, col - 2];
                                block_2 = blocks[row, col - 3];

                                if (checkBlock.elementType == block_0.elementType &&
                                    checkBlock.elementType == block_1.elementType &&
                                    checkBlock.elementType == block_2.elementType)
                                {
                                    block_0.col = checkBlock.col;
                                    block_1.col = checkBlock.col;
                                    block_2.col = checkBlock.col;

                                    yield return new WaitForSeconds(.3f);

                                    // 풀에 리턴
                                    blockPool.ReturnPoolableObject(block_0);
                                    blockPool.ReturnPoolableObject(block_1);
                                    blockPool.ReturnPoolableObject(block_2);

                                    // 점수를 더합니다!
                                    DM.SetScore(block_0.BlockScore);
                                    DM.SetScore(block_1.BlockScore);
                                    DM.SetScore(block_2.BlockScore);

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
                            // Row이 음수 또는 0
                            else if (checkBlock.row <= 0)
                            {
                                block_0 = blocks[row + 1, col];
                                block_1 = blocks[row + 2, col];
                                block_2 = blocks[row + 3, col];

                                if (checkBlock.elementType == block_0.elementType &&
                                    checkBlock.elementType == block_1.elementType &&
                                    checkBlock.elementType == block_2.elementType)
                                {
                                    block_0.row = checkBlock.row;
                                    block_1.row = checkBlock.row;
                                    block_2.row = checkBlock.row;

                                    yield return new WaitForSeconds(.3f);

                                    // 풀에 리턴
                                    blockPool.ReturnPoolableObject(block_0);
                                    blockPool.ReturnPoolableObject(block_1);
                                    blockPool.ReturnPoolableObject(block_2);

                                    // 점수를 더합니다!
                                    DM.SetScore(block_0.BlockScore);
                                    DM.SetScore(block_1.BlockScore);
                                    DM.SetScore(block_2.BlockScore);

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
                            // Row이 양수
                            else if (checkBlock.row > 0)
                            {
                                block_0 = blocks[row - 1, col];
                                block_1 = blocks[row - 2, col];
                                block_2 = blocks[row - 3, col];

                                if (checkBlock.elementType == block_0.elementType &&
                                    checkBlock.elementType == block_1.elementType &&
                                    checkBlock.elementType == block_2.elementType)
                                {
                                    block_0.row = checkBlock.row;
                                    block_1.row = checkBlock.row;
                                    block_2.row = checkBlock.row;

                                    yield return new WaitForSeconds(.3f);

                                    // 풀에 리턴
                                    blockPool.ReturnPoolableObject(block_0);
                                    blockPool.ReturnPoolableObject(block_1);
                                    blockPool.ReturnPoolableObject(block_2);

                                    // 점수를 더합니다!
                                    DM.SetScore(block_0.BlockScore);
                                    DM.SetScore(block_1.BlockScore);
                                    DM.SetScore(block_2.BlockScore);

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

                                // GameState -> Move
                                GM.SetGameState(GameState.Move);

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

                                // GameState -> Checking
                                GM.SetGameState(GameState.Checking);

                                BlockSort();
                            }
                        }
                    }
                }
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