using Unity.VisualScripting;
using UnityEngine;
using XR_3MatchGame.Util;
using XR_3MatchGame_InGame;
using XR_3MatchGame_Resource;
using XR_3MatchGame_Util;

namespace XR_3MatchGame_Object
{
    public class Block : MonoBehaviour, IPoolableObject
    {
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

        public int downCount;

        [SerializeField]
        private ParticleSystem blockParticle;

        [SerializeField]
        private SpriteRenderer spriteRenderer;

        public bool CanRecycle { get; set; } = true;


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
            // GM.SetGameState(GameState.Checking);

            var blocks = GM.Board.blocks;

            int height = GM.Board.height;
            int width = GM.Board.width;

            // Top
            if ((swipeAngle > 45 && swipeAngle <= 135) && row < 4)
            {
                for (int row = 0; row < height; row++)
                {
                    for (int col = 0; col < width; col++)
                    {
                        if (blocks[row, col].col == this.col && blocks[row, col].row == this.row + 1)
                        {
                            otherBlock = blocks[row, col];
                            otherBlock.row -= 1;
                            this.row += 1;

                            /// Test
                            GM.isMatch = true;

                            return;
                        }
                    }
                }
            }
            // Bottom
            else if ((swipeAngle < -45 && swipeAngle >= -135) && row > -4)
            {
                for (int row = 0; row < height; row++)
                {
                    for (int col = 0; col < width; col++)
                    {
                        if (blocks[row, col].col == this.col && blocks[row, col].row == this.row - 1)
                        {
                            otherBlock = blocks[row, col];
                            otherBlock.row += 1;
                            this.row -= 1;

                            /// Test
                            GM.isMatch = true;

                            return;
                        }
                    }
                }
            }
            // Left
            else if ((swipeAngle > 135 || swipeAngle <= -135) && col > -4)
            {
                for (int row = 0; row < height; row++)
                {
                    for (int col = 0; col < width; col++)
                    {
                        if (blocks[row, col].col == this.col - 1 && blocks[row, col].row == this.row)
                        {
                            otherBlock = blocks[row, col];
                            otherBlock.col += 1;
                            this.col -= 1;

                            /// Test
                            GM.isMatch = true;

                            return;
                        }
                    }
                }
            }
            // Right
            else if ((swipeAngle > -45 && swipeAngle <= 45) && col < 4)
            {
                for (int row = 0; row < height; row++)
                {
                    for (int col = 0; col < width; col++)
                    {
                        if (blocks[row, col].col == this.col + 1 && blocks[row, col].row == this.row)
                        {
                            otherBlock = blocks[row, col];
                            otherBlock.col -= 1;
                            this.col += 1;

                            /// Test
                            GM.isMatch = true;

                            return;
                        }
                    }
                }
            }
        }
    }
}