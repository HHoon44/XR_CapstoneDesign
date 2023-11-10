using TMPro;
using UnityEngine.SceneManagement;
using XR_3MatchGame.Util;
using XR_3MatchGame_Data;
using XR_3MatchGame_InGame;

namespace XR_3MatchGame_UI
{
    public class UIEnd : UIWindow
    {
        public TextMeshProUGUI score;

        public override void Start()
        {
            base.Start();

            // 현재 스코어랑 이것저것 여기서 작성
            score.text = DataManager.Instance.PlayerScore.ToString();
        }

        public void ReStartBtn()
        {
            // 게임 재시작
        }

        public void ReturnBtn()
        {
            // 스테이지로 돌아가기
        }
    }
}