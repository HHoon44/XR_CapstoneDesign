using TMPro;
using UnityEngine.SceneManagement;
using XR_3MatchGame.Util;
using XR_3MatchGame_Data;
using XR_3MatchGame_InGame;

namespace XR_3MatchGame_UI
{
    public class UIEnd : UIWindow
    {
        public TextMeshProUGUI score;

        public override void Start()
        {
            base.Start();

            // ���� ���ھ�� �̰����� ���⼭ �ۼ�
            score.text = DataManager.Instance.PlayerScore.ToString();
        }

        public void ReStartBtn()
        {
            // ���� �����
        }

        public void ReturnBtn()
        {
            // ���������� ���ư���
        }
    }
}