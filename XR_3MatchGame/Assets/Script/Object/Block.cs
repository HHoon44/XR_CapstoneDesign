using System.Collections;
using UnityEngine;
using XR_3MatchGame.Util;
using XR_3MatchGame_InGame;
using XR_3MatchGame_Resource;
using XR_3MatchGame_Util;

namespace XR_3MatchGame_Object
{
    public class Block : MonoBehaviour, IPoolableObject
    {
        [SerializeField]
        private ParticleSystem blockParticle;

        [SerializeField]
        private SpriteRenderer spriteRenderer;

        public bool CanRecycle { get; set; } = true;

        public int BlockScore
        {
            get
            {
                int blockScore = 10;
                return blockScore;
            }
        }

        public float ElementValue
        {
            get
            {
                float elementValue = .05f;
                return elementValue;
            }
        }

        public int col;                                     // ���� ���� X ��
        public int row;                                     // ���� ���� Y ��

        public int targetCol;                               // ��� ���� X ��
        public int targetRow;                               // ��� ���� Y ��

        private float swipeAngle = 0;                       // �������� ����

        private Vector2 firstTouchPosition;                 // ���콺 Ŭ�� ����
        private Vector2 finalTouchPosition;                 // ���콺 Ŭ���� �������� ����
        private Vector2 tempPosition;                       // ��ǥ ����

        private Block otherBlock;                           // ���� ���� �ڸ��� �ٲ� ��
        private Board board;                                // ���� �����ϴ� ����
        private GameManager GM;

        public ElementType elementType = ElementType.None;  // ���� ���� Ÿ��
        private SwipeDir swipeDir = SwipeDir.None;          // ���� �̵� ����

        [Header("���� ��")]
        public Block topBlock;                              // ���� �����ϴ� ��
        public Block bottomBlock;                           // �Ʒ��� �����ϴ� ��
        public Block leftBlock;                             // ���ʿ� �����ϴ� ��
        public Block rightBlock;                            // �����ʿ� �����ϴ� ��

        public void BlockParticle()
        {
            blockParticle.Play();
        }

        /// <summary>
        /// �� �ʱ� ���� �޼���
        /// </summary>
        /// <param name="col">X ��</param>
        /// <param name="row">Y ��</param>
        public void Initialize(int col, int row)
        {
            GM = GameManager.Instance;
            board = GM.Board;

            var blockNum = Random.Range(1, 7);

            // �������� ���� ��������Ʈ�� ����
            switch (blockNum)
            {
                case (int)ElementType.Dark:
                    spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Dark.ToString());
                    elementType = ElementType.Dark;
                    break;

                case (int)ElementType.Fire:
                    spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Fire.ToString());
                    elementType = ElementType.Fire;
                    break;

                case (int)ElementType.Light:
                    spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Light.ToString());
                    elementType = ElementType.Light;
                    break;

