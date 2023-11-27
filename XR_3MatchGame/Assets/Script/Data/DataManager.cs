using UnityEngine;
using XR_3MatchGame.Util;
using XR_3MatchGame_UI;

namespace XR_3MatchGame_Data
{
    public class DataManager : Singleton<DataManager>
    {
        /// <summary>
        /// ���� ���� ������Ƽ
        /// </summary>
        // public int PlayerScore { get; private set; }

        public int PlayerScore;

        /// <summary>
        /// ���� �̸� ������Ƽ
        /// </summary>
        // public string PlayerName { get; private set; }

        public string PlayerName;

        public void Initialize()
        {
            // PlayerPrefs.DeleteAll();

            // ��� ó������ 0���� �ʱ�ȭ
            if (!PlayerPrefs.HasKey(StringName.HighScore_0))
            {
                PlayerPrefs.SetInt(StringName.HighScore_0, 0);
                PlayerPrefs.SetString(StringName.HighScore_0, "�����̸�");
            }

            if (!PlayerPrefs.HasKey(StringName.HighScore_1))
            {
                PlayerPrefs.SetInt(StringName.HighScore_1, 0);
                PlayerPrefs.SetString(StringName.HighScore_1, "�����̸�");
            }

            if (!PlayerPrefs.HasKey(StringName.HighScore_2))
            {
                PlayerPrefs.SetInt(StringName.HighScore_2, 0);
                PlayerPrefs.SetString(StringName.HighScore_2, "�����̸�");
            }
        }

        /// <summary>
        /// ���ھ� ������Ʈ �޼���
        /// </summary>
        /// <param name="score"></param>
        public void SetScore(int score)
        {
            PlayerScore += score;
            UIWindowManager.Instance.GetWindow<UIDetail>().SetScore();
        }

        /// <summary>
        /// ���� �̸� ���� �޼���
        /// </summary>
        /// <param name="name"></param>
        public void SetName(string name)
        {
            PlayerName = name;
        }
    }
}
