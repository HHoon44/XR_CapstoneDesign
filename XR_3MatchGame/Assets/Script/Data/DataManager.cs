using UnityEngine;
using XR_3MatchGame.Util;
using XR_3MatchGame_UI;

namespace XR_3MatchGame_Data
{
    public class DataManager : Singleton<DataManager>
    {
        [Header("유저 정보")]
        public int playerScore;
        public string playerName;

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
                PlayerPrefs.SetString(HighScore.Name_0, "조");
            }

            if (!PlayerPrefs.HasKey(HighScore.Name_1))
            {
                PlayerPrefs.SetString(HighScore.Name_1, "형");
            }

            if (!PlayerPrefs.HasKey(HighScore.Name_2))
            {
                PlayerPrefs.SetString(HighScore.Name_2, "훈");
            }

            #endregion
        }

        /// <summary>
        /// 스코어 업데이트 메서드
        /// </summary>
        /// <param name="score"></param>
        public void SetScore(int score)
        {
            playerScore += score;
            UIWindowManager.Instance.GetWindow<UIDetail>().SetScore();
        }

        /// <summary>
        /// 유저 이름 세팅 메서드
        /// </summary>
        /// <param name="name"></param>
        public void SetName(string name)
        {
            playerName = name;
        }
    }
}
