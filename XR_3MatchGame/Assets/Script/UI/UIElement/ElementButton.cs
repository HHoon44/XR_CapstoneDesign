using UnityEngine;
using UnityEngine.UI;
using XR_3MatchGame.Util;
using XR_3MatchGame_InGame;
using XR_3MatchGame_Resource;

namespace XR_3MatchGame_UI
{
    public class ElementButton : MonoBehaviour
    {
        public ElementType SelectElement;       // 유저가 선택한 원소

        private GameManager GM;
        private Image image;

        private void Start()
        {
            GM = GameManager.Instance;

            image = GetComponent<Image>();

            // 이름에 있는 숫자를 사용
            var index = int.Parse(name.Split('_')[2]);

            // 현재 오브젝트의 인덱스에 따라서 원소 타입 설정
            switch (index)
            {
                case 0:
                    SelectElement = GM.selectType[0];
                    break;

                case 1:
                    SelectElement = GM.selectType[1];
                    break;

                case 2:
                    SelectElement = GM.selectType[2];
                    break;
            }

            image.sprite = SpriteLoader.GetSprite(AtlasType.IconAtlas, SelectElement.ToString());
        }

        /// <summary>
        /// 원소 변경 버튼에 바인딩할 메서드
        /// </summary>
        public void ChangeElement()
        {
            if (GM.ElementType == SelectElement || GM.GameState != GameState.Play)
            {
                return;
            }

            GM.ElementType = SelectElement;

            // 스킬 게이지 변경
            UIWindowManager.Instance.GetWindow<UIElement>().SetGauge(GM.ElementType);
        }
    }
}
