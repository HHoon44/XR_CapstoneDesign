using TMPro;
using UnityEngine;
using XR_3MatchGame.Util;
using XR_3MatchGame_Data;

namespace XR_3MatchGame_UI
{
    public class UILobby : MonoBehaviour
    {
        [SerializeField]
        private GameObject profilObj;           // 프로필 오브젝트

        [SerializeField]
        private GameObject coinObj;             // 코인 오브젝트

        [SerializeField]
        private GameObject userScore_0;         // 최대 점수 오브젝트

        [SerializeField]
        private GameObject userScore_1;         // 최대 점수 오브젝트

        private void Start()
        {
            ProfilUpdate();
            CoinUpdate();
            HighScoreUpdate();
        }

        /// <summary>
        /// 유저 프로필 업데이트 메서드
        /// </summary>
        private void ProfilUpdate()
        {
            // 유저 이름 업데이트
            var name = profilObj.transform.Find("Name").GetComponent<TextMeshProUGUI>();
            name.text = DataManager.Instance.PlayerName;
        }

        /// <summary>
        /// 유저가 소지한 금액 업데이트 메서드
        /// </summary>
        private void CoinUpdate()
        {
            // 코인 업데이트
            var coin = coinObj.transform.Find("Coin").GetComponent<TextMeshProUGUI>();
            coin.text = DataManager.Instance.PlayerCoin.ToString();
        }

        /// <summary>
        /// 최고점수 업데이트 메서드
        /// </summary>
        private void HighScoreUpdate()
        {
            // 0번째가 1등
            // 1번째가 2등
            // 항상 이렇게 저장 할거임
            var scoreText_0 = userScore_0.transform.Find("Score_0").GetComponent<TextMeshProUGUI>();
            var scoreText_1 = userScore_1.transform.Find("Score_1").GetComponent<TextMeshProUGUI>();

            // 최고 점수
            scoreText_0.text = PlayerPrefs.GetInt(StringName.HighScore_0).ToString();
            scoreText_1.text = PlayerPrefs.GetInt(StringName.HighScore_1).ToString();
        }
    }
}