using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XR_3MatchGame.Util;
using XR_3MatchGame_InGame;
using XR_3MatchGame_Resource;

namespace XR_3MatchGame
{
    public class UISelect : MonoBehaviour
    {
        [SerializeField]
        private GameObject frame;

        [SerializeField]
        private GameObject backButton;

        [SerializeField]
        private Image firstSelect;

        [SerializeField]
        private Image twoSelect;

        [SerializeField]
        private Image threeSelect;

        private GameManager GM;

        private void Start()
        {
            GM = GameManager.Instance;

            // 선택씬 BGM 설정
            SoundManager.Instance.Initialize(SceneType.Select);
        }

        private void UpdateImage()
        {
            if (GM.selectType.Count != 0)
            {
                if (GM.selectType.Count == 1)
                {
                    var sprite_0 = SpriteLoader.GetSprite(AtlasType.IconAtlas, GM.selectType[0].ToString());
                    firstSelect.sprite = sprite_0;

                    var sprite_1 = SpriteLoader.GetSprite(AtlasType.IconAtlas, ElementType.Balance.ToString());
                    twoSelect.sprite = sprite_1;

                    var sprite_2 = SpriteLoader.GetSprite(AtlasType.IconAtlas, ElementType.Balance.ToString());
                    threeSelect.sprite = sprite_2;
                }
                else if (GM.selectType.Count == 2)
                {
                    var sprite_0 = SpriteLoader.GetSprite(AtlasType.IconAtlas, GM.selectType[0].ToString());
                    firstSelect.sprite = sprite_0;

                    var sprite_1 = SpriteLoader.GetSprite(AtlasType.IconAtlas, GM.selectType[1].ToString());
                    twoSelect.sprite = sprite_1;

                    var sprite_2 = SpriteLoader.GetSprite(AtlasType.IconAtlas, ElementType.Balance.ToString());
                    threeSelect.sprite = sprite_2;
                }
                else if (GM.selectType.Count == 3)
                {
                    var sprite_0 = SpriteLoader.GetSprite(AtlasType.IconAtlas, GM.selectType[0].ToString());
                    firstSelect.sprite = sprite_0;

                    var sprite_1 = SpriteLoader.GetSprite(AtlasType.IconAtlas, GM.selectType[1].ToString());
                    twoSelect.sprite = sprite_1;

                    var sprite_2 = SpriteLoader.GetSprite(AtlasType.IconAtlas, GM.selectType[2].ToString());
                    threeSelect.sprite = sprite_2;
                }
            }
            else
            {
                var sprite_0 = SpriteLoader.GetSprite(AtlasType.IconAtlas, ElementType.Balance.ToString());
                firstSelect.sprite = sprite_0;

                var sprite_1 = SpriteLoader.GetSprite(AtlasType.IconAtlas, ElementType.Balance.ToString());
                twoSelect.sprite = sprite_1;

                var sprite_2 = SpriteLoader.GetSprite(AtlasType.IconAtlas, ElementType.Balance.ToString());
                threeSelect.sprite = sprite_2;
            }
        }

        #region ButtonFun

        public void StartButton()
        {
            // 원소 3개 고르면 게임 시작
            if (GM.selectType.Count == 3)
            {
                GM.LoadScene(SceneType.InGame);
                backButton.SetActive(true);
            }
        }

        public void ReturnButton()
        {
            // 창 비활성화
            frame.SetActive(false);
            backButton.SetActive(true);
        }

        public void BackButton()
        {
            SceneManager.LoadScene(SceneType.Lobby.ToString());
        }

        #endregion

        #region StageButtonFun

        public void FireStageButton()
        {
            // 창 활성하
            frame.SetActive(true);

            backButton.SetActive(false);

            // 스테이지 정보 세팅
            var stageDetail = frame.transform.GetChild(0).GetComponent<StageDetail>();
            stageDetail.Initialize("불");

            var detail = frame.transform.GetChild(2).GetComponent<TextMeshProUGUI>();

            if (GM.selectType.Count == 3)
            {
                detail.text = "3가지 선택완료! 게임시작!";
            }
            else
            {
                detail.text = "3가지 원소를 선택해주세요!";
            }

            GameManager.Instance.stageType = ElementType.Fire;
            GameManager.Instance.stageName = "Fire";
        }

        public void IceStageButton()
        {
            frame.SetActive(true);

            backButton.SetActive(false);

            // 스테이지 정보 세팅
            var stageDetail = frame.transform.GetChild(0).GetComponent<StageDetail>();
            stageDetail.Initialize("얼음");

            var detail = frame.transform.GetChild(2).GetComponent<TextMeshProUGUI>();

            if (GM.selectType.Count == 3)
            {
                detail.text = "3가지 선택완료! 게임시작!";
            }
            else
            {
                detail.text = "3가지 원소를 선택해주세요!";
            }

            GameManager.Instance.stageType = ElementType.Ice;
            GameManager.Instance.stageName = "Ice";
        }

