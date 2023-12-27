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
    // -최적화-
    // 나중에 yield return new WaitForSeconds는 선언해놓고 사용하자
    public class GameManager : Singleton<GameManager>
    {
        /// <summary>
        /// 유저가 선택한 3가지 원소
        /// </summary>
        public List<ElementType> selectType = new List<ElementType>();

        [Header("인 게임 사용")]
        public GameState GameState;     // 게임 상태
        public Board Board;             // 게임 보드
        public bool isMatch = false;    // 블럭 매칭 여부
        public bool isPlus = false;     
        public float loadProgress;      // 로딩 진행도
        public InGameManager inGameManager;

        [Header("현재 스테이지 정보")]
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

            // 게임 시작
            var StartController = FindObjectOfType<StartController>();
            StartController?.Initialize();

            // GameState -> Play
            GameState = GameState.Play;

            // 초기 설정
            Application.targetFrameRate = 60;
        }

        public void GameEndFunction()
        {
            inGameManager.Initiazlie();
        }

        /// <summary>
        /// 현재 게임 상태 세팅 메서드
        /// </summary>
        /// <param name="gameState"></param>
        public void SetGameState(GameState gameState)
        {
            GameState = gameState;
        }

        /// <summary>
        /// 비동기로 씬을 로드하는 메서드
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

                // 요청한 씬을 가져와서 비활성화
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

                        // 작업이 끝났으면 활성화
                        asyncOper.allowSceneActivation = true;
                    }
                    else
                    {
                        loadProgress = asyncOper.progress;
                    }

                    // 메인 로직 실행을 위해서 null
                    yield return null;
                }

                yield return SceneManager.UnloadSceneAsync(SceneType.Loading.ToString());

                loadComplete?.Invoke();
            }
        }
    }
}