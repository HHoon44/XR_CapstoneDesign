using UnityEngine;

namespace XR_3MatchGame_UI
{
    public class UIWindow : MonoBehaviour
    {
        public virtual void Start()
        {
            UIWindowManager.Instance.AddUIWindow(this);
        }

    }
}
