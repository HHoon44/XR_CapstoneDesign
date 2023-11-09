using UnityEngine;
using XR_3MatchGame.Util;
using XR_3MatchGame_InGame;

namespace XR_3MatchGame_UI
{
    public class ElementButton : MonoBehaviour
    {
        public ElementType SelectElement;       // 유저가 선택한 원소

        private GameManager GM;

        private void Start()
        {
            GM = GameManager.Instance;

            // 현재 오브젝트의 인덱스에 따라서 원소 타입 설정
            switch (transform.GetSiblingIndex())
            {
                case 0:
                    SelectElement = GM.SelectElement_0;
                    break;

                case 1:
                    SelectElement = GM.SelectElement_1;
                    break;

                case 2:
                    SelectElement = GM.SelectElement_2;
                    break;
            }

            // 여기서 스프라이트 설정 하면 될듯
        }

        /// <summary>
        /// 원소 변경 버튼에 바인딩할 메서드
        /// </summary>
        public void ChangeElement()
        {
            if (GM.ElementType == SelectElement)
            {
                // 플레이어의 원소와 버튼의 원소가 같다면 return
                return;
            }

            Debug.Log("원소 변경");

            // 원소 변경
            GM.ElementType = SelectElement;

            // 여기서 스프라이트 설정 및 게임 내 UI 설정하면 될듯
            var uiElement = UIWindowManager.Instance.GetWindow<UIElement>();

            uiElement.OnElementGauge(GM.ElementType);
        }
    }
}
