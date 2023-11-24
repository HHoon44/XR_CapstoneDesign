using UnityEngine;
using UnityEngine.UI;
using XR_3MatchGame.Util;
using XR_3MatchGame_InGame;
using XR_3MatchGame_Resource;

namespace XR_3MatchGame_UI
{
    public class ElementButton : MonoBehaviour
    {
        public ElementType SelectElement;       // ������ ������ ����

        private GameManager GM;
        private Image image;

        private void Start()
        {
            GM = GameManager.Instance;

            image = GetComponent<Image>();

            // �̸��� �ִ� ���ڸ� ���
            var index = int.Parse(name.Split('_')[2]);

            // ���� ������Ʈ�� �ε����� ���� ���� Ÿ�� ����
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
        /// ���� ���� ��ư�� ���ε��� �޼���
        /// </summary>
        public void ChangeElement()
        {
            if (GM.ElementType == SelectElement || GM.GameState != GameState.Play)
            {
                return;
            }

            GM.ElementType = SelectElement;

            // ��ų ������ ����
            UIWindowManager.Instance.GetWindow<UIElement>().SetGauge(GM.ElementType);
        }
    }
}
