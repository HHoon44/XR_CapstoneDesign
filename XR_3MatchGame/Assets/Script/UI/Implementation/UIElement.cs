using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace XR_3MatchGame_UI
{
    public class UIElement : UIWindow
    {
        public TextMeshProUGUI stateText;       // ��ų ��� �� ���� ��ų�� ������ �˷��ִ� Text
        public Image gaugeFill;                 // ��ų ������ �̹���
        public GameObject fullEffect;           // ��ų �������� ���� �Ǿ��ٸ� Ȱ��ȭ�� ����Ʈ

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

            // ������ �� 0
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
                // ��ų ������ ���� �Ϸ�
                if (!fullEffect.activeSelf)
                {
                    fullEffect.SetActive(true);

                }

                gaugeFill.fillAmount = 1f;
                return;
            }

            // �Ű������� ���� ���� �����ش�
            gaugeFill.fillAmount += amount;
        }

        /// <summary>
        /// ��ų ������ ���� �����ϴ� �޼���
        /// </summary>
        /// <returns></returns>
        public float GetGauge()
        {
            return gaugeFill.fillAmount;
        }

        /// <summary>
        /// ��ų ��� �� ��ų ȿ���� ��Ÿ������ �ϴ� �޼���
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