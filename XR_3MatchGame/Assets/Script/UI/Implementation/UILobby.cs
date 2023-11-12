using TMPro;
using UnityEngine;
using XR_3MatchGame.Util;
using XR_3MatchGame_Data;

namespace XR_3MatchGame_UI
{
    public class UILobby : MonoBehaviour
    {
        [SerializeField]
        private GameObject profilObj;           // ������ ������Ʈ

        [SerializeField]
        private GameObject coinObj;             // ���� ������Ʈ

        [SerializeField]
        private GameObject userScore_0;         // �ִ� ���� ������Ʈ

        [SerializeField]
        private GameObject userScore_1;         // �ִ� ���� ������Ʈ

        private void Start()
        {
            ProfilUpdate();
            CoinUpdate();
            HighScoreUpdate();
        }

        /// <summary>
        /// ���� ������ ������Ʈ �޼���
        /// </summary>
        private void ProfilUpdate()
        {
            // ���� �̸� ������Ʈ
            var name = profilObj.transform.Find("Name").GetComponent<TextMeshProUGUI>();
            name.text = DataManager.Instance.PlayerName;
        }

        /// <summary>
        /// ������ ������ �ݾ� ������Ʈ �޼���
        /// </summary>
        private void CoinUpdate()
        {
            // ���� ������Ʈ
            var coin = coinObj.transform.Find("Coin").GetComponent<TextMeshProUGUI>();
            coin.text = DataManager.Instance.PlayerCoin.ToString();
        }

        /// <summary>
        /// �ְ����� ������Ʈ �޼���
        /// </summary>
        private void HighScoreUpdate()
        {
            // 0��°�� 1��
            // 1��°�� 2��
            // �׻� �̷��� ���� �Ұ���
            var scoreText_0 = userScore_0.transform.Find("Score_0").GetComponent<TextMeshProUGUI>();
            var scoreText_1 = userScore_1.transform.Find("Score_1").GetComponent<TextMeshProUGUI>();

            // �ְ� ����
            scoreText_0.text = PlayerPrefs.GetInt(StringName.HighScore_0).ToString();
            scoreText_1.text = PlayerPrefs.GetInt(StringName.HighScore_1).ToString();
        }
    }
}