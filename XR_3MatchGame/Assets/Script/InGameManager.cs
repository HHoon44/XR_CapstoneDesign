using UnityEngine;
using XR_3MatchGame.Util;
using XR_3MatchGame_Data;
using XR_3MatchGame_InGame;
using XR_3MatchGame_Object;
using XR_3MatchGame_UI;
using XR_3MatchGame_Util;

public class InGameManager : MonoBehaviour
{
    public GameObject board;
    public GameObject uiEnd;

    private GameManager GM;

    private void Start()
    {
        GM = GameManager.Instance;
        GM.inGameManager = this;
    }

    public void Initiazlie()
    {
        // ���� ��Ȱ��ȭ
        GM.Board.gameObject.SetActive(false);

        // End UI Ȱ��ȭ
        uiEnd.gameObject.SetActive(true);
    }

    public void InputPlayerName()
    {
        // �̸� ����
        DataManager.Instance.SetName(uiEnd.GetComponent<UIEnd>().nameField.text);

        // ���� ����
        uiEnd.GetComponent<UIEnd>().HighScore();
    }
}