using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XR_3MatchGame.Util;

public class Item : MonoBehaviour
{
    public ItemType itemType;

    public void Initialize()
    {
        var name = this.name.Split()[0];

        // �б⹮���� ������ Ÿ���̶� ��������Ʈ ����
        switch (name)
        {
            case "Boom":
                itemType = ItemType.Boom;
                break;

            case "Time":
                itemType = ItemType.Time;
                break;

            case "Skill":
                itemType = ItemType.Skill;
                break;
        }
    }

    /// <summary>
    /// ������ ��ư�� ���ε��� �޼���
    /// </summary>
    public void ItemButton()
    {
        // �б⹮�� ���� ������ �ɷ� ���
        switch (itemType)
        {
            case ItemType.Boom:
                Debug.Log("��ź ������");
                break;

            case ItemType.Time:
                Debug.Log("�ð� ������");
                break;

            case ItemType.Skill:
                Debug.Log("��ų ������");
                break;
        }
    }
}
