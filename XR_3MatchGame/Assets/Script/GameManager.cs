using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using XR_3MatchGame;
using XR_3MatchGame.Util;
using XR_3MatchGame_Object;

namespace XR_3MatchGame_InGame
{
    // -����ȭ-
    // ���߿� yield return new WaitForSeconds�� �����س��� �������
    public class GameManager : Singleton<GameManager>
    {
        /// <summary>
        /// ���� ���� ���� ������Ƽ
        /// </summary>
        // public GameState GameState { get; private set; }
        public GameState GameState;

        /// <summary>
        /// �⺻ ���� ������Ƽ
        /// </summary>
        //public ElementType ElementType { get; private set; }

        public ElementType ElementType = ElementType.Fire;

        // ������ ������ 3���� ����
        public List<ElementType> selectType = new List<ElementType>();

        /// <summary>
        /// ���� ������Ʈ ������Ƽ
        /// </summary>
        public Board Board { get; private set; }

        public bool isMatch = false;                                 // �� üũ�� �����Ұ��ΰ�?
        public bool isPlus = false;
        public float loadProgress;

        // �������� ����
        public string stageName;
        public ElementType stageType;

        public Vector2Int BoardSize
        {
            get
            {
                // (-3 ~ 3)
                Vector2Int boardSize = new Vector2Int(-3, 3);

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

            // ���� ����
            var StartController = FindObjectOfType<StartController>();
            StartController?.Initialize();

            GameState = GameState.Play;
            SetElementType(ElementType.Fire);

            // �ʱ� ����
            Application.targetFrameRate = 60;
        }

        public void Initialize(Board board)
        {
            Board = board;
        }

        /// <summary>
        /// ���� ���� ���� ���� �޼���
        /// </summary>
        /// <param name="gameState"></param>
        public void SetGameState(GameState gameState)
        {
            GameState = gameState;
        }

        /// <summary>
        /// ���� ĳ���� �Ӽ� ���� �޼���
        /// </summary>
        /// <param name="elementType"></param>
        public void SetElementType(ElementType elementType)
        {
            ElementType = elementType;
        }

        public void LoadScene(SceneType sceneName, IEnumerator loadCoroutine = null, Action loadComplete = null)
        {
            StartCoroutine(WaitForLoad());

            IEnumerator WaitForLoad()
            {
                loadProgress = 0;

                yield return SceneManager.LoadSceneAsync(SceneType.Loading.ToString());

                // ��û�� ���� �����ͼ� ��Ȱ��ȭ
                var asyncOper = SceneManager.LoadSceneAsync(sceneName.ToString(), LoadSceneMode.Additive);
                asyncOper.allowSceneActivation = false;

                if (loadCoroutine != null)
                {
                    yield return StartCoroutine(loadCoroutine);
                }

                while (!asyncOper.isDone)
                {
                    if (loadProgress >= .9f)
                    {
                        loadProgress = 1f;

                        yield return new WaitForSeconds(1f);

                        // �۾��� �������� Ȱ��ȭ
                        asyncOper.allowSceneActivation = true;
                    }
                    else
                    {
                        loadProgress = asyncOper.progress;
                    }

                    // ���� ���� ������ ���ؼ� null
                    yield return null;
                }

                yield return SceneManager.UnloadSceneAsync(SceneType.Loading.ToString());

                loadComplete?.Invoke();
            }
        }
    }
}