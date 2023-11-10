using UnityEngine;
using UnityEngine.UI;
using XR_3MatchGame.Util;
using XR_3MatchGame_InGame;

namespace XR_3MatchGame_UI
{
    public class UITime : UIWindow
    {
        [SerializeField]
        private Image gaugeFill;

        [SerializeField]
        private Image clock;

        public bool timeStop;

        private void Update()
        {
            if (!timeStop)
            {
                TimeUpdate();
            }
        }

        private void TimeUpdate()
        {
            if (GameManager.Instance.GameState == GameState.Play)
            {
                if (gaugeFill.fillAmount <= .05f)
                {
                    gaugeFill.fillAmount = 0;
                    GameManager.Instance.SetGameState(GameState.End);
                }
                else
                {
                    gaugeFill.fillAmount = Mathf.Lerp(gaugeFill.fillAmount, 0, Time.deltaTime * .05f);
                }
            }
        }

        private void SetTime()
        {
            // 시계 이미지 변경
            // 게이지 색상 변경
        }
    }
}