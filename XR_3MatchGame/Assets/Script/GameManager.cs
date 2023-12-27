using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using XR_3MatchGame;
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
        /// ������ ������ 3���� ����
        /// </summary>
        public List<ElementType> selectType = new List<ElementType>();

        [Header("�� ���� ���")]
        public GameState GameState;     // ���� ����
        public Board Board;             // ���� ����
        public bool isMatch = false;    // �� ��Ī ����
        public bool isPlus = false;     
        public float loadProgress;      // �ε� ���൵
        public InGameManager inGameManager;

        [Header("���� �������� ����")]
        public string stageName;
        public ElementType stageType;

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

            // GameState -> Play
            GameState = GameState.Play;

            // �ʱ� ����
            Application.targetFrameRate = 60;
        }

        public void GameEndFunction()
        {
            inGameManager.Initiazlie();
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
        /// �񵿱�� ���� �ε��ϴ� �޼���
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="loadCoroutine"></param>
        /// <param name="loadComplete"></param>
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