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
        private GameObject stageFrame;

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
            // ���� 3�� ���� ���� ����
            if (GM.selectType.Count == 3)
            {
                GM.LoadScene(SceneType.InGame);
                backButton.SetActive(true);
            }
        }

        public void ReturnButton()
        {
            stageFrame.SetActive(false);
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
            stageFrame.SetActive(true);

            backButton.SetActive(false);

            // �������� ���� ����
            var stageDetail = stageFrame.transform.GetChild(0).GetComponent<StageDetail>();
            stageDetail.Initialize("��");

            var detail = stageFrame.transform.GetChild(2).GetComponent<TextMeshProUGUI>();

            if (GM.selectType.Count == 3)
            {
                detail.text = "3���� ���Ҹ� �������� ������ �������ּ���!";
            }
            else
            {
                detail.text = "���Ҹ� �������ּ���!";
            }

            GameManager.Instance.stageType = ElementType.Fire;
            GameManager.Instance.stageName = "��";
        }

        public void IceStageButton()
        {
            stageFrame.SetActive(true);

            backButton.SetActive(false);

            // �������� ���� ����
            var stageDetail = stageFrame.transform.GetChild(0).GetComponent<StageDetail>();
            stageDetail.Initialize("����");

            var detail = stageFrame.transform.GetChild(2).GetComponent<TextMeshProUGUI>();

            if (GM.selectType.Count == 3)
            {
                detail.text = "3���� ���Ҹ� �������� ������ �������ּ���!";
            }
            else
            {
                detail.text = "���Ҹ� �������ּ���!";
            }

            GameManager.Instance.stageType = ElementType.Ice;
            GameManager.Instance.stageName = "����";
        }

        public void GrassStageButton()
        {
            stageFrame.SetActive(true);

            backButton.SetActive(false);

            // �������� ���� ����
            var stageDetail = stageFrame.transform.GetChild(0).GetComponent<StageDetail>();
            stageDetail.Initialize("Ǯ");

            var detail = stageFrame.transform.GetChild(2).GetComponent<TextMeshProUGUI>();

            if (GM.selectType.Count == 3)
            {
                detail.text = "3���� ���Ҹ� �������� ������ �������ּ���!";
            }
            else
            {
                detail.text = "���Ҹ� �������ּ���!";
            }

            GameManager.Instance.stageType = ElementType.Grass;
            GameManager.Instance.stageName = "Ǯ";
        }

        #endregion

        #region ElementSelectButton

        public void FireSelectButton()
        {
            var same = false;

            // �ߺ��� ���Ҹ� ã�� �۾�
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

                var detail = stageFrame.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
                detail.text = "�� ���Ҹ� ���� �ϼ̽��ϴ�!";

                if (GM.selectType.Count == 3)
                {
                    detail.text = "3���� ���Ҹ� �������� ������ �������ּ���!";
                }
            }

            UpdateImage();
        }

        public void IceSelectButton()
        {
            var same = false;

            // �ߺ��� ���Ҹ� ã�� �۾�
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

                var detail = stageFrame.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
                detail.text = "���� ���Ҹ� ���� �ϼ̽��ϴ�!";

                if (GM.selectType.Count == 3)
                {
                    detail.text = "3���� ���Ҹ� �������� ������ �������ּ���!";
                }
            }

            UpdateImage();
        }

        public void GrassSelectButton()
        {
            var same = false;

            // �ߺ��� ���Ҹ� ã�� �۾�
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

                var detail = stageFrame.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
                detail.text = "Ǯ ���Ҹ� ���� �ϼ̽��ϴ�!";

                if (GM.selectType.Count == 3)
                {
                    detail.text = "3���� ���Ҹ� �������� ������ �������ּ���!";
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
                    var detail = stageFrame.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
                    detail.text = "���Ҹ� �������ּ���!";
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
                        var detail = stageFrame.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
                        detail.text = "���Ҹ� �������ּ���!";
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
                        var detail = stageFrame.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
                        detail.text = "���Ҹ� �������ּ���!";
                    }
                }
            }
        }

        #endregion
    }
}