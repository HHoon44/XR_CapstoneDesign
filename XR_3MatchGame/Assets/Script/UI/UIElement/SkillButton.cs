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
        public ElementType btnElement;          // 현재 버튼의 원소

        public Image icon;
        public GameObject skillEffectObj;       // 스킬 이펙트 부모 오브젝트
        public GameObject backBlack;            // 스킬 사용 시 뒷 배경

        private ParticleSystem magicCircle;     // 스킬 이펙트 자식 오브젝트 1
        private ParticleSystem magicEffect;     // 스킬 이펙트 자식 오브젝트 2

        private GameManager GM;                 // 현 로직에서 사용할 게임 매니저

        private void Start()
        {
            GM = GameManager.Instance;

            Initialize();
        }

        /// <summary>
        /// 초기 세팅 메서드
        /// </summary>
        public void Initialize()
        {
            int index = int.Parse(name.Split('_')[1]);

            // 현재 오브젝트 이름 뒤에 있는 인덱스 값을 이용하여
            // 유저가 선택한 원소 데이터에 접근한다
            switch (index)
            {
                case 0:
                    // 0번째에 저장 되어있는 원소 데이터를 가져온다
                    btnElement = GM.selectType[0];

                    for (int i = 0; i < skillEffectObj.transform.childCount; i++)
                    {
                        var name = skillEffectObj.transform.GetChild(i).name.Split('_')[0];

                        if (btnElement.ToString() == name)
                        {
                            // 가져온 원소 데이터의 이름과 접근한 오브젝트의 이름이 같다면
                            // 이펙트 오브젝트에 접근한다
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

            // 스킬 버튼에 원소 Icon을 업데이트 한다
            icon.sprite = SpriteLoader.GetSprite(AtlasType.IconAtlas, btnElement.ToString());
        }

        /// <summary>
        /// 스킬 버튼에 바인딩될 메서드
        /// </summary>
        public void SkillBtn()
        {
            var uiElement = UIWindowManager.Instance.GetWindow<UIElement>();

            // 현재 게임 상태가  Play 상태일때만 스킬 사용 가능
            if (uiElement.GetGauge() >= 1f && GameManager.Instance.GameState == GameState.Play)
            {
                // 검은 배경 활성화
                backBlack.SetActive(true);

                // 스킬 게이지 0으로 초기화
                uiElement.Initialize();

                // GameState -> Skill
                GM.SetGameState(GameState.SKill);

                // 스킬을 시작합니다
                StartCoroutine(StartSkill());
            }
        }

        /// <summary>
        /// 스킬을 발동하는 코루틴
        /// </summary>
        /// <returns></returns>
        private IEnumerator StartSkill()
        {
            /// 불 - 얼음 - 풀
            /// 모두 공통적으로 특정 지역의 블록을 파괴 하도록 한다



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