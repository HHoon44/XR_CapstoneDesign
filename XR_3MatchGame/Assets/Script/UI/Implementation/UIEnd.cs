using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using XR_3MatchGame.Util;
using XR_3MatchGame_Data;
using XR_3MatchGame_InGame;

namespace XR_3MatchGame_UI
{
    public class UIEnd : UIWindow
    {
        public TextMeshProUGUI score;

        public override void Start()
        {
            base.Start();

            // ���� ���ھ�� �̰����� ���⼭ �ۼ�
            score.text = DataManager.Instance.PlayerScore.ToString();

            // �ְ����� �ۼ��ϱ�
            HighScore();
        }

        private void HighScore()
        {
            var currentScore = DataManager.Instance.PlayerScore;
            var high_0 = PlayerPrefs.GetInt(StringName.HighScore_0);
            var high_1 = PlayerPrefs.GetInt(StringName.HighScore_1);

            if (currentScore > high_0)
            {
                PlayerPrefs.SetInt(StringName.HighScore_0, currentScore);
                PlayerPrefs.SetInt(StringName.HighScore_1, high_0);
            }
            else if (currentScore > high_1)
            {
                PlayerPrefs.SetInt(StringName.HighScore_0, high_0);
                PlayerPrefs.SetInt(StringName.HighScore_1, currentScore);
            }
        }

        public void ReStartBtn()
        {
            GameManager.Instance.Board.gameObject.SetActive(true);
            GameManager.Instance.ReStart();
        }

        public void ReturnBtn()
        {
            // ���������� ���ư���
            SceneManager.LoadScene(SceneType.Lobby.ToString());
        }
    }
}