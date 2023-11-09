using UnityEngine.UI;
using XR_3MatchGame.Util;
using XR_3MatchGame_InGame;

namespace XR_3MatchGame_UI
{
    public class UIPortrait : UIWindow
    {
        private Image portrait;
        private GameManager GM;

        public override void Start()
        {
            base.Start();

            portrait = transform.GetChild(0).GetComponent<Image>();
            GM = GameManager.Instance;
            SetPortrait(GM.ElementType);
        }

        /// <summary>
        /// ���� Ÿ�Կ� ���� ĳ���� �Ϸ���Ʈ�� �����ϴ� �޼���
        /// </summary>
        /// <param name="type">����</param>
        public void SetPortrait(ElementType type)
        {
            // �÷��̾��� ĳ���� ���� ����
        }
    }
}
