using UnityEngine;
using UnityEngine.SceneManagement;
using XR_3MatchGame.Util;

public class OneFuntion : MonoBehaviour
{
    #region Logo

    /// <summary>
    /// �ΰ� ��ư�� ���ε��� �޼���
    /// </summary>
    public void LobbyLoad()
    {
        SceneManager.LoadScene(SceneType.FirstLoading.ToString());
    }

    #endregion

    #region Lobby

    /// <summary>
    /// �������� ����â�� �ε��ϴ� �޼���
    /// </summary>
    public void SelectLoad()
    {
        SceneManager.LoadScene(SceneType.Select.ToString());
    }

    #endregion

    /// <summary>
    /// �������� ����â���� ���ӽ��� ��ư�� ���ε��� �޼���
    /// </summary>
    public void StageLoad()
    {
        SceneManager.LoadScene(SceneType.InGame.ToString());
    }
}