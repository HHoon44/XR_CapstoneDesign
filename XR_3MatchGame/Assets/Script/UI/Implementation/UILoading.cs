using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace XR_3MatchGame_UI
{
    public class UILoading : MonoBehaviour
    {
        public Image loadGauge;
        public TextMeshProUGUI loadStateDesc;

        private static string dot = string.Empty;
        private static string loadStateDescription = "�ε��� �Դϴ�";

        private void Update()
        {
            if (loadGauge.fillAmount >= .88f)
            {
                loadGauge.fillAmount = 1f;
            }

            loadGauge.fillAmount = Mathf.Lerp(loadGauge.fillAmount, 1f, Time.deltaTime * 2f);

            if (Time.frameCount % 20 == 0)
            {
                if (dot.Length >= 3)
                {
                    dot = string.Empty;
                }
                else
                {
                    dot = string.Concat(dot, ".");
                }

                loadStateDesc.text = $"{loadStateDescription}{dot}";
            }
        }
    }
}