                case (int)ElementType.Lightning:
                    spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Lightning.ToString());
                    elementType = ElementType.Lightning;
                    break;

                case (int)ElementType.Ice:
                    spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Ice.ToString());
                    elementType = ElementType.Ice;
                    break;

                case (int)ElementType.Grass:
                    spriteRenderer.sprite = SpriteLoader.GetSprite(AtlasType.BlockAtlas, ElementType.Grass.ToString());
                    elementType = ElementType.Grass;
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
            BlockUpdate();
        }

        /// <summary>
        /// ���� ���������� üũ�ϴ� �޼���
        /// </summary>
        private void BlockSwipe()
        {
            // Horizontal
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

            // Vertical
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

        public void BlockUpdate()
        {
            var blocks = board.blocks;

            for (int i = 0; i < blocks.Count; i++)
            {
                for (int j = 0; j < blocks.Count; j++)
                {
                    // Top
                    if (blocks[i].row == 3)
                    {
                        blocks[i].topBlock = null;
                    }
                    else
                    {
                        if (blocks[i].row + 1 == blocks[j].row && blocks[i].col == blocks[j].col)
                        {
                            blocks[i].topBlock = blocks[j];
                        }
                    }

                    // Bottom
                    if (blocks[i].row == -3)
                    {
                        blocks[i].bottomBlock = null;
                    }
                    else
                    {
                        if (blocks[i].row - 1 == blocks[j].row && blocks[i].col == blocks[j].col)
                        {
                            blocks[i].bottomBlock = blocks[j];
                        }
                    }

                    // Left
                    if (blocks[i].col == -3)
                    {
                        blocks[i].leftBlock = null;
                    }
                    else
                    {
                        if (blocks[i].col - 1 == blocks[j].col && blocks[i].row == blocks[j].row)
                        {
                            blocks[i].leftBlock = blocks[j];
                        }
                    }

                    // Right
                    if (blocks[i].col == 3)
                    {
                        blocks[i].rightBlock = null;
                    }
                    else
                    {
                        if (blocks[i].col + 1 == blocks[j].col && blocks[i].row == blocks[j].row)
                        {
                            blocks[i].rightBlock = blocks[j];
                        }
                    }
                }
            }
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
            if (GM.GameState != GameState.Play)
            {
                return;
            }

            // ���콺 �巡�� ������ ����մϴ�
            swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;

            BlockMove();
        }

        /// <summary>
        /// ����� ������ �̿��ؼ� ���� �̵���Ű�� �޼���
        /// </summary>
        private void BlockMove()
        {
            GM.SetGameState(GameState.Checking);

            var blocks = GM.Board.blocks;

            // Top
            if ((swipeAngle > 45 && swipeAngle <= 135) && row < GM.BoardSize.y)
            {
                for (int i = 0; i < blocks.Count; i++)
                {
                    if (blocks[i].col == col &&
                        blocks[i].row == row + 1)
                    {
                        // ���� �̵��̹Ƿ� ��ǥ ���� -1 �̵�
                        // ���� �̵��̹Ƿ� �̵� ���� +1 �̵�
                        otherBlock = blocks[i];
                        otherBlock.row -= 1;
                        row += 1;

                        swipeDir = SwipeDir.Top;
                        //StartCoroutine(BlockCheck());
                        return;
                    }
                }
            }
            // Bottom
            else if ((swipeAngle < -45 && swipeAngle >= -135) && row > 0)
            {
                for (int i = 0; i < blocks.Count; i++)
                {
                    if (blocks[i].col == col &&
                        blocks[i].row == row - 1)
                    {
                        // �Ʒ��� �̵��̹Ƿ� ��ǥ ���� + 1 �̵�
                        // �Ʒ��� �̵��̹Ƿ� �̵� ���� - 1 �̵�
                        otherBlock = blocks[i];
                        otherBlock.row += 1;
                        row -= 1;

                        swipeDir = SwipeDir.Bottom;
                        //StartCoroutine(BlockCheck());
                        return;
                    }
                }
            }
            // Left
            else if ((swipeAngle > 135 || swipeAngle <= -135) && col > 0)
            {
                for (int i = 0; i < blocks.Count; i++)
                {
                    if (blocks[i].col == col - 1 &&
                        blocks[i].row == row)
                    {
                        // ���� �̵��̹Ƿ� ��ǥ ���� + 1 �̵�
                        // ���� �̵��̹Ƿ� �̵� ���� - 1 �̵�
                        otherBlock = blocks[i];
                        otherBlock.col += 1;
                        col -= 1;

                        swipeDir = SwipeDir.Left;
                        //StartCoroutine(BlockCheck());
                        return;
                    }
                }
            }
            // Right
            else if ((swipeAngle > -45 && swipeAngle <= 45) && col < GM.BoardSize.x)
            {
                for (int i = 0; i < blocks.Count; i++)
                {
                    if (blocks[i].col == col + 1 &&
                        blocks[i].row == row)
                    {
                        // ������ �̵��̹Ƿ� ��ǥ ���� - 1 �̵�
                        // ������ �̵��̹Ƿ� �̵� ���� + 1 �̵�
                        otherBlock = blocks[i];
                        otherBlock.col -= 1;
                        col += 1;

                        swipeDir = SwipeDir.Right;
                        //StartCoroutine(BlockCheck());
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
            // ������ �ű� ���� ���� ����
            switch (swipeDir)
            {
                case SwipeDir.Top:
                    if (board.BlockCheck(this, null, swipeDir))
                    {
                        // �� ��Ī ����
                        GM.isStart = true;
                    }
                    else
                    {
                        yield return new WaitForSeconds(.2f);

                        // OtherBlock�� ��Ī ���� �Ǵ�
                        if (board.BlockCheck(null, otherBlock, SwipeDir.None))
                        {
                            // �� ��Ī ����
                            GM.isStart = true;
                        }
                        else
                        {
                            // �� ����ġ
                            otherBlock.row += 1;
                            row -= 1;

                            yield return new WaitForSeconds(.4f);

                            GM.SetGameState(GameState.Play);
                        }
                    }
                    break;

                case SwipeDir.Bottom:
                    if (board.BlockCheck(this, null, swipeDir))
                    {
                        // �� ��Ī ����
                        GM.isStart = true;
                    }
                    else
                    {
                        yield return new WaitForSeconds(.2f);

                        // OtherBlock�� ��Ī ���� �Ǵ�
                        if (board.BlockCheck(null, otherBlock, SwipeDir.None))
                        {
                            // �� ��Ī ����
                            GM.isStart = true;
                        }
                        else
                        {
                            // �� ����ġ
                            otherBlock.row -= 1;
                            row += 1;

                            yield return new WaitForSeconds(.4f);

                            GM.SetGameState(GameState.Play);
                        }
                    }
                    break;

                case SwipeDir.Left:
                    if (board.BlockCheck(this, null, swipeDir))
                    {
                        // �� ��Ī ����
                        GM.isStart = true;
                    }
                    else
                    {
                        yield return new WaitForSeconds(.2f);

                        // OtherBlock�� ��Ī ���� �Ǵ�
                        if (board.BlockCheck(null, otherBlock, SwipeDir.None))
                        {
                            // �� ��Ī ����
                            GM.isStart = true;
                        }
                        else
                        {
                            // �� ����ġ
                            otherBlock.col -= 1;
                            col += 1;

                            yield return new WaitForSeconds(.4f);

                            GM.SetGameState(GameState.Play);
                        }
                    }
                    break;

                case SwipeDir.Right:
                    if (board.BlockCheck(this, null, swipeDir))
                    {
                        // �� ��Ī ����
                        GM.isStart = true;
                    }
                    else
                    {
                        yield return new WaitForSeconds(.2f);

                        // OtherBlock�� ��Ī ���� �Ǵ�
                        if (board.BlockCheck(null, otherBlock, SwipeDir.None))
                        {
                            // �� ��Ī ����
                            GM.isStart = true;
                        }
                        else
                        {
                            // �� ����ġ
                            otherBlock.col += 1;
                            col -= 1;

                            yield return new WaitForSeconds(.4f);

                            GM.SetGameState(GameState.Play);
                        }
                    }
                    break;
            }
        }
    }
}