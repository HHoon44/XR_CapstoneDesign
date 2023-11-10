using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using XR_3MatchGame.Util;
using XR_3MatchGame_UI;
using ResourceManager = XR_3MatchGame_Resource.ResourceManager;

namespace XR_3MatchGame
{
    public class StartController : MonoBehaviour
    {
        public UIStart uiStart;

        private bool allLoaded;
        private IntroPhase introPhase = IntroPhase.Start;
        private Coroutine loadGaugeUpdateCoroutine;
        private bool loadComplete;

        public bool LoadComplete
        {
            get => loadComplete;

            set
            {
                loadComplete = value;

                if (loadComplete && !allLoaded)
                {
                    NextPhase();
                }
            }
        }

        public void Initialize()
        {
            OnPhase(introPhase);
        }

        private void OnPhase(IntroPhase phase)
        {
            uiStart.SetLoadStateDescription(phase.ToString());

            if (loadGaugeUpdateCoroutine != null)
            {
                StopCoroutine(loadGaugeUpdateCoroutine);
                loadGaugeUpdateCoroutine = null;
            }

            if (phase != IntroPhase.Complete)
            {
                var loadPer = (float)phase / (float)IntroPhase.Complete;

                loadGaugeUpdateCoroutine = StartCoroutine(uiStart.LoadGaugeUpdate(loadPer));
            }
            else
            {
                uiStart.loadFillGauge.fillAmount = 1f;
            }

            switch (phase)
            {
                case IntroPhase.Start:
                    LoadComplete = true;
                    break;

                case IntroPhase.Resource:
                    ResourceManager.Instance.Initialize();
                    LoadComplete = true;
                    break;

                case IntroPhase.UI:
                    LoadComplete = true;
                    break;

                case IntroPhase.Complete:
                    SceneManager.LoadScene(SceneType.Lobby.ToString());
                    

                    allLoaded = true;
                    LoadComplete = true;
                    break;
            }

        }

        private void NextPhase()
        {
            StartCoroutine(WaitForSeconds());

            IEnumerator WaitForSeconds()
            {
                yield return new WaitForSeconds(1f);
                LoadComplete = false;
                OnPhase(++introPhase);
            }
        }
    }
}