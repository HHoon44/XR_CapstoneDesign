using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using XR_3MatchGame.Util;
using XR_3MatchGame_Data;
using XR_3MatchGame_InGame;
using XR_3MatchGame_Object;
using XR_3MatchGame_Resource;
using XR_3MatchGame_Util;

namespace XR_3MatchGame_UI
{
    public class SkillButton : MonoBehaviour
    {
        public ElementType selectElement;   // ������ ����

        [SerializeField]
        private Image icon;

        [SerializeField]
        private GameObject skillEffectObj;      // ū��

        [SerializeField]
        private ParticleSystem magicCircle;     // ������ 1

        [SerializeField]
        private ParticleSystem magicEffect;     // ������ 2

        [SerializeField]
        private GameObject backBlack;           // ��ų ��� �� ���� ���� ���

        private void Start()
        {
            Initialize();
        }

        /// <summary>
        /// �ʱ� ���� �޼���
        /// </summary>
        public void Initialize()
        {
            var GM = GameManager.Instance;

            var index = int.Parse(name.Split('_')[1]);

            switch (index)
            {
                case 0:
                    selectElement = GM.selectType[0];

                    for (int i = 0; i < skillEffectObj.transform.childCount; i++)
                    {
                        var name = skillEffectObj.transform.GetChild(i).name.Split('_')[0];

                        if (selectElement.ToString() == name)
                        {
                            var obj = skillEffectObj.transform.GetChild(i).gameObject;

                            magicCircle = obj.transform.GetChild(0).GetComponent<ParticleSystem>();
                            magicEffect = obj.transform.GetChild(1).GetComponent<ParticleSystem>();
                        }
                    }

                    break;

                case 1:
                    selectElement = GM.selectType[1];

                    for (int i = 0; i < skillEffectObj.transform.childCount; i++)
                    {
                        var name = skillEffectObj.transform.GetChild(i).name.Split('_')[0];

                        if (selectElement.ToString() == name)
                        {
                            var obj = skillEffectObj.transform.GetChild(i).gameObject;

                            magicCircle = obj.transform.GetChild(0).GetComponent<ParticleSystem>();
                            magicEffect = obj.transform.GetChild(1).GetComponent<ParticleSystem>();
                        }
                    }

                    break;

                case 2:
                    selectElement = GM.selectType[2];

                    for (int i = 0; i < skillEffectObj.transform.childCount; i++)
                    {
                        var name = skillEffectObj.transform.GetChild(i).name.Split('_')[0];

                        if (selectElement.ToString() == name)
                        {
                            var obj = skillEffectObj.transform.GetChild(i).gameObject;

                            magicCircle = obj.transform.GetChild(0).GetComponent<ParticleSystem>();
                            magicEffect = obj.transform.GetChild(1).GetComponent<ParticleSystem>();
                        }
                    }

                    break;
            }



            icon.sprite = SpriteLoader.GetSprite(AtlasType.IconAtlas, selectElement.ToString());
        }

        public void OnSkill()
        {
            var uiElement = UIWindowManager.Instance.GetWindow<UIElement>();

            // ���� ���� ���°�  Play �����϶��� ��ų ��� ����
            if (uiElement.GetGauge() >= 1f && GameManager.Instance.GameState == GameState.Play)
            {
                // ���� ��� Ȱ��ȭ
                backBlack.SetActive(true);

                Debug.Log("��ų�� ����մϴ�!");

                // ��ų ������ 0���� �ʱ�ȭ
                uiElement.Initialize();

                var GM = GameManager.Instance;
                GM.SetGameState(GameState.SKill);

                // ��ų�� �����մϴ�.
                StartCoroutine(StartSkill());
            }
        }

