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
            // �������� ������ ����
            // �������� ���� ����
        }

        public void SetProfilDetail()
        {
            // ���� ���� ������ ����
        }

        public void SetScore(int score)
        {
            // ���� ���ھ� ������Ʈ
            this.score.text = score.ToString();
        }
    }
}
