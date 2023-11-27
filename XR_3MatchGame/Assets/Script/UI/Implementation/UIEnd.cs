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
        private TextMeshProUGUI score;

        public TMP_InputField nameField;

        private void Start()
        {
            score.text = DataManager.Instance.PlayerScore.ToString();
        }

        /// 게임 종료 후 최고 점수를 업데이트하는 작업
        /// </summary>
        public void HighScore()
        {
            var currentScore = DataManager.Instance.PlayerScore;
            var currentName = DataManager.Instance.PlayerName;

            Debug.Log(currentScore);

            var score_0 = PlayerPrefs.GetInt(StringName.HighScore_0);
            var name_0 = PlayerPrefs.GetString(StringName.HighScore_0);

            var score_1 = PlayerPrefs.GetInt(StringName.HighScore_1);
            var name_1 = PlayerPrefs.GetString(StringName.HighScore_1);

            var score_2 = PlayerPrefs.GetInt(StringName.HighScore_2);
            var name_2 = PlayerPrefs.GetString(StringName.HighScore_2);

            if (currentScore > score_0)
            {
                // 1등
                PlayerPrefs.SetInt(StringName.HighScore_0, currentScore);
                PlayerPrefs.SetString(StringName.HighScore_0, currentName);

                // 2등
                PlayerPrefs.SetInt(StringName.HighScore_1, score_0);
                PlayerPrefs.SetString(StringName.HighScore_1, name_0);

                // 3등
                PlayerPrefs.SetInt(StringName.HighScore_2, score_1);
                PlayerPrefs.SetString(StringName.HighScore_2, name_1);
            }
            else if (currentScore > score_1)
            {
                // 1등
                PlayerPrefs.SetInt(StringName.HighScore_0, score_0);
                PlayerPrefs.SetString(StringName.HighScore_0, name_0);

                // 2등
                PlayerPrefs.SetInt(StringName.HighScore_1, currentScore);
                PlayerPrefs.SetString(StringName.HighScore_1, currentName);

                // 3등
                PlayerPrefs.SetInt(StringName.HighScore_2, score_1);
                PlayerPrefs.SetString(StringName.HighScore_2, name_1);
            }
            else if (currentScore > score_2)
            {
                // 1등
                PlayerPrefs.SetInt(StringName.HighScore_0, score_0);
                PlayerPrefs.SetString(StringName.HighScore_0, name_0);

                // 2등
                PlayerPrefs.SetInt(StringName.HighScore_1, score_1);
                PlayerPrefs.SetString(StringName.HighScore_1, name_1);

                // 3등
                PlayerPrefs.SetInt(StringName.HighScore_2, currentScore);
                PlayerPrefs.SetString(StringName.HighScore_2, currentName);
            }

            Debug.Log(PlayerPrefs.GetInt(StringName.HighScore_0));
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
            DataManager.Instance.SetScore(-DataManager.Instance.PlayerScore);
            UIWindowManager.Instance.GetWindow<UIElement>().Initialize();
            GameManager.Instance.SetGameState(GameState.Play);

            GameManager.Instance.Board.isReStart = true;

            this.gameObject.SetActive(false);
        }

        /// <summary>
        /// 로비씬으로 돌아가는 버튼에 바인딩할 메서드
        /// </summary>
        public void ReturnBtn()
        {
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