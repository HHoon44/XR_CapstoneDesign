using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace XR_3MatchGame_UI
{
    public class UIElement : UIWindow
    {
        [SerializeField]
        private TextMeshProUGUI stateText;

        [SerializeField]
        private Image gaugeFill;

        public GameObject fullEffect;

        public override void Start()
        {
            base.Start();

            Initialize();
        }

        /// <summary>
        /// UI �ʱ�ȭ �޼���
        /// </summary>
        public void Initialize()
        {
            if (fullEffect.activeSelf)
            {
                fullEffect.SetActive(false);
            }

            gaugeFill.fillAmount = 0;
        }

        /// <summary>
        /// ������ ���� �߰��ϴ� �޼���
        /// </summary>
        /// <param name="amount"></param>
        public void SetGauge(float amount)
        {
            if (gaugeFill.fillAmount >= 1f)
            {
                if (!fullEffect.activeSelf)
                {
                    fullEffect.SetActive(true);
                }

                gaugeFill.fillAmount = 1f;
                return;
            }

            gaugeFill.fillAmount += amount;
        }

        public float GetGauge()
        {
            return gaugeFill.fillAmount;
        }

        /// <summary>
        /// ���� �ؽ�Ʈ�� �����ϴ� �޼���
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