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
        public bool CanRecycle { get; set; } = true;

        public int BlockScore
        {
            get
            {
                int blockScore = 5;
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

        public SpriteRenderer spriteRenderer;

        public int col;     // 현재 블럭의 X 값
        public int row;     // 현재 블럭의 Y 값

        public int targetCol;       // 상대 블럭의 X 값
        public int targetRow;       // 상대 블럭의 Y 값

        public Block topBlock;      // 위에 존재하는 블럭
        public Block bottomBlock;   // 아래에 존재하는 블럭
        public Block leftBlock;     // 왼쪽에 존재하는 블럭
        public Block rightBlock;    // 오른쪽에 존재하는 블럭

        private float swipeAngle = 0;           // 스와이프 각도

        private Vector2 firstTouchPosition;     // 마우스 클릭 지점
        private Vector2 finalTouchPosition;     // 마우스 클릭을 마무리한 지점
        private Vector2 tempPosition;

        private Block otherBlock;               // 현재 블럭과 자리를 바꿀 블럭
        private GameManager GM;
        private Board board;                    // 블럭이 존재하는 보드

        public ElementType elementType = ElementType.None;        // 현재 블럭의 타입
        private SwipeDir swipeDir = SwipeDir.None;

        [Header("Test")]
        public ElementType Top_T = ElementType.None;
        public ElementType Bottom_T = ElementType.None;
        public ElementType Left_T = ElementType.None;
        public ElementType Right_T = ElementType.None;

        /// <summary>
        /// 블럭 초기 세팅 메서드
        /// </summary>
        /// <param name="col">X 값</param>
        /// <param name="row">Y 값</param>
        public void Initialize(int col, int row)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();

            GM = GameManager.Instance;
            board = GM.Board;

            var blockNum = UnityEngine.Random.Range(1, 7);

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
            swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y,
                finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;

            BlockMove();
        }

        /// <summary>
        /// 블럭을 스와이프를 체크하는 메서드
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
        /// 계산한 각도를 이용해서 블럭을 이동시키는 메서드
        /// </summary>
        private void BlockMove()
        {
            GM.SetGameState(GameState.Checking);

            // Top
            if ((swipeAngle > 45 && swipeAngle <= 135) && row < GM.BoardSize.y)
            {
                for (int i = 0; i < GM.blocks.Count; i++)
                {
                    if (GM.blocks[i].col == col &&
                        GM.blocks[i].row == row + 1)
                    {
                        // 위쪽 이동이므로 목표 블럭은 -1 이동
                        // 위쪽 이동이므로 이동 블럭은 +1 이동
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
                        // 아래쪽 이동이므로 목표 블럭은 + 1 이동
                        // 아래쪽 이동이므로 이동 블럭은 - 1 이동
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
                        // 왼쪽 이동이므로 목표 블럭은 + 1 이동
                        // 왼쪽 이동이므로 이동 블럭은 - 1 이동
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
                        // 오른쪽 이동이므로 목표 블럭은 - 1 이동
                        // 오른쪽 이동이므로 이동 블럭은 + 1 이동
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
        /// 블럭 매칭을 체크하는 메서드
        /// </summary>
        /// <returns></returns>
        private IEnumerator BlockCheck()
        {
            board.BlockUpdate();

            // 유저가 옮긴 블럭에 대한 로직
            switch (swipeDir)
            {
                case SwipeDir.Top:
                    if (board.BlockCheck(this, null, swipeDir))
                    {
                        // 블럭 매칭 시작
                        GM.isStart = true;
                    }
                    else
                    {
                        yield return new WaitForSeconds(.2f);

                        // OtherBlock의 매칭 여부 판단
                        if (board.BlockCheck(null, otherBlock, SwipeDir.None))
                        {
                            // 블럭 매칭 시작
                            GM.isStart = true;
                        }
                        else
                        {
                            // 블럭 원위치
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
                        // 블럭 매칭 시작
                        GM.isStart = true;
                    }
                    else
                    {
                        yield return new WaitForSeconds(.2f);

                        // OtherBlock의 매칭 여부 판단
                        if (board.BlockCheck(null, otherBlock, SwipeDir.None))
                        {
                            // 블럭 매칭 시작
                            GM.isStart = true;
                        }
                        else
                        {
                            // 블럭 원위치
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
                        // 블럭 매칭 시작
                        GM.isStart = true;
                    }
                    else
                    {
                        yield return new WaitForSeconds(.2f);

                        // OtherBlock의 매칭 여부 판단
                        if (board.BlockCheck(null, otherBlock, SwipeDir.None))
                        {
                            // 블럭 매칭 시작
                            GM.isStart = true;
                        }
                        else
                        {
                            // 블럭 원위치
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
                        // 블럭 매칭 시작
                        GM.isStart = true;
                    }
                    else
                    {
                        yield return new WaitForSeconds(.2f);

                        // OtherBlock의 매칭 여부 판단
                        if (board.BlockCheck(null, otherBlock, SwipeDir.None))
                        {
                            // 블럭 매칭 시작
                            GM.isStart = true;
                        }
                        else
                        {
                            // 블럭 원위치
                            otherBlock.col += 1;
                            col -= 1;

                            yield return new WaitForSeconds(.4f);

                            GM.SetGameState(GameState.Play);
                        }
                    }
                    break;
            }

            board.BlockUpdate();
        }
    }
}