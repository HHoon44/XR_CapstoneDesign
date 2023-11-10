using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using XR_3MatchGame_InGame;
using XR_3MatchGame_UI;

namespace XR_3MatchGame_Data
{
    public class DataManager : Singleton<DataManager>
    {
        private GameManager GM;

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

        /// <summary>
        /// ���ھ� ������Ʈ �޼���
        /// </summary>
        /// <param name="score"></param>
        public void ScoreUpdate(int score)
        {
            PlayerScore += score;
            UIWindowManager.Instance.GetWindow<UIDetail>().SetScore(PlayerScore);
        }

        /// <summary>
        /// ���� �̸� ���� �޼���
        /// </summary>
        /// <param name="name"></param>
        public void SetPlayerName(string name)
        {
            PlayerName = name;
        }

        /// <summary>
        /// ������ �ݾ� ������Ʈ �޼���
        /// </summary>
        /// <param name="coin"></param>
        public void SetPlayerCoin(int coin)
        {
            PlayerCoin = coin;
        }

        /// <summary>
        /// �����͸� �����ϴ� �޼���
        /// </summary>
        /// <param name="key"></param>
        public void SaveUserData(string key)
        {
            if (!GM)
            {
                GM = GameManager.Instance;
            }

            // key�� �������� �ʴ´ٸ� ���ο� ���� ����
            if (!PlayerPrefs.HasKey(key))
            {
                // �̸��� ������ ����
                PlayerPrefs.SetString(key, PlayerName);
                PlayerPrefs.SetInt(key, PlayerScore);
            }
            else
            {
                // �����ϴ� ������ ���� ������ ������Ʈ
                PlayerPrefs.SetInt(key, PlayerScore);
            }
        }

        /// <summary>
        /// ��û ���� ���� �����͸� �����ϴ� �޼���
        /// </summary>
        /// <param name="key"></param>
        /// <param name="choice"></param>
        /// <returns></returns>
        public string LoadUserData(string key, int choice)
        {
            if (PlayerPrefs.HasKey(key))
            {
                switch (choice)
                {
                    // �÷��̾� �̸�
                    case 0:
                        return PlayerPrefs.GetString(key);

                    // �÷��̾� ����
                    case 1:
                        return PlayerPrefs.GetInt(key).ToString();

                    default:
                        return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
