using UnityEngine;

namespace XR_3MatchGame_UI
{
    public class UIWindow : MonoBehaviour
    {
        public virtual void Start()
        {
            Debug.Log("UIWindow");

            UIWindowManager.Instance.AddUIWindow(this);
        }

    }
}
