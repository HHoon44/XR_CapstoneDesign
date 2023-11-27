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
        /// UI 초기화 메서드
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
        /// 게이지 값을 추가하는 메서드
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
        /// 상태 텍스트를 세팅하는 메서드
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