using UnityEngine;
using UnityEngine.SceneManagement;
using XR_3MatchGame.Util;

public class OneFuntion : MonoBehaviour
{
    #region Logo

    /// <summary>
    /// 로고 버튼에 바인딩할 메서드
    /// </summary>
    public void LobbyLoad()
    {
        SceneManager.LoadScene(SceneType.FirstLoading.ToString());
    }

    #endregion

    #region Lobby

    /// <summary>
    /// 스테이지 선택창을 로드하는 메서드
    /// </summary>
    public void SelectLoad()
    {
        SceneManager.LoadScene(SceneType.Select.ToString());
    }

    #endregion

    /// <summary>
    /// 스테이지 선택창에서 게임시작 버튼에 바인딩할 메서드
    /// </summary>
    public void StageLoad()
    {
        SceneManager.LoadScene(SceneType.InGame.ToString());
    }
}