        private IEnumerator StartSkill()
        {
            var GM = GameManager.Instance;

            var blocks = GM.Board.blocks;
            var delBlocks = GM.Board.delBlocks;
            var downBlocks = GM.Board.downBlocks;
            var blockPool = ObjectPoolManager.Instance.GetPool<Block>(PoolType.Block);
            var uiElement = UIWindowManager.Instance.GetWindow<UIElement>();

            switch (selectElement)
            {
                case ElementType.Fire:

                    // ù��° ����Ʈ ����
                    magicCircle.Play();

                    yield return new WaitForSeconds(1.5f);

                    // �ι�° ����Ʈ ����
                    magicEffect.Play();

                    // �ı��� �� ã��
                    for (int i = 0; i < blocks.Count; i++)
                    {
                        if (blocks[i].col == 2 || blocks[i].col == 3 || blocks[i].col == 4)
                        {
                            if (blocks[i].row == 2 || blocks[i].row == 3 || blocks[i].row == 4)
                            {
                                delBlocks.Add(blocks[i]);
                            }
                        }
                    }

                    // ���� ��� ��Ȱ��ȭ
                    backBlack.SetActive(false);

                    // �� ��ƼŬ ����
                    for (int i = 0; i < delBlocks.Count; i++)
                    {
                        delBlocks[i].BlockParticle();
                    }

                    // �� �ı�

                    yield return new WaitForSeconds(.3f);

                    // �� �ı�
                    for (int i = 0; i < delBlocks.Count; i++)
                    {
                        blockPool.ReturnPoolableObject(delBlocks[i]);
                        blocks.Remove(delBlocks[i]);

                        DataManager.Instance.SetScore(delBlocks[i].BlockScore);
                    }

                    // ���� �� ã��
                    for (int i = 0; i < blocks.Count; i++)
                    {
                        if (blocks[i].col == 2 || blocks[i].col == 3 || blocks[i].col == 4)
                        {
                            if (blocks[i].row > 4)
                            {
                                downBlocks.Add(blocks[i]);
                            }
                        }
                    }

                    yield return new WaitForSeconds(.4f);

                    // �� ������
                    // ���� ������ �۾�
                    for (int i = 0; i < downBlocks.Count; i++)
                    {
                        var targetRow = (downBlocks[i].row -= 3);

                        if (Mathf.Abs(targetRow - downBlocks[i].transform.position.y) > .1f)
                        {
                            Vector2 tempPosition = new Vector2(downBlocks[i].transform.position.x, targetRow);
                            downBlocks[i].transform.position = Vector2.Lerp(downBlocks[i].transform.position, tempPosition, .05f);
                        }
                    }

                    // ���ڸ��� �� ä���ֱ�
                    for (int row = 4; row < GM.BoardSize.y; row++)
                    {
                        for (int col = 2; col < 5; col++)
                        {
                            var block = blockPool.GetPoolableObject(obj => obj.CanRecycle);
                            block.transform.position = new Vector3(col, 7, 0);
                            block.Initialize(col, 7);
                            block.gameObject.SetActive(true);

                            blocks.Add(block);

                            // ������ �Ʒ��� �������� ��ó��
                            var targetRow = (block.row = row);

                            if (Mathf.Abs(targetRow - block.transform.position.y) > .1f)
                            {
                                Vector2 tempPosition = new Vector2(block.transform.position.x, targetRow);
                                block.transform.position = Vector2.Lerp(block.transform.position, tempPosition, .05f);
                            }
                        }

                        yield return new WaitForSeconds(.3f);
                    }

                    delBlocks.Clear();
                    downBlocks.Clear();

                    uiElement.SetStateText("���� �ð����� �߰� ����!");

                    GM.isPlus = true;

                    // �� ������Ʈ
                    //GM.Board.BlockUpdate();

                    if (GM.Board.BlockCheck())
                    {
                        GM.isStart = true;
                        GM.SetGameState(GameState.Checking);
                    }
                    else
                    {
                        GM.SetGameState(GameState.Play);
                    }

                    yield return new WaitForSeconds(5f);

                    GM.isPlus = false;
                    uiElement.SetStateText();

                    break;

                case ElementType.Ice:

                    // ù��° ����Ʈ ����
                    magicCircle.Play();

                    yield return new WaitForSeconds(1.5f);

                    // �ι�° ����Ʈ ����
                    magicEffect.Play();

                    // �ı��� �� ã��
                    for (int i = 0; i < blocks.Count; i++)
                    {
                        if (blocks[i].col == 2 || blocks[i].col == 3 || blocks[i].col == 4)
                        {
                            if (blocks[i].row == 2 || blocks[i].row == 3 || blocks[i].row == 4)
                            {
                                delBlocks.Add(blocks[i]);
                            }
                        }
                    }

                    // ���� ��� ��Ȱ��ȭ
                    backBlack.SetActive(false);

                    // �� ��ƼŬ ����
                    for (int i = 0; i < delBlocks.Count; i++)
                    {
                        delBlocks[i].BlockParticle();
                    }

                    yield return new WaitForSeconds(.3f);

                    // �� �ı�
                    for (int i = 0; i < delBlocks.Count; i++)
                    {
                        blockPool.ReturnPoolableObject(delBlocks[i]);
                        blocks.Remove(delBlocks[i]);

                        DataManager.Instance.SetScore(delBlocks[i].BlockScore);
                    }

                    // ���� �� ã��
                    for (int i = 0; i < blocks.Count; i++)
                    {
                        if (blocks[i].col == 2 || blocks[i].col == 3 || blocks[i].col == 4)
                        {
                            if (blocks[i].row > 4)
                            {
                                downBlocks.Add(blocks[i]);
                            }
                        }
                    }

                    yield return new WaitForSeconds(.4f);

                    // ���� ������ �۾�
                    for (int i = 0; i < downBlocks.Count; i++)
                    {
                        var targetRow = (downBlocks[i].row -= 3);

                        if (Mathf.Abs(targetRow - downBlocks[i].transform.position.y) > .1f)
                        {
                            Vector2 tempPosition = new Vector2(downBlocks[i].transform.position.x, targetRow);
                            downBlocks[i].transform.position = Vector2.Lerp(downBlocks[i].transform.position, tempPosition, .05f);
                        }
                    }

                    // ���ڸ��� �� ä���ֱ�
                    for (int row = 4; row < GM.BoardSize.y; row++)
                    {
                        for (int col = 2; col < 5; col++)
                        {
                            var block = blockPool.GetPoolableObject(obj => obj.CanRecycle);
                            block.transform.position = new Vector3(col, 7, 0);
                            block.Initialize(col, 7);
                            block.gameObject.SetActive(true);

                            blocks.Add(block);

                            // ������ �Ʒ��� �������� ��ó��
                            var targetRow = (block.row = row);

                            if (Mathf.Abs(targetRow - block.transform.position.y) > .1f)
                            {
                                Vector2 tempPosition = new Vector2(block.transform.position.x, targetRow);
                                block.transform.position = Vector2.Lerp(block.transform.position, tempPosition, .05f);
                            }
                        }

                        yield return new WaitForSeconds(.3f);
                    }

                    uiElement.SetStateText("���� �ð����� �ð� ����!");
                    UIWindowManager.Instance.GetWindow<UITime>().timeStop = true;

                    delBlocks.Clear();
                    downBlocks.Clear();

                    // �� ������Ʈ
                    //GM.Board.BlockUpdate();

                    if (GM.Board.BlockCheck())
                    {
                        GM.isStart = true;
                        GM.SetGameState(GameState.Checking);
                    }
                    else
                    {
                        GM.SetGameState(GameState.Play);
                    }

                    yield return new WaitForSeconds(10f);

                    uiElement.SetStateText();
                    UIWindowManager.Instance.GetWindow<UITime>().timeStop = false;

                    break;

                case ElementType.Grass:

                    // ù��° ����Ʈ ����
                    magicCircle.Play();

                    yield return new WaitForSeconds(1.5f);

                    // �ι�° ����Ʈ ����
                    magicEffect.Play();

                    // �ı��� �� ã��
                    for (int i = 0; i < blocks.Count; i++)
                    {
                        if (blocks[i].col == 2 || blocks[i].col == 3 || blocks[i].col == 4)
                        {
                            if (blocks[i].row == 2 || blocks[i].row == 3 || blocks[i].row == 4)
                            {
                                delBlocks.Add(blocks[i]);
                            }
                        }
                    }

                    // ���� ��� ��Ȱ��ȭ
                    backBlack.SetActive(false);

                    // �� ��ƼŬ ����
                    for (int i = 0; i < delBlocks.Count; i++)
                    {
                        delBlocks[i].BlockParticle();
                    }

                    yield return new WaitForSeconds(.3f);

                    // �� �ı�
                    for (int i = 0; i < delBlocks.Count; i++)
                    {
                        blockPool.ReturnPoolableObject(delBlocks[i]);
                        blocks.Remove(delBlocks[i]);

                        DataManager.Instance.SetScore(delBlocks[i].BlockScore);
                    }

                    // ���� �� ã��
                    for (int i = 0; i < blocks.Count; i++)
                    {
                        if (blocks[i].col == 2 || blocks[i].col == 3 || blocks[i].col == 4)
                        {
                            if (blocks[i].row > 4)
                            {
                                downBlocks.Add(blocks[i]);
                            }
                        }
                    }

                    yield return new WaitForSeconds(.4f);

                    // �� ������
                    // ���� ������ �۾�
                    for (int i = 0; i < downBlocks.Count; i++)
                    {
                        var targetRow = (downBlocks[i].row -= 3);

                        if (Mathf.Abs(targetRow - downBlocks[i].transform.position.y) > .1f)
                        {
                            Vector2 tempPosition = new Vector2(downBlocks[i].transform.position.x, targetRow);
                            downBlocks[i].transform.position = Vector2.Lerp(downBlocks[i].transform.position, tempPosition, .05f);
                        }
                    }


                    // ���ڸ��� �� ä���ֱ�
                    for (int row = 4; row < GM.BoardSize.y; row++)
                    {
                        for (int col = 2; col < 5; col++)
                        {
                            var block = blockPool.GetPoolableObject(obj => obj.CanRecycle);
                            block.transform.position = new Vector3(col, 7, 0);
                            block.Initialize(col, 7);
                            block.gameObject.SetActive(true);

                            blocks.Add(block);

                            // ������ �Ʒ��� �������� ��ó��
                            var targetRow = (block.row = row);

                            if (Mathf.Abs(targetRow - block.transform.position.y) > .1f)
                            {
                                Vector2 tempPosition = new Vector2(block.transform.position.x, targetRow);
                                block.transform.position = Vector2.Lerp(block.transform.position, tempPosition, .05f);
                            }
                        }

                        yield return new WaitForSeconds(.3f);
                    }

                    uiElement.SetStateText("���� �ð����� ��ų ������ ȸ��!");

                    delBlocks.Clear();
                    downBlocks.Clear();

                    for (int i = 0; i < 9; i++)
                    {
                        uiElement.SetGauge(.05f);
                        yield return new WaitForSeconds(.3f);
                    }

                    // �� ������Ʈ
                    //GM.Board.BlockUpdate();

                    if (GM.Board.BlockCheck())
                    {
                        GM.isStart = true;
                        GM.SetGameState(GameState.Checking);
                    }
                    else
                    {
                        GM.SetGameState(GameState.Play);
                    }

                    break;
            }

            // �ѹ��� üũ
            //GM.Board.BlockUpdate();

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
}