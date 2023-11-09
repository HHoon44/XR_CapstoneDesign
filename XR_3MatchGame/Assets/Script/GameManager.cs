using System.Collections.Generic;
using UnityEngine;
using XR_3MatchGame.Util;
using XR_3MatchGame_Object;
using XR_3MatchGame_UI;

namespace XR_3MatchGame_InGame
{
    // -최적화-
    // 나중에 yield return new WaitForSeconds는 선언해놓고 사용하자
    public class GameManager : Singleton<GameManager>
    {
        /// <summary>
        /// 현재 게임 상태 프로퍼티
        /// </summary>
        public GameState GameState { get; private set; }

        /// <summary>
        /// 기본 원소 프로퍼티
        /// </summary>
        //public ElementType ElementType { get; private set; }

        public ElementType ElementType = ElementType.Fire;

        // 유저가 선택한 3가지 원소
        public ElementType SelectElement_0;
        public ElementType SelectElement_1;
        public ElementType SelectElement_2;

        /// <summary>
        /// 현재 게임의 점수 프로퍼티
        /// </summary>
        public int Score { get; private set; }

        /// <summary>
        /// 보드 컴포넌트 프로퍼티
        /// </summary>
        public Board Board { get; private set; }

        [Header("Blocks")]
        public List<Block> blocks = new List<Block>();               // 인 게임 내에서 모든 블럭을 담아놓을 리스트
        public List<Block> downBlocks = new List<Block>();           // 내릴 블럭을 담아놓을 리스트
        public List<Block> delBlocks = new List<Block>();            // 삭제할 블럭을 담아놓을 리스트

        public bool isStart = false;                                 // 블럭 체크를 실행할것인가?

        private UIWindowManager UM;

        public Vector2Int BoardSize
        {
            get
            {
                // (0 ~ 7)
                Vector2Int boardSize = new Vector2Int(7, 7);

                return boardSize;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            if (gameObject == null)
            {
                return;
            }

            DontDestroyOnLoad(this);
        }

        public void Initialize(Board board)
        {
            // 나중에 게임 매니저 옮기면 따로 처리 해줘야할듯
            SetElementType(ElementType.Fire);

            // 게임 시작
            GameState = GameState.Play;
            Board = board;
            XR_3MatchGame_Resource.ResourceManager.Instance.Initialize();
            UM = UIWindowManager.Instance;
        }

        /// <summary>
        /// 스코어를 업데이트 하는 메서드
        /// </summary>
        /// <param name="score">스코어</param>
        public void ScoreUpdate(int score)
        {
            Score += score;
            UIWindowManager.Instance.GetWindow<UIDetail>().SetScore(Score);
        }

        public void SkillGaugeUpdate(float value)
        {
            UM.GetWindow<UIElement>().SetSkillAmount(value);
        }

        public void SetGameState(GameState gameState)
        {
            GameState = gameState;
        }

        public void SetElementType(ElementType elementType)
        {
            ElementType = elementType;
        }
    }
}