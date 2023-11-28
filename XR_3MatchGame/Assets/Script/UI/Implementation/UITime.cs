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
            // �� üŷ�߿��� �ð��� ���ߵ���
            if (GameManager.Instance.GameState == GameState.Play)
            {
                if (gaugeFill.fillAmount <= .05f)
                {
                    gaugeFill.fillAmount = 0;

                    // ���� ���� �� ���� ���� ���� ����
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
        /// �ð��� ���ϴ� �޼���
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