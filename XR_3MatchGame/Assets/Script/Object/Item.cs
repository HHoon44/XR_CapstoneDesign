using System.Collections;
using TMPro;
using UnityEngine;
using XR_3MatchGame.Util;
using XR_3MatchGame_Data;
using XR_3MatchGame_InGame;
using XR_3MatchGame_Object;
using XR_3MatchGame_UI;
using XR_3MatchGame_Util;

public class Item : MonoBehaviour
{
    public ItemType itemType;       // 현재 아이템 타입
    public int itemCount;           // 현재 아이템 개수

    [SerializeField]
    private TextMeshProUGUI countText;

    [SerializeField]
    private ParticleSystem itemEffect;

    public bool isItem;

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        var name = this.name.Split('_')[0];

        // 분기문으로 아이템 타입이랑 스프라이트 설정
        switch (name)
        {
            case "Boom":
                itemCount = 3;
                itemType = ItemType.Boom;
                countText.text = itemCount.ToString();
                break;

            case "Time":
                itemCount = 4;
                itemType = ItemType.Time;
                countText.text = itemCount.ToString();
                break;

            case "Skill":
                itemCount = 4;
                itemType = ItemType.Skill;
                countText.text = itemCount.ToString();
                break;
        }
    }

    /// <summary>
    /// 아이템 버튼에 바인딩할 메서드
    /// </summary>
    public void ItemButton()
    {
        if (itemCount <= 0)
        {
            return;
        }

        if (isItem == true)
        {
            return;
        }

        if (GameManager.Instance.GameState == GameState.Play)
        {
            GameManager.Instance.SetGameState(GameState.Item);

            isItem = true;

            itemEffect.Play();

            // 분기문에 따라 아이템 능력 사용
            switch (itemType)
            {
                case ItemType.Boom:
                    Debug.Log("폭탄 아이템");

                    // 개수 설정
                    itemCount--;
                    countText.text = itemCount.ToString();

                    // 아이템 효과 -> 한줄 파괴
                    // Col = 3
                    // Row = 0 ~ 6

                    StartCoroutine(BoomItem());

                    IEnumerator BoomItem()
                    {
                        yield return new WaitForSeconds(.5f);

                        var blocks = GameManager.Instance.Board.blocks;
                        var delBlocks = GameManager.Instance.Board.delBlocks;
                        var pool = ObjectPoolManager.Instance.GetPool<Block>(PoolType.Block);

                        // 파괴할 블럭 찾기
                        for (int i = 0; i < blocks.Count; i++)
                        {
                            if (blocks[i].col == 3 && (blocks[i].row == 0 || blocks[i].row == 1 || blocks[i].row == 2 ||
                                blocks[i].row == 3 || blocks[i].row == 4 || blocks[i].row == 5 || blocks[i].row == 6))
                            {
                                delBlocks.Add(blocks[i]);
                            }
                        }

                        // 파티클 실행
                        for (int i = 0; i < delBlocks.Count; i++)
                        {
                            delBlocks[i].BlockParticle();
                        }

                        yield return new WaitForSeconds(.3f);

                        // 블럭 파괴
                        for (int i = 0; i < delBlocks.Count; i++)
                        {
                            pool.ReturnPoolableObject(delBlocks[i]);
                            blocks.Remove(delBlocks[i]);
                            DataManager.Instance.SetScore(delBlocks[i].BlockScore);
                        }

                        yield return new WaitForSeconds(.5f);

                        // 빈 자리에 블럭 채워넣기
                        for (int i = 0; i < GameManager.Instance.BoardSize.y; i++)
                        {
                            // 새로운 블럭 생성 로직 작성
                            var newBlock = pool.GetPoolableObject(obj => obj.CanRecycle);
                            newBlock.transform.position = new Vector3(3, GameManager.Instance.BoardSize.y, 0);
                            newBlock.gameObject.SetActive(true);
                            newBlock.Initialize(3, GameManager.Instance.BoardSize.y);
                            blocks.Add(newBlock);

                            // 위에서 아래로 내려가는 것처럼
                            var targetRow = (newBlock.row = i);

                            if (Mathf.Abs(targetRow - newBlock.transform.position.y) > .1f)
                            {
                                Vector2 tempPosition = new Vector2(newBlock.transform.position.x, targetRow);
                                newBlock.transform.position = Vector2.Lerp(newBlock.transform.position, tempPosition, .05f);
                            }
                        }

                        // 블럭 업데이트
                        delBlocks.Clear();

                        // 블럭 업데이트
                        GameManager.Instance.Board.BlockUpdate();

                        if (GameManager.Instance.Board.BlockCheck())
                        {
                            yield return new WaitForSeconds(.5f);

                            GameManager.Instance.isStart = true;
                            GameManager.Instance.SetGameState(GameState.Checking);
                        }
                        else
                        {
                            GameManager.Instance.SetGameState(GameState.Play);
                        }

                        isItem = false;
                    }
                    break;

                case ItemType.Time:
                    Debug.Log("시간 아이템");

                    // 개수 설정
                    itemCount--;
                    countText.text = itemCount.ToString();

                    // 아이템 효과 -> 시간을 늘려준다
                    UIWindowManager.Instance.GetWindow<UITime>().SetTimeAmount(.2f);
                    GameManager.Instance.SetGameState(GameState.Play);
                    isItem = false;
                    break;

                case ItemType.Skill:
                    Debug.Log("스킬 아이템");

                    // 개수 설정
                    itemCount--;
                    countText.text = itemCount.ToString();

                    // 아이템 효과 -> 스킬 게이지를 채워준다
                    // UIWindowManager.Instance.GetWindow<UIElement>().currentGauge.SetSkillAmount(.2f);
                    GameManager.Instance.SetGameState(GameState.Play);
                    isItem = false;
                    break;
            }
        }
    }
}
