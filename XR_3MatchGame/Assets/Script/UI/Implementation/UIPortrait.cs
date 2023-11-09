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
        /// 원소 타입에 따라 캐릭터 일러스트를 변경하는 메서드
        /// </summary>
        /// <param name="type">원소</param>
        public void SetPortrait(ElementType type)
        {
            // 플레이어의 캐릭터 사진 변경
        }
    }
}
