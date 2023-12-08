using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XR_3MatchGame.Util;
using XR_3MatchGame_Data;

namespace XR_3MatchGame_UI
{
    public class UILobby : MonoBehaviour
    {
        public TextMeshProUGUI scoreText;   // 현재 스코어 Text

        [Header("유저")]
        public GameObject user_0;
        public GameObject user_1;
        public GameObject user_2;

        private void Start()
        {
            Initialize();
        }

        /// <summary>
        /// 로비 화면 초기화 메서드
        /// </summary>
        public void Initialize()
        {
            // 내 점수 업데이트
            scoreText.text = DataManager.Instance.playerScore.ToString();

            // 점수 업데이트
            // 1등
            var userScore_0 = user_0.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            userScore_0.text = PlayerPrefs.GetInt(HighScore.Score_0).ToString();

            // 2등
            var userScore_1 = user_1.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            userScore_1.text = PlayerPrefs.GetInt(HighScore.Score_1).ToString();

            // 3등
            var userScore_2 = user_2.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            userScore_2.text = PlayerPrefs.GetInt(HighScore.Score_2).ToString();

            // 유저 이름 업데이트
            // 1등
            var userName_0 = user_0.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            userName_0.text = PlayerPrefs.GetString(HighScore.Name_0);

            // 2등
            var userName_1 = user_1.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            userName_1.text = PlayerPrefs.GetString(HighScore.Name_1);

            // 3등
            var userName_2 = user_2.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            userName_2.text = PlayerPrefs.GetString(HighScore.Name_2);

            // 로비 씬 BGM 실행
            SoundManager.Instance.Initialize(SceneType.Lobby);
        }

        /// <summary>
        /// 로비 씬의 StageBtn에 바인딩할 메서드
        /// </summary>
        public void SelectLoadBtn()
        {
            SceneManager.LoadScene(SceneType.Select.ToString());
        }
    }
}