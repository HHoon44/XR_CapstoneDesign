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
        //public GameState GameState { get; private set; }

        // Test
        public GameState GameState;

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
            // ���� ����
            GameState = GameState.Play;
            Board = board;
            XR_3MatchGame_Resource.ResourceManager.Instance.Initialize();
        }

        /// <summary>
        /// ���ھ ������Ʈ �ϴ� �޼���
        /// </summary>
        /// <param name="score">���ھ�</param>
        public void ScoreUpdate(int score)
        {
            Score += score;
            UIWindowManager.Instance.GetWindow<UIDetail>().SetScore(score);
        }

        public void GameStateUpdate(GameState gameState)
        {
            GameState = gameState;
        }
    }
}