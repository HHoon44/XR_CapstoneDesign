using UnityEngine;
using XR_3MatchGame.Util;
using XR_3MatchGame_InGame;

namespace XR_3MatchGame_UI
{
    public class ElementButton : MonoBehaviour
    {
        public ElementType SelectElement;       // ������ ������ ����

        private GameManager GM;

        private void Start()
        {
            GM = GameManager.Instance;

            // ���� ������Ʈ�� �ε����� ���� ���� Ÿ�� ����
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

            // ���⼭ ��������Ʈ ���� �ϸ� �ɵ�
        }

        /// <summary>
        /// ���� ���� ��ư�� ���ε��� �޼���
        /// </summary>
        public void ChangeElement()
        {
            if (GM.ElementType == SelectElement)
            {
                // �÷��̾��� ���ҿ� ��ư�� ���Ұ� ���ٸ� return
                return;
            }

            Debug.Log("���� ����");

            // ���� ����
            GM.ElementType = SelectElement;

            // ���⼭ ��������Ʈ ���� �� ���� �� UI �����ϸ� �ɵ�
            var uiElement = UIWindowManager.Instance.GetWindow<UIElement>();

            uiElement.OnElementGauge(GM.ElementType);
        }
    }
}
