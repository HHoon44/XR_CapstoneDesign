using System.Collections;
using UIHealthAlchemy;
using UnityEngine;
using XR_3MatchGame.Util;
using XR_3MatchGame_InGame;
using XR_3MatchGame_Object;
using XR_3MatchGame_Util;

namespace XR_3MatchGame_UI
{
    public class UIElement : UIWindow
    {
        [SerializeField]
        private GameObject fire;            // �� ���� ��ų ������

        [SerializeField]
        private GameObject ice;             // ���� ���� ��ų ������

        [SerializeField]
        private GameObject grass;           // Ǯ ���� ��ų ������

        [SerializeField]
        private GameObject skillEffect;     // ��ų �ƾ�

        private MaterialHealhBar skillGauge;
        private GameManager GM;

        private float skillValue;           // ��ų ������ ��

        public override void Start()
        {
            base.Start();
            GM = GameManager.Instance;

            // ���� ������ Ȱ��ȭ
            OnElementGauge(GM.ElementType);
            skillValue = skillGauge.Value;
        }

        /// <summary>
        /// ��û ���� ���� Ÿ�Կ� ���� ��ų �������� Ȱ��ȭ ���ִ� �޼���
        /// </summary>
        /// <param name="type">���� Ÿ��</param>
        public void OnElementGauge(ElementType type)
        {
            switch (type)
            {
                case ElementType.Fire:

                    // �� ��ų ������ Ȱ��ȭ
                    fire.SetActive(true);

                    if (ice.activeSelf)
                    {
                        ice.SetActive(false);
                    }
                    else if (grass.activeSelf)
                    {
                        grass.SetActive(false);
                    }

                    skillGauge = fire.GetComponent<MaterialHealhBar>();
                    skillGauge.Value = skillValue;
                    break;

                case ElementType.Ice:

                    // ���� ��ų ������ Ȱ��ȭ
                    ice.SetActive(true);

                    if (fire.activeSelf)
                    {
                        fire.SetActive(false);
                    }
                    else if (grass.activeSelf)
                    {
                        grass.SetActive(false);
                    }

                    skillGauge = ice.GetComponent<MaterialHealhBar>();
                    skillGauge.Value = skillValue;
                    break;

                case ElementType.Grass:

                    // Ǯ ��ų ������ Ȱ��ȭ
                    grass.SetActive(true);

                    if (fire.activeSelf)
                    {
                        fire.SetActive(false);
                    }
                    else if (ice.activeSelf)
                    {
                        ice.SetActive(false);
                    }

                    skillGauge = grass.GetComponent<MaterialHealhBar>();
                    skillGauge.Value = skillValue;
                    break;
            }
        }

        /// <summary>
        /// ��ų ������ ���� �����ϴ� �޼���
        /// </summary>
        /// <param name="value">��</param>
        public void SetSkillAmount(float value)
        {
            if (skillGauge.Value >= 1f)
            {
                skillGauge.Value = 1f;
                return;
            }

            skillValue += value;
            skillGauge.Value += skillValue;
        }

        /// <summary>
        /// ��ų ��ư�� ���ε��� �޼���
        /// </summary>
        public void SkillBtn()
        {
            // ��ų ����
            if (skillGauge.Value >= 1f && GM.GameState == GameState.Play)
            {
                skillEffect.SetActive(true);
                skillValue = 0;
                skillGauge.Value = skillValue;

                GM.SetGameState(GameState.Skill);

                StartCoroutine(ElementSkill(GM.ElementType));
            }
        }

        private IEnumerator ElementSkill(ElementType type)
        {
            var blocks = GM.blocks;
            var downBlocks = GM.downBlocks;
            var delBlocks = GM.delBlocks;
            var pool = ObjectPoolManager.Instance.GetPool<Block>(PoolType.Block);
            var size = (GM.BoardSize.x * GM.BoardSize.y);

            // Ÿ�Կ� ���� ��ų �ߵ�
            switch (type)
            {
                case ElementType.Fire:
                    Debug.Log("�� ���� ��ų �ߵ�");

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

                    // �� �ı�
                    for (int i = 0; i < delBlocks.Count; i++)
                    {
                        pool.ReturnPoolableObject(delBlocks[i]);
                        blocks.Remove(delBlocks[i]);
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

                    yield return new WaitForSeconds(.3f);

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
                    break;

                case ElementType.Ice:
                    Debug.Log("���� ���� ��ų �ߵ�");
                    UIWindowManager.Instance.GetWindow<UITime>().timeStop = true;
                    GM.SetGameState(GameState.Play);

                    // 10������ �ð��� �����
                    yield return new WaitForSeconds(10f);

                    UIWindowManager.Instance.GetWindow<UITime>().timeStop = false;
                    GM.SetGameState(GameState.Skill);
                    break;

                case ElementType.Grass:
                    Debug.Log("Ǯ ���� ��ų �ߵ�");
                    break;
            }

            yield return new WaitForSeconds(1f);

            skillEffect.SetActive(false);

            yield return new WaitForSeconds(.1f);

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
}