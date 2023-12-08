using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace XR_3MatchGame_UI
{
    public class UIElement : UIWindow
    {
        public TextMeshProUGUI stateText;       // 스킬 사용 시 현재 스킬의 버프를 알려주는 Text
        public Image gaugeFill;                 // 스킬 게이지 이미지
        public GameObject fullEffect;           // 스킬 게이지가 충전 되었다면 활성화할 이펙트

        public override void Start()
        {
            base.Start();

            Initialize();
        }

        /// <summary>
        /// UI 초기화 메서드
        /// </summary>
        public void Initialize()
        {
            if (fullEffect.activeSelf)
            {
                fullEffect.SetActive(false);
            }

            // 게이지 값 0
            gaugeFill.fillAmount = 0;
        }

        /// <summary>
        /// 게이지 값을 추가하는 메서드
        /// </summary>
        /// <param name="amount"></param>
        public void SetGauge(float amount)
        {
            if (gaugeFill.fillAmount >= 1f)
            {
                // 스킬 게이지 충전 완료
                if (!fullEffect.activeSelf)
                {
                    fullEffect.SetActive(true);

                }

                gaugeFill.fillAmount = 1f;
                return;
            }

            // 매개변수로 받은 값을 더해준다
            gaugeFill.fillAmount += amount;
        }

        /// <summary>
        /// 스킬 게이지 값을 리턴하는 메서드
        /// </summary>
        /// <returns></returns>
        public float GetGauge()
        {
            return gaugeFill.fillAmount;
        }

        /// <summary>
        /// 스킬 사용 시 스킬 효과를 나타내도록 하는 메서드
        /// </summary>
        /// <param name="value"></param>
        public void SetStateText(string value = null)
        {
            if (!stateText.gameObject.activeSelf)
            {
                stateText.gameObject.SetActive(true);
                stateText.text = value;
            }
            else
            {
                stateText.gameObject.SetActive(false);
            }
        }
    }
}