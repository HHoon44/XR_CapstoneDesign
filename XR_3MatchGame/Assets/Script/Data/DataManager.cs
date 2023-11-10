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
        /// 유저 점수 프로퍼티
        /// </summary>
        public int PlayerScore { get; private set; }

        /// <summary>
        /// 유저 이름 프로퍼티
        /// </summary>
        public string PlayerName { get; private set; }

        /// <summary>
        /// 유저 코인 프로퍼티
        /// </summary>
        public int PlayerCoin { get; private set; }

        /// <summary>
        /// 스코어 업데이트 메서드
        /// </summary>
        /// <param name="score"></param>
        public void ScoreUpdate(int score)
        {
            PlayerScore += score;
            UIWindowManager.Instance.GetWindow<UIDetail>().SetScore(PlayerScore);
        }

        /// <summary>
        /// 유저 이름 세팅 메서드
        /// </summary>
        /// <param name="name"></param>
        public void SetPlayerName(string name)
        {
            PlayerName = name;
        }

        /// <summary>
        /// 소지한 금액 업데이트 메서드
        /// </summary>
        /// <param name="coin"></param>
        public void SetPlayerCoin(int coin)
        {
            PlayerCoin = coin;
        }

        /// <summary>
        /// 데이터를 저장하는 메서드
        /// </summary>
        /// <param name="key"></param>
        public void SaveUserData(string key)
        {
            if (!GM)
            {
                GM = GameManager.Instance;
            }

            // key가 존재하지 않는다면 새로운 유저 생성
            if (!PlayerPrefs.HasKey(key))
            {
                // 이름과 점수를 저장
                PlayerPrefs.SetString(key, PlayerName);
                PlayerPrefs.SetInt(key, PlayerScore);
            }
            else
            {
                // 존재하는 유저는 점수 데이터 업데이트
                PlayerPrefs.SetInt(key, PlayerScore);
            }
        }

        /// <summary>
        /// 요청 값에 따라 데이터를 리턴하는 메서드
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
                    // 플레이어 이름
                    case 0:
                        return PlayerPrefs.GetString(key);

                    // 플레이어 점수
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
