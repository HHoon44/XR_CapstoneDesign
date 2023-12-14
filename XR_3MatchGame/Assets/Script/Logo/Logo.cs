using UnityEngine;
using UnityEngine.SceneManagement;
using XR_3MatchGame.Util;

public class Logo : MonoBehaviour
{
    /// <summary>
    /// 로고에 존재하는 StartBtn에 바인딩할 메서드
    /// </summary>
    public void StartBtn()
    {
        SceneManager.LoadScene(SceneType.FirstLoading.ToString());
    }
}