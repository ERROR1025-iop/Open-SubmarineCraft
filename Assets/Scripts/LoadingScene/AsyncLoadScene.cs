using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Scraft
{

    public class AsyncLoadScene : MonoBehaviour
    {
        public Image background;
        static public Sprite sprite;

        public IProgressbar progressbar;
        private float imaginaryValue;
        private float imaginaryValueStep;

        private float loadingSpeed = 1;
        private float targetValue;
        private AsyncOperation operation;

        // Use this for initialization
        void Start()
        {
            background.sprite = sprite;

            imaginaryValueStep = GameSetting.isAndroid ? 0.02f : 0.5f;
            progressbar.setValue01(0);
            if (SceneManager.GetActiveScene().name == "Loading")
            {
                //启动协程
                StartCoroutine(AsyncLoading());
            }            
        }

        IEnumerator AsyncLoading()
        {
            operation = SceneManager.LoadSceneAsync(World.nextSceneName);
            //阻止当加载完成自动切换
            operation.allowSceneActivation = false;

            yield return operation;
        }

        // Update is called once per frame
        void Update()
        {
            targetValue = operation.progress;
            if (operation.progress >= 0.9f)
            {
                targetValue = 1.0f;
            }

            if (imaginaryValue < 1)
            {
                imaginaryValue += imaginaryValueStep;
            }
            else
            {
                imaginaryValue = 1.0f;
            }

            if (imaginaryValue != progressbar.percent)
            {
                progressbar.setValue01(imaginaryValue);
            }



            if ((int)(targetValue * 100) == 100 && progressbar.percent >= 1f)
            {
                //允许异步加载完毕后自动切换场景
                operation.allowSceneActivation = true;
            }
        }

        public static void asyncloadScene(string sceneName)
        {
            World.nextSceneName = sceneName;
            SceneManager.LoadScene("Loading");
        }
    }
}