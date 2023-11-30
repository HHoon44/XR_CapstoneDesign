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
            PlayerPrefs.DeleteAll();

            #region 점수 공간 생성

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

            #region 이름 공간 생성

            if (!PlayerPrefs.HasKey(HighScore.Name_0))
            {
                PlayerPrefs.SetString(HighScore.Name_0, "마법사");
            }

            if (!PlayerPrefs.HasKey(HighScore.Name_1))
            {
                PlayerPrefs.SetString(HighScore.Name_1, "대마법사");
            }

            if (!PlayerPrefs.HasKey(HighScore.Name_2))
            {
                PlayerPrefs.SetString(HighScore.Name_2, "대대마법사");
            }

            #endregion
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
