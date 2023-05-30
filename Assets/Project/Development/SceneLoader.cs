using System;
using System.Collections;
using Project.Development.Events;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.Development
{
    public class SceneLoader: MonoBehaviour
    {
        public static SceneLoader Instance { get; private set; }

        [SerializeField] private GameObject eventSystemPrefab;
        [SerializeField] private GameObject gameplayCanvasPrefab;
        [SerializeField] private Spawner spawnerPrefab;
        [SerializeField] private GameObject mainCanvasPrefab;

        private Coroutine loadSceneCoroutine;
        
        
        
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else 
            {
                if (Instance != this)
                {
                    Destroy(this);
                    
                    return;
                }
                
                return;
            }
            
            EventManager.OnSceneLoaded.AddListener(OnSceneLoad);
        }

        private void OnDestroy()
        {
            EventManager.OnSceneLoaded.RemoveListener(OnSceneLoad);
        }
        
        


        public void LoadScene(string scene, Action onSceneLoaded = null)
        {
            if (this.loadSceneCoroutine != null)
            {
                return;
            }
            
            this.loadSceneCoroutine = StartCoroutine(LoadingScene(scene, onSceneLoaded));
        }
        


        private IEnumerator LoadingScene(string scene, Action onSceneLoaded = null)
        {
            EventManager.OnSceneLoad?.Invoke(scene);
            
            var asyncLoad = SceneManager.LoadSceneAsync(scene);

            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            onSceneLoaded?.Invoke();

            EventManager.OnSceneLoaded?.Invoke(scene);

            this.loadSceneCoroutine = null;
        }
        
        private void OnSceneLoad(string scene)
        {
            Instantiate(this.eventSystemPrefab);
            Instantiate(this.gameplayCanvasPrefab);
            Instantiate(this.spawnerPrefab);
            Instantiate(this.mainCanvasPrefab);
        }
    }
}