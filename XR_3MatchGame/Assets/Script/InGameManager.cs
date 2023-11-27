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
        // StartCoroutine(GameEnd());

        var GM = GameManager.Instance;
        var blocks = GM.Board.blocks;
        var blockPool = ObjectPoolManager.Instance.GetPool<Block>(PoolType.Block);

        if (GM.GameState == GameState.End)
        {
            GM.Board.gameObject.SetActive(false);

            uiEnd.gameObject.SetActive(true);

            for (int i = 0; i < blocks.Count; i++)
            {
                //   blocks[i].BlockParticle();
                blockPool.ReturnPoolableObject(blocks[i]);
            }

            blocks.Clear();

            gameObject.SetActive(false);
        }
    }

    public void InputPlayerName()
    {
        // 이름 저장
        DataManager.Instance.SetName(uiEnd.GetComponent<UIEnd>().nameField.text);

        // 점수 갱신
        uiEnd.GetComponent<UIEnd>().HighScore();
    }
}