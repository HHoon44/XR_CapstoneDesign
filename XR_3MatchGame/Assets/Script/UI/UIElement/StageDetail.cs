using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace XR_3MatchGame
{
    public class StageDetail : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI title;

        [SerializeField]
        private TextMeshProUGUI detail;

        public void Initialize(string title)
        {
            this.title.text = title + "�� �÷�";

            detail.text = title + " ���Ұ� �����ϱ� �����߽��ϴ�..! " + title + " ���Ҹ� ���������ּ���!";
        }
    }
}
