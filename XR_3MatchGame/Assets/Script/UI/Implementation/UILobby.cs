using TMPro;
using UnityEngine;
using UnityEngine.UI;
using XR_3MatchGame.Util;
using XR_3MatchGame_Data;

namespace XR_3MatchGame_UI
{
    public class UILobby : MonoBehaviour
    {
        [SerializeField]
        private GameObject optionDetail;

        [SerializeField]
        private TextMeshProUGUI coinText;

        #region User Score Object
        [SerializeField]
        private GameObject user_0;

        [SerializeField]
        private GameObject user_1;

        [SerializeField]
        private GameObject user_2;
        #endregion

        private void Start()
        {
            CoinUpdate();
            RankUpdate();
        }

        /// <summary>
        /// �ɼ� ��ư�� ���ε��� �޼���
        /// </summary>
        public void OptionButton()
        {
            optionDetail.SetActive(true);
        }

        /// <summary>
        /// �ɼ� â �ݱ� ��ư�� ���ε��� �޼���
        /// </summary>
        public void CloseButton()
        {
            optionDetail.SetActive(false);
        }

        /// <summary>
        /// ������ ������ �ݾ� ������Ʈ �޼���
        /// </summary>
        private void CoinUpdate()
        {
            // ���� ������Ʈ
            coinText.text = DataManager.Instance.PlayerCoin.ToString();
        }

        /// <summary>
        /// �ְ����� ������Ʈ �޼���
        /// </summary>
        private void RankUpdate()
        {
            #region User_0

            // ������
            var uesrProfil_0 = user_0.transform.GetChild(0).GetComponent<Image>();

            // ���� �̸�
            var userName_0 = user_0.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

            // ����
            var userScore_0 = user_0.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            userScore_0.text = PlayerPrefs.GetInt(StringName.HighScore_0).ToString();

            // Ʈ����
            var userTrophy_0 = user_0.transform.GetChild(3).GetComponent<Image>();

            #endregion

            #region User_1

            var uesrProfil_1 = user_0.transform.GetChild(0).GetComponent<Image>();

            // ���� �̸�
            var userName_1 = user_0.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

            // ����
            var userScore_1 = user_0.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            userScore_1.text = PlayerPrefs.GetInt(StringName.HighScore_0).ToString();

            // Ʈ����
            var userTrophy_1 = user_0.transform.GetChild(3).GetComponent<Image>();

            #endregion

            #region User_2


            var uesrProfil_2 = user_0.transform.GetChild(0).GetComponent<Image>();

            // ���� �̸�
            var userName_2 = user_0.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

            // ����
            var userScore_2 = user_0.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            userScore_1.text = PlayerPrefs.GetInt(StringName.HighScore_0).ToString();

            // Ʈ����
            var userTrophy_2 = user_0.transform.GetChild(3).GetComponent<Image>();

            #endregion

        }
    }
}