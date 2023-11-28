using TMPro;
using UnityEngine;
using UnityEngine.UI;
using XR_3MatchGame.Util;
using XR_3MatchGame_Data;

namespace XR_3MatchGame_UI
{
    public class UILobby : MonoBehaviour
    {
        [SerializeField]
        private GameObject optionDetail;

        [SerializeField]
        private TextMeshProUGUI myScore;

        #region User Object

        [SerializeField]
        private GameObject user_0;

        [SerializeField]
        private GameObject user_1;

        [SerializeField]
        private GameObject user_2;

        #endregion

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
            myScore.text = DataManager.Instance.PlayerScore.ToString();

            // 점수
            var userScore_0 = user_0.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            userScore_0.text = PlayerPrefs.GetInt(HighScore.Score_0).ToString();

            var userScore_1 = user_1.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            userScore_1.text = PlayerPrefs.GetInt(HighScore.Score_1).ToString();

            var userScore_2 = user_2.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            userScore_2.text = PlayerPrefs.GetInt(HighScore.Score_2).ToString();

            // 유저 이름
            var userName_0 = user_0.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            userName_0.text = PlayerPrefs.GetString(HighScore.Name_0);

            var userName_1 = user_1.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            userName_1.text = PlayerPrefs.GetString(HighScore.Name_1);

            var userName_2 = user_2.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            userName_2.text = PlayerPrefs.GetString(HighScore.Name_2);
        }

        /// <summary>
        /// 옵션 버튼에 바인딩할 메서드
        /// </summary>
        public void OptionButton()
        {
            optionDetail.SetActive(true);
        }

        /// <summary>
        /// 옵션 창 닫기 버튼에 바인딩할 메서드
        /// </summary>
        public void CloseButton()
        {
            optionDetail.SetActive(false);
        }
    }
}