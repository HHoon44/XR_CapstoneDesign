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


        public int col;                                     // 현재 블럭의 X 값
        public int row;                                     // 현재 블럭의 Y 값

        public int targetCol;                               // 상대 블럭의 X 값
        public int targetRow;                               // 상대 블럭의 Y 값

        private float swipeAngle = 0;                       // 스와이프 각도

        private Vector2 firstTouchPosition;                 // 마우스 클릭 지점
        private Vector2 finalTouchPosition;                 // 마우스 클릭을 마무리한 지점
        private Vector2 tempPosition;                       // 목표 지점

        private Block otherBlock;                           // 현재 블럭과 자리를 바꿀 블럭
        private Board board;                                // 블럭이 존재하는 보드
        private GameManager GM;

        public ElementType elementType = ElementType.None;  // 현재 블럭의 타입
        private SwipeDir swipeDir = SwipeDir.None;          // 블럭의 이동 방향

        public void BlockParticle()
        {
            blockParticle.Play();
        }

        /// <summary>
        /// 블럭 초기 세팅 메서드
        /// </summary>
        /// <param name="col">X 값</param>
        /// <param name="row">Y 값</param>
        public void Initialize(int col, int row)
        {
            GM = GameManager.Instance;
            board = GM.Board;

            var blockNum = Random.Range(1, 7);

            // 랜덤으로 블럭의 스프라이트를 설정
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
        /// 블럭을 스와이프를 체크하는 메서드
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
            // 마우스 클릭 위치 저장
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        private void OnMouseUp()
        {
            // 마우스 클릭 끝난 위치 저장
            finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }

        /// <summary>
        /// 마우스 드래그 각도를 계산하는 메서드
        /// </summary>
        private void CalculateAngle()
        {
            // 체크중 일땐 입력 막기
            if (GM.GameState != GameState.Play)
            {
                return;
            }

            // 마우스 드래그 각도를 계산합니다
            swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;

            BlockMove();
        }

        /// <summary>
        /// 계산한 각도를 이용해서 블럭을 이동시키는 메서드
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