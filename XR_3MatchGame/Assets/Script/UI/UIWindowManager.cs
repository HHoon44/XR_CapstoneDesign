using System;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace XR_3MatchGame_UI
{
    public class UIWindowManager : Singleton<UIWindowManager>
    {
        /// <summary>
        /// 인 게임에서 사용되는 UI를 저장 해놓을 Dictionary
        /// </summary>
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