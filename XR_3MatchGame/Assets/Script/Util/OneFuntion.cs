using UnityEngine;
using UnityEngine.SceneManagement;
using XR_3MatchGame.Util;

public class OneFuntion : MonoBehaviour
{
    /// <summary>
    /// �ΰ� ��ư�� ���ε��� �޼���
    /// </summary>
    public void LobbyLoad()
    {
        SceneManager.LoadScene(SceneType.FirstLoading.ToString());
    }

    /// <summary>
    /// �������� ����â�� �ε��ϴ� �޼���
    /// </summary>
    public void SelectLoad()
    {
        SceneManager.LoadScene(SceneType.Select.ToString());
    }

    /// <summary>
    /// �������� ����â���� ���ӽ��� ��ư�� ���ε��� �޼���
    /// </summary>
    public void StageLoad()
    {
        SceneManager.LoadScene(SceneType.InGame.ToString());
    }
}