using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using XR_3MatchGame.Util;
using XR_3MatchGame_Data;
using XR_3MatchGame_InGame;
using XR_3MatchGame_Resource;
using Image = UnityEngine.UI.Image;

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
            SetScore();
        }

        public void SetStageDetail()
        {
            var stageIcon = SpriteLoader.GetSprite(AtlasType.IconAtlas, GameManager.Instance.stageType.ToString());
            icon.sprite = stageIcon;
            detail.text = GameManager.Instance.stageName + "ÀÇ ½Ã·Ã";
        }

        public void SetScore()
        {
            score.text = DataManager.Instance.PlayerScore.ToString();
        }
    }
}