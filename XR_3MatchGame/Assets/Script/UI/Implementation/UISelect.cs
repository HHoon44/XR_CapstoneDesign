using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using XR_3MatchGame.Util;
using XR_3MatchGame_InGame;
using XR_3MatchGame_Resource;

namespace XR_3MatchGame
{
    public class UISelect : MonoBehaviour
    {
        [SerializeField]
        public StageDetail stageDetail;

        private GameManager GM;

        [SerializeField]
        private Image firstSelect;

        [SerializeField]
        private Image twoSelect;

        [SerializeField]
        private Image threeSelect;

        private void Start()
        {
            GM = GameManager.Instance;
        }

        public void UpdateImage()
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
            GM.LoadScene(SceneType.InGame);
        }

        public void ReturnButton()
        {
            stageDetail.gameObject.SetActive(false);
        }

        public void FireStageButton()
        {
            stageDetail.gameObject.SetActive(true);

            stageDetail.Initialize("불");
        }

        public void IceStageButton()
        {
            stageDetail.gameObject.SetActive(true);

            stageDetail.Initialize("얼음");
        }

        public void GrassStageButton()
        {
            stageDetail.gameObject.SetActive(true);

            stageDetail.Initialize("풀");
        }

        public void FireSelectButton()
        {
            /// 모든 원소는 비어있는곳에 들어가야한다
            /// 중복이 되서는 안된다
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
            }

            UpdateImage();
        }

        public void DarkSelectButton()
        {
            Debug.Log("어둠 원소를 선택");
        }

        public void LightSelectButton()
        {
            Debug.Log("빛 원소를 선택");
        }

        public void LightningSelectButton()
        {
            Debug.Log("번개 원소를 선택");
        }

        public void FirstSelectButton()
        {
            if (GM.selectType.Count != 0)
            {
                var deleteType = GM.selectType[0];
                GM.selectType.Remove(deleteType);

                UpdateImage();
            }
        }

        public void TwoSelectButton()
        {
            if (GM.selectType.Count != 0)
            {
                // 이러면 카운트가 3일때 가운데가 안들어옴
                if (GM.selectType.Count > 1)
                {
                    var deleteType = GM.selectType[1];
                    GM.selectType.Remove(deleteType);

                    UpdateImage();
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
                }
            }
        }

        #endregion
    }
}