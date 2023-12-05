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
        //    // 보드 비활성화
        //    GM.Board.gameObject.SetActive(false);

        //    // 게이지 풀 이펙트 비활성화
        //    if (UIWindowManager.Instance.GetWindow<UIElement>().fullEffect.activeSelf)
        //    {
        //        UIWindowManager.Instance.GetWindow<UIElement>().fullEffect.SetActive(false);
        //    }


        //    // End UI 활성화
        //    uiEnd.gameObject.SetActive(true);

        //    // 음악정지
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
        // 이름 저장
        DataManager.Instance.SetName(uiEnd.GetComponent<UIEnd>().nameField.text);

        // 점수 갱신
        uiEnd.GetComponent<UIEnd>().HighScore();
    }
}