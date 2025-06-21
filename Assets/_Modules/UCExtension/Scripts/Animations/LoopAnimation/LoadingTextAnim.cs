using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UCExtension.Animation
{
    [RequireComponent(typeof(Text))]
    public class LoadingTextAnim : LoopingAnim
    {
        Text mainText;

        [SerializeField] string prefix = "Loading";

        [SerializeField] string parttern = ".";

        [SerializeField] int loop = 3;

        [SerializeField] float time = 0.5f;
        public override bool MustStartInEnable => false;

        private void Awake()
        {
            mainText = GetComponent<Text>();
        }


        IEnumerator IEAnimation;

        IEnumerator IEPlayAnim()
        {
            while (true)
            {
                for (int i = 0; i <= loop; i++)
                {
                    string dot = "";
                    for (int j = 1; j <= i; j++)
                    {
                        dot += parttern;
                    }
                    mainText.text = prefix + dot;
                    yield return new WaitForSecondsRealtime(time);
                }
            }
        }

        public override void Play()
        {
            Stop();
            IEAnimation = IEPlayAnim();
            StartCoroutine(IEAnimation);
        }

        public override void Stop()
        {
            if (IEAnimation != null)
            {
                StopCoroutine(IEAnimation);
            }
        }
    }
}