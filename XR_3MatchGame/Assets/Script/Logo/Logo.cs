using UnityEngine;
using UnityEngine.SceneManagement;
using XR_3MatchGame.Util;

public class Logo : MonoBehaviour
{
    /// <summary>
    /// �ΰ� �����ϴ� StartBtn�� ���ε��� �޼���
    /// </summary>
    public void StartBtn()
    {
        SceneManager.LoadScene(SceneType.FirstLoading.ToString());
    }
}