using Mono.Cecil;
using UnityEngine.UI;

namespace XR_3MatchGame_UI
{
    public class UIPortrait : UIWindow
    {
        private Image portrait;

        public override void Start()
        {
            base.Start();

            portrait =  transform.GetChild(0).GetComponent<Image>();
        }

        public void SetPortrait()
        { 
            // �÷��̾��� ĳ���� ���� ����
        }
    }
}
