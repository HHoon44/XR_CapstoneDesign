using TMPro;
using UnityEngine;
using UnityEngine.UI;
using XR_3MatchGame.Util;
using XR_3MatchGame_InGame;
using XR_3MatchGame_Resource;
using XR_3MatchGame_Util;

namespace XR_3MatchGame_UI
{
    public class UIDetail : UIWindow
    {
        [SerializeField]
        private Image icon;

        [SerializeField]
        private TextMeshProUGUI detail;

        [SerializeField]
        private TextMeshProUGUI score;

        public override void Start()
        {
            base.Start();

            SetStageDetail();
        }

        public void SetStageDetail()
        {
            var stageIcon = SpriteLoader.GetSprite(AtlasType.IconAtlas, GameManager.Instance.stageType.ToString());
            icon.sprite = stageIcon;
            detail.text = GameManager.Instance.stageName + "ÀÇ ½Ã·Ã";
        }

        public void SetScore(int score)
        {
            this.score.text = score.ToString();
        }
    }
}
