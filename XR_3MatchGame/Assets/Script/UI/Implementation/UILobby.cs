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
        private TextMeshProUGUI coinText;

        #region User Score Object
        [SerializeField]
        private GameObject user_0;

        [SerializeField]
        private GameObject user_1;

        [SerializeField]
        private GameObject user_2;
        #endregion

        private void Start()
        {
            CoinUpdate();
            RankUpdate();
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

        /// <summary>
        /// 유저가 소지한 금액 업데이트 메서드
        /// </summary>
        private void CoinUpdate()
        {
            // 코인 업데이트
            coinText.text = DataManager.Instance.PlayerCoin.ToString();
        }

        /// <summary>
        /// 최고점수 업데이트 메서드
        /// </summary>
        private void RankUpdate()
        {
            #region User_0

            // 프로필
            var uesrProfil_0 = user_0.transform.GetChild(0).GetComponent<Image>();

            // 유저 이름
            var userName_0 = user_0.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

            // 점수
            var userScore_0 = user_0.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            userScore_0.text = PlayerPrefs.GetInt(StringName.HighScore_0).ToString();

            // 트로피
            var userTrophy_0 = user_0.transform.GetChild(3).GetComponent<Image>();

            #endregion

            #region User_1

            var uesrProfil_1 = user_0.transform.GetChild(0).GetComponent<Image>();

            // 유저 이름
            var userName_1 = user_0.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

            // 점수
            var userScore_1 = user_0.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            userScore_1.text = PlayerPrefs.GetInt(StringName.HighScore_0).ToString();

            // 트로피
            var userTrophy_1 = user_0.transform.GetChild(3).GetComponent<Image>();

            #endregion

            #region User_2


            var uesrProfil_2 = user_0.transform.GetChild(0).GetComponent<Image>();

            // 유저 이름
            var userName_2 = user_0.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

            // 점수
            var userScore_2 = user_0.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            userScore_1.text = PlayerPrefs.GetInt(StringName.HighScore_0).ToString();

            // 트로피
            var userTrophy_2 = user_0.transform.GetChild(3).GetComponent<Image>();

            #endregion

        }
    }
}