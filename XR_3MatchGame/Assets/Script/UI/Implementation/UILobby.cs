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
            myScore.text = DataManager.Instance.PlayerScore.ToString();

            #region User_0

            // 프로필
            var uesrProfil_0 = user_0.transform.GetChild(0).GetComponent<Image>();

            // 유저 이름
            var userName_0 = user_0.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            userName_0.text = PlayerPrefs.GetString(StringName.HighScore_0);

            // 점수
            var userScore_0 = user_0.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            userScore_0.text = PlayerPrefs.GetInt(StringName.HighScore_0).ToString();

            #endregion

            #region User_1

            var uesrProfil_1 = user_1.transform.GetChild(0).GetComponent<Image>();

            // 유저 이름
            var userName_1 = user_1.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            userName_1.text = PlayerPrefs.GetString(StringName.HighScore_1);

            // 점수
            var userScore_1 = user_1.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            userScore_1.text = PlayerPrefs.GetInt(StringName.HighScore_1).ToString();

            #endregion

            #region User_2

            var uesrProfil_2 = user_2.transform.GetChild(0).GetComponent<Image>();

            // 유저 이름
            var userName_2 = user_2.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            userName_2.text = PlayerPrefs.GetString(StringName.HighScore_2);

            // 점수
            var userScore_2 = user_2.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            userScore_2.text = PlayerPrefs.GetInt(StringName.HighScore_2).ToString();

            #endregion
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