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

        public override void Start()
        {
            base.Start();

            SetTime();
        }

        public void SetTime()
        {
            gaugeFill.fillAmount = 1f;

            var stageName = string.Empty;

            switch (GameManager.Instance.stageName)
            {
                case "불":
                    stageName = ElementType.Fire.ToString();
                    break;

                case "얼음":
                    stageName = ElementType.Ice.ToString();
                    break;

                case "풀":
                    stageName = ElementType.Grass.ToString();
                    break;
            }
        }

        private void TimeUpdate()
        {
            if (GameManager.Instance.GameState != GameState.Checking)
            {
                if (gaugeFill.fillAmount <= .05f)
                {
                    gaugeFill.fillAmount = 0;

                    if (GameManager.Instance.GameState == GameState.Play)
                    {
                        // 플레이 상태에서만 엔드로 들어가도록
                        GameManager.Instance.SetGameState(GameState.End);
                    }
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