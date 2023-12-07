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

            // ���� ��� Ȱ��ȭ
            backBlack.SetActive(true);
        }

        /// ���� ���� �� �ְ� ������ ������Ʈ�ϴ� �޼���
        /// </summary>
        public void HighScore()
        {
            // ����
            var currentScore = DataManager.Instance.playerScore;
            var score_0 = PlayerPrefs.GetInt(XR_3MatchGame.Util.HighScore.Score_0);
            var score_1 = PlayerPrefs.GetInt(XR_3MatchGame.Util.HighScore.Score_1);
            var score_2 = PlayerPrefs.GetInt(XR_3MatchGame.Util.HighScore.Score_2);

            // �̸�
            var currentName = DataManager.Instance.playerName;
            var name_0 = PlayerPrefs.GetString(XR_3MatchGame.Util.HighScore.Name_0);
            var name_1 = PlayerPrefs.GetString(XR_3MatchGame.Util.HighScore.Name_1);
            var name_2 = PlayerPrefs.GetString(XR_3MatchGame.Util.HighScore.Name_2);

            if (currentScore > score_0)
            {
                // 1��
                PlayerPrefs.SetInt(XR_3MatchGame.Util.HighScore.Score_0, currentScore);
                PlayerPrefs.SetString(XR_3MatchGame.Util.HighScore.Name_0, currentName);

                // 2��
                PlayerPrefs.SetInt(XR_3MatchGame.Util.HighScore.Score_1, score_0);
                PlayerPrefs.SetString(XR_3MatchGame.Util.HighScore.Name_1, name_0);

                // 3��
                PlayerPrefs.SetInt(XR_3MatchGame.Util.HighScore.Score_2, score_1);
                PlayerPrefs.SetString(XR_3MatchGame.Util.HighScore.Name_2, name_1);
            }
            else if (currentScore > score_1)
            {
                // 1��
                PlayerPrefs.SetInt(XR_3MatchGame.Util.HighScore.Score_0, score_0);
                PlayerPrefs.SetString(XR_3MatchGame.Util.HighScore.Name_0, name_0);

                // 2��
                PlayerPrefs.SetInt(XR_3MatchGame.Util.HighScore.Score_1, currentScore);
                PlayerPrefs.SetString(XR_3MatchGame.Util.HighScore.Name_1, currentName);

                // 3��
                PlayerPrefs.SetInt(XR_3MatchGame.Util.HighScore.Score_2, score_1);
                PlayerPrefs.SetString(XR_3MatchGame.Util.HighScore.Name_2, name_1);
            }
            else if (currentScore > score_2)
            {
                // 1��
                PlayerPrefs.SetInt(XR_3MatchGame.Util.HighScore.Score_0, score_0);
                PlayerPrefs.SetString(XR_3MatchGame.Util.HighScore.Name_0, name_0);

                // 2��
                PlayerPrefs.SetInt(XR_3MatchGame.Util.HighScore.Score_1, score_1);
                PlayerPrefs.SetString(XR_3MatchGame.Util.HighScore.Name_1, name_1);

                // 3��
                PlayerPrefs.SetInt(XR_3MatchGame.Util.HighScore.Score_2, currentScore);
                PlayerPrefs.SetString(XR_3MatchGame.Util.HighScore.Name_2, currentName);
            }

        }

        public void ReStartBtn()
        {
            /*
             * ���� Ȱ��ȭ
             * ���� �ʱ�ȭ
             * ��ų ������ �ʱ�ȭ
             * �ð� �ʱ�ȭ
             * ���� ���� ����
             * �� ����
             */

            GameManager.Instance.Board.gameObject.SetActive(true);
            DataManager.Instance.SetScore(-DataManager.Instance.playerScore);
            UIWindowManager.Instance.GetWindow<UIElement>().Initialize();
            GameManager.Instance.SetGameState(GameState.Play);

            GameManager.Instance.Board.isReStart = true;

            // ���� ��� ��Ȱ��ȭ
            backBlack.SetActive(false);
            this.gameObject.SetActive(false);
        }

        /// <summary>
        /// �κ������ ���ư��� ��ư�� ���ε��� �޼���
        /// </summary>
        public void ReturnBtn()
        {
            // ���� ��� ��Ȱ��ȭ
            backBlack.SetActive(false);

            var GM = GameManager.Instance;

            // �ε� â ����
            GM.LoadScene(SceneType.Lobby, ReturnLobby());

            IEnumerator ReturnLobby()
            {
                // ������ ������ 3���� ���� �ʱ�ȭ
                GM.selectType.Clear();

                yield return null;
            }
        }
    }
}