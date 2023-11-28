using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace XR_3MatchGame_UI
{
    public class UIStart : MonoBehaviour
    {
        public TextMeshProUGUI loadStateDesc;
        public Image loadFillGauge;

        /// <summary>
        /// 현재 진행 상태를 텍스트에 띄울 메서드
        /// </summary>
        /// <param name="loadState"></param>
        public void SetLoadStateDescription(string loadState)
        {
            loadStateDesc.text = $"Load{loadState}...";
        }

        /// <summary>
        /// 현재 진행 상태를 게이지로 표현할 코루틴
        /// </summary>
        /// <param name="loadPer"></param>
        /// <returns></returns>
        public IEnumerator LoadGaugeUpdate(float loadPer)
        {
            while (!Mathf.Approximately(loadFillGauge.fillAmount, loadPer))
            {
                loadFillGauge.fillAmount = Mathf.Lerp(loadFillGauge.fillAmount, loadPer, Time.deltaTime * 2f);
                yield return null;
            }
        }
    }
}