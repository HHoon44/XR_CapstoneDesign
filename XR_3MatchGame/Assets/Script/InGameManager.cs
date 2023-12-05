using UnityEngine;
using XR_3MatchGame.Util;
using XR_3MatchGame_Data;
using XR_3MatchGame_InGame;
using XR_3MatchGame_Object;
using XR_3MatchGame_UI;
using XR_3MatchGame_Util;

public class InGameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject board;

    [SerializeField]
    private GameObject uiEnd;

    public void Initiazlie()
    {
        var GM = GameManager.Instance;
        var blocks = GM.Board.blocks;
        var blockPool = ObjectPoolManager.Instance.GetPool<Block>(PoolType.Block);

        //if (GM.GameState == GameState.End)
        //{
        //    // ���� ��Ȱ��ȭ
        //    GM.Board.gameObject.SetActive(false);

        //    // ������ Ǯ ����Ʈ ��Ȱ��ȭ
        //    if (UIWindowManager.Instance.GetWindow<UIElement>().fullEffect.activeSelf)
        //    {
        //        UIWindowManager.Instance.GetWindow<UIElement>().fullEffect.SetActive(false);
        //    }


        //    // End UI Ȱ��ȭ
        //    uiEnd.gameObject.SetActive(true);

        //    // ��������
        //    SoundManager.Instance.AllStop();

        //    for (int i = 0; i < blocks.Count; i++)
        //    {
        //        blockPool.ReturnPoolableObject(blocks[i]);
        //    }

        //    blocks.Clear();
        //}
    }

    public void InputPlayerName()
    {
        // �̸� ����
        DataManager.Instance.SetName(uiEnd.GetComponent<UIEnd>().nameField.text);

        // ���� ����
        uiEnd.GetComponent<UIEnd>().HighScore();
    }
}