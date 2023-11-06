using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XR_3MatchGame.Util;
using XR_3MatchGame_InGame;
using XR_3MatchGame_Resource;
using XR_3MatchGame_Util;

namespace XR_3MatchGame_Object
{
    public class Block : MonoBehaviour, IPoolableObject
    {
        public bool CanRecycle { get; set; } = true;

        public int BlockScore
        {
            get
            {
                int blockScore = 5;
                return blockScore;
            }
        }

        public SpriteRenderer spriteRenderer;

        public int col;     // ���� ���� X ��
        public int row;     // ���� ���� Y ��

        public int targetCol;       // ��� ���� X ��
        public int targetRow;       // ��� ���� Y ��

        public Block topBlock;      // ���� �����ϴ� ��
        public Block bottomBlock;   // �Ʒ��� �����ϴ� ��
        public Block leftBlock;     // ���ʿ� �����ϴ� ��
        public Block rightBlock;    // �����ʿ� �����ϴ� ��

        private float swipeAngle = 0;           // �������� ����

        private Vector2 firstTouchPosition;     // ���콺 Ŭ�� ����
        private Vector2 finalTouchPosition;     // ���콺 Ŭ���� �������� ����
        private Vector2 tempPosition;

        private Block otherBlock;               // ���� ���� �ڸ��� �ٲ� ��
        private GameManager GM;
        private Board board;                    // ���� �����ϴ� ����

        public BlockType blockType = BlockType.None;        // ���� ���� Ÿ��
        public BoomType boomType = BoomType.None;           // ���࿡ ���� ��ź�̶�� � ��ź������ ���� Ÿ��
        private SwipeDir swipeDir = SwipeDir.None;

        [Header("Test")]
        public BlockType Top_T = BlockType.None;
        public BlockType Bottom_T = BlockType.None;
        public BlockType Left_T = BlockType.None;
        public BlockType Right_T = BlockType.None;

        /// <summary>
        /// �� �ʱ� ���� �޼���
        /// </summary>
        /// <param name="col">X ��</param>
        /// <param name="row">Y ��</param>
        public void Initialize(int col, int row)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();

            GM = GameManager.Instance;
            board = GM.Board;

            var blockNum = UnityEngine.Random.Range(1, 7);

            // �������� ���� ��������Ʈ�� ����
            switch (blockNum)
            {
                case (int)BlockType.Blue:
                    spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, BlockType.Blue.ToString());
                    blockType = BlockType.Blue;
                    break;

                case (int)BlockType.Cream:
                    spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, BlockType.Cream.ToString());
                    blockType = BlockType.Cream;
                    break;

