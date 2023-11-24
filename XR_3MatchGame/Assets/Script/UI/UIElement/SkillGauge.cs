using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using XR_3MatchGame.Util;
using XR_3MatchGame_Data;
using XR_3MatchGame_InGame;
using XR_3MatchGame_Object;
using XR_3MatchGame_UI;
using XR_3MatchGame_Util;

public class SkillGauge : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem circleEffect;

    [SerializeField]
    private ParticleSystem skillEffect;

    [SerializeField]
    private Image fillGauge;

    [SerializeField]
    private Animator anim;

    private GameManager GM;
    private DataManager DM;

    public bool isFill = false;

    public ElementType gaugeType;

    private void Awake()
    {
        var name = gameObject.name.Split('_')[0];

        switch (name)
        {
            case "Fire":
                gaugeType = ElementType.Fire;
                break;

            case "Ice":
                gaugeType = ElementType.Ice;
                break;

            case "Grass":
                gaugeType = ElementType.Grass;
                break;
        }
    }

    /// <summary>
    /// ��ų ��ư �ʱ�ȭ �޼���
    /// </summary>
    public void Initialize()
    {
        GM = GameManager.Instance;
        DM = DataManager.Instance;

        fillGauge.fillAmount = DM.saveValue;

        if (fillGauge.fillAmount >= 1f)
        {
            isFill = true;

            // ���� �� ���¿��� ���Ҹ� �ٲ��� ���
            // ����� �����ܵ� �ִϸ��̼� ����ǵ���
            anim.SetBool("isFill", isFill);
        }
    }

    public void SetSkillAmount(float value)
    {
        if (fillGauge.fillAmount >= 1f)
        {
            DataManager.Instance.saveValue = 1f;
            fillGauge.fillAmount = DataManager.Instance.saveValue;

            if (!isFill)
            {
                isFill = true;
                anim.SetBool("isFill", isFill);
            }

            return;
        }

        DataManager.Instance.saveValue += value;
        fillGauge.fillAmount = DataManager.Instance.saveValue;
    }

    /// <summary>
    /// ��ų ��ư�� ���ε��� �޼���
    /// </summary>
    public void SkillButton()
    {
        if (fillGauge.fillAmount >= 1f && GM.GameState == GameState.Play)
        {
            if (isFill)
            {
                isFill = false;
            }

            anim.SetBool("isFill", isFill);

            // ������ ����Ʈ ����
            circleEffect.Play();

            DataManager.Instance.saveValue = 0;
            fillGauge.fillAmount = 0;

            //switch (gaugeType)
            //{
            //    case ElementType.Fire:

            //        GM.SetGameState(GameState.FireSkill);
            //        break;

            //    case ElementType.Ice:

            //        GM.SetGameState(GameState.IceSkill);
            //        break;

            //    case ElementType.Grass:

            //        GM.SetGameState(GameState.GrassSkill);
            //        break;

            //    case ElementType.Dark:
            //        break;

            //    case ElementType.Light:
            //        break;

            //    case ElementType.Lightning:
            //        break;
            //}

            GM.SetGameState(GameState.SKill);

            // ��ų ȿ�� ����
            StartCoroutine(SkillStart(GM.ElementType));
        }
    }

    private IEnumerator SkillStart(ElementType type)
    {
        yield return new WaitForSeconds(1.5f);

        var blocks = GM.Board.blocks;
        var delBlocks = GM.Board.delBlocks;
        var downBlocks = GM.Board.downBlocks;

        var pool = ObjectPoolManager.Instance.GetPool<Block>(PoolType.Block);
        var size = (GM.BoardSize.x * GM.BoardSize.y);

        // ��ų ����Ʈ �ߵ�
        skillEffect.Play();

        switch (type)
        {
            case ElementType.Fire:
                // ������ 1 ~ 5
                // �ı��� �� ã��
                for (int i = 0; i < blocks.Count; i++)
                {
                    if (blocks[i].col != 0 && blocks[i].col != 6)
                    {
                        if (blocks[i].row != 0 && blocks[i].row != 6)
                        {
                            delBlocks.Add(blocks[i]);
                        }
                    }
                }

                // �� �ı� ��ƼŬ ����
                for (int i = 0; i < delBlocks.Count; i++)
                {
                    delBlocks[i].BlockParticle();
                }

                yield return new WaitForSeconds(.4f);

                // �� �ı�
                for (int i = 0; i < delBlocks.Count; i++)
                {
                    pool.ReturnPoolableObject(delBlocks[i]);
                    blocks.Remove(delBlocks[i]);

                    // ���� ������Ʈ
                    DataManager.Instance.SetScore(delBlocks[i].BlockScore);
                }

                // ���� �� ã��
                for (int i = 0; i < blocks.Count; i++)
                {
                    if (blocks[i].col != 0 && blocks[i].col != 6)
                    {
                        if (blocks[i].row == 6)
                        {
                            downBlocks.Add(blocks[i]);
                        }
                    }
                }

                // �� ������
                for (int i = 0; i < downBlocks.Count; i++)
                {
                    var targetRow = (downBlocks[i].row = 1);

                    if (Mathf.Abs(targetRow - downBlocks[i].transform.position.y) > .1f)
                    {
                        Vector2 tempPosition = new Vector2(downBlocks[i].transform.position.x, targetRow);
                        downBlocks[i].transform.position = Vector2.Lerp(downBlocks[i].transform.position, tempPosition, .05f);
                    }
                }

                yield return new WaitForSeconds(.5f);

                // ���ڸ��� �� �����ϱ�
                var emptyCount = size - blocks.Count;
                var colValue = 1;
                var rowValue = downBlocks[downBlocks.Count - 1].row + 1;

                for (int i = 0; i < emptyCount; i++)
                {
                    if (rowValue < GM.BoardSize.y)
                    {
                        // ���ο� �� ���� ���� �ۼ�
                        var newBlock = pool.GetPoolableObject(obj => obj.CanRecycle);
                        newBlock.transform.position = new Vector3(colValue, GM.BoardSize.y, 0);
                        newBlock.gameObject.SetActive(true);
                        newBlock.Initialize(colValue, GM.BoardSize.y);
                        blocks.Add(newBlock);

                        // ������ �Ʒ��� �������� ��ó��
                        var targetRow = (newBlock.row = rowValue);

                        if (Mathf.Abs(targetRow - newBlock.transform.position.y) > .1f)
                        {
                            Vector2 tempPosition = new Vector2(newBlock.transform.position.x, targetRow);
                            newBlock.transform.position = Vector2.Lerp(newBlock.transform.position, tempPosition, .05f);
                        }

                        colValue++;

                        if (colValue > 5)
                        {
                            colValue = 1;
                            rowValue++;
                        }
                    }
                }

                delBlocks.Clear();
                downBlocks.Clear();

                // ������ ���߿� ��Ī�Ǵ� ���� �ִٸ� �ı�
                GM.Board.BlockUpdate();

                if (GM.Board.BlockCheck())
                {
                    GM.isStart = true;
                }

                // �� ��ų �г�Ƽ
                // 5�ʰ� �� �̵� ����
                UIWindowManager.Instance.GetWindow<UIElement>().SetStateText("��ų ���ۿ����� 5�ʰ� �� �̵� ����!");

                /// Test
                if (GM.GameState == GameState.Play)
                {
                    GM.SetGameState(GameState.SKill);
                }

                yield return new WaitForSeconds(5f);

                UIWindowManager.Instance.GetWindow<UIElement>().SetStateText();
                break;

            case ElementType.Ice:

                // �� ã��
                for (int i = 0; i < blocks.Count; i++)
                {
                    if (blocks[i].row == 0 || blocks[i].row == 1)
                    {
                        delBlocks.Add(blocks[i]);
                    }
                }

                // ��ƼŬ ����
                for (int i = 0; i < delBlocks.Count; i++)
                {
                    delBlocks[i].BlockParticle();
                }

                yield return new WaitForSeconds(.4f);

                // �� ����
                for (int i = 0; i < delBlocks.Count; i++)
                {
                    pool.ReturnPoolableObject(delBlocks[i]);
                    blocks.Remove(delBlocks[i]);

                    // ���� ������Ʈ
                    DataManager.Instance.SetScore(delBlocks[i].BlockScore);
                }

                yield return new WaitForSeconds(.4f);

                // ���� �� ã��
                for (int i = 0; i < blocks.Count; i++)
                {
                    if (blocks[i].row > 1)
                    {
                        downBlocks.Add(blocks[i]);
                    }
                }

                // �� ������
                for (int i = 0; i < downBlocks.Count; i++)
                {
                    var targetRow = (downBlocks[i].row -= 2);

                    if (Mathf.Abs(targetRow - downBlocks[i].transform.position.y) > .1f)
                    {
                        Vector2 tempPosition = new Vector2(downBlocks[i].transform.position.x, targetRow);
                        downBlocks[i].transform.position = Vector2.Lerp(downBlocks[i].transform.position, tempPosition, .05f);
                    }
                }

                // ���ڸ��� �� �����ϱ�
                emptyCount = size - blocks.Count;
                colValue = 0;
                rowValue = downBlocks[downBlocks.Count - 1].row + 1;

                for (int i = 0; i < emptyCount; i++)
                {
                    if (rowValue < GM.BoardSize.y)
                    {
                        // ���ο� �� ���� ���� �ۼ�
                        var newBlock = pool.GetPoolableObject(obj => obj.CanRecycle);
                        newBlock.transform.position = new Vector3(colValue, GM.BoardSize.y, 0);
                        newBlock.gameObject.SetActive(true);
                        newBlock.Initialize(colValue, GM.BoardSize.y);
                        blocks.Add(newBlock);

                        // ������ �Ʒ��� �������� ��ó��
                        var targetRow = (newBlock.row = rowValue);

                        if (Mathf.Abs(targetRow - newBlock.transform.position.y) > .1f)
                        {
                            Vector2 tempPosition = new Vector2(newBlock.transform.position.x, targetRow);
                            newBlock.transform.position = Vector2.Lerp(newBlock.transform.position, tempPosition, .05f);
                        }

                        colValue++;

                        if (colValue > 6)
                        {
                            colValue = 0;
                            rowValue++;
                        }
                    }
                }

                delBlocks.Clear();
                downBlocks.Clear();

                // ������ ���߿� ��Ī�Ǵ� ���� �ִٸ� �ı�
                UIWindowManager.Instance.GetWindow<UIElement>().SetStateText("��ų ȿ���� �ð� ����!");
                UIWindowManager.Instance.GetWindow<UITime>().timeStop = true;

                GM.Board.BlockUpdate();

                if (GM.Board.BlockCheck())
                {
                    GM.isStart = true;
                }
                else
                {
                    GM.SetGameState(GameState.Play);
                }

                // 10������ �ð��� �����
                yield return new WaitForSeconds(10f);

                UIWindowManager.Instance.GetWindow<UIElement>().SetStateText();
                UIWindowManager.Instance.GetWindow<UITime>().timeStop = false;
                break;

            case ElementType.Grass:

                // �� ã��
                for (int i = 0; i < blocks.Count; i++)
                {
                    if (blocks[i].row == 0 || blocks[i].row == 1)
                    {
                        delBlocks.Add(blocks[i]);
                    }
                }

                // ��ƼŬ ����
                for (int i = 0; i < delBlocks.Count; i++)
                {
                    delBlocks[i].BlockParticle();
                }

                yield return new WaitForSeconds(.4f);

                // �� ����
                for (int i = 0; i < delBlocks.Count; i++)
                {
                    pool.ReturnPoolableObject(delBlocks[i]);
                    blocks.Remove(delBlocks[i]);

                    // ���� ������Ʈ
                    DataManager.Instance.SetScore(delBlocks[i].BlockScore);
                }

                yield return new WaitForSeconds(.4f);

                // ���� �� ã��
                for (int i = 0; i < blocks.Count; i++)
                {
                    if (blocks[i].row > 1)
                    {
                        downBlocks.Add(blocks[i]);
                    }
                }

                // �� ������
                for (int i = 0; i < downBlocks.Count; i++)
                {
                    var targetRow = (downBlocks[i].row -= 2);

                    if (Mathf.Abs(targetRow - downBlocks[i].transform.position.y) > .1f)
                    {
                        Vector2 tempPosition = new Vector2(downBlocks[i].transform.position.x, targetRow);
                        downBlocks[i].transform.position = Vector2.Lerp(downBlocks[i].transform.position, tempPosition, .05f);
                    }
                }

                // ���ڸ��� �� �����ϱ�
                emptyCount = size - blocks.Count;
                colValue = 0;
                rowValue = downBlocks[downBlocks.Count - 1].row + 1;

                for (int i = 0; i < emptyCount; i++)
                {
                    if (rowValue < GM.BoardSize.y)
                    {
                        // ���ο� �� ���� ���� �ۼ�
                        var newBlock = pool.GetPoolableObject(obj => obj.CanRecycle);
                        newBlock.transform.position = new Vector3(colValue, GM.BoardSize.y, 0);
                        newBlock.gameObject.SetActive(true);
                        newBlock.Initialize(colValue, GM.BoardSize.y);
                        blocks.Add(newBlock);

                        // ������ �Ʒ��� �������� ��ó��
                        var targetRow = (newBlock.row = rowValue);

                        if (Mathf.Abs(targetRow - newBlock.transform.position.y) > .1f)
                        {
                            Vector2 tempPosition = new Vector2(newBlock.transform.position.x, targetRow);
                            newBlock.transform.position = Vector2.Lerp(newBlock.transform.position, tempPosition, .05f);
                        }

                        colValue++;

                        if (colValue > 6)
                        {
                            colValue = 0;
                            rowValue++;
                        }
                    }
                }

                delBlocks.Clear();
                downBlocks.Clear();

                // ������ ���߿� ��Ī�Ǵ� ���� �ִٸ� �ı�
                GM.Board.BlockUpdate();

                if (GM.Board.BlockCheck())
                {
                    GM.isStart = true;
                }
                else
                {
                    GM.SetGameState(GameState.Play);
                }

                UIWindowManager.Instance.GetWindow<UIElement>().SetStateText("��ų ȿ���� �ð� ���� �� ��ų ������ ȸ��!");

                UIWindowManager.Instance.GetWindow<UITime>().timeStop = true;

                for (int i = 0; i < 6; i++)
                {
                    yield return new WaitForSeconds(1f);
                    SetSkillAmount(.05f);
                }

                UIWindowManager.Instance.GetWindow<UIElement>().SetStateText();
                UIWindowManager.Instance.GetWindow<UITime>().timeStop = false;
                break;
        }

        yield return new WaitForSeconds(.5f);

        // �� ������Ʈ
        GM.Board.BlockUpdate();

        if (GM.Board.BlockCheck())
        {
            GM.isStart = true;
            GM.SetGameState(GameState.Checking);
        }
        else
        {
            GM.SetGameState(GameState.Play);
        }
    }
}