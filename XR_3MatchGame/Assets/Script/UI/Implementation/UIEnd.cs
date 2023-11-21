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
        public TextMeshProUGUI score;

        public void Initialize()
        {
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
            /*
             * ���� Ȱ��ȭ
             * ���� �ʱ�ȭ
             * ��ų ������ �ʱ�ȭ
             * �ð� �ʱ�ȭ
             * ���� ���� ����
             * �� ����
             */

            GameManager.Instance.Board.gameObject.SetActive(true);
            DataManager.Instance.SetScore(-DataManager.Instance.PlayerScore);
            UIWindowManager.Instance.GetWindow<UIElement>().Initialize();
            UIWindowManager.Instance.GetWindow<UITime>().SetTime();
            GameManager.Instance.SetGameState(GameState.Play);

            GameManager.Instance.Board.isReStart = true;

            this.gameObject.SetActive(false);
        }

        public void ReturnBtn()
        {
            // ���������� ���ư���
            SceneManager.LoadScene(SceneType.Lobby.ToString());
            DataManager.Instance.SetScore(-DataManager.Instance.PlayerScore);
            GameManager.Instance.selectType.Clear();
        }
    }
}