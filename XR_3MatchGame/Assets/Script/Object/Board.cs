using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.TextCore.Text;
using UnityEngine;
using XR_3MatchGame.Util;
using XR_3MatchGame_Data;
using XR_3MatchGame_InGame;
using XR_3MatchGame_Util;

namespace XR_3MatchGame_Object
{
    public class Board : MonoBehaviour
    {
        // [row, col]
        // [����, ����]
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

            GM.Initialize(this);

            // BGM ����
            SoundManager.Instance.Initialize(SceneType.InGame);

            StartCoroutine(SpawnBlock());
        }

        private void Update()
        {
            //if (GM.GameState == GameState.Checking || GM.GameState == GameState.SKill)
            //{
            //    if (GM.isMatch == true)
            //    {
            //        GM.isMatch = false;
            //        // StartCoroutine(BlockClear());
            //    }
            //}

            if (GM.isMatch == true)
            {
                BlockSort();

                GM.isMatch = false;

                // GameState -> Match
                GM.SetGameState(GameState.Match);

                StartCoroutine(BlockMatch());
            }
        }

        /// <summary>
        /// ���忡 ���� �����ϴ� �޼���
        /// </summary>
        /// <returns></returns>
        public IEnumerator SpawnBlock()
        {
            // GameState -> Spawn
            GM.SetGameState(GameState.Spawn);

            var blockPool = ObjectPoolManager.Instance.GetPool<Block>(PoolType.Block);

            for (int row = 0; row < 7; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    Block block = blockPool.GetPoolableObject(obj => obj.CanRecycle);

                    // �θ� ����
                    block.transform.SetParent(this.transform);

                    // ��ġ�� ����
                    block.transform.localPosition = new Vector3(-3 + col, 4 + row, 0);

                    // ȸ���� ����
                    block.transform.localRotation = Quaternion.identity;

                    // ����� ����
                    block.transform.localScale = new Vector3(.19f, .19f, .19f);

                    // Ȱ��ȭ
                    block.gameObject.SetActive(true);

                    // �� �ʱ�ȭ
                    block.Initialize(-3 + col, 4 + row);

                    // ������ �Ʒ��� �۾�
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

            GM.SetGameState(GameState.Play);
        }

        public IEnumerator ColBoom()
        {
            var blockPool = ObjectPoolManager.Instance.GetPool<Block>(PoolType.Block);
            bool isMake = false;

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (blocks[row, col] != null)
                    {
                        Block checkBlock = blocks[row, col];

                        Block block_0 = null;
                        Block block_1 = null;
                        Block block_2 = null;

                        switch (checkBlock.col)
                        {
                            case -3:
                                // ��OOO
                                block_0 = blocks[row, col + 1];
                                block_1 = blocks[row, col + 2];
                                block_2 = blocks[row, col + 3];

                                if (checkBlock.elementType == block_0.elementType && checkBlock.elementType == block_1.elementType && checkBlock.elementType == block_2.elementType)
                                {
                                    block_0.col = checkBlock.col;
                                    block_1.col = checkBlock.col;
                                    block_2.col = checkBlock.col;

                                    yield return new WaitForSeconds(.3f);

                                    // Ǯ�� ����
                                    blockPool.ReturnPoolableObject(block_0);
                                    blockPool.ReturnPoolableObject(block_1);
                                    blockPool.ReturnPoolableObject(block_2);

                                    // �� ����ҿ��� ����
                                    blocks[row, col + 1] = null;
                                    blocks[row, col + 2] = null;
                                    blocks[row, col + 3] = null;

                                    isMake = true;
                                }
                                break;

                            case -2:
                                // O��OO
                                block_0 = blocks[row, col - 1];
                                block_1 = blocks[row, col + 1];
                                block_2 = blocks[row, col + 2];

                                if (checkBlock.elementType == block_0.elementType && checkBlock.elementType == block_1.elementType && checkBlock.elementType == block_2.elementType)
                                {
                                    block_0.col = checkBlock.col;
                                    block_1.col = checkBlock.col;
                                    block_2.col = checkBlock.col;

                                    yield return new WaitForSeconds(.3f);

                                    // Ǯ�� ����
                                    blockPool.ReturnPoolableObject(block_0);
                                    blockPool.ReturnPoolableObject(block_1);
                                    blockPool.ReturnPoolableObject(block_2);

                                    // �� ����ҿ��� ����
                                    blocks[row, col - 1] = null;
                                    blocks[row, col + 1] = null;
                                    blocks[row, col + 2] = null;

                                    isMake = true;
                                }
                                else
                                {
                                    // ��OOO
                                    block_0 = blocks[row, col + 1];
                                    block_1 = blocks[row, col + 2];
                                    block_2 = blocks[row, col + 3];

                                    if (checkBlock.elementType == block_0.elementType && checkBlock.elementType == block_1.elementType && checkBlock.elementType == block_2.elementType)
                                    {
                                        block_0.col = checkBlock.col;
                                        block_1.col = checkBlock.col;
                                        block_2.col = checkBlock.col;

                                        yield return new WaitForSeconds(.3f);

                                        // Ǯ�� ����
                                        blockPool.ReturnPoolableObject(block_0);
                                        blockPool.ReturnPoolableObject(block_1);
                                        blockPool.ReturnPoolableObject(block_2);

                                        // �� ����ҿ��� ����
                                        blocks[row, col + 1] = null;
                                        blocks[row, col + 2] = null;
                                        blocks[row, col + 3] = null;

                                        isMake = true;
                                    }
                                }
                                break;

                            case -1:
                            case 0:
                                // OO��O
                                block_0 = blocks[row, col - 1];
                                block_1 = blocks[row, col - 2];
                                block_2 = blocks[row, col + 1];

                                if (checkBlock.elementType == block_0.elementType && checkBlock.elementType == block_1.elementType && checkBlock.elementType == block_2.elementType)
                                {
                                    block_0.col = checkBlock.col;
                                    block_1.col = checkBlock.col;
                                    block_2.col = checkBlock.col;

                                    yield return new WaitForSeconds(.3f);

                                    // Ǯ�� ����
                                    blockPool.ReturnPoolableObject(block_0);
                                    blockPool.ReturnPoolableObject(block_1);
                                    blockPool.ReturnPoolableObject(block_2);

                                    // �� ����ҿ��� ����
                                    blocks[row, col - 1] = null;
                                    blocks[row, col - 2] = null;
                                    blocks[row, col + 1] = null;

                                    isMake = true;
                                }
                                else
                                {
                                    // O��OO
                                    block_0 = blocks[row, col - 1];
                                    block_1 = blocks[row, col + 1];
                                    block_2 = blocks[row, col + 2];

                                    if (checkBlock.elementType == block_0.elementType && checkBlock.elementType == block_1.elementType && checkBlock.elementType == block_2.elementType)
                                    {
                                        block_0.col = checkBlock.col;
                                        block_1.col = checkBlock.col;
                                        block_2.col = checkBlock.col;

                                        yield return new WaitForSeconds(.3f);

                                        // Ǯ�� ����
                                        blockPool.ReturnPoolableObject(block_0);
                                        blockPool.ReturnPoolableObject(block_1);
                                        blockPool.ReturnPoolableObject(block_2);

                                        // �� ����ҿ��� ����
                                        blocks[row, col - 1] = null;
                                        blocks[row, col + 1] = null;
                                        blocks[row, col + 2] = null;

                                        isMake = true;
                                    }
                                    else
                                    {
                                        // ��OOO
                                        block_0 = blocks[row, col + 1];
                                        block_1 = blocks[row, col + 2];
                                        block_2 = blocks[row, col + 3];

                                        if (checkBlock.elementType == block_0.elementType && checkBlock.elementType == block_1.elementType && checkBlock.elementType == block_2.elementType)
                                        {
                                            block_0.col = checkBlock.col;
                                            block_1.col = checkBlock.col;
                                            block_2.col = checkBlock.col;

                                            yield return new WaitForSeconds(.3f);

                                            // Ǯ�� ����
                                            blockPool.ReturnPoolableObject(block_0);
                                            blockPool.ReturnPoolableObject(block_1);
                                            blockPool.ReturnPoolableObject(block_2);

                                            // �� ����ҿ��� ����
                                            blocks[row, col + 1] = null;
                                            blocks[row, col + 2] = null;
                                            blocks[row, col + 3] = null;

                                            isMake = true;
                                        }
                                    }
                                }
                                break;

                            case 1:
                                // O��OO
                                block_0 = blocks[row, col - 1];
                                block_1 = blocks[row, col + 1];
                                block_2 = blocks[row, col + 2];

                                if (checkBlock.elementType == block_0.elementType && checkBlock.elementType == block_1.elementType && checkBlock.elementType == block_2.elementType)
                                {
                                    block_0.col = checkBlock.col;
                                    block_1.col = checkBlock.col;
                                    block_2.col = checkBlock.col;

                                    yield return new WaitForSeconds(.3f);

                                    // Ǯ�� ����
                                    blockPool.ReturnPoolableObject(block_0);
                                    blockPool.ReturnPoolableObject(block_1);
                                    blockPool.ReturnPoolableObject(block_2);

                                    // �� ����ҿ��� ����
                                    blocks[row, col - 1] = null;
                                    blocks[row, col + 1] = null;
                                    blocks[row, col + 2] = null;

                                    isMake = true;
                                }
                                else
                                {
                                    // OO��O
                                    block_0 = blocks[row, col - 1];
                                    block_1 = blocks[row, col - 2];
                                    block_2 = blocks[row, col + 1];

                                    if (checkBlock.elementType == block_0.elementType && checkBlock.elementType == block_1.elementType && checkBlock.elementType == block_2.elementType)
                                    {
                                        block_0.col = checkBlock.col;
                                        block_1.col = checkBlock.col;
                                        block_2.col = checkBlock.col;

                                        yield return new WaitForSeconds(.3f);

                                        // Ǯ�� ����
                                        blockPool.ReturnPoolableObject(block_0);
                                        blockPool.ReturnPoolableObject(block_1);
                                        blockPool.ReturnPoolableObject(block_2);

                                        // �� ����ҿ��� ����
                                        blocks[row, col - 1] = null;
                                        blocks[row, col - 2] = null;
                                        blocks[row, col + 1] = null;

                                        isMake = true;
                                    }
                                    else
                                    {
                                        // OOO��
                                        block_0 = blocks[row, col - 1];
                                        block_1 = blocks[row, col - 2];
                                        block_2 = blocks[row, col - 3];


                                        if (checkBlock.elementType == block_0.elementType && checkBlock.elementType == block_1.elementType && checkBlock.elementType == block_2.elementType)
                                        {
                                            block_0.col = checkBlock.col;
                                            block_1.col = checkBlock.col;
                                            block_2.col = checkBlock.col;

                                            yield return new WaitForSeconds(.3f);

                                            // Ǯ�� ����
                                            blockPool.ReturnPoolableObject(block_0);
                                            blockPool.ReturnPoolableObject(block_1);
                                            blockPool.ReturnPoolableObject(block_2);

                                            // �� ����ҿ��� ����
                                            blocks[row, col - 1] = null;
                                            blocks[row, col - 2] = null;
                                            blocks[row, col - 3] = null;

                                            isMake = true;
                                        }
                                    }
                                }
                                break;

                            case 2:
                                // OO��O
                                block_0 = blocks[row, col + 1];
                                block_1 = blocks[row, col - 1];
                                block_2 = blocks[row, col - 2];

                                if (checkBlock.elementType == block_0.elementType && checkBlock.elementType == block_1.elementType && checkBlock.elementType == block_2.elementType)
                                {
                                    block_0.col = checkBlock.col;
                                    block_1.col = checkBlock.col;
                                    block_2.col = checkBlock.col;

                                    yield return new WaitForSeconds(.3f);

                                    // Ǯ�� ����
                                    blockPool.ReturnPoolableObject(block_0);
                                    blockPool.ReturnPoolableObject(block_1);
                                    blockPool.ReturnPoolableObject(block_2);

                                    // �� ����ҿ��� ����
                                    blocks[row, col + 1] = null;
                                    blocks[row, col - 1] = null;
                                    blocks[row, col - 2] = null;

                                    isMake = true;
                                }
                                else
                                {
                                    // OOO��
                                    block_0 = blocks[row, col - 1];
                                    block_1 = blocks[row, col - 2];
                                    block_2 = blocks[row, col - 3];

                                    if (checkBlock.elementType == block_0.elementType && checkBlock.elementType == block_1.elementType && checkBlock.elementType == block_2.elementType)
                                    {
                                        block_0.col = checkBlock.col;
                                        block_1.col = checkBlock.col;
                                        block_2.col = checkBlock.col;

                                        yield return new WaitForSeconds(.3f);

                                        // Ǯ�� ����
                                        blockPool.ReturnPoolableObject(block_0);
                                        blockPool.ReturnPoolableObject(block_1);
                                        blockPool.ReturnPoolableObject(block_2);

                                        // �� ����ҿ��� ����
                                        blocks[row, col - 1] = null;
                                        blocks[row, col - 2] = null;
                                        blocks[row, col - 3] = null;

                                        isMake = true;
                                    }
                                }
                                break;

                            case 3:
                                // OOO��
                                block_0 = blocks[row, col - 1];
                                block_1 = blocks[row, col - 2];
                                block_2 = blocks[row, col - 3];

                                if (checkBlock.elementType == block_0.elementType && checkBlock.elementType == block_1.elementType && checkBlock.elementType == block_2.elementType)
                                {
                                    block_0.col = checkBlock.col;
                                    block_1.col = checkBlock.col;
                                    block_2.col = checkBlock.col;

                                    yield return new WaitForSeconds(.3f);

                                    // Ǯ�� ����
                                    blockPool.ReturnPoolableObject(block_0);
                                    blockPool.ReturnPoolableObject(block_1);
                                    blockPool.ReturnPoolableObject(block_2);

                                    // �� ����ҿ��� ����
                                    blocks[row, col - 1] = null;
                                    blocks[row, col - 2] = null;
                                    blocks[row, col - 3] = null;

                                    isMake = true;
                                }
                                break;
                        }

                        if (isMake)
                        {
                            // �� �ٿ� ī��Ʈ ���
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

                            // �� ������ �۾�
                            for (int d_Row = 0; d_Row < height; d_Row++)
                            {
                                for (int d_Col = 0; d_Col < width; d_Col++)
                                {
                                    if (blocks[d_Row, d_Col] != null)
                                    {
                                        if (blocks[d_Row, d_Col].downCount > 0)
                                        {
                                            // ���� ������ �����Ѵ�

                                            var targetrow = (blocks[d_Row, d_Col].row -= blocks[d_Row, d_Col].downCount);

                                            if (Mathf.Abs(targetrow - blocks[d_Row, d_Col].transform.localPosition.y) > .1f)
                                            {
                                                Vector2 tempposition = new Vector2(blocks[d_Row, d_Col].transform.localPosition.x, targetrow);
                                                blocks[d_Row, d_Col].transform.localPosition = Vector2.Lerp(blocks[d_Row, d_Col].transform.localPosition, tempposition, .05f);
                                            }

                                            // �ٿ� ī��Ʈ �ʱ�ȭ
                                            blocks[d_Row, d_Col].downCount = 0;
                                        }
                                    }
                                }
                            }

                            yield return new WaitForSeconds(.3f);

                            // �� ���� �� ����
                            int newCol = -3;
                            int newRow = 3;

                            for (int n_Col = 0; n_Col < width; n_Col++)
                            {
                                for (int n_Row = 0; n_Row < height; n_Row++)
                                {
                                    if (blocks[n_Row, n_Col] == null)
                                    {
                                        // ��? ���� ����ִµ�? Ż���ε�?
                                        // �׷��� �Ӹ� �ɾ������~
                                        Block newBlock = blockPool.GetPoolableObject(obj => obj.CanRecycle);

                                        // �θ� ����
                                        newBlock.transform.SetParent(this.transform);

                                        // ��ġ�� ����
                                        newBlock.transform.localPosition = new Vector3(newCol, 4 + n_Row, 0);

                                        // ȸ���� ����
                                        newBlock.transform.localRotation = Quaternion.identity;

                                        // ����� ����
                                        newBlock.transform.localScale = new Vector3(.19f, .19f, .19f);

                                        // Ȱ��ȭ
                                        newBlock.gameObject.SetActive(true);

                                        // �� �ʱ�ȭ
                                        newBlock.Initialize(newCol, 4 + n_Row);

                                        // �� ����
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

                            // �� ����
                            BlockSort();

                            isMake = false;

                            yield return new WaitForSeconds(.5f);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// ������ �� ������ ��ź ���� Ȯ�� �޼���
        /// </summary>
        /// <param name="moveBlock"></param>
        /// <returns></returns>
        public IEnumerator MoveBoom(Block moveBlock = null)
        {
            Block block_0 = null;
            Block block_1 = null;
            Block block_2 = null;

            if (moveBlock != null)
            {
                for (int row = 0; row < height; row++)
                {
                    for (int col = 0; col < width; col++)
                    {
                        if (blocks[row, col] == moveBlock)
                        {
                            switch (moveBlock.col)
                            {
                                case -3:

                                    break;

                                case -2:
                                    break;

                                case -1:

                                case 0:
                                    break;

                                case 1:
                                    break;

                                case 2:
                                    break;

                                case 3:
                                    break;

                            }
                        }
                    }
                }
            }

            yield return null;
        }

        /// <summary>
        /// �� ��Ī�� �����ϴ� �ڷ�ƾ
        /// </summary>
        /// <returns></returns>
        public IEnumerator BlockMatch()
        {
            StartCoroutine(ColBoom());

            yield return new WaitForSeconds(.5f);

            var blockPool = ObjectPoolManager.Instance.GetPool<Block>(PoolType.Block);

            // 3X3 �� �ı�
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (blocks[row, col] != null)
                    {
                        // 3X3 Col
                        if (col != 5 && col != 6)
                        {
                            Block checkBlock = blocks[row, col];
                            Block col_0 = blocks[row, col + 1];
                            Block col_1 = blocks[row, col + 2];

                            if (col_0 != null && col_1 != null)
                            {
                                // Col
                                if (checkBlock.elementType == col_0.elementType && checkBlock.elementType == col_1.elementType)
                                {
                                    blocks[row, col] = null;
                                    blocks[row, col + 1] = null;
                                    blocks[row, col + 2] = null;

                                    blockPool.ReturnPoolableObject(checkBlock);
                                    blockPool.ReturnPoolableObject(col_0);
                                    blockPool.ReturnPoolableObject(col_1);
                                }
                            }
                        }
                    }

                    // 3X3 Row
                    if (blocks[row, col] != null)
                    {
                        if (row != 5 && row != 6)
                        {
                            Block checkBlock = blocks[row, col];
                            Block row_0 = blocks[row + 1, col];
                            Block row_1 = blocks[row + 2, col];

                            if (row_0 != null && row_1 != null)
                            {
                                if (checkBlock.elementType == row_0.elementType && checkBlock.elementType == row_1.elementType)
                                {
                                    blocks[row, col] = null;
                                    blocks[row + 1, col] = null;
                                    blocks[row + 2, col] = null;

                                    blockPool.ReturnPoolableObject(checkBlock);
                                    blockPool.ReturnPoolableObject(row_0);
                                    blockPool.ReturnPoolableObject(row_1);
                                }
                            }
                        }
                    }
                }
            }

            // �� �ٿ� ī��Ʈ ���
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

            // �� ������ �۾�
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (blocks[row, col] != null)
                    {
                        if (blocks[row, col].downCount > 0)
                        {
                            // ���� ������ �����Ѵ�

                            var targetrow = (blocks[row, col].row -= blocks[row, col].downCount);

                            if (Mathf.Abs(targetrow - blocks[row, col].transform.localPosition.y) > .1f)
                            {
                                Vector2 tempposition = new Vector2(blocks[row, col].transform.localPosition.x, targetrow);
                                blocks[row, col].transform.localPosition = Vector2.Lerp(blocks[row, col].transform.localPosition, tempposition, .05f);
                            }

                            // �ٿ� ī��Ʈ �ʱ�ȭ
                            blocks[row, col].downCount = 0;
                        }
                    }
                }
            }

            yield return new WaitForSeconds(.3f);

            // �� ���� �� ����
            int newCol = -3;
            int newRow = 3;

            for (int col = 0; col < width; col++)
            {
                for (int row = 0; row < height; row++)
                {
                    if (blocks[row, col] == null)
                    {
                        // ��? ���� ����ִµ�? Ż���ε�?
                        // �׷��� �Ӹ� �ɾ������~
                        Block newBlock = blockPool.GetPoolableObject(obj => obj.CanRecycle);

                        // �θ� ����
                        newBlock.transform.SetParent(this.transform);

                        // ��ġ�� ����
                        newBlock.transform.localPosition = new Vector3(newCol, 4 + row, 0);

                        // ȸ���� ����
                        newBlock.transform.localRotation = Quaternion.identity;

                        // ����� ����
                        newBlock.transform.localScale = new Vector3(.19f, .19f, .19f);

                        // Ȱ��ȭ
                        newBlock.gameObject.SetActive(true);

                        // �� �ʱ�ȭ
                        newBlock.Initialize(newCol, 4 + row);

                        // �� ����
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
                StartCoroutine(BlockMatch());
            }
            else
            {
                // GameState -> Play
                GM.SetGameState(GameState.Play);
            }
        }

        /// <summary>
        /// �� ������ ���� ���� �ִ��� üũ�ϴ� �޼���
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
        /// ���� �����ϴ� �޼���
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

            // ������ ���� ����
            blocks = testArr;
        }
    }
}