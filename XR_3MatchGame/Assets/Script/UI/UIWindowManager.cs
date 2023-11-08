using System;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace XR_3MatchGame_UI
{
    public class UIWindowManager : Singleton<UIWindowManager>
    {
        private Dictionary<string, UIWindow> totalUIWindow = new Dictionary<string, UIWindow>();


        public void AddUIWindow(UIWindow uiWindow)
        { 
            var key = uiWindow.GetType().Name;

            totalUIWindow.Add(key, uiWindow);
        }

        public T GetWindow<T>() where T : UIWindow
        {
            string key = typeof(T).Name;

            return (T)totalUIWindow[key];
        }
    }
}