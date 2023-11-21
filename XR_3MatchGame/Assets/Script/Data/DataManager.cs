using UnityEngine;
using XR_3MatchGame.Util;
using XR_3MatchGame_UI;

namespace XR_3MatchGame_Data
{
    public class DataManager : Singleton<DataManager>
    {
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

        public float saveValue;

        public void Initialize()
        {
            SetName("마법사");
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
        /// 스코어 업데이트 메서드
        /// </summary>
        /// <param name="score"></param>
        public void SetScore(int score)
        {
            PlayerScore += score;
            UIWindowManager.Instance.GetWindow<UIDetail>().SetScore();
        }

        /// <summary>
        /// 유저 이름 세팅 메서드
        /// </summary>
        /// <param name="name"></param>
        public void SetName(string name)
        {
            PlayerName = name;
        }

        /// <summary>
        /// 소지한 금액 업데이트 메서드
        /// </summary>
        /// <param name="coin"></param>
        public void SetCoin(int coin)
        {
            PlayerCoin = coin;
        }
    }
}
