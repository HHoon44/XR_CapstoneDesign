using System.Collections.Generic;

namespace XR_3MatchGame_UI
{
    public class UIWindowManager : Singleton<UIWindowManager>
    {
        /// <summary>
        /// �� ���ӿ��� ���Ǵ� UI�� ���� �س��� Dictionary
        /// </summary>
        private Dictionary<string, UIWindow> totalUIWindow = new Dictionary<string, UIWindow>();

        public void AddUIWindow(UIWindow uiWindow)
        {
            var key = uiWindow.GetType().Name;

            if (totalUIWindow.ContainsKey(key))
            {
                return;
            }

            totalUIWindow.Add(key, uiWindow);
        }

        public T GetWindow<T>() where T : UIWindow
        {
            string key = typeof(T).Name;

            return (T)totalUIWindow[key];
        }
    }
}