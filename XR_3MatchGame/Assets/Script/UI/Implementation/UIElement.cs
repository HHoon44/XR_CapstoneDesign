using System.Collections;
using System.ComponentModel;
using System.Linq.Expressions;
using TMPro;
using UIHealthAlchemy;
using UnityEngine;
using XR_3MatchGame.Util;
using XR_3MatchGame_Data;
using XR_3MatchGame_InGame;
using XR_3MatchGame_Object;
using XR_3MatchGame_Util;

namespace XR_3MatchGame_UI
{
    public class UIElement : UIWindow
    {
        public SkillGauge currentGauge;

        [SerializeField]
        private TextMeshProUGUI stateText;

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
            GM = GameManager.Instance;

            DataManager.Instance.saveValue = 0;

            SetGauge(GM.ElementType);
        }

        /// <summary>
        /// ��ų ������ Ȱ��ȭ �޼���
        /// </summary>
        public void SetGauge(ElementType type)
        {
            if (currentGauge != null)
            {
                currentGauge.gameObject.SetActive(false);
            }

            switch (type)
            {
                case ElementType.Fire:
                    currentGauge = transform.GetChild(0).GetComponent<SkillGauge>();
                    break;

                case ElementType.Ice:
                    currentGauge = transform.GetChild(1).GetComponent<SkillGauge>();
                    break;

                case ElementType.Grass:
                    currentGauge = transform.GetChild(2).GetComponent<SkillGauge>();
                    break;
            }

            currentGauge.gameObject.SetActive(true);
            currentGauge.Initialize();
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