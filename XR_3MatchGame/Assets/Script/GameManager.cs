using System.Collections.Generic;
using UnityEngine;
using XR_3MatchGame.Util;
using XR_3MatchGame_Object;
using XR_3MatchGame_UI;

namespace XR_3MatchGame_InGame
{
    // -����ȭ-
    // ���߿� yield return new WaitForSeconds�� �����س��� �������
    public class GameManager : Singleton<GameManager>
    {
        /// <summary>
        /// ���� ���� ���� ������Ƽ
        /// </summary>
        public GameState GameState { get; private set; }

        /// <summary>
        /// �⺻ ���� ������Ƽ
        /// </summary>
        //public ElementType ElementType { get; private set; }

        public ElementType ElementType = ElementType.Fire;

        // ������ ������ 3���� ����
        public ElementType SelectElement_0;
        public ElementType SelectElement_1;
        public ElementType SelectElement_2;

        /// <summary>
        /// ���� ������ ���� ������Ƽ
        /// </summary>
        public int Score { get; private set; }

        /// <summary>
        /// ���� ������Ʈ ������Ƽ
        /// </summary>
        public Board Board { get; private set; }

        [Header("Blocks")]
        public List<Block> blocks = new List<Block>();               // �� ���� ������ ��� ���� ��Ƴ��� ����Ʈ
        public List<Block> downBlocks = new List<Block>();           // ���� ���� ��Ƴ��� ����Ʈ
        public List<Block> delBlocks = new List<Block>();            // ������ ���� ��Ƴ��� ����Ʈ

        public bool isStart = false;                                 // �� üũ�� �����Ұ��ΰ�?

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
            // ���߿� ���� �Ŵ��� �ű�� ���� ó�� ������ҵ�
            SetElementType(ElementType.Fire);

            // ���� ����
            GameState = GameState.Play;
            Board = board;
            XR_3MatchGame_Resource.ResourceManager.Instance.Initialize();
            UM = UIWindowManager.Instance;
        }

        /// <summary>
        /// ���ھ ������Ʈ �ϴ� �޼���
        /// </summary>
        /// <param name="score">���ھ�</param>
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