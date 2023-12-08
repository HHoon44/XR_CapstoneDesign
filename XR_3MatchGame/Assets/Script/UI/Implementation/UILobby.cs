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
        public TextMeshProUGUI scoreText;   // ���� ���ھ� Text

        [Header("����")]
        public GameObject user_0;
        public GameObject user_1;
        public GameObject user_2;

        private void Start()
        {
            Initialize();
        }

        /// <summary>
        /// �κ� ȭ�� �ʱ�ȭ �޼���
        /// </summary>
        public void Initialize()
        {
            // �� ���� ������Ʈ
            scoreText.text = DataManager.Instance.playerScore.ToString();

            // ���� ������Ʈ
            // 1��
            var userScore_0 = user_0.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            userScore_0.text = PlayerPrefs.GetInt(HighScore.Score_0).ToString();

            // 2��
            var userScore_1 = user_1.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            userScore_1.text = PlayerPrefs.GetInt(HighScore.Score_1).ToString();

            // 3��
            var userScore_2 = user_2.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            userScore_2.text = PlayerPrefs.GetInt(HighScore.Score_2).ToString();

            // ���� �̸� ������Ʈ
            // 1��
            var userName_0 = user_0.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            userName_0.text = PlayerPrefs.GetString(HighScore.Name_0);

            // 2��
            var userName_1 = user_1.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            userName_1.text = PlayerPrefs.GetString(HighScore.Name_1);

            // 3��
            var userName_2 = user_2.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            userName_2.text = PlayerPrefs.GetString(HighScore.Name_2);

            // �κ� �� BGM ����
            SoundManager.Instance.Initialize(SceneType.Lobby);
        }

        /// <summary>
        /// �κ� ���� StageBtn�� ���ε��� �޼���
        /// </summary>
        public void SelectLoadBtn()
        {
            SceneManager.LoadScene(SceneType.Select.ToString());
        }
    }
}