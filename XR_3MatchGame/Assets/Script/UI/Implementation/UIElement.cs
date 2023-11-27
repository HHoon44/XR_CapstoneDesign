using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using TMPro;
using UIHealthAlchemy;
using UnityEngine;
using UnityEngine.UI;
using XR_3MatchGame.Util;
using XR_3MatchGame_Data;
using XR_3MatchGame_InGame;

namespace XR_3MatchGame_UI
{
    public class UIElement : UIWindow
    {
        [SerializeField]
        private TextMeshProUGUI stateText;

        [SerializeField]
        private Image gaugeFill;

        [SerializeField]
        private GameObject fullEffect;

        private GameManager GM;

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