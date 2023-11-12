using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.TextCore.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using XR_3MatchGame;
using XR_3MatchGame.Util;
using XR_3MatchGame_Data;
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
        public List<ElementType> selectType = new List<ElementType>();

        /// <summary>
        /// ���� ������Ʈ ������Ƽ
        /// </summary>
        public Board Board { get; private set; }

        public bool isStart = false;                                 // �� üũ�� �����Ұ��ΰ�?
        public float loadProgress;

        public string stageName;
        public ElementType stageType;

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

            // ���� ����
            var StartController = FindObjectOfType<StartController>();
            StartController?.Initialize();

            GameState = GameState.Play;
            SetElementType(ElementType.Fire);
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

        public void ReStart()
        {
            StartCoroutine(Board.SpawnBlock());                             // ��
            UIWindowManager.Instance.GetWindow<UITime>().SetTime();         // �ð�
            UIWindowManager.Instance.GetWindow<UIElement>().Initialize();   // ��ų ������
            DataManager.Instance.SetScore(0);                               // ���ھ�
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