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
        public int PlayerScore { get; private set; }

        /// <summary>
        /// ���� �̸� ������Ƽ
        /// </summary>
        public string PlayerName { get; private set; }

        /// <summary>
        /// ���� ���� ������Ƽ
        /// </summary>
        public int PlayerCoin { get; private set; }

        public float saveValue;

        public void Initialize()
        {
            SetName("������");
            SetCoin(1000);

            if (!PlayerPrefs.HasKey(StringName.HighScore_0))
            {
                PlayerPrefs.SetInt(StringName.HighScore_0, 0);
            }

            if (!PlayerPrefs.HasKey(StringName.HighScore_1))
            {
                PlayerPrefs.SetInt(StringName.HighScore_1, 0);
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

        /// <summary>
        /// ������ �ݾ� ������Ʈ �޼���
        /// </summary>
        /// <param name="coin"></param>
        public void SetCoin(int coin)
        {
            PlayerCoin = coin;
        }
    }
}
