using UnityEngine;
using UnityEngine.SceneManagement;
using XR_3MatchGame.Util;

namespace XR_3MatchGame
{
    public class Logo : MonoBehaviour
    {
        /// <summary>
        /// �ΰ� �ִϸ��̼ǿ� ���ε��� �޼���
        /// </summary>
        public void LogoAnimFun()
        {
            SceneManager.LoadScene(SceneType.Main.ToString());
        }
    }
}