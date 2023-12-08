using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using XR_3MatchGame.Util;
using XR_3MatchGame_Data;
using XR_3MatchGame_InGame;
using XR_3MatchGame_Object;
using XR_3MatchGame_Resource;
using XR_3MatchGame_Util;

namespace XR_3MatchGame_UI
{
    public class SkillButton : MonoBehaviour
    {
        public ElementType btnElement;          // ���� ��ư�� ����

        public Image icon;
        public GameObject skillEffectObj;       // ��ų ����Ʈ �θ� ������Ʈ
        public GameObject backBlack;            // ��ų ��� �� �� ���

        private ParticleSystem magicCircle;     // ��ų ����Ʈ �ڽ� ������Ʈ 1
        private ParticleSystem magicEffect;     // ��ų ����Ʈ �ڽ� ������Ʈ 2

        private GameManager GM;                 // �� �������� ����� ���� �Ŵ���

        private void Start()
        {
            GM = GameManager.Instance;

            Initialize();
        }

        /// <summary>
        /// �ʱ� ���� �޼���
        /// </summary>
        public void Initialize()
        {
            int index = int.Parse(name.Split('_')[1]);

            // ���� ������Ʈ �̸� �ڿ� �ִ� �ε��� ���� �̿��Ͽ�
            // ������ ������ ���� �����Ϳ� �����Ѵ�
            switch (index)
            {
                case 0:
                    // 0��°�� ���� �Ǿ��ִ� ���� �����͸� �����´�
                    btnElement = GM.selectType[0];

                    for (int i = 0; i < skillEffectObj.transform.childCount; i++)
                    {
                        var name = skillEffectObj.transform.GetChild(i).name.Split('_')[0];

                        if (btnElement.ToString() == name)
                        {
                            // ������ ���� �������� �̸��� ������ ������Ʈ�� �̸��� ���ٸ�
                            // ����Ʈ ������Ʈ�� �����Ѵ�
                            var obj = skillEffectObj.transform.GetChild(i).gameObject;

                            magicCircle = obj.transform.GetChild(0).GetComponent<ParticleSystem>();
                            magicEffect = obj.transform.GetChild(1).GetComponent<ParticleSystem>();
                        }
                    }

                    break;

                case 1:
                    btnElement = GM.selectType[1];

                    for (int i = 0; i < skillEffectObj.transform.childCount; i++)
                    {
                        var name = skillEffectObj.transform.GetChild(i).name.Split('_')[0];

                        if (btnElement.ToString() == name)
                        {
                            var obj = skillEffectObj.transform.GetChild(i).gameObject;

                            magicCircle = obj.transform.GetChild(0).GetComponent<ParticleSystem>();
                            magicEffect = obj.transform.GetChild(1).GetComponent<ParticleSystem>();
                        }
                    }

                    break;

                case 2:
                    btnElement = GM.selectType[2];

                    for (int i = 0; i < skillEffectObj.transform.childCount; i++)
                    {
                        var name = skillEffectObj.transform.GetChild(i).name.Split('_')[0];

                        if (btnElement.ToString() == name)
                        {
                            var obj = skillEffectObj.transform.GetChild(i).gameObject;

                            magicCircle = obj.transform.GetChild(0).GetComponent<ParticleSystem>();
                            magicEffect = obj.transform.GetChild(1).GetComponent<ParticleSystem>();
                        }
                    }

                    break;
            }

            // ��ų ��ư�� ���� Icon�� ������Ʈ �Ѵ�
            icon.sprite = SpriteLoader.GetSprite(AtlasType.IconAtlas, btnElement.ToString());
        }

        /// <summary>
        /// ��ų ��ư�� ���ε��� �޼���
        /// </summary>
        public void SkillBtn()
        {
            var uiElement = UIWindowManager.Instance.GetWindow<UIElement>();

            // ���� ���� ���°�  Play �����϶��� ��ų ��� ����
            if (uiElement.GetGauge() >= 1f && GameManager.Instance.GameState == GameState.Play)
            {
                // ���� ��� Ȱ��ȭ
                backBlack.SetActive(true);

                // ��ų ������ 0���� �ʱ�ȭ
                uiElement.Initialize();

                // GameState -> Skill
                GM.SetGameState(GameState.SKill);

                // ��ų�� �����մϴ�
                StartCoroutine(StartSkill());
            }
        }

        /// <summary>
        /// ��ų�� �ߵ��ϴ� �ڷ�ƾ
        /// </summary>
        /// <returns></returns>
        private IEnumerator StartSkill()
        {
            /// �� - ���� - Ǯ
            /// ��� ���������� Ư�� ������ ����� �ı� �ϵ��� �Ѵ�



            switch (btnElement)
            {
                case ElementType.Fire:
                    break;

                case ElementType.Ice:
                    break;

                case ElementType.Grass:
                    break;

                case ElementType.Dark:
                    break;

                case ElementType.Light:
                    break;

                case ElementType.Lightning:
                    break;
            }



            yield return null;
        }
    }
}