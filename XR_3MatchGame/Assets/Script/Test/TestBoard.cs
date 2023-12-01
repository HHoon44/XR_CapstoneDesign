using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using XR_3MatchGame.Util;
using XR_3MatchGame_Object;

public class TestBoard : MonoBehaviour
{
    public List<Sprite> sprites = new List<Sprite>();

    [Header("Block Obj")]
    // Block ������
    public GameObject block;

    // Block�� ���� ����Ʈ
    public List<TestBlock> blocks = new List<TestBlock>();

    // ��Ī ����
    public bool isMatch;

    private void Start()
    {
        TestGameManager.Instance.testBoard = this;

        SetBoard();
    }

    private void Update()
    {
        if (isMatch)
        {
            // �� ��ġ ����
        }
    }

    /// <summary>
    /// ���忡 ���� �����ϴ� �޼���
    /// </summary>
    public void SetBoard()
    {
        // Block Row
        for (int row = -3; row < 4; row++)
        {
            // Block Col
            for (int col = -3; col < 4; col++)
            {
                var newBlock = Instantiate(block);

                // �θ� ����
                newBlock.transform.SetParent(this.transform);

                // ��ġ �� ����
                newBlock.transform.localPosition = new Vector3(col, row, 0);

                // ȸ�� �� ����
                newBlock.transform.localRotation = Quaternion.identity;

                // ������ �� ����
                newBlock.transform.localScale = new Vector3(.19f, .19f, .19f);

                // Ȱ��ȭ
                newBlock.SetActive(true);

                // Block �ʱ�ȭ
                newBlock.GetComponent<TestBlock>().Initialize(col, row);

                var num = Random.Range(0, 6);

                newBlock.GetComponent<TestBlock>().icon.sprite = sprites[num];

                switch (num + 1)
                {
                    case (int)ElementType.Fire:
                        newBlock.GetComponent<TestBlock>().elementType = ElementType.Fire;
                        break;

                    case (int)ElementType.Ice:
                        newBlock.GetComponent<TestBlock>().elementType = ElementType.Ice;
                        break;

                    case (int)ElementType.Grass:
                        newBlock.GetComponent<TestBlock>().elementType = ElementType.Grass;
                        break;

                    case (int)ElementType.Lightning:
                        newBlock.GetComponent<TestBlock>().elementType = ElementType.Lightning;
                        break;

                    case (int)ElementType.Light:
                        newBlock.GetComponent<TestBlock>().elementType = ElementType.Light;
                        break;

                    case (int)ElementType.Dark:
                        newBlock.GetComponent<TestBlock>().elementType = ElementType.Dark;
                        break;
                }

                // Block ����
                blocks.Add(newBlock.GetComponent<TestBlock>());
            }
        }
    }