                case (int)BlockType.DarkBlue:
                    spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, BlockType.DarkBlue.ToString());
                    blockType = BlockType.DarkBlue;
                    break;

                case (int)BlockType.Green:
                    spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, BlockType.Green.ToString());
                    blockType = BlockType.Green;
                    break;

                case (int)BlockType.Pink:
                    spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, BlockType.Pink.ToString());
                    blockType = BlockType.Pink;
                    break;

                case (int)BlockType.Purple:
                    spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, BlockType.Purple.ToString());
                    blockType = BlockType.Purple;
                    break;
            }

            this.col = col;
            this.row = row;

            targetCol = col;
            targetRow = row;
        }

        private void Update()
        {
            targetCol = col;
            targetRow = row;

            BlockSwipe();
        }

        private void OnMouseDown()
        {
            // ���콺 Ŭ�� ��ġ ����
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        private void OnMouseUp()
        {
            // ���콺 Ŭ�� ���� ��ġ ����
            finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }

        /// <summary>
        /// ���콺 �巡�� ������ ����ϴ� �޼���
        /// </summary>
        private void CalculateAngle()
        {
            // üũ�� �϶� �Է� ����
            if (GM.GameState == GameState.Checking)
            {
                return;
            }

            // ���콺 �巡�� ������ ����մϴ�
            swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y,
                finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;

            BlockMove();
        }

        /// <summary>
        /// ���� ���������� üũ�ϴ� �޼���
        /// </summary>
        private void BlockSwipe()
        {
            if (Mathf.Abs(targetCol - transform.position.x) > .1f)
            {
                tempPosition = new Vector2(targetCol, transform.position.y);
                transform.position = Vector2.Lerp(transform.position, tempPosition, .3f);
            }
            else
            {
                tempPosition = new Vector2(targetCol, transform.position.y);
                transform.position = tempPosition;
            }

            if (Mathf.Abs(targetRow - transform.position.y) > .1f)
            {
                tempPosition = new Vector2(transform.position.x, targetRow);
                transform.position = Vector2.Lerp(transform.position, tempPosition, .3f);
            }
            else
            {
                tempPosition = new Vector2(transform.position.x, targetRow);
                transform.position = tempPosition;
            }
        }

        /// <summary>
        /// ����� ������ �̿��ؼ� ���� �̵���Ű�� �޼���
        /// </summary>
        private void BlockMove()
        {
            GM.GameStateUpdate(GameState.Checking);

            // Top
            if ((swipeAngle > 45 && swipeAngle <= 135) && row < GM.BoardSize.y)
            {
                for (int i = 0; i < GM.blocks.Count; i++)
                {
                    if (GM.blocks[i].col == col &&
                        GM.blocks[i].row == row + 1)
                    {
                        // ���� �̵��̹Ƿ� ��ǥ ���� -1 �̵�
                        // ���� �̵��̹Ƿ� �̵� ���� +1 �̵�
                        otherBlock = GM.blocks[i];
                        otherBlock.row -= 1;
                        row += 1;

                        swipeDir = SwipeDir.Top;
                        StartCoroutine(BlockCheck());
                        return;
                    }
                }
            }
            // Bottom
            else if ((swipeAngle < -45 && swipeAngle >= -135) && row > 0)
            {
                for (int i = 0; i < GM.blocks.Count; i++)
                {
                    if (GM.blocks[i].col == col &&
                        GM.blocks[i].row == row - 1)
                    {
                        // �Ʒ��� �̵��̹Ƿ� ��ǥ ���� + 1 �̵�
                        // �Ʒ��� �̵��̹Ƿ� �̵� ���� - 1 �̵�
                        otherBlock = GM.blocks[i];
                        otherBlock.row += 1;
                        row -= 1;

                        swipeDir = SwipeDir.Bottom;
                        StartCoroutine(BlockCheck());
                        return;
                    }
                }
            }
            // Left
            else if ((swipeAngle > 135 || swipeAngle <= -135) && col > 0)
            {
                for (int i = 0; i < GM.blocks.Count; i++)
                {
                    if (GM.blocks[i].col == col - 1 &&
                        GM.blocks[i].row == row)
                    {
                        // ���� �̵��̹Ƿ� ��ǥ ���� + 1 �̵�
                        // ���� �̵��̹Ƿ� �̵� ���� - 1 �̵�
                        otherBlock = GM.blocks[i];
                        otherBlock.col += 1;
                        col -= 1;

                        swipeDir = SwipeDir.Left;
                        StartCoroutine(BlockCheck());
                        return;
                    }
                }
            }
            // Right
            else if ((swipeAngle > -45 && swipeAngle <= 45) && col < GM.BoardSize.x)
            {
                for (int i = 0; i < GM.blocks.Count; i++)
                {
                    if (GM.blocks[i].col == col + 1 &&
                        GM.blocks[i].row == row)
                    {
                        // ������ �̵��̹Ƿ� ��ǥ ���� - 1 �̵�
                        // ������ �̵��̹Ƿ� �̵� ���� + 1 �̵�
                        otherBlock = GM.blocks[i];
                        otherBlock.col -= 1;
                        col += 1;

                        swipeDir = SwipeDir.Right;
                        StartCoroutine(BlockCheck());
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// �� ��Ī�� üũ�ϴ� �޼���
        /// </summary>
        /// <returns></returns>
        private IEnumerator BlockCheck()
        {
            board.BlockUpdate();

            var blocks = GM.blocks;

            // ���� �� ����
            var count_T = 0;
            var count_B = 0;
            var count_M = 0;
            var count_L = 0;
            var count_R = 0;

            // ������ �ű� ���� ���� ����
            switch (swipeDir)
            {
                case SwipeDir.Top:
                    for (int i = 0; i < blocks.Count; i++)
                    {
                        // Top
                        if ((row + 1 == blocks[i].row || row + 2 == blocks[i].row) && col == blocks[i].col)
                        {
                            if (blocks[i].blockType == blockType)
                            {
                                count_T++;
                            }
                        }

                        // Middle
                        if ((col - 1 == blocks[i].col || col + 1 == blocks[i].col) && row == blocks[i].row)
                        {
                            if (blocks[i].blockType == blockType)
                            {
                                count_M++;
                            }
                        }

                        // Left
                        if ((col - 1 == blocks[i].col || col - 2 == blocks[i].col) && row == blocks[i].row)
                        {
                            if (blocks[i].blockType == blockType)
                            {
                                count_L++;
                            }
                        }

                        // Right
                        if ((col + 1 == blocks[i].col || col + 2 == blocks[i].col) && row == blocks[i].row)
                        {
                            if (blocks[i].blockType == blockType)
                            {
                                count_R++;
                            }
                        }
                    }

                    // �̵��� ��ġ�� ��Ī�Ǵ� ���� ���ٸ�
                    if (count_T < 2 && count_M < 2 && count_L < 2 && count_R < 2)
                    {
                        yield return new WaitForSeconds(.2f);

                        // OtherBlock�� ��Ī ���� �Ǵ�
                        if (OtherBlockCheck(count_T, count_B, count_M, count_L, count_R, blocks))
                        {
                            // �� ����ġ
                            otherBlock.row += 1;
                            row -= 1;

                            yield return new WaitForSeconds(.4f);

                            GM.GameStateUpdate(GameState.Play);
                        }
                        else
                        {
                            // �� ��Ī ����
                            GM.isStart = true;
                        }
                    }
                    else
                    {
                        // �� ��Ī ����
                        GM.isStart = true;
                    }
                    break;

                case SwipeDir.Bottom:
                    for (int i = 0; i < blocks.Count; i++)
                    {
                        // Bottom
                        if ((row - 1 == blocks[i].row || row - 2 == blocks[i].row) && col == blocks[i].col)
                        {
                            if (blocks[i].blockType == blockType)
                            {
                                count_B++;
                            }
                        }

                        // Middle
                        if ((col - 1 == blocks[i].col || col + 1 == blocks[i].col) && row == blocks[i].row)
                        {
                            if (blocks[i].blockType == blockType)
                            {
                                count_M++;
                            }
                        }

                        // Left
                        if ((col - 1 == blocks[i].col || col - 2 == blocks[i].col) && row == blocks[i].row)
                        {
                            if (blocks[i].blockType == blockType)
                            {
                                count_L++;
                            }
                        }

                        //Right
                        if ((col + 1 == blocks[i].col || col + 2 == blocks[i].col) && row == blocks[i].row)
                        {
                            if (blocks[i].blockType == blockType)
                            {
                                count_R++;
                            }
                        }
                    }

                    if (count_B < 2 && count_M < 2 && count_L < 2 && count_R < 2)
                    {
                        yield return new WaitForSeconds(.2f);

                        // OtherBlock ��Ī ���� �Ǵ�
                        if (OtherBlockCheck(count_T, count_B, count_M, count_L, count_R, blocks))
                        {
                            otherBlock.row -= 1;
                            row += 1;

                            yield return new WaitForSeconds(.4f);

                            GM.GameStateUpdate(GameState.Play);
                        }
                        else
                        {
                            // OtherBlock�� ��Ī�Ǵ� ���� ����
                            GM.isStart = true;
                        }
                    }
                    else
                    {
                        // ���� ���� ��Ī�Ǵ� ���� ����
                        GM.isStart = true;
                    }
                    break;

                case SwipeDir.Left:
                    for (int i = 0; i < blocks.Count; i++)
                    {
                        // Top
                        if ((row + 1 == blocks[i].row || row + 2 == blocks[i].row) && col == blocks[i].col)
                        {
                            if (blocks[i].blockType == blockType)
                            {
                                count_T++;
                            }
                        }

                        // Bottom
                        if ((row - 1 == blocks[i].row || row - 2 == blocks[i].row) && col == blocks[i].col)
                        {
                            if (blocks[i].blockType == blockType)
                            {
                                count_B++;
                            }
                        }

                        // Middle
                        if ((row - 1 == blocks[i].row || row + 1 == blocks[i].row) && col == blocks[i].col)
                        {
                            if (blocks[i].blockType == blockType)
                            {
                                count_M++;
                            }
                        }

                        // Left
                        if ((col - 1 == blocks[i].col || col - 2 == blocks[i].col) && row == blocks[i].row)
                        {
                            if (blocks[i].blockType == blockType)
                            {
                                count_L++;
                            }
                        }
                    }

                    if (count_T < 2 && count_B < 2 && count_M < 2 && count_L < 2)
                    {
                        yield return new WaitForSeconds(.2f);

                        // OtherBlock�� ��Ī�Ǵ� ���� �ִ��� Ȯ��
                        if (OtherBlockCheck(count_T, count_B, count_M, count_L, count_R, blocks))
                        {
                            otherBlock.col -= 1;
                            col += 1;

                            yield return new WaitForSeconds(.4f);

                            GM.GameStateUpdate(GameState.Play);
                        }
                        else
                        {
                            // OtherBlock�� ��Ī�Ǵ� ���� ����
                            GM.isStart = true;
                        }
                    }
                    else
                    {
                        // ���� ���� ��Ī�Ǵ� ���� ����
                        GM.isStart = true;
                    }
                    break;

                case SwipeDir.Right:
                    for (int i = 0; i < blocks.Count; i++)
                    {
                        // Top
                        if ((row + 1 == blocks[i].row || row + 2 == blocks[i].row) && col == blocks[i].col)
                        {
                            if (blocks[i].blockType == blockType)
                            {
                                count_T++;
                            }
                        }

                        // Bottom
                        if ((row - 1 == blocks[i].row || row - 2 == blocks[i].row) && col == blocks[i].col)
                        {
                            if (blocks[i].blockType == blockType)
                            {
                                count_B++;
                            }
                        }

                        // Middle
                        if ((row - 1 == blocks[i].row || row + 1 == blocks[i].row) && col == blocks[i].col)
                        {
                            if (blocks[i].blockType == blockType)
                            {
                                count_M++;
                            }
                        }

                        // Right
                        if ((col + 1 == blocks[i].col || col + 2 == blocks[i].col) && row == blocks[i].row)
                        {
                            if (blocks[i].blockType == blockType)
                            {
                                count_R++;
                            }
                        }
                    }

                    if (count_T < 2 && count_B < 2 && count_M < 2 && count_R < 2)
                    {
                        yield return new WaitForSeconds(.2f);

                        // OtherBlock ��Ī ���� �Ǵ�
                        if (OtherBlockCheck(count_T, count_B, count_M, count_L, count_R, blocks))
                        {
                            otherBlock.col += 1;
                            col -= 1;

                            yield return new WaitForSeconds(.4f);

                            GM.GameStateUpdate(GameState.Play);
                        }
                        else
                        {
                            // OtherBlock ��Ī ����
                            GM.isStart = true;
                        }
                    }
                    else
                    {
                        // ���� �� ��Ī ����
                        GM.isStart = true;
                    }
                    break;
            }

            board.BlockUpdate();
        }

        /// <summary>
        /// OtherBlock ��Ī�� Ž�����ִ� �޼���
        /// </summary>
        /// <param name="count_T">Top ����</param>
        /// <param name="count_B">Bottom ����</param>
        /// <param name="count_M">Middle ����</param>
        /// <param name="count_L">Left ����</param>
        /// <param name="count_R">Right ����</param>
        /// <returns></returns>
        private bool OtherBlockCheck(int count_T, int count_B, int count_M, int count_L, int count_R, List<Block> blocks)
        {
            // �����ϱ� ���� 0���� �ʱ�ȭ
            count_T = 0;
            count_B = 0;
            count_M = 0;
            var count_M2 = 0;
            count_L = 0;
            count_R = 0;

            // OtherBlock ��Ī �� Ž�� �۾�
            for (int i = 0; i < blocks.Count; i++)
            {
                // Top
                if ((otherBlock.row + 1 == blocks[i].row || otherBlock.row + 2 == blocks[i].row) && otherBlock.col == blocks[i].col)
                {
                    if (otherBlock.blockType == blocks[i].blockType)
                    {
                        count_T++;
                    }
                }

                // Horizontal Middle
                if ((otherBlock.col + 1 == blocks[i].col || otherBlock.col - 1 == blocks[i].col) && otherBlock.row == blocks[i].row)
                {
                    if (otherBlock.blockType == blocks[i].blockType)
                    {
                        count_M++;
                    }
                }

                // Vertical Middle
                // Horizontal���� ��Ī�Ǵ� ���� �����Ƿ� ����
                if ((otherBlock.row + 1 == blocks[i].row || otherBlock.row - 1 == blocks[i].row) && otherBlock.col == blocks[i].col)
                {
                    if (otherBlock.blockType == blocks[i].blockType)
                    {
                        count_M2++;
                    }
                }

                // Bottom
                if ((otherBlock.row - 1 == blocks[i].row || otherBlock.row - 2 == blocks[i].row) && otherBlock.col == blocks[i].col)
                {
                    if (otherBlock.blockType == blocks[i].blockType)
                    {
                        count_B++;
                    }
                }

                // Left
                if ((otherBlock.col - 1 == blocks[i].col || otherBlock.col - 2 == blocks[i].col) && otherBlock.row == blocks[i].row)
                {
                    if (otherBlock.blockType == blocks[i].blockType)
                    {
                        count_L++;
                    }
                }

                // Right
                if ((otherBlock.col + 1 == blocks[i].col || otherBlock.col + 2 == blocks[i].col) && otherBlock.row == blocks[i].row)
                {
                    if (otherBlock.blockType == blocks[i].blockType)
                    {
                        count_R++;
                    }
                }
            }

            if (count_T < 2 && count_M < 2 && count_M2 < 2 && count_B < 2 && count_L < 2 && count_R < 2)
            {
                // ��Ī �߻� ����
                return true;
            }
            else
            {
                // ��Ī �߻�
                return false;
            }
        }
    }
}