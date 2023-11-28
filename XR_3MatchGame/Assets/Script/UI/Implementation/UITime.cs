using UnityEngine;
using UnityEngine.UI;
using XR_3MatchGame.Util;
using XR_3MatchGame_InGame;
using XR_3MatchGame_Resource;

namespace XR_3MatchGame_UI
{
    public class UITime : UIWindow
    {
        [SerializeField]
        private Image gaugeFill;

        public bool timeStop;

        private void Update()
        {
            if (!timeStop && GameManager.Instance.GameState != GameState.End)
            {
                TimeUpdate();
            }
        }

        public override void Start()
        {
            base.Start();

            Initialize();
        }

        public void Initialize()
        {
            gaugeFill.fillAmount = 1f;
        }

        private void TimeUpdate()
        {
            // 블럭 체킹중에는 시간이 멈추도록
            if (GameManager.Instance.GameState == GameState.Play)
            {
                if (gaugeFill.fillAmount <= .05f)
                {
                    gaugeFill.fillAmount = 0;

                    // 게임 종료 및 게임 종료 로직 실행
                    GameManager.Instance.SetGameState(GameState.End);
                    GameManager.Instance.Board.IGM.Initiazlie();
                }
                else
                {
                    gaugeFill.fillAmount = Mathf.Lerp(gaugeFill.fillAmount, 0, Time.deltaTime * .08f);
                }
            }
        }

        /// <summary>
        /// 시간을 더하는 메서드
        /// </summary>
        public void SetTimeAmount(float value)
        {
            if (gaugeFill.fillAmount >= 1f)
            {
                gaugeFill.fillAmount = 1f;
                return;
            }

            gaugeFill.fillAmount += value;
        }
    }
}