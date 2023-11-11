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
            this.title.text = title + "의 시련";

            detail.text = title + " 원소가 폭주하기 시작했습니다..! " + title + " 원소를 진정시켜주세요!";
        }
    }
}
