using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using XR_3MatchGame.Util;
using XR_3MatchGame_Data;
using XR_3MatchGame_InGame;
using XR_3MatchGame_Object;

namespace XR_3MatchGame_UI
{
    public class UIEnd : MonoBehaviour
    {
        [SerializeField]
        private GameObject backBlack;

        [SerializeField]
        private TextMeshProUGUI score;

        public TMP_InputField nameField;

        private void Start()
        {
            score.text = DataManager.Instance.playerScore.ToString();

            // 검은 배경 활성화
            backBlack.SetActive(true);
        }

        /// 게임 종료 후 최고 점수를 업데이트하는 메서드
        /// </summary>
        public void HighScore()
        {
            // 점수
            var currentScore = DataManager.Instance.playerScore;
            var score_0 = PlayerPrefs.GetInt(XR_3MatchGame.Util.HighScore.Score_0);
            var score_1 = PlayerPrefs.GetInt(XR_3MatchGame.Util.HighScore.Score_1);
            var score_2 = PlayerPrefs.GetInt(XR_3MatchGame.Util.HighScore.Score_2);

            // 이름
            var currentName = DataManager.Instance.playerName;
            var name_0 = PlayerPrefs.GetString(XR_3MatchGame.Util.HighScore.Name_0);
            var name_1 = PlayerPrefs.GetString(XR_3MatchGame.Util.HighScore.Name_1);
            var name_2 = PlayerPrefs.GetString(XR_3MatchGame.Util.HighScore.Name_2);

            if (currentScore > score_0)
            {
                // 1등
                PlayerPrefs.SetInt(XR_3MatchGame.Util.HighScore.Score_0, currentScore);
                PlayerPrefs.SetString(XR_3MatchGame.Util.HighScore.Name_0, currentName);

                // 2등
                PlayerPrefs.SetInt(XR_3MatchGame.Util.HighScore.Score_1, score_0);
                PlayerPrefs.SetString(XR_3MatchGame.Util.HighScore.Name_1, name_0);

                // 3등
                PlayerPrefs.SetInt(XR_3MatchGame.Util.HighScore.Score_2, score_1);
                PlayerPrefs.SetString(XR_3MatchGame.Util.HighScore.Name_2, name_1);
            }
            else if (currentScore > score_1)
            {
                // 1등
                PlayerPrefs.SetInt(XR_3MatchGame.Util.HighScore.Score_0, score_0);
                PlayerPrefs.SetString(XR_3MatchGame.Util.HighScore.Name_0, name_0);

                // 2등
                PlayerPrefs.SetInt(XR_3MatchGame.Util.HighScore.Score_1, currentScore);
                PlayerPrefs.SetString(XR_3MatchGame.Util.HighScore.Name_1, currentName);

                // 3등
                PlayerPrefs.SetInt(XR_3MatchGame.Util.HighScore.Score_2, score_1);
                PlayerPrefs.SetString(XR_3MatchGame.Util.HighScore.Name_2, name_1);
            }
            else if (currentScore > score_2)
            {
                // 1등
                PlayerPrefs.SetInt(XR_3MatchGame.Util.HighScore.Score_0, score_0);
                PlayerPrefs.SetString(XR_3MatchGame.Util.HighScore.Name_0, name_0);

                // 2등
                PlayerPrefs.SetInt(XR_3MatchGame.Util.HighScore.Score_1, score_1);
                PlayerPrefs.SetString(XR_3MatchGame.Util.HighScore.Name_1, name_1);

                // 3등
                PlayerPrefs.SetInt(XR_3MatchGame.Util.HighScore.Score_2, currentScore);
                PlayerPrefs.SetString(XR_3MatchGame.Util.HighScore.Name_2, currentName);
            }

        }

        public void ReStartBtn()
        {
            /*
             * 보드 활성화
             * 점수 초기화
             * 스킬 게이지 초기화
             * 시간 초기화
             * 게임 상태 변경
             * 블럭 스폰
             */

            GameManager.Instance.Board.gameObject.SetActive(true);
            DataManager.Instance.SetScore(-DataManager.Instance.playerScore);
            UIWindowManager.Instance.GetWindow<UIElement>().Initialize();
            GameManager.Instance.SetGameState(GameState.Play);

            GameManager.Instance.Board.isReStart = true;

            // 검은 배경 비활성화
            backBlack.SetActive(false);
            this.gameObject.SetActive(false);
        }

        /// <summary>
        /// 로비씬으로 돌아가는 버튼에 바인딩할 메서드
        /// </summary>
        public void ReturnBtn()
        {
            // 검은 배경 비활성화
            backBlack.SetActive(false);

            var GM = GameManager.Instance;

            // 로딩 창 띄우기
            GM.LoadScene(SceneType.Lobby, ReturnLobby());

            IEnumerator ReturnLobby()
            {
                // 유저가 선택한 3가지 원소 초기화
                GM.selectType.Clear();

                yield return null;
            }
        }
    }
}