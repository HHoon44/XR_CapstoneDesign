using UnityEngine;
using XR_3MatchGame.Util;
using XR_3MatchGame_UI;

namespace XR_3MatchGame_Data
{
    public class DataManager : Singleton<DataManager>
    {
        [Header("���� ����")]
        public int playerScore;
        public string playerName;

        public void Initialize()
        {
            PlayerPrefs.DeleteAll();

            #region ���� ���� ����

            if (!PlayerPrefs.HasKey(HighScore.Score_0))
            {
                PlayerPrefs.SetInt(HighScore.Score_0, 0);
            }

            if (!PlayerPrefs.HasKey(HighScore.Score_1))
            {
                PlayerPrefs.SetInt(HighScore.Score_1, 0);
            }

            if (!PlayerPrefs.HasKey(HighScore.Score_2))
            {
                PlayerPrefs.SetInt(HighScore.Score_2, 0);
            }

            #endregion

            #region �̸� ���� ����

            if (!PlayerPrefs.HasKey(HighScore.Name_0))
            {
                PlayerPrefs.SetString(HighScore.Name_0, "��");
            }

            if (!PlayerPrefs.HasKey(HighScore.Name_1))
            {
                PlayerPrefs.SetString(HighScore.Name_1, "��");
            }

            if (!PlayerPrefs.HasKey(HighScore.Name_2))
            {
                PlayerPrefs.SetString(HighScore.Name_2, "��");
            }

            #endregion
        }

        /// <summary>
        /// ���ھ� ������Ʈ �޼���
        /// </summary>
        /// <param name="score"></param>
        public void SetScore(int score)
        {
            playerScore += score;
            UIWindowManager.Instance.GetWindow<UIDetail>().SetScore();
        }

        /// <summary>
        /// ���� �̸� ���� �޼���
        /// </summary>
        /// <param name="name"></param>
        public void SetName(string name)
        {
            playerName = name;
        }
    }
}