    public bool MatchCheck(TestBlock checkBlock = null, TestBlock otherBlock = null, SwipeDir swipeDir = SwipeDir.None)
    {
        if (checkBlock != null || otherBlock != null)
        {
            // ���� �� ����
            int count_T = 0;
            int count_B = 0;
            int count_L = 0;
            int count_R = 0;
            int count_M = 0;
            int count_M2 = 0;

            switch (swipeDir)
            {
                case SwipeDir.None:

                    // 3X3
                    for (int i = 0; i < blocks.Count; i++)
                    {
                        // Top
                        if ((blocks[i].row == otherBlock.row + 1 || blocks[i].row == otherBlock.row + 2) &&
                            blocks[i].col == otherBlock.col)
                        {
                            // ���� ������ üũ
                            if (blocks[i].elementType == otherBlock.elementType)
                            {
                                count_T++;
                            }
                        }

                        // Bottom
                        if ((blocks[i].row == otherBlock.row - 1 || blocks[i].row == otherBlock.row - 2) &&
                            blocks[i].col == otherBlock.col)
                        {
                            if (blocks[i].elementType == otherBlock.elementType)
                            {
                                count_B++;
                            }
                        }

                        // Left
                        if ((blocks[i].col == otherBlock.col - 1 || blocks[i].col == otherBlock.col - 2) &&
                            blocks[i].row == otherBlock.row)
                        {
                            if (blocks[i].elementType == otherBlock.elementType)
                            {
                                count_L++;
                            }
                        }


                        // Right
                        if ((blocks[i].col == otherBlock.col + 1 || blocks[i].col == otherBlock.col + 2) &&
                            blocks[i].row == otherBlock.row)
                        {
                            if (blocks[i].elementType == otherBlock.elementType)
                            {
                                count_R++;
                            }
                        }

                        // Horizontal Middle
                        if ((blocks[i].col == otherBlock.col + 1 || blocks[i].col == otherBlock.col - 1) && 
                            blocks[i].row == otherBlock.row)
                        {
                            if (blocks[i].elementType == otherBlock.elementType)
                            {
                                count_M++;
                            }
                        }

                        // Verticla Middle
                        if ((blocks[i].row == otherBlock.row + 1 || blocks[i].row == otherBlock.row - 1) && 
                            blocks[i].col == otherBlock.col)
                        {
                            if (blocks[i].elementType == otherBlock.elementType)
                            {
                                count_M2++;
                            }
                        }
                    }

                    break;

                case SwipeDir.Top:

                    for (int i = 0; i < blocks.Count; i++)
                    {
                        // Top
                        if ((checkBlock.row + 1 == blocks[i].row || checkBlock.row + 2 == blocks[i].row) && checkBlock.col == blocks[i].col)
                        {
                            // ���� ������ üũ
                            if (checkBlock.elementType == blocks[i].elementType)
                            {
                                count_T++;
                            }
                        }

                        // Middle
                        if ((checkBlock.col - 1 == blocks[i].col || checkBlock.col + 1 == blocks[i].col) && checkBlock.row == blocks[i].row)
                        {
                            // ���� ������ üũ
                            if (checkBlock.elementType == blocks[i].elementType)
                            {
                                count_M++;
                            }
                        }

                        // Left
                        if ((checkBlock.col - 1 == blocks[i].col || checkBlock.col - 2 == blocks[i].col) && checkBlock.row == blocks[i].row)
                        {
                            // ���� ������ üũ
                            if (checkBlock.elementType == blocks[i].elementType)
                            {
                                count_L++;
                            }
                        }

                        // Right
                        if ((checkBlock.col + 1 == blocks[i].col || checkBlock.col + 2 == blocks[i].col) && checkBlock.row == blocks[i].row)
                        {
                            // ���� ������ üũ
                            if (checkBlock.elementType == blocks[i].elementType)
                            {
                                count_R++;
                            }
                        }
                    }

                    break;

                case SwipeDir.Bottom:

                    for (int i = 0; i < blocks.Count; i++)
                    {
                        // Bottom
                        if ((checkBlock.row - 1 == blocks[i].row || checkBlock.row - 2 == blocks[i].row) && checkBlock.col == blocks[i].col)
                        {
                            if (checkBlock.elementType == blocks[i].elementType)
                            {
                                count_B++;
                            }
                        }

                        // Middle
                        if ((checkBlock.col - 1 == blocks[i].col || checkBlock.col + 1 == blocks[i].col) && checkBlock.row == blocks[i].row)
                        {
                            if (checkBlock.elementType == blocks[i].elementType)
                            {
                                count_M++;
                            }
                        }

                        // Left
                        if ((checkBlock.col - 1 == blocks[i].col || checkBlock.col - 2 == blocks[i].col) && checkBlock.row == blocks[i].row)
                        {
                            if (checkBlock.elementType == blocks[i].elementType)
                            {
                                count_L++;
                            }
                        }

                        //Right
                        if ((checkBlock.col + 1 == blocks[i].col || checkBlock.col + 2 == blocks[i].col) && checkBlock.row == blocks[i].row)
                        {
                            if (checkBlock.elementType == blocks[i].elementType)
                            {
                                count_R++;
                            }
                        }
                    }

                    break;

                case SwipeDir.Left:


                    break;

                case SwipeDir.Right:



                    break;
            }

        }

        return true;
    }
}