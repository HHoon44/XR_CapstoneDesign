using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace XR_3MatchGame_UI
{
    public class UIStart : MonoBehaviour
    {
        public Text loadStateDesc;
        public Image loadFillGauge;

        /// <summary>
        /// ���� ���� ���¸� �ؽ�Ʈ�� ��� �޼���
        /// </summary>
        /// <param name="loadState"></param>
        public void SetLoadStateDescription(string loadState)
        {
            loadStateDesc.text = $"Load{loadState}...";
        }

        /// <summary>
        /// ���� ���� ���¸� �������� ǥ���� �ڷ�ƾ
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