using System.Collections;
using UIHealthAlchemy;
using UnityEngine;
using UnityEngine.UI;
using XR_3MatchGame.Util;
using XR_3MatchGame_Data;
using XR_3MatchGame_InGame;
using XR_3MatchGame_Object;
using XR_3MatchGame_Util;

namespace XR_3MatchGame_UI
{
    public class UIElement : UIWindow
    {
        #region GaugeObj
        [SerializeField]
        private GameObject fireGauge;            // �� ���� ��ų ������

        [SerializeField]
        private GameObject iceGauge;             // ���� ���� ��ų ������

        [SerializeField]
        private GameObject grassGauge;           // Ǯ ���� ��ų ������

        #endregion

        #region SkillObj

        [SerializeField]
        private GameObject fireSkill;         // ��ų �ƾ�

        [SerializeField]
        private GameObject iceSkill;         // ��ų �ƾ�

        [SerializeField]
        private GameObject grassSkill;         // ��ų �ƾ�

        #endregion

        private GameManager GM;

        public float TEST;

        private Image fillGauge;

        public override void Start()
        {
            base.Start();
            GM = GameManager.Instance;

            Initialize();
        }

        /// <summary>
        /// UI �ʱ�ȭ �޼���
        /// </summary>
        public void Initialize()
        {
            TEST = 0;
            SetGauge();
        }

        /// <summary>
        /// ��ų ������ Ȱ��ȭ �޼���
        /// </summary>
        public void SetGauge()
        {
            var type = GameManager.Instance.ElementType;

            switch (type)
            {
                case ElementType.Fire:

                    // �� Ȱ��ȭ
                    fireGauge.SetActive(true);

                    if (iceGauge.activeSelf)
                    {
                        iceGauge.SetActive(false);
                    }
                    else if (grassGauge.activeSelf)
                    {
                        grassGauge.SetActive(false);
                    }

                    fillGauge = fireGauge.GetComponent<MaterialHealhBar>().fillGauge;
                    break;

                case ElementType.Ice:

                    // ���� Ȱ��ȭ
                    iceGauge.SetActive(true);

                    if (fireGauge.activeSelf)
                    {
                        fireGauge.SetActive(false);
                    }
                    else if (grassGauge.activeSelf)
                    {
                        grassGauge.SetActive(false);
                    }

                    fillGauge = fireGauge.GetComponent<MaterialHealhBar>().fillGauge;
                    break;

                case ElementType.Grass:

                    // Ǯ Ȱ��ȭ
                    grassGauge.SetActive(true);

                    if (fireGauge.activeSelf)
                    {
                        fireGauge.SetActive(false);
                    }
                    else if (iceGauge.activeSelf)
                    {
                        iceGauge.SetActive(false);
                    }

                    fillGauge = fireGauge.GetComponent<MaterialHealhBar>().fillGauge;
                    break;
            }

            // ������ �� ����
            fillGauge.fillAmount = TEST;
        }

        /// <summary>
        /// ��ų ������ ���� �����ϴ� �޼���
        /// </summary>
        /// <param name="value">��</param>
        public void SetSkillAmount(float value)
        {

            if (fillGauge.fillAmount >= 1f)
            {
                TEST = 1f;
                fillGauge.fillAmount = TEST;
                return;
            }

            TEST += value;
            fillGauge.fillAmount = TEST;
            Debug.Log(TEST);
        }

        /// <summary>
        /// ��ų ��ư�� ���ε��� �޼���
        /// </summary>
        public void SkillBtn()
        {
            // ��ų ����
            if (fillGauge.fillAmount >= 1f && GM.GameState == GameState.Play)
            {
                switch (GM.ElementType)
                {
                    case ElementType.Fire:
                        fireSkill.SetActive(true);
                        break;

                    case ElementType.Ice:
                        iceSkill.SetActive(true);
                        break;

                    case ElementType.Grass:
                        grassSkill.SetActive(true);
                        break;

                    case ElementType.Dark:
                        break;

                    case ElementType.Light:
                        break;

                    case ElementType.Lightning:
                        break;
                }

                TEST = 0;
                fillGauge.fillAmount = TEST;

                GM.SetGameState(GameState.Skill);
                StartCoroutine(SkillStart(GM.ElementType));
            }
        }

        private IEnumerator SkillStart(ElementType type)
        {
            var blocks = GameManager.Instance.Board.blocks;
            var delBlocks = GameManager.Instance.Board.delBlocks;
            var downBlocks = GameManager.Instance.Board.downBlocks;

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

                    yield return new WaitForSeconds(1f);

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

            fireSkill.SetActive(false);

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