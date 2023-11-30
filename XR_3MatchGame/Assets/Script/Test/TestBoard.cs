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

                // Block ����
                blocks.Add(newBlock.GetComponent<TestBlock>());
            }
        }
    }

    public bool BlockUpdate(TestBlock checkBlock = null, TestBlock otherBlock = null, SwipeDir swipeDir = SwipeDir.None)
    {
        return true;
    }
}