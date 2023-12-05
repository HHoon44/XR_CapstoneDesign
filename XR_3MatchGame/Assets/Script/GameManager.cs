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
    // -최적화-
    // 나중에 yield return new WaitForSeconds는 선언해놓고 사용하자
    public class GameManager : Singleton<GameManager>
    {
        /// <summary>
        /// 현재 게임 상태 프로퍼티
        /// </summary>
        // public GameState GameState { get; private set; }
        public GameState GameState;

        /// <summary>
        /// 기본 원소 프로퍼티
        /// </summary>
        //public ElementType ElementType { get; private set; }

        public ElementType ElementType = ElementType.Fire;

        // 유저가 선택한 3가지 원소
        public List<ElementType> selectType = new List<ElementType>();

        /// <summary>
        /// 보드 컴포넌트 프로퍼티
        /// </summary>
        public Board Board { get; private set; }

        public bool isMatch = false;                                 // 블럭 체크를 실행할것인가?
        public bool isPlus = false;
        public float loadProgress;

        // 스테이지 정보
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

            // 게임 시작
            var StartController = FindObjectOfType<StartController>();
            StartController?.Initialize();

            GameState = GameState.Play;
            SetElementType(ElementType.Fire);

            // 초기 설정
            Application.targetFrameRate = 60;
        }

        public void Initialize(Board board)
        {
            Board = board;
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
        /// 현재 캐릭터 속성 세팅 메서드
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