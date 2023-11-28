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

            Initialize();
        }

        /// <summary>
        /// 스테이지 초기 정보 세팅 메서드
        /// </summary>
        public void Initialize()
        {
            // 스테이지 아이콘 + 정보 세팅
            var stageIcon = SpriteLoader.GetSprite(AtlasType.IconAtlas, GameManager.Instance.stageType.ToString());

            icon.sprite = stageIcon;
            detail.text = GameManager.Instance.stageName + " STAGE";

            // 점수 세팅
            SetScore();
        }

        //public void SetStageDetail()
        //{
        //    var stageIcon = SpriteLoader.GetSprite(AtlasType.IconAtlas, GameManager.Instance.stageType.ToString());
        //    icon.sprite = stageIcon;
        //    detail.text = GameManager.Instance.stageName + "의 시련";
        //}

        /// <summary>
        /// 스코어 업데이트 메서드
        /// </summary>
        public void SetScore()
        {
            score.text = DataManager.Instance.PlayerScore.ToString();
        }
    }
}