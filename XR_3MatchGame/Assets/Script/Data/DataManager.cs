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
        // public int PlayerScore { get; private set; }

        public int PlayerScore;

        /// <summary>
        /// 유저 이름 프로퍼티
        /// </summary>
        // public string PlayerName { get; private set; }

        public string PlayerName;

        public void Initialize()
        {
            // PlayerPrefs.DeleteAll();

            // 모두 처음에는 0으로 초기화
            if (!PlayerPrefs.HasKey(StringName.HighScore_0))
            {
                PlayerPrefs.SetInt(StringName.HighScore_0, 0);
                PlayerPrefs.SetString(StringName.HighScore_0, "유저이름");
            }

            if (!PlayerPrefs.HasKey(StringName.HighScore_1))
            {
                PlayerPrefs.SetInt(StringName.HighScore_1, 0);
                PlayerPrefs.SetString(StringName.HighScore_1, "유저이름");
            }

            if (!PlayerPrefs.HasKey(StringName.HighScore_2))
            {
                PlayerPrefs.SetInt(StringName.HighScore_2, 0);
                PlayerPrefs.SetString(StringName.HighScore_2, "유저이름");
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
    }
}
