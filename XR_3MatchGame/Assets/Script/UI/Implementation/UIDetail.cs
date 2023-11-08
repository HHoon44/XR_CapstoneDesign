using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace XR_3MatchGame_UI
{
    public class UIDetail : UIWindow
    {
        [SerializeField]
        private GameObject stageDetail;
        private Image icon;
        private TextMeshProUGUI detail;


        [SerializeField]
        private GameObject profilDetail;
        private Image portrait;
        private TextMeshProUGUI score;

        public override void Start()
        {
            base.Start();

            // Stage
            icon = stageDetail.transform.GetChild(0).GetComponent<Image>();
            detail = stageDetail.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

            // Profil
            portrait = profilDetail.transform.GetChild(0).GetComponent<Image>();
            score = profilDetail.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        }

        public void SetStateDetail()
        {
            // 스테이지 아이콘 변경
            // 스테이지 설명 변경
        }

        public void SetProfilDetail()
        {
            // 현재 유저 프로필 설정
        }

        public void SetScore(int score)
        {
            // 현재 스코어 업데이트
            this.score.text = score.ToString();
        }
    }
}
