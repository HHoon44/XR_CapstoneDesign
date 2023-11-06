using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using XR_3MatchGame.Util;

namespace XR_3MatchGame_InGame
{
    public class LobbyManager : MonoBehaviour
    {
        public void ButtonOption()
        {

            switch (EventSystem.current.currentSelectedGameObject.transform.GetSiblingIndex())
            {
                // ���� ��ư
                case 0:
                    Debug.Log("���� �Դϴ�.");
                    break;

                // ���� ��ư
                case 1:
                    Debug.Log("���� �Դϴ�.");
                    break;
            }
        }

        /// <summary>
        /// �������� ����â�� �ε��ϴ� �޼���
        /// </summary>
        public void StageSelect()
        {
            SceneManager.LoadScene(SceneType.Select.ToString());
        }
    }
}
