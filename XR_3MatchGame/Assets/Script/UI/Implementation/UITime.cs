using UnityEngine;
using UnityEngine.UI;

namespace XR_3MatchGame_UI
{
    public class UITime : UIWindow
    {
        [SerializeField]
        private GameObject timeGauge;
        private Image gaugeFill;

        [SerializeField]
        private Image clock;


        public void SetTimeAmount()
        { 
            // 시간 줄어들도록 하기
        }

        public void SetTime()
        { 
            // 시계 이미지 변경
            // 게이지 색상 변경
        }
    }
}