        public void GrassStageButton()
        {
            frame.SetActive(true);

            backButton.SetActive(false);

            // 스테이지 정보 세팅
            var stageDetail = frame.transform.GetChild(0).GetComponent<StageDetail>();
            stageDetail.Initialize("풀");

            var detail = frame.transform.GetChild(2).GetComponent<TextMeshProUGUI>();

            if (GM.selectType.Count == 3)
            {
                detail.text = "3가지 선택완료! 게임시작!";
            }
            else
            {
                detail.text = "3가지 원소를 선택해주세요!";
            }

            GameManager.Instance.stageType = ElementType.Grass;
            GameManager.Instance.stageName = "Grass";
        }

        #endregion

        #region ElementSelectButton

        public void FireSelectButton()
        {
            var same = false;

            // 중복된 원소를 찾는 작업
            if (GM.selectType.Count < 4 && GM.selectType.Count != 0)
            {
                for (int i = 0; i < GM.selectType.Count; i++)
                {
                    if (same == false)
                    {
                        if (GM.selectType[i] == ElementType.Fire)
                        {
                            same = true;
                        }
                    }
                }
            }

            if (same == false)
            {
                GM.selectType.Add(ElementType.Fire);

                var detail = frame.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
                detail.text = "불 원소를 선택 하셨습니다!";

                if (GM.selectType.Count == 3)
                {
                    detail.text = "3가지 선택완료! 게임시작!";
                }
            }

            UpdateImage();
        }

        public void IceSelectButton()
        {
            var same = false;

            // 중복된 원소를 찾는 작업
            if (GM.selectType.Count < 4 && GM.selectType.Count != 0)
            {
                for (int i = 0; i < GM.selectType.Count; i++)
                {
                    if (same == false)
                    {
                        if (GM.selectType[i] == ElementType.Ice)
                        {
                            same = true;
                        }
                    }
                }
            }

            if (same == false)
            {
                GM.selectType.Add(ElementType.Ice);

                var detail = frame.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
                detail.text = "얼음 원소를 선택 하셨습니다!";

                if (GM.selectType.Count == 3)
                {
                    detail.text = "3가지 선택완료! 게임시작!ㄴ";
                }
            }

            UpdateImage();
        }

        public void GrassSelectButton()
        {
            var same = false;

            // 중복된 원소를 찾는 작업
            if (GM.selectType.Count < 4 && GM.selectType.Count != 0)
            {
                for (int i = 0; i < GM.selectType.Count; i++)
                {
                    if (same == false)
                    {
                        if (GM.selectType[i] == ElementType.Grass)
                        {
                            same = true;
                        }
                    }
                }
            }

            if (same == false)
            {
                GM.selectType.Add(ElementType.Grass);

                var detail = frame.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
                detail.text = "풀 원소를 선택 하셨습니다!";

                if (GM.selectType.Count == 3)
                {
                    detail.text = "3가지 선택완료! 게임시작!";
                }
            }

            UpdateImage();
        }

        public void DarkSelectButton()
        {
        }

        public void LightSelectButton()
        {
        }

        public void LightningSelectButton()
        {
        }

        #endregion

        #region SelectButton

        public void FirstSelectButton()
        {
            if (GM.selectType.Count != 0)
            {
                var deleteType = GM.selectType[0];
                GM.selectType.Remove(deleteType);

                UpdateImage();

                if (GM.selectType.Count != 3)
                {
                    var detail = frame.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
                    detail.text = "3가지 원소를 선택해주세요!";
                }
            }
        }

        public void TwoSelectButton()
        {
            if (GM.selectType.Count != 0)
            {
                if (GM.selectType.Count > 1)
                {
                    var deleteType = GM.selectType[1];
                    GM.selectType.Remove(deleteType);

                    UpdateImage();

                    if (GM.selectType.Count != 3)
                    {
                        var detail = frame.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
                        detail.text = "3가지 원소를 선택해주세요!";
                    }
                }
            }
        }

        public void ThreeSelectButton()
        {
            if (GM.selectType.Count != 0)
            {
                if (GM.selectType.Count > 2)
                {
                    var deleteType = GM.selectType[2];
                    GM.selectType.Remove(deleteType);

                    UpdateImage();

                    if (GM.selectType.Count != 3)
                    {
                        var detail = frame.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
                        detail.text = "3가지 원소를 선택해주세요!";
                    }
                }
            }
        }

        #endregion
    }
}