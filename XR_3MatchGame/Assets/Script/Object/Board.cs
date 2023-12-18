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
        // [����, ����]
        public int width = 7;
        public int height = 7;

        public Block[,] blocks = new Block[7, 7];

        public Block moveBlock;
        public Block boomBlock;

        public bool isReStart;

        #region Manager

        public InGameManager IGM;

        private GameManager GM;
        private DataManager DM;

        #endregion


        private Block checkBlock;

        private Block col_0;
        private Block col_1;
        private Block col_2;

        private Block row_0;
        private Block row_1;
        private Block row_2;


        private void Start()
        {
            GM = GameManager.Instance;
            DM = DataManager.Instance;

            GM.Board = this;

            // BGM ����
            SoundManager.Instance.Initialize(SceneType.InGame);

            // ���� ����
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
        /// ���忡 ���� �����ϴ� �޼���
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

            GM.SetGameState(GameState.Checking);

            StartCoroutine(BlockMatch());
        }

        /// <summary>
        /// �� ��Ī�� �����ϴ� �ڷ�ƾ
        /// </summary>
        /// <returns></returns>
        public IEnumerator BlockMatch()
        {
            var blockPool = ObjectPoolManager.Instance.GetPool<Block>(PoolType.Block);
            var uiElement = UIWindowManager.Instance.GetWindow<UIElement>();

            // 4X4 - �Ϲ� ��Ȳ
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

                            if (col_0 != null && col_1 != null && col_2 != null)
                            {
                                if (checkBlock.elementType == col_0.elementType &&
                                    checkBlock.elementType == col_1.elementType &&
                                    checkBlock.elementType == col_2.elementType)
                                {
                                    col_0.col = checkBlock.col;
                                    col_1.col = checkBlock.col;
                                    col_2.col = checkBlock.col;

                                    yield return new WaitForSeconds(.3f);

                                    // Ǯ�� ����
                                    blockPool.ReturnPoolableObject(col_0);
                                    blockPool.ReturnPoolableObject(col_1);
                                    blockPool.ReturnPoolableObject(col_2);

                                    // ������ ���մϴ�!
                                    DM.SetScore(col_0.BlockScore);
                                    DM.SetScore(col_1.BlockScore);
                                    DM.SetScore(col_2.BlockScore);

                                    // �� ����ҿ��� ����
                                    blocks[row, col + 1] = null;
                                    blocks[row, col + 2] = null;
                                    blocks[row, col + 3] = null;

                                    // ������ �Ǵ� ���� ��ź���� �ٲ������!!
                                    checkBlock.elementType = ElementType.Balance;
                                    checkBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());
                                }
                            }
                        }

                        if (checkBlock.row <= 0)
                        {
                            row_0 = blocks[row + 1, col];
                            row_1 = blocks[row + 2, col];
                            row_2 = blocks[row + 3, col];

                            if (row_0 != null && row_1 != null && row_2 != null)
                            {
                                if (checkBlock.elementType == row_0.elementType &&
                                    checkBlock.elementType == row_1.elementType &&
                                    checkBlock.elementType == row_2.elementType)
                                {
                                    row_0.row = checkBlock.row;
                                    row_1.row = checkBlock.row;
                                    row_2.row = checkBlock.row;

                                    yield return new WaitForSeconds(.3f);

                                    // Ǯ�� ����
                                    blockPool.ReturnPoolableObject(row_0);
                                    blockPool.ReturnPoolableObject(row_1);
                                    blockPool.ReturnPoolableObject(row_2);

                                    // ������ ���մϴ�!
                                    DM.SetScore(row_0.BlockScore);
                                    DM.SetScore(row_1.BlockScore);
                                    DM.SetScore(row_2.BlockScore);

                                    // �� ����ҿ��� ����
                                    blocks[row + 1, col] = null;
                                    blocks[row + 2, col] = null;
                                    blocks[row + 3, col] = null;

                                    // ������ �Ǵ� ���� ��ź���� �ٲ������!!
                                    checkBlock.elementType = ElementType.Balance;
                                    checkBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());
                                }
                            }
                        }
                    }
                }
            }

            yield return new WaitForSeconds(.3f);

            // 3X3 ��� �ı� - ���� ��Ȳ
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

                            // 1��° Col Ư�� ��Ȳ ������ 
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
                                        Debug.Log("Ư�� ��Ȳ");

                                        // ���� ������Ʈ
                                        DM.SetScore(checkBlock.BlockScore);
                                        DM.SetScore(col_0.BlockScore);
                                        DM.SetScore(col_1.BlockScore);
                                        DM.SetScore(row_0.BlockScore);
                                        DM.SetScore(row_1.BlockScore);

                                        // ��ų ������ ������Ʈ
                                        uiElement.SetGauge(checkBlock.ElementValue);
                                        uiElement.SetGauge(col_0.ElementValue);
                                        uiElement.SetGauge(col_1.ElementValue);
                                        uiElement.SetGauge(row_0.ElementValue);
                                        uiElement.SetGauge(row_1.ElementValue);

                                        // ����ҿ��� ����
                                        blocks[row, col] = null;
                                        blocks[row, col + 1] = null;
                                        blocks[row, col + 2] = null;

                                        blocks[row + 1, col] = null;
                                        blocks[row + 2, col] = null;

                                        // Ǯ�� ��ȯ
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
                                                // ���� ������Ʈ
                                                DM.SetScore(checkBlock.BlockScore);
                                                DM.SetScore(col_0.BlockScore);
                                                DM.SetScore(col_1.BlockScore);
                                                DM.SetScore(row_0.BlockScore);
                                                DM.SetScore(row_1.BlockScore);

                                                // ��ų ������ ������Ʈ
                                                uiElement.SetGauge(checkBlock.ElementValue);
                                                uiElement.SetGauge(col_0.ElementValue);
                                                uiElement.SetGauge(col_1.ElementValue);
                                                uiElement.SetGauge(row_0.ElementValue);
                                                uiElement.SetGauge(row_1.ElementValue);

                                                // ����ҿ��� ����
                                                blocks[row, col] = null;
                                                blocks[row, col + 1] = null;
                                                blocks[row, col + 2] = null;

                                                blocks[row + 1, col + 1] = null;
                                                blocks[row + 2, col + 1] = null;

                                                // Ǯ�� ��ȯ
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
                                                        // ���� ������Ʈ
                                                        DM.SetScore(checkBlock.BlockScore);
                                                        DM.SetScore(col_0.BlockScore);
                                                        DM.SetScore(col_1.BlockScore);
                                                        DM.SetScore(row_0.BlockScore);
                                                        DM.SetScore(row_1.BlockScore);

                                                        // ��ų ������ ������Ʈ
                                                        uiElement.SetGauge(checkBlock.ElementValue);
                                                        uiElement.SetGauge(col_0.ElementValue);
                                                        uiElement.SetGauge(col_1.ElementValue);
                                                        uiElement.SetGauge(row_0.ElementValue);
                                                        uiElement.SetGauge(row_1.ElementValue);

                                                        // ����ҿ��� ����
                                                        blocks[row, col] = null;
                                                        blocks[row, col + 1] = null;
                                                        blocks[row, col + 2] = null;

                                                        blocks[row + 1, col + 2] = null;
                                                        blocks[row + 2, col + 2] = null;

                                                        // Ǯ�� ��ȯ
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

                            // 2��° Col Ư�� ��Ȳ ������
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
                                        // ���� ������Ʈ
                                        DM.SetScore(checkBlock.BlockScore);
                                        DM.SetScore(col_0.BlockScore);
                                        DM.SetScore(col_1.BlockScore);
                                        DM.SetScore(row_0.BlockScore);
                                        DM.SetScore(row_1.BlockScore);

                                        // ��ų ������ ������Ʈ
                                        uiElement.SetGauge(checkBlock.ElementValue);
                                        uiElement.SetGauge(col_0.ElementValue);
                                        uiElement.SetGauge(col_1.ElementValue);
                                        uiElement.SetGauge(row_0.ElementValue);
                                        uiElement.SetGauge(row_1.ElementValue);

                                        // ����ҿ��� ����
                                        blocks[row, col] = null;
                                        blocks[row, col + 1] = null;
                                        blocks[row, col + 2] = null;

                                        blocks[row - 1, col] = null;
                                        blocks[row - 2, col] = null;

                                        // Ǯ�� ��ȯ
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
                                                // ���� ������Ʈ
                                                DM.SetScore(checkBlock.BlockScore);
                                                DM.SetScore(col_0.BlockScore);
                                                DM.SetScore(col_1.BlockScore);
                                                DM.SetScore(row_0.BlockScore);
                                                DM.SetScore(row_1.BlockScore);

                                                // ��ų ������ ������Ʈ
                                                uiElement.SetGauge(checkBlock.ElementValue);
                                                uiElement.SetGauge(col_0.ElementValue);
                                                uiElement.SetGauge(col_1.ElementValue);
                                                uiElement.SetGauge(row_0.ElementValue);
                                                uiElement.SetGauge(row_1.ElementValue);

                                                // ����ҿ��� ����
                                                blocks[row, col] = null;
                                                blocks[row, col + 1] = null;
                                                blocks[row, col + 2] = null;

                                                blocks[row - 1, col + 1] = null;
                                                blocks[row - 2, col + 1] = null;

                                                // Ǯ�� ��ȯ
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
                                                        // ���� ������Ʈ
                                                        DM.SetScore(checkBlock.BlockScore);
                                                        DM.SetScore(col_0.BlockScore);
                                                        DM.SetScore(col_1.BlockScore);
                                                        DM.SetScore(row_0.BlockScore);
                                                        DM.SetScore(row_1.BlockScore);

                                                        // ��ų ������ ������Ʈ
                                                        uiElement.SetGauge(checkBlock.ElementValue);
                                                        uiElement.SetGauge(col_0.ElementValue);
                                                        uiElement.SetGauge(col_1.ElementValue);
                                                        uiElement.SetGauge(row_0.ElementValue);
                                                        uiElement.SetGauge(row_1.ElementValue);

                                                        // ����ҿ��� ����
                                                        blocks[row, col] = null;
                                                        blocks[row, col + 1] = null;
                                                        blocks[row, col + 2] = null;

                                                        blocks[row - 1, col + 2] = null;
                                                        blocks[row - 2, col + 2] = null;

                                                        // Ǯ�� ��ȯ
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

                            // 1��° Row Ư�� ��Ȳ ������
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
                                        // ���� ������Ʈ
                                        DM.SetScore(checkBlock.BlockScore);
                                        DM.SetScore(col_0.BlockScore);
                                        DM.SetScore(col_1.BlockScore);
                                        DM.SetScore(row_0.BlockScore);
                                        DM.SetScore(row_1.BlockScore);

                                        // ��ų ������ ������Ʈ
                                        uiElement.SetGauge(checkBlock.ElementValue);
                                        uiElement.SetGauge(col_0.ElementValue);
                                        uiElement.SetGauge(col_1.ElementValue);
                                        uiElement.SetGauge(row_0.ElementValue);
                                        uiElement.SetGauge(row_1.ElementValue);

                                        // ����ҿ��� ����
                                        blocks[row, col] = null;
                                        blocks[row + 1, col] = null;
                                        blocks[row + 2, col] = null;

                                        blocks[row + 1, col + 1] = null;
                                        blocks[row + 1, col + 2] = null;

                                        // Ǯ�� ��ȯ
                                        blockPool.ReturnPoolableObject(checkBlock);
                                        blockPool.ReturnPoolableObject(col_0);
                                        blockPool.ReturnPoolableObject(col_1);
                                        blockPool.ReturnPoolableObject(row_0);
                                        blockPool.ReturnPoolableObject(row_1);
                                    }
                                }
                            }

                            // 2��° Row Ư�� ��Ȳ ������
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
                                        // ���� ������Ʈ
                                        DM.SetScore(checkBlock.BlockScore);
                                        DM.SetScore(col_0.BlockScore);
                                        DM.SetScore(col_1.BlockScore);
                                        DM.SetScore(row_0.BlockScore);
                                        DM.SetScore(row_1.BlockScore);

                                        // ��ų ������ ������Ʈ
                                        uiElement.SetGauge(checkBlock.ElementValue);
                                        uiElement.SetGauge(col_0.ElementValue);
                                        uiElement.SetGauge(col_1.ElementValue);
                                        uiElement.SetGauge(row_0.ElementValue);
                                        uiElement.SetGauge(row_1.ElementValue);

                                        // ����ҿ��� ����
                                        blocks[row, col] = null;
                                        blocks[row + 1, col] = null;
                                        blocks[row + 2, col] = null;

                                        blocks[row + 1, col - 1] = null;
                                        blocks[row + 1, col - 2] = null;

                                        // Ǯ�� ��ȯ
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

            // 3X3 ��� �ı� - �Ϲ� ��Ȳ
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (blocks[row, col] != null)
                    {
                        checkBlock = blocks[row, col];

                        // Col �Ϲ� ��Ȳ
                        if (checkBlock.col != 2 && checkBlock.col != 3)
                        {
                            col_0 = blocks[row, col + 1];
                            col_1 = blocks[row, col + 2];

                            if (checkBlock != null && col_0 != null && col_1 != null)
                            {
                                if (checkBlock.elementType == col_0.elementType &&
                                    checkBlock.elementType == col_1.elementType)
                                {
                                    // ���� ������Ʈ
                                    DM.SetScore(checkBlock.BlockScore);
                                    DM.SetScore(col_0.BlockScore);
                                    DM.SetScore(col_1.BlockScore);

                                    // ��ų ������ ������Ʈ
                                    uiElement.SetGauge(checkBlock.ElementValue);
                                    uiElement.SetGauge(col_0.ElementValue);
                                    uiElement.SetGauge(col_1.ElementValue);

                                    // ����ҿ��� ����
                                    blocks[row, col] = null;
                                    blocks[row, col + 1] = null;
                                    blocks[row, col + 2] = null;

                                    // Ǯ�� ��ȯ
                                    blockPool.ReturnPoolableObject(checkBlock);
                                    blockPool.ReturnPoolableObject(col_0);
                                    blockPool.ReturnPoolableObject(col_1);
                                }
                            }
                        }

                        // Row �Ϲ� ��Ȳ
                        if (checkBlock.row != 2 && checkBlock.row != 3)
                        {
                            row_0 = blocks[row + 1, col];
                            row_1 = blocks[row + 2, col];

                            if (checkBlock != null && row_0 != null && row_1 != null)
                            {
                                if (checkBlock.elementType == row_0.elementType && checkBlock.elementType == row_1.elementType)
                                {

                                    // ���� ������Ʈ
                                    DM.SetScore(blocks[row, col].BlockScore);
                                    DM.SetScore(blocks[row + 1, col].BlockScore);
                                    DM.SetScore(blocks[row + 2, col].BlockScore);

                                    // ��ų ������ ������Ʈ
                                    uiElement.SetGauge(blocks[row, col].ElementValue);
                                    uiElement.SetGauge(blocks[row + 1, col].ElementValue);
                                    uiElement.SetGauge(blocks[row + 2, col].ElementValue);

                                    blocks[row, col] = null;
                                    blocks[row + 1, col] = null;
                                    blocks[row + 2, col] = null;

                                    // �� ����
                                    blockPool.ReturnPoolableObject(checkBlock);
                                    blockPool.ReturnPoolableObject(row_0);
                                    blockPool.ReturnPoolableObject(row_1);
                                }
                            }
                        }

                    }
                }
            }

            #region ��� �ı� ���� �۾�

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

            // �� ���� �� ����
            int newCol = -3;
            int newRow = 3;

            // �� ���� �� �̵�
            for (int col = 0; col < width; col++)
            {
                for (int row = 0; row < height; row++)
                {
                    if (blocks[row, col] == null)
                    {
                        Block newBlock = blockPool.GetPoolableObject(obj => obj.CanRecycle);

                        // �θ� ����
                        newBlock.transform.SetParent(this.transform);

                        // ��ġ�� ����
                        newBlock.transform.localPosition = new Vector3(newCol, row + 4, 0);

                        // ȸ���� ����
                        newBlock.transform.localRotation = Quaternion.identity;

                        // ����� ����
                        newBlock.transform.localScale = new Vector3(.19f, .19f, .19f);

                        // Ȱ��ȭ
                        newBlock.gameObject.SetActive(true);

                        // �� �ʱ�ȭ
                        newBlock.Initialize(newCol, row + 4);

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

            #endregion

            BlockSort();

            if (MatchCheck())
            {
                // �� ��Ī �����
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

        public IEnumerator MakeBoom()
        {
            var blockPool = ObjectPoolManager.Instance.GetPool<Block>(PoolType.Block);
            var uiElement = UIWindowManager.Instance.GetWindow<UIElement>();

            bool isMake = false;

            // �̵� ���� �ִ� ���
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
                                        // ��OOO
                                        col_0 = blocks[row, col + 1];
                                        col_1 = blocks[row, col + 2];
                                        col_2 = blocks[row, col + 3];

                                        if (moveBlock.elementType == col_0.elementType &&
                                            moveBlock.elementType == col_1.elementType &&
                                            moveBlock.elementType == col_2.elementType)
                                        {
                                            // �� �̵�
                                            col_0.col = moveBlock.col;
                                            col_1.col = moveBlock.col;
                                            col_2.col = moveBlock.col;

                                            yield return new WaitForSeconds(.3f);

                                            // �� ����
                                            blockPool.ReturnPoolableObject(col_0);
                                            blockPool.ReturnPoolableObject(col_1);
                                            blockPool.ReturnPoolableObject(col_2);

                                            // ���� ������Ʈ
                                            DM.SetScore(col_0.BlockScore);
                                            DM.SetScore(col_1.BlockScore);
                                            DM.SetScore(col_2.BlockScore);

                                            // ��ų ������ ������Ʈ
                                            uiElement.SetGauge(col_0.ElementValue);
                                            uiElement.SetGauge(col_1.ElementValue);
                                            uiElement.SetGauge(col_2.ElementValue);

                                            blocks[row, col + 1] = null;
                                            blocks[row, col + 2] = null;
                                            blocks[row, col + 3] = null;

                                            // ������ �Ǵ� ���� ��ź���� �ٲ������!!
                                            moveBlock.elementType = ElementType.Balance;
                                            moveBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                            isMake = true;
                                        }
                                        break;

                                    case -2:
                                        // ��OOO
                                        col_0 = blocks[row, col + 1];
                                        col_1 = blocks[row, col + 2];
                                        col_2 = blocks[row, col + 3];

                                        if (moveBlock.elementType == col_0.elementType &&
                                            moveBlock.elementType == col_1.elementType &&
                                            moveBlock.elementType == col_2.elementType)
                                        {
                                            // �� �̵�
                                            col_0.col = moveBlock.col;
                                            col_1.col = moveBlock.col;
                                            col_2.col = moveBlock.col;

                                            yield return new WaitForSeconds(.3f);

                                            // �� ����
                                            blockPool.ReturnPoolableObject(col_0);
                                            blockPool.ReturnPoolableObject(col_1);
                                            blockPool.ReturnPoolableObject(col_2);

                                            // ���� ������Ʈ
                                            DM.SetScore(col_0.BlockScore);
                                            DM.SetScore(col_1.BlockScore);
                                            DM.SetScore(col_2.BlockScore);

                                            // ��ų ������ ������Ʈ
                                            uiElement.SetGauge(col_0.ElementValue);
                                            uiElement.SetGauge(col_1.ElementValue);
                                            uiElement.SetGauge(col_2.ElementValue);

                                            blocks[row, col + 1] = null;
                                            blocks[row, col + 2] = null;
                                            blocks[row, col + 3] = null;

                                            // ������ �Ǵ� ���� ��ź���� �ٲ������!!
                                            moveBlock.elementType = ElementType.Balance;
                                            moveBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                            isMake = true;
                                        }
                                        else
                                        {
                                            // O��OO
                                            col_0 = blocks[row, col - 1];
                                            col_1 = blocks[row, col + 1];
                                            col_2 = blocks[row, col + 2];

                                            if (moveBlock.elementType == col_0.elementType &&
                                                moveBlock.elementType == col_1.elementType &&
                                                moveBlock.elementType == col_2.elementType)
                                            {
                                                // �� �̵�
                                                col_0.col = moveBlock.col;
                                                col_1.col = moveBlock.col;
                                                col_2.col = moveBlock.col;

                                                yield return new WaitForSeconds(.3f);

                                                // �� ����
                                                blockPool.ReturnPoolableObject(col_0);
                                                blockPool.ReturnPoolableObject(col_1);
                                                blockPool.ReturnPoolableObject(col_2);

                                                // ���� ������Ʈ
                                                DM.SetScore(col_0.BlockScore);
                                                DM.SetScore(col_1.BlockScore);
                                                DM.SetScore(col_2.BlockScore);

                                                // ��ų ������ ������Ʈ
                                                uiElement.SetGauge(col_0.ElementValue);
                                                uiElement.SetGauge(col_1.ElementValue);
                                                uiElement.SetGauge(col_2.ElementValue);

                                                blocks[row, col - 1] = null;
                                                blocks[row, col + 1] = null;
                                                blocks[row, col + 2] = null;

                                                // ������ �Ǵ� ���� ��ź���� �ٲ������!!
                                                moveBlock.elementType = ElementType.Balance;
                                                moveBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                isMake = true;
                                            }
                                        }
                                        break;

                                    case -1:
                                    case 0:
                                        // ��OOO
                                        col_0 = blocks[row, col + 1];
                                        col_1 = blocks[row, col + 2];
                                        col_2 = blocks[row, col + 3];

                                        if (moveBlock.elementType == col_0.elementType &&
                                            moveBlock.elementType == col_1.elementType &&
                                            moveBlock.elementType == col_2.elementType)
                                        {
                                            // �� �̵�
                                            col_0.col = moveBlock.col;
                                            col_1.col = moveBlock.col;
                                            col_2.col = moveBlock.col;

                                            yield return new WaitForSeconds(.3f);

                                            // �� ����
                                            blockPool.ReturnPoolableObject(col_0);
                                            blockPool.ReturnPoolableObject(col_1);
                                            blockPool.ReturnPoolableObject(col_2);

                                            // ���� ������Ʈ
                                            DM.SetScore(col_0.BlockScore);
                                            DM.SetScore(col_1.BlockScore);
                                            DM.SetScore(col_2.BlockScore);

                                            // ��ų ������ ������Ʈ
                                            uiElement.SetGauge(col_0.ElementValue);
                                            uiElement.SetGauge(col_1.ElementValue);
                                            uiElement.SetGauge(col_2.ElementValue);

                                            blocks[row, col + 1] = null;
                                            blocks[row, col + 2] = null;
                                            blocks[row, col + 3] = null;

                                            // ������ �Ǵ� ���� ��ź���� �ٲ������!!
                                            moveBlock.elementType = ElementType.Balance;
                                            moveBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                            isMake = true;
                                        }
                                        else
                                        {
                                            // O��OO
                                            col_0 = blocks[row, col - 1];
                                            col_1 = blocks[row, col + 1];
                                            col_2 = blocks[row, col + 2];

                                            if (moveBlock.elementType == col_0.elementType &&
                                                moveBlock.elementType == col_1.elementType &&
                                                moveBlock.elementType == col_2.elementType)
                                            {
                                                // �� �̵�
                                                col_0.col = moveBlock.col;
                                                col_1.col = moveBlock.col;
                                                col_2.col = moveBlock.col;

                                                yield return new WaitForSeconds(.3f);

                                                // �� ����
                                                blockPool.ReturnPoolableObject(col_0);
                                                blockPool.ReturnPoolableObject(col_1);
                                                blockPool.ReturnPoolableObject(col_2);

                                                // ���� ������Ʈ
                                                DM.SetScore(col_0.BlockScore);
                                                DM.SetScore(col_1.BlockScore);
                                                DM.SetScore(col_2.BlockScore);

                                                // ��ų ������ ������Ʈ
                                                uiElement.SetGauge(col_0.ElementValue);
                                                uiElement.SetGauge(col_1.ElementValue);
                                                uiElement.SetGauge(col_2.ElementValue);

                                                blocks[row, col - 1] = null;
                                                blocks[row, col + 1] = null;
                                                blocks[row, col + 2] = null;

                                                // ������ �Ǵ� ���� ��ź���� �ٲ������!!
                                                moveBlock.elementType = ElementType.Balance;
                                                moveBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                isMake = true;
                                            }
                                            else
                                            {
                                                // OO��O
                                                col_0 = blocks[row, col - 1];
                                                col_1 = blocks[row, col - 2];
                                                col_2 = blocks[row, col + 1];

                                                if (moveBlock.elementType == col_0.elementType &&
                                                    moveBlock.elementType == col_1.elementType &&
                                                    moveBlock.elementType == col_2.elementType)
                                                {
                                                    // �� �̵�
                                                    col_0.col = moveBlock.col;
                                                    col_1.col = moveBlock.col;
                                                    col_2.col = moveBlock.col;

                                                    yield return new WaitForSeconds(.3f);

                                                    // �� ����
                                                    blockPool.ReturnPoolableObject(col_0);
                                                    blockPool.ReturnPoolableObject(col_1);
                                                    blockPool.ReturnPoolableObject(col_2);

                                                    // ���� ������Ʈ
                                                    DM.SetScore(col_0.BlockScore);
                                                    DM.SetScore(col_1.BlockScore);
                                                    DM.SetScore(col_2.BlockScore);

                                                    // ��ų ������ ������Ʈ
                                                    uiElement.SetGauge(col_0.ElementValue);
                                                    uiElement.SetGauge(col_1.ElementValue);
                                                    uiElement.SetGauge(col_2.ElementValue);

                                                    blocks[row, col - 1] = null;
                                                    blocks[row, col - 2] = null;
                                                    blocks[row, col + 1] = null;

                                                    // ������ �Ǵ� ���� ��ź���� �ٲ������!!
                                                    moveBlock.elementType = ElementType.Balance;
                                                    moveBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                    isMake = true;
                                                }
                                            }
                                        }
                                        break;

                                    case 1:
                                        // OOO��
                                        col_0 = blocks[row, col - 1];
                                        col_1 = blocks[row, col - 2];
                                        col_2 = blocks[row, col - 3];

                                        if (moveBlock.elementType == col_0.elementType &&
                                            moveBlock.elementType == col_1.elementType &&
                                            moveBlock.elementType == col_2.elementType)
                                        {
                                            // �� �̵�
                                            col_0.col = moveBlock.col;
                                            col_1.col = moveBlock.col;
                                            col_2.col = moveBlock.col;

                                            yield return new WaitForSeconds(.3f);

                                            // �� ����
                                            blockPool.ReturnPoolableObject(col_0);
                                            blockPool.ReturnPoolableObject(col_1);
                                            blockPool.ReturnPoolableObject(col_2);

                                            // ���� ������Ʈ
                                            DM.SetScore(col_0.BlockScore);
                                            DM.SetScore(col_1.BlockScore);
                                            DM.SetScore(col_2.BlockScore);

                                            // ��ų ������ ������Ʈ
                                            uiElement.SetGauge(col_0.ElementValue);
                                            uiElement.SetGauge(col_1.ElementValue);
                                            uiElement.SetGauge(col_2.ElementValue);

                                            blocks[row, col - 1] = null;
                                            blocks[row, col - 2] = null;
                                            blocks[row, col - 3] = null;

                                            // ������ �Ǵ� ���� ��ź���� �ٲ������!!
                                            moveBlock.elementType = ElementType.Balance;
                                            moveBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                            isMake = true;
                                        }
                                        else
                                        {
                                            // OO��O
                                            col_0 = blocks[row, col - 1];
                                            col_1 = blocks[row, col - 2];
                                            col_2 = blocks[row, col + 1];

                                            if (moveBlock.elementType == col_0.elementType &&
                                                moveBlock.elementType == col_1.elementType &&
                                                moveBlock.elementType == col_2.elementType)
                                            {
                                                // �� �̵�
                                                col_0.col = moveBlock.col;
                                                col_1.col = moveBlock.col;
                                                col_2.col = moveBlock.col;

                                                yield return new WaitForSeconds(.3f);

                                                // �� ����
                                                blockPool.ReturnPoolableObject(col_0);
                                                blockPool.ReturnPoolableObject(col_1);
                                                blockPool.ReturnPoolableObject(col_2);

                                                // ���� ������Ʈ
                                                DM.SetScore(col_0.BlockScore);
                                                DM.SetScore(col_1.BlockScore);
                                                DM.SetScore(col_2.BlockScore);

                                                // ��ų ������ ������Ʈ
                                                uiElement.SetGauge(col_0.ElementValue);
                                                uiElement.SetGauge(col_1.ElementValue);
                                                uiElement.SetGauge(col_2.ElementValue);

                                                blocks[row, col - 1] = null;
                                                blocks[row, col - 2] = null;
                                                blocks[row, col + 1] = null;

                                                // ������ �Ǵ� ���� ��ź���� �ٲ������!!
                                                moveBlock.elementType = ElementType.Balance;
                                                moveBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                isMake = true;
                                            }
                                            else
                                            {
                                                // O��OO
                                                col_0 = blocks[row, col - 1];
                                                col_1 = blocks[row, col + 1];
                                                col_2 = blocks[row, col + 2];

                                                if (moveBlock.elementType == col_0.elementType &&
                                                    moveBlock.elementType == col_1.elementType &&
                                                    moveBlock.elementType == col_2.elementType)
                                                {
                                                    // �� �̵�
                                                    col_0.col = moveBlock.col;
                                                    col_1.col = moveBlock.col;
                                                    col_2.col = moveBlock.col;

                                                    yield return new WaitForSeconds(.3f);

                                                    // �� ����
                                                    blockPool.ReturnPoolableObject(col_0);
                                                    blockPool.ReturnPoolableObject(col_1);
                                                    blockPool.ReturnPoolableObject(col_2);

                                                    // ���� ������Ʈ
                                                    DM.SetScore(col_0.BlockScore);
                                                    DM.SetScore(col_1.BlockScore);
                                                    DM.SetScore(col_2.BlockScore);

                                                    // ��ų ������ ������Ʈ
                                                    uiElement.SetGauge(col_0.ElementValue);
                                                    uiElement.SetGauge(col_1.ElementValue);
                                                    uiElement.SetGauge(col_2.ElementValue);

                                                    blocks[row, col - 1] = null;
                                                    blocks[row, col + 1] = null;
                                                    blocks[row, col + 2] = null;

                                                    // ������ �Ǵ� ���� ��ź���� �ٲ������!!
                                                    moveBlock.elementType = ElementType.Balance;
                                                    moveBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                    isMake = true;
                                                }
                                            }
                                        }
                                        break;

                                    case 2:
                                        // OOO��
                                        col_0 = blocks[row, col - 1];
                                        col_1 = blocks[row, col - 2];
                                        col_2 = blocks[row, col - 3];

                                        if (moveBlock.elementType == col_0.elementType &&
                                            moveBlock.elementType == col_1.elementType &&
                                            moveBlock.elementType == col_2.elementType)
                                        {
                                            // �� �̵�
                                            col_0.col = moveBlock.col;
                                            col_1.col = moveBlock.col;
                                            col_2.col = moveBlock.col;

                                            yield return new WaitForSeconds(.3f);

                                            // �� ����
                                            blockPool.ReturnPoolableObject(col_0);
                                            blockPool.ReturnPoolableObject(col_1);
                                            blockPool.ReturnPoolableObject(col_2);

                                            // ���� ������Ʈ
                                            DM.SetScore(col_0.BlockScore);
                                            DM.SetScore(col_1.BlockScore);
                                            DM.SetScore(col_2.BlockScore);

                                            // ��ų ������ ������Ʈ
                                            uiElement.SetGauge(col_0.ElementValue);
                                            uiElement.SetGauge(col_1.ElementValue);
                                            uiElement.SetGauge(col_2.ElementValue);

                                            blocks[row, col - 1] = null;
                                            blocks[row, col - 2] = null;
                                            blocks[row, col - 3] = null;

                                            // ������ �Ǵ� ���� ��ź���� �ٲ������!!
                                            moveBlock.elementType = ElementType.Balance;
                                            moveBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                            isMake = true;
                                        }
                                        else
                                        {
                                            // OO��O
                                            col_0 = blocks[row, col - 1];
                                            col_1 = blocks[row, col - 2];
                                            col_2 = blocks[row, col + 1];

                                            if (moveBlock.elementType == col_0.elementType &&
                                                moveBlock.elementType == col_1.elementType &&
                                                moveBlock.elementType == col_2.elementType)
                                            {
                                                // �� �̵�
                                                col_0.col = moveBlock.col;
                                                col_1.col = moveBlock.col;
                                                col_2.col = moveBlock.col;

                                                yield return new WaitForSeconds(.3f);

                                                // �� ����
                                                blockPool.ReturnPoolableObject(col_0);
                                                blockPool.ReturnPoolableObject(col_1);
                                                blockPool.ReturnPoolableObject(col_2);

                                                // ���� ������Ʈ
                                                DM.SetScore(col_0.BlockScore);
                                                DM.SetScore(col_1.BlockScore);
                                                DM.SetScore(col_2.BlockScore);

                                                // ��ų ������ ������Ʈ
                                                uiElement.SetGauge(col_0.ElementValue);
                                                uiElement.SetGauge(col_1.ElementValue);
                                                uiElement.SetGauge(col_2.ElementValue);

                                                blocks[row, col - 1] = null;
                                                blocks[row, col - 2] = null;
                                                blocks[row, col + 1] = null;

                                                // ������ �Ǵ� ���� ��ź���� �ٲ������!!
                                                moveBlock.elementType = ElementType.Balance;
                                                moveBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                isMake = true;
                                            }
                                        }
                                        break;

                                    case 3:
                                        // OOO��
                                        col_0 = blocks[row, col - 1];
                                        col_1 = blocks[row, col - 2];
                                        col_2 = blocks[row, col - 3];

                                        if (moveBlock.elementType == col_0.elementType &&
                                            moveBlock.elementType == col_1.elementType &&
                                            moveBlock.elementType == col_2.elementType)
                                        {
                                            // �� �̵�
                                            col_0.col = moveBlock.col;
                                            col_1.col = moveBlock.col;
                                            col_2.col = moveBlock.col;

                                            yield return new WaitForSeconds(.3f);

                                            // �� ����
                                            blockPool.ReturnPoolableObject(col_0);
                                            blockPool.ReturnPoolableObject(col_1);
                                            blockPool.ReturnPoolableObject(col_2);

                                            // ���� ������Ʈ
                                            DM.SetScore(col_0.BlockScore);
                                            DM.SetScore(col_1.BlockScore);
                                            DM.SetScore(col_2.BlockScore);

                                            // ��ų ������ ������Ʈ
                                            uiElement.SetGauge(col_0.ElementValue);
                                            uiElement.SetGauge(col_1.ElementValue);
                                            uiElement.SetGauge(col_2.ElementValue);

                                            blocks[row, col - 1] = null;
                                            blocks[row, col - 2] = null;
                                            blocks[row, col - 3] = null;

                                            // ������ �Ǵ� ���� ��ź���� �ٲ������!!
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
                                        // ��OOO
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

                                            // ������ ���մϴ�!
                                            DM.SetScore(col_0.BlockScore);
                                            DM.SetScore(col_1.BlockScore);
                                            DM.SetScore(col_2.BlockScore);

                                            blocks[row + 1, col] = null;
                                            blocks[row + 2, col] = null;
                                            blocks[row + 3, col] = null;

                                            // ������ �Ǵ� ���� ��ź���� �ٲ������!!
                                            moveBlock.elementType = ElementType.Balance;
                                            moveBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                            isMake = true;
                                        }
                                        break;

                                    case -2:
                                        // ��OOO
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

                                            // ������ ���մϴ�!
                                            DM.SetScore(col_0.BlockScore);
                                            DM.SetScore(col_1.BlockScore);
                                            DM.SetScore(col_2.BlockScore);

                                            blocks[row + 1, col] = null;
                                            blocks[row + 2, col] = null;
                                            blocks[row + 3, col] = null;

                                            // ������ �Ǵ� ���� ��ź���� �ٲ������!!
                                            moveBlock.elementType = ElementType.Balance;
                                            moveBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                            isMake = true;
                                        }
                                        else
                                        {
                                            // O��OO
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

                                                // ������ ���մϴ�!
                                                DM.SetScore(col_0.BlockScore);
                                                DM.SetScore(col_1.BlockScore);
                                                DM.SetScore(col_2.BlockScore);

                                                blocks[row - 1, col] = null;
                                                blocks[row + 1, col] = null;
                                                blocks[row + 2, col] = null;

                                                // ������ �Ǵ� ���� ��ź���� �ٲ������!!
                                                moveBlock.elementType = ElementType.Balance;
                                                moveBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                isMake = true;
                                            }
                                        }
                                        break;

                                    case -1:
                                    case 0:
                                        // ��OOO
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

                                            // ������ ���մϴ�!
                                            DM.SetScore(col_0.BlockScore);
                                            DM.SetScore(col_1.BlockScore);
                                            DM.SetScore(col_2.BlockScore);

                                            blocks[row + 1, col] = null;
                                            blocks[row + 2, col] = null;
                                            blocks[row + 3, col] = null;

                                            // ������ �Ǵ� ���� ��ź���� �ٲ������!!
                                            moveBlock.elementType = ElementType.Balance;
                                            moveBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                            isMake = true;
                                        }
                                        else
                                        {
                                            // O��OO
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

                                                // ������ ���մϴ�!
                                                DM.SetScore(col_0.BlockScore);
                                                DM.SetScore(col_1.BlockScore);
                                                DM.SetScore(col_2.BlockScore);

                                                blocks[row - 1, col] = null;
                                                blocks[row + 1, col] = null;
                                                blocks[row + 2, col] = null;

                                                // ������ �Ǵ� ���� ��ź���� �ٲ������!!
                                                moveBlock.elementType = ElementType.Balance;
                                                moveBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                isMake = true;
                                            }
                                            else
                                            {
                                                // OO��O
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

                                                    // ������ ���մϴ�!
                                                    DM.SetScore(col_0.BlockScore);
                                                    DM.SetScore(col_1.BlockScore);
                                                    DM.SetScore(col_2.BlockScore);

                                                    blocks[row - 1, col] = null;
                                                    blocks[row - 2, col] = null;
                                                    blocks[row + 1, col] = null;

                                                    // ������ �Ǵ� ���� ��ź���� �ٲ������!!
                                                    moveBlock.elementType = ElementType.Balance;
                                                    moveBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                    isMake = true;
                                                }
                                            }
                                        }
                                        break;

                                    case 1:
                                        // OOO��
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

                                            // ������ ���մϴ�!
                                            DM.SetScore(col_0.BlockScore);
                                            DM.SetScore(col_1.BlockScore);
                                            DM.SetScore(col_2.BlockScore);

                                            blocks[row - 1, col] = null;
                                            blocks[row - 2, col] = null;
                                            blocks[row - 3, col] = null;

                                            // ������ �Ǵ� ���� ��ź���� �ٲ������!!
                                            moveBlock.elementType = ElementType.Balance;
                                            moveBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                            isMake = true;
                                        }
                                        else
                                        {
                                            // OO��O
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

                                                // ������ ���մϴ�!
                                                DM.SetScore(col_0.BlockScore);
                                                DM.SetScore(col_1.BlockScore);
                                                DM.SetScore(col_2.BlockScore);

                                                blocks[row + 1, col] = null;
                                                blocks[row - 1, col] = null;
                                                blocks[row - 2, col] = null;

                                                // ������ �Ǵ� ���� ��ź���� �ٲ������!!
                                                moveBlock.elementType = ElementType.Balance;
                                                moveBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                isMake = true;
                                            }
                                            else
                                            {
                                                // O��OO
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

                                                    // ������ ���մϴ�!
                                                    DM.SetScore(col_0.BlockScore);
                                                    DM.SetScore(col_1.BlockScore);
                                                    DM.SetScore(col_2.BlockScore);

                                                    blocks[row + 1, col] = null;
                                                    blocks[row + 2, col] = null;
                                                    blocks[row - 1, col] = null;

                                                    // ������ �Ǵ� ���� ��ź���� �ٲ������!!
                                                    moveBlock.elementType = ElementType.Balance;
                                                    moveBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                    isMake = true;
                                                }
                                            }
                                        }
                                        break;

                                    case 2:
                                        // OOO��
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

                                            // ������ ���մϴ�!
                                            DM.SetScore(col_0.BlockScore);
                                            DM.SetScore(col_1.BlockScore);
                                            DM.SetScore(col_2.BlockScore);

                                            blocks[row - 1, col] = null;
                                            blocks[row - 2, col] = null;
                                            blocks[row - 3, col] = null;

                                            // ������ �Ǵ� ���� ��ź���� �ٲ������!!
                                            moveBlock.elementType = ElementType.Balance;
                                            moveBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                            isMake = true;
                                        }
                                        else
                                        {
                                            // OO��O
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

                                                // ������ ���մϴ�!
                                                DM.SetScore(col_0.BlockScore);
                                                DM.SetScore(col_1.BlockScore);
                                                DM.SetScore(col_2.BlockScore);

                                                blocks[row + 1, col] = null;
                                                blocks[row - 1, col] = null;
                                                blocks[row - 2, col] = null;

                                                // ������ �Ǵ� ���� ��ź���� �ٲ������!!
                                                moveBlock.elementType = ElementType.Balance;
                                                moveBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                isMake = true;
                                            }
                                        }
                                        break;

                                    case 3:
                                        // OOO��
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

                                            // ������ ���մϴ�!
                                            DM.SetScore(col_0.BlockScore);
                                            DM.SetScore(col_1.BlockScore);
                                            DM.SetScore(col_2.BlockScore);

                                            blocks[row - 1, col] = null;
                                            blocks[row - 2, col] = null;
                                            blocks[row - 3, col] = null;

                                            // ������ �Ǵ� ���� ��ź���� �ٲ������!!
                                            moveBlock.elementType = ElementType.Balance;
                                            moveBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                            isMake = true;
                                        }
                                        break;
                                }
                            }

                            /// ���⼭ Null
                            if (moveBlock.otherBlock != null)
                            {
                                // OtherBlock
                                if (blocks[row, col] == moveBlock.otherBlock)
                                {
                                    // Col
                                    switch (moveBlock.otherBlock.col)
                                    {
                                        case -3:
                                            // ��OOO
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

                                                // ������ ���մϴ�!
                                                DM.SetScore(col_0.BlockScore);
                                                DM.SetScore(col_1.BlockScore);
                                                DM.SetScore(col_2.BlockScore);

                                                blocks[row, col + 1] = null;
                                                blocks[row, col + 2] = null;
                                                blocks[row, col + 3] = null;

                                                // ������ �Ǵ� ���� ��ź���� �ٲ������!!
                                                moveBlock.otherBlock.elementType = ElementType.Balance;
                                                moveBlock.otherBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                isMake = true;
                                            }
                                            break;

                                        case -2:
                                            // ��OOO
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

                                                // ������ ���մϴ�!
                                                DM.SetScore(col_0.BlockScore);
                                                DM.SetScore(col_1.BlockScore);
                                                DM.SetScore(col_2.BlockScore);

                                                blocks[row, col + 1] = null;
                                                blocks[row, col + 2] = null;
                                                blocks[row, col + 3] = null;

                                                // ������ �Ǵ� ���� ��ź���� �ٲ������!!
                                                moveBlock.otherBlock.elementType = ElementType.Balance;
                                                moveBlock.otherBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                isMake = true;
                                            }
                                            else
                                            {
                                                // O��OO
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

                                                    // ������ ���մϴ�!
                                                    DM.SetScore(col_0.BlockScore);
                                                    DM.SetScore(col_1.BlockScore);
                                                    DM.SetScore(col_2.BlockScore);

                                                    blocks[row, col - 1] = null;
                                                    blocks[row, col + 1] = null;
                                                    blocks[row, col + 2] = null;

                                                    // ������ �Ǵ� ���� ��ź���� �ٲ������!!
                                                    moveBlock.otherBlock.elementType = ElementType.Balance;
                                                    moveBlock.otherBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                    isMake = true;
                                                }
                                            }
                                            break;

                                        case -1:
                                        case 0:
                                            // ��OOO
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

                                                // ������ ���մϴ�!
                                                DM.SetScore(col_0.BlockScore);
                                                DM.SetScore(col_1.BlockScore);
                                                DM.SetScore(col_2.BlockScore);

                                                blocks[row, col + 1] = null;
                                                blocks[row, col + 2] = null;
                                                blocks[row, col + 3] = null;

                                                // ������ �Ǵ� ���� ��ź���� �ٲ������!!
                                                moveBlock.otherBlock.elementType = ElementType.Balance;
                                                moveBlock.otherBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                isMake = true;
                                            }
                                            else
                                            {
                                                // O��OO
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

                                                    // ������ ���մϴ�!
                                                    DM.SetScore(col_0.BlockScore);
                                                    DM.SetScore(col_1.BlockScore);
                                                    DM.SetScore(col_2.BlockScore);

                                                    blocks[row, col - 1] = null;
                                                    blocks[row, col + 1] = null;
                                                    blocks[row, col + 2] = null;

                                                    // ������ �Ǵ� ���� ��ź���� �ٲ������!!
                                                    moveBlock.otherBlock.elementType = ElementType.Balance;
                                                    moveBlock.otherBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                    isMake = true;
                                                }
                                                else
                                                {
                                                    // OO��O
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

                                                        // ������ ���մϴ�!
                                                        DM.SetScore(col_0.BlockScore);
                                                        DM.SetScore(col_1.BlockScore);
                                                        DM.SetScore(col_2.BlockScore);

                                                        blocks[row, col - 1] = null;
                                                        blocks[row, col - 2] = null;
                                                        blocks[row, col + 1] = null;

                                                        // ������ �Ǵ� ���� ��ź���� �ٲ������!!
                                                        moveBlock.otherBlock.elementType = ElementType.Balance;
                                                        moveBlock.otherBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                        isMake = true;
                                                    }
                                                }
                                            }
                                            break;

                                        case 1:
                                            // OOO��
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

                                                // ������ ���մϴ�!
                                                DM.SetScore(col_0.BlockScore);
                                                DM.SetScore(col_1.BlockScore);
                                                DM.SetScore(col_2.BlockScore);

                                                blocks[row, col - 1] = null;
                                                blocks[row, col - 2] = null;
                                                blocks[row, col - 3] = null;

                                                // ������ �Ǵ� ���� ��ź���� �ٲ������!!
                                                moveBlock.otherBlock.elementType = ElementType.Balance;
                                                moveBlock.otherBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                isMake = true;
                                            }
                                            else
                                            {
                                                // OO��O
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

                                                    // ������ ���մϴ�!
                                                    DM.SetScore(col_0.BlockScore);
                                                    DM.SetScore(col_1.BlockScore);
                                                    DM.SetScore(col_2.BlockScore);

                                                    // ������ �Ǵ� ���� ��ź���� �ٲ������!!
                                                    moveBlock.otherBlock.elementType = ElementType.Balance;
                                                    moveBlock.otherBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                    isMake = true;
                                                }
                                                else
                                                {
                                                    // O��OO
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

                                                        // ������ ���մϴ�!
                                                        DM.SetScore(col_0.BlockScore);
                                                        DM.SetScore(col_1.BlockScore);
                                                        DM.SetScore(col_2.BlockScore);

                                                        blocks[row, col - 1] = null;
                                                        blocks[row, col + 1] = null;
                                                        blocks[row, col + 2] = null;

                                                        // ������ �Ǵ� ���� ��ź���� �ٲ������!!
                                                        moveBlock.otherBlock.elementType = ElementType.Balance;
                                                        moveBlock.otherBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                        isMake = true;
                                                    }
                                                }
                                            }
                                            break;

                                        case 2:
                                            // OOO��
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

                                                // ������ ���մϴ�!
                                                DM.SetScore(col_0.BlockScore);
                                                DM.SetScore(col_1.BlockScore);
                                                DM.SetScore(col_2.BlockScore);

                                                blocks[row, col - 1] = null;
                                                blocks[row, col - 2] = null;
                                                blocks[row, col - 3] = null;

                                                // ������ �Ǵ� ���� ��ź���� �ٲ������!!
                                                moveBlock.otherBlock.elementType = ElementType.Balance;
                                                moveBlock.otherBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                isMake = true;
                                            }
                                            else
                                            {
                                                // OO��O
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

                                                    // ������ ���մϴ�!
                                                    DM.SetScore(col_0.BlockScore);
                                                    DM.SetScore(col_1.BlockScore);
                                                    DM.SetScore(col_2.BlockScore);

                                                    blocks[row, col - 1] = null;
                                                    blocks[row, col - 2] = null;
                                                    blocks[row, col + 1] = null;

                                                    // ������ �Ǵ� ���� ��ź���� �ٲ������!!
                                                    moveBlock.otherBlock.elementType = ElementType.Balance;
                                                    moveBlock.otherBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                    isMake = true;
                                                }
                                            }
                                            break;

                                        case 3:
                                            // OOO��
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

                                                // ������ ���մϴ�!
                                                DM.SetScore(col_0.BlockScore);
                                                DM.SetScore(col_1.BlockScore);
                                                DM.SetScore(col_2.BlockScore);

                                                blocks[row, col - 1] = null;
                                                blocks[row, col - 2] = null;
                                                blocks[row, col - 3] = null;

                                                // ������ �Ǵ� ���� ��ź���� �ٲ������!!
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
                                            // ��OOO
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

                                                // ������ ���մϴ�!
                                                DM.SetScore(col_0.BlockScore);
                                                DM.SetScore(col_1.BlockScore);
                                                DM.SetScore(col_2.BlockScore);

                                                blocks[row + 1, col] = null;
                                                blocks[row + 2, col] = null;
                                                blocks[row + 3, col] = null;

                                                // ������ �Ǵ� ���� ��ź���� �ٲ������!!
                                                moveBlock.otherBlock.elementType = ElementType.Balance;
                                                moveBlock.otherBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                isMake = true;
                                            }
                                            break;

                                        case -2:
                                            // ��OOO
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

                                                // ������ ���մϴ�!
                                                DM.SetScore(col_0.BlockScore);
                                                DM.SetScore(col_1.BlockScore);
                                                DM.SetScore(col_2.BlockScore);

                                                blocks[row + 1, col] = null;
                                                blocks[row + 2, col] = null;
                                                blocks[row + 3, col] = null;

                                                // ������ �Ǵ� ���� ��ź���� �ٲ������!!
                                                moveBlock.otherBlock.elementType = ElementType.Balance;
                                                moveBlock.otherBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                isMake = true;
                                            }
                                            else
                                            {
                                                // O��OO
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

                                                    // ������ ���մϴ�!
                                                    DM.SetScore(col_0.BlockScore);
                                                    DM.SetScore(col_1.BlockScore);
                                                    DM.SetScore(col_2.BlockScore);

                                                    blocks[row - 1, col] = null;
                                                    blocks[row + 1, col] = null;
                                                    blocks[row + 2, col] = null;

                                                    // ������ �Ǵ� ���� ��ź���� �ٲ������!!
                                                    moveBlock.otherBlock.elementType = ElementType.Balance;
                                                    moveBlock.otherBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                    isMake = true;
                                                }
                                            }
                                            break;

                                        case -1:
                                        case 0:
                                            // ��OOO
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

                                                // ������ ���մϴ�!
                                                DM.SetScore(col_0.BlockScore);
                                                DM.SetScore(col_1.BlockScore);
                                                DM.SetScore(col_2.BlockScore);

                                                blocks[row + 1, col] = null;
                                                blocks[row + 2, col] = null;
                                                blocks[row + 3, col] = null;

                                                // ������ �Ǵ� ���� ��ź���� �ٲ������!!
                                                moveBlock.otherBlock.elementType = ElementType.Balance;
                                                moveBlock.otherBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                isMake = true;
                                            }
                                            else
                                            {
                                                // O��OO
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

                                                    // ������ ���մϴ�!
                                                    DM.SetScore(col_0.BlockScore);
                                                    DM.SetScore(col_1.BlockScore);
                                                    DM.SetScore(col_2.BlockScore);

                                                    blocks[row - 1, col] = null;
                                                    blocks[row + 1, col] = null;
                                                    blocks[row + 2, col] = null;

                                                    // ������ �Ǵ� ���� ��ź���� �ٲ������!!
                                                    moveBlock.otherBlock.elementType = ElementType.Balance;
                                                    moveBlock.otherBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                    isMake = true;
                                                }
                                                else
                                                {
                                                    // OO��O
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

                                                        // ������ ���մϴ�!
                                                        DM.SetScore(col_0.BlockScore);
                                                        DM.SetScore(col_1.BlockScore);
                                                        DM.SetScore(col_2.BlockScore);

                                                        blocks[row - 1, col] = null;
                                                        blocks[row - 2, col] = null;
                                                        blocks[row + 1, col] = null;

                                                        // ������ �Ǵ� ���� ��ź���� �ٲ������!!
                                                        moveBlock.otherBlock.elementType = ElementType.Balance;
                                                        moveBlock.otherBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                        isMake = true;
                                                    }
                                                }
                                            }
                                            break;

                                        case 1:
                                            // OOO��
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

                                                // ������ ���մϴ�!
                                                DM.SetScore(col_0.BlockScore);
                                                DM.SetScore(col_1.BlockScore);
                                                DM.SetScore(col_2.BlockScore);

                                                blocks[row - 1, col] = null;
                                                blocks[row - 2, col] = null;
                                                blocks[row - 3, col] = null;

                                                // ������ �Ǵ� ���� ��ź���� �ٲ������!!
                                                moveBlock.otherBlock.elementType = ElementType.Balance;
                                                moveBlock.otherBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                isMake = true;
                                            }
                                            else
                                            {
                                                // OO��O
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

                                                    // ������ ���մϴ�!
                                                    DM.SetScore(col_0.BlockScore);
                                                    DM.SetScore(col_1.BlockScore);
                                                    DM.SetScore(col_2.BlockScore);

                                                    blocks[row + 1, col] = null;
                                                    blocks[row - 1, col] = null;
                                                    blocks[row - 2, col] = null;

                                                    // ������ �Ǵ� ���� ��ź���� �ٲ������!!
                                                    moveBlock.otherBlock.elementType = ElementType.Balance;
                                                    moveBlock.otherBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                    isMake = true;
                                                }
                                                else
                                                {
                                                    // O��OO
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

                                                        // ������ ���մϴ�!
                                                        DM.SetScore(col_0.BlockScore);
                                                        DM.SetScore(col_1.BlockScore);
                                                        DM.SetScore(col_2.BlockScore);

                                                        blocks[row + 1, col] = null;
                                                        blocks[row + 2, col] = null;
                                                        blocks[row - 1, col] = null;

                                                        // ������ �Ǵ� ���� ��ź���� �ٲ������!!
                                                        moveBlock.otherBlock.elementType = ElementType.Balance;
                                                        moveBlock.otherBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                        isMake = true;
                                                    }
                                                }
                                            }
                                            break;

                                        case 2:
                                            // OOO��
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

                                                // ������ ���մϴ�!
                                                DM.SetScore(col_0.BlockScore);
                                                DM.SetScore(col_1.BlockScore);
                                                DM.SetScore(col_2.BlockScore);

                                                blocks[row - 1, col] = null;
                                                blocks[row - 2, col] = null;
                                                blocks[row - 3, col] = null;

                                                // ������ �Ǵ� ���� ��ź���� �ٲ������!!
                                                moveBlock.otherBlock.elementType = ElementType.Balance;
                                                moveBlock.otherBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                isMake = true;
                                            }
                                            else
                                            {
                                                // OO��O
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

                                                    // ������ ���մϴ�!
                                                    DM.SetScore(col_0.BlockScore);
                                                    DM.SetScore(col_1.BlockScore);
                                                    DM.SetScore(col_2.BlockScore);

                                                    blocks[row + 1, col] = null;
                                                    blocks[row - 1, col] = null;
                                                    blocks[row - 2, col] = null;

                                                    // ������ �Ǵ� ���� ��ź���� �ٲ������!!
                                                    moveBlock.otherBlock.elementType = ElementType.Balance;
                                                    moveBlock.otherBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                    isMake = true;
                                                }
                                            }
                                            break;

                                        case 3:
                                            // OOO��
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

                                                // ������ ���մϴ�!
                                                DM.SetScore(col_0.BlockScore);
                                                DM.SetScore(col_1.BlockScore);
                                                DM.SetScore(col_2.BlockScore);

                                                blocks[row - 1, col] = null;
                                                blocks[row - 2, col] = null;
                                                blocks[row - 3, col] = null;

                                                // ������ �Ǵ� ���� ��ź���� �ٲ������!!
                                                moveBlock.otherBlock.elementType = ElementType.Balance;
                                                moveBlock.otherBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                                isMake = true;
                                            }
                                            break;
                                    }
                                }
                            }

                            // �ı� ���� �۾�
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

                                isMake = false;

                                BlockSort();
                            }
                        }
                    }
                }

            }

            // 4X4 - Ư�� ��Ȳ
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    // Col
                    if (blocks[row, col] != null)
                    {
                        checkBlock = blocks[row, col];

                        if (checkBlock.col <= 0)
                        {
                            col_0 = blocks[row, col + 1];
                            col_1 = blocks[row, col + 2];
                            col_2 = blocks[row, col + 3];

                            // 4X4 Ư�� ��Ȳ ������ 1ź
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
                                        // �� �̵�
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

                                        // �� ����
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
                                                // �� �̵�
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

                                                // �� ����
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

                                //if (isMake)
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

                                    /// Test
                                    yield return new WaitForSeconds(.3f);

                                    isMake = false;

                                    // ��� ����
                                    BlockSort();
                                }
                            }

                            // 4X4 Ư�� ��Ȳ ������ 2ź
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

                                //if (isMake)
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

                                    /// Test
                                    yield return new WaitForSeconds(.3f);

                                    isMake = false;

                                    // ��� ����
                                    BlockSort();
                                }
                            }
                        }
                    }

                    // Row
                    if (blocks[row, col] != null)
                    {
                        checkBlock = blocks[row, col];

                        if (checkBlock.row <= 0)
                        {
                            row_0 = blocks[row + 1, col];
                            row_1 = blocks[row + 2, col];
                            row_2 = blocks[row + 3, col];

                            // 4X4 Ư�� ��Ȳ
                            if (checkBlock.col <= 0 && (row_0 != null && row_1 != null && row_2 != null))
                            {
                                /*
                                 * O
                                 * O
                                 * O O O O
                                 * O
                                 */
                                checkBlock = blocks[row + 1, col];
                                row_0 = blocks[row, col];

                                col_0 = blocks[row + 1, col + 1];
                                col_1 = blocks[row + 1, col + 2];
                                col_2 = blocks[row + 1, col + 3];

                                if ((checkBlock.elementType == row_0.elementType && checkBlock.elementType == row_1.elementType && checkBlock.elementType == row_2.elementType) &&
                                    (checkBlock.elementType == col_0.elementType && checkBlock.elementType == col_1.elementType && checkBlock.elementType == col_2.elementType))
                                {
                                    // �� �̵�
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

                                    // �� ����
                                    blocks[row, col] = null;
                                    blocks[row + 2, col] = null;
                                    blocks[row + 3, col] = null;

                                    blocks[row + 1, col + 1] = null;
                                    blocks[row + 1, col + 2] = null;
                                    blocks[row + 1, col + 3] = null;

                                    checkBlock.elementType = ElementType.Balance;
                                    checkBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                    isMake = true;
                                }
                                else
                                {
                                    /*
                                     * O
                                     * O O O O
                                     * O
                                     * O
                                     */
                                    checkBlock = blocks[row + 2, col];
                                    row_1 = blocks[row + 1, col];

                                    col_0 = blocks[row + 2, col + 1];
                                    col_1 = blocks[row + 2, col + 2];
                                    col_2 = blocks[row + 2, col + 3];

                                    if ((checkBlock.elementType == row_0.elementType && checkBlock.elementType == row_1.elementType && checkBlock.elementType == row_2.elementType) &&
                                        (checkBlock.elementType == col_0.elementType && checkBlock.elementType == col_1.elementType && checkBlock.elementType == col_2.elementType))
                                    {
                                        // �� �̵�
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

                                        // �� ����
                                        blocks[row, col] = null;
                                        blocks[row + 1, col] = null;
                                        blocks[row + 3, col] = null;

                                        blocks[row + 2, col + 1] = null;
                                        blocks[row + 2, col + 2] = null;
                                        blocks[row + 2, col + 3] = null;

                                        checkBlock.elementType = ElementType.Balance;
                                        checkBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                        isMake = true;
                                    }
                                }

                                //if (isMake)
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

                                    isMake = false;

                                    BlockSort();
                                }
                            }

                            // 4X4 Ư�� ��Ȳ
                            if (checkBlock.col >= 0 && (row_0 != null && row_1 != null && row_2 != null))
                            {
                                /*
                                 *       O
                                 *       O
                                 * O O O O
                                 *       O
                                 */
                                checkBlock = blocks[row + 1, col];
                                row_0 = blocks[row, col];

                                col_0 = blocks[row + 1, col - 1];
                                col_1 = blocks[row + 1, col - 2];
                                col_2 = blocks[row + 1, col - 3];

                                if ((checkBlock.elementType == row_0.elementType && checkBlock.elementType == row_1.elementType && checkBlock.elementType == row_2.elementType) &&
                                    (checkBlock.elementType == col_0.elementType && checkBlock.elementType == col_1.elementType && checkBlock.elementType == col_2.elementType))
                                {
                                    // �� �̵�
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

                                    // �� ����
                                    blocks[row, col] = null;
                                    blocks[row + 2, col] = null;
                                    blocks[row + 3, col] = null;

                                    blocks[row + 1, col - 1] = null;
                                    blocks[row + 1, col - 2] = null;
                                    blocks[row + 1, col - 3] = null;

                                    checkBlock.elementType = ElementType.Balance;
                                    checkBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                    isMake = true;
                                }
                                else
                                {
                                    /*
                                     *       O
                                     * O O O O
                                     *       O
                                     *       O
                                     */
                                    checkBlock = blocks[row + 2, col];
                                    row_1 = blocks[row + 1, col];

                                    col_0 = blocks[row + 2, col - 1];
                                    col_1 = blocks[row + 2, col - 2];
                                    col_2 = blocks[row + 2, col - 3];

                                    if ((checkBlock.elementType == row_0.elementType && checkBlock.elementType == row_1.elementType && checkBlock.elementType == row_2.elementType) &&
                                        (checkBlock.elementType == col_0.elementType && checkBlock.elementType == col_1.elementType && checkBlock.elementType == col_2.elementType))
                                    {
                                        // �� �̵�
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

                                        // �� ����
                                        blocks[row, col] = null;
                                        blocks[row + 1, col] = null;
                                        blocks[row + 3, col] = null;

                                        blocks[row + 2, col - 1] = null;
                                        blocks[row + 2, col - 2] = null;
                                        blocks[row + 2, col - 3] = null;

                                        checkBlock.elementType = ElementType.Balance;
                                        checkBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                        isMake = true;

                                    }
                                }

                                //if (isMake)
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

                                    /// Test
                                    yield return new WaitForSeconds(.3f);

                                    isMake = false;

                                    // ��� ����
                                    BlockSort();
                                }
                            }
                        }
                    }
                }
            }

            // 4X4 - �Ϲ� ��Ȳ
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

                            if (col_0 != null && col_1 != null && col_2 != null)
                            {
                                if (checkBlock.elementType == col_0.elementType &&
                                    checkBlock.elementType == col_1.elementType &&
                                    checkBlock.elementType == col_2.elementType)
                                {
                                    col_0.col = checkBlock.col;
                                    col_1.col = checkBlock.col;
                                    col_2.col = checkBlock.col;

                                    yield return new WaitForSeconds(.3f);

                                    // Ǯ�� ����
                                    blockPool.ReturnPoolableObject(col_0);
                                    blockPool.ReturnPoolableObject(col_1);
                                    blockPool.ReturnPoolableObject(col_2);

                                    // ������ ���մϴ�!
                                    DM.SetScore(col_0.BlockScore);
                                    DM.SetScore(col_1.BlockScore);
                                    DM.SetScore(col_2.BlockScore);

                                    // �� ����ҿ��� ����
                                    blocks[row, col + 1] = null;
                                    blocks[row, col + 2] = null;
                                    blocks[row, col + 3] = null;

                                    // ������ �Ǵ� ���� ��ź���� �ٲ������!!
                                    checkBlock.elementType = ElementType.Balance;
                                    checkBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                    isMake = true;
                                }
                            }

                            //if (isMake)
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

                                /// Test
                                yield return new WaitForSeconds(.3f);

                                isMake = false;

                                // ��� ����
                                BlockSort();
                            }
                        }
                        else if (checkBlock.row <= 0)
                        {
                            row_0 = blocks[row + 1, col];
                            row_1 = blocks[row + 2, col];
                            row_2 = blocks[row + 3, col];

                            if (row_0 != null && row_1 != null && row_2 != null)
                            {
                                if (checkBlock.elementType == row_0.elementType &&
                                    checkBlock.elementType == row_1.elementType &&
                                    checkBlock.elementType == row_2.elementType)
                                {
                                    yield return new WaitForSeconds(.3f);

                                    // Ǯ�� ����
                                    blockPool.ReturnPoolableObject(row_0);
                                    blockPool.ReturnPoolableObject(row_1);
                                    blockPool.ReturnPoolableObject(row_2);

                                    // ������ ���մϴ�!
                                    DM.SetScore(row_0.BlockScore);
                                    DM.SetScore(row_1.BlockScore);
                                    DM.SetScore(row_2.BlockScore);

                                    // �� ����ҿ��� ����
                                    blocks[row + 1, col] = null;
                                    blocks[row + 2, col] = null;
                                    blocks[row + 3, col] = null;

                                    // ������ �Ǵ� ���� ��ź���� �ٲ������!!
                                    checkBlock.elementType = ElementType.Balance;
                                    checkBlock.spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Balance.ToString());

                                    isMake = true;
                                }
                            }

                            //if (isMake)
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

                                /// Test
                                yield return new WaitForSeconds(.3f);

                                isMake = false;

                                // ��� ����
                                BlockSort();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// ��ź ����� ���� �ڷ�ƾ
        /// </summary>
        /// <returns></returns>
        private IEnumerator BoomFun(int boomCol)
        {
            /// ��ƼŬ ����
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
                            // ���� ������Ʈ
                            DataManager.Instance.SetScore(blocks[row, col].BlockScore);

                            // ��ų ������ ������Ʈ
                            uiElement.SetGauge(blocks[row, col].ElementValue);

                            // �� �ı�
                            blockPool.ReturnPoolableObject(blocks[row, col]);

                            // ���� Col�� �ִ� �� �ı�
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
                            // ���� ������Ʈ
                            DataManager.Instance.SetScore(blocks[row, col].BlockScore);

                            // ��ų ������ ������Ʈ
                            uiElement.SetGauge(blocks[row, col].ElementValue);

                            // �� �ı�
                            blockPool.ReturnPoolableObject(blocks[row, col]);

                            // ���� Col�� �ִ� �� �ı�
                            blocks[row, col] = null;
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

            int newCol = -3;
            int newRow = 3;

            // �� ���� �� �̵�
            for (int col = 0; col < width; col++)
            {
                for (int row = 0; row < height; row++)
                {
                    if (blocks[row, col] == null)
                    {
                        Block newBlock = blockPool.GetPoolableObject(obj => obj.CanRecycle);

                        // �θ� ����
                        newBlock.transform.SetParent(this.transform);

                        // ��ġ�� ����
                        newBlock.transform.localPosition = new Vector3(newCol, row + 4, 0);

                        // ȸ���� ����
                        newBlock.transform.localRotation = Quaternion.identity;

                        // ����� ����
                        newBlock.transform.localScale = new Vector3(.19f, .19f, .19f);

                        // Ȱ��ȭ
                        newBlock.gameObject.SetActive(true);

                        // �� �ʱ�ȭ
                        newBlock.Initialize(newCol, row + 4);

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

            // ��� ����
            BlockSort();

            if (MatchCheck())
            {
                // �� ��Ī �����
                StartCoroutine(BlockMatch());
            }
            else
            {

                // GameState -> Play
                GM.SetGameState(GameState.Play);
            }
        }
    }
}