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
    public ItemType itemType;       // ���� ������ Ÿ��
    public int itemCount;           // ���� ������ ����

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

        // �б⹮���� ������ Ÿ���̶� ��������Ʈ ����
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
    /// ������ ��ư�� ���ε��� �޼���
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

            // �б⹮�� ���� ������ �ɷ� ���
            switch (itemType)
            {
                case ItemType.Boom:
                    Debug.Log("��ź ������");

                    // ���� ����
                    itemCount--;
                    countText.text = itemCount.ToString();

                    // ������ ȿ�� -> ���� �ı�
                    // Col = 3
                    // Row = 0 ~ 6

                    StartCoroutine(BoomItem());

                    IEnumerator BoomItem()
                    {
                        yield return new WaitForSeconds(.5f);

                        var blocks = GameManager.Instance.Board.blocks;
                        var delBlocks = GameManager.Instance.Board.delBlocks;
                        var pool = ObjectPoolManager.Instance.GetPool<Block>(PoolType.Block);

                        // �ı��� �� ã��
                        for (int i = 0; i < blocks.Count; i++)
                        {
                            if (blocks[i].col == 3 && (blocks[i].row == 0 || blocks[i].row == 1 || blocks[i].row == 2 ||
                                blocks[i].row == 3 || blocks[i].row == 4 || blocks[i].row == 5 || blocks[i].row == 6))
                            {
                                delBlocks.Add(blocks[i]);
                            }
                        }

                        // ��ƼŬ ����
                        for (int i = 0; i < delBlocks.Count; i++)
                        {
                            delBlocks[i].BlockParticle();
                        }

                        yield return new WaitForSeconds(.3f);

                        // �� �ı�
                        for (int i = 0; i < delBlocks.Count; i++)
                        {
                            pool.ReturnPoolableObject(delBlocks[i]);
                            blocks.Remove(delBlocks[i]);
                            DataManager.Instance.SetScore(delBlocks[i].BlockScore);
                        }

                        yield return new WaitForSeconds(.5f);

                        // �� �ڸ��� �� ä���ֱ�
                        for (int i = 0; i < GameManager.Instance.BoardSize.y; i++)
                        {
                            // ���ο� �� ���� ���� �ۼ�
                            var newBlock = pool.GetPoolableObject(obj => obj.CanRecycle);
                            newBlock.transform.position = new Vector3(3, GameManager.Instance.BoardSize.y, 0);
                            newBlock.gameObject.SetActive(true);
                            newBlock.Initialize(3, GameManager.Instance.BoardSize.y);
                            blocks.Add(newBlock);

                            // ������ �Ʒ��� �������� ��ó��
                            var targetRow = (newBlock.row = i);

                            if (Mathf.Abs(targetRow - newBlock.transform.position.y) > .1f)
                            {
                                Vector2 tempPosition = new Vector2(newBlock.transform.position.x, targetRow);
                                newBlock.transform.position = Vector2.Lerp(newBlock.transform.position, tempPosition, .05f);
                            }
                        }

                        // �� ������Ʈ
                        delBlocks.Clear();

                        // �� ������Ʈ
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
                    Debug.Log("�ð� ������");

                    // ���� ����
                    itemCount--;
                    countText.text = itemCount.ToString();

                    // ������ ȿ�� -> �ð��� �÷��ش�
                    UIWindowManager.Instance.GetWindow<UITime>().SetTimeAmount(.2f);
                    GameManager.Instance.SetGameState(GameState.Play);
                    isItem = false;
                    break;

                case ItemType.Skill:
                    Debug.Log("��ų ������");

                    // ���� ����
                    itemCount--;
                    countText.text = itemCount.ToString();

                    // ������ ȿ�� -> ��ų �������� ä���ش�
                    // UIWindowManager.Instance.GetWindow<UIElement>().currentGauge.SetSkillAmount(.2f);
                    GameManager.Instance.SetGameState(GameState.Play);
                    isItem = false;
                    break;
            }
        }
    }
}
