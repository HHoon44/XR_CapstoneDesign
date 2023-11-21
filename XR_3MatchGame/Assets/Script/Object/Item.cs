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

        // 분기문으로 아이템 타입이랑 스프라이트 설정
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
    /// 아이템 버튼에 바인딩할 메서드
    /// </summary>
    public void ItemButton()
    {
        // 분기문에 따라 아이템 능력 사용
        switch (itemType)
        {
            case ItemType.Boom:
                Debug.Log("폭탄 아이템");
                break;

            case ItemType.Time:
                Debug.Log("시간 아이템");
                break;

            case ItemType.Skill:
                Debug.Log("스킬 아이템");
                break;
        }
    }
}
