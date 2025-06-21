using DG.Tweening;
using System.Collections;
using UCExtension.Common;
using UnityEngine;
using UnityEngine.Events;


namespace UCExtension.UI
{
    public class GetPointBar : MonoBehaviour
    {
        [SerializeField] RectTransform destination;

        [SerializeField] Sprite icon;

        [SerializeField] float jumpPower = 50f;

        [SerializeField] float jumpRange = 100f;

        JumpIcon iconPrefab;

        float timeIncrease = 0.15f;

        float totalTimeRoot = 1f;

        Sequence iconSeq;

        private void Awake()
        {
            iconPrefab = Resources.Load<JumpIcon>("Prefabs/JumpIcon");
        }

        public void Get(int quantities, Vector3 position, UnityAction stepFinish = null, UnityAction finish = null)
        {
            StartCoroutine(IEGet(position, quantities, stepFinish, finish));
        }

        IEnumerator IEGet(Vector3 position, int quantities, UnityAction stepFinish = null, UnityAction finish = null)
        {
            int numWait = quantities;
            for (int i = 0; i < quantities; i++)
            {
                var item = CreateIcon(position);
                Vector3 JumpPos = item.transform.position.GetRandomInRange(jumpRange, jumpRange, 0);
                float totalTime = totalTimeRoot + timeIncrease * i;
                float timeJump = totalTime * Random.Range(0.4f, 0.6f);
                float timeMove = totalTime - timeJump;
                item.transform.DOJump(JumpPos, jumpPower, 1, timeJump).OnComplete(() =>
                {
                    item.transform.DOMove(destination.position, timeMove).OnComplete(() =>
                    {
                        numWait--;
                        stepFinish?.Invoke();
                        item.Recycle();
                        DestinationSpring();
                    }).SetUpdate(true);
                }).SetUpdate(true);
            }
            yield return new WaitUntil(() => numWait <= 0);
            finish?.Invoke();
        }

        void DestinationSpring()
        {
            iconSeq?.Kill();
            iconSeq = DOTween.Sequence();
            destination.localScale = Vector3.one;
            iconSeq.Insert(0, destination.DOScale(Vector3.one * 1.3f, 0.1f));
            iconSeq.Insert(0.1f, destination.DOScale(Vector3.one, 0.2f));
            iconSeq.SetUpdate(true);
        }

        JumpIcon CreateIcon(Vector3 pos)
        {
            var item = PoolObjects.Ins.Spawn(iconPrefab);
            item.gameObject.SetActive(true);
            item.transform.SetParent(transform);
            item.transform.localScale = Vector3.one;
            item.transform.position = pos;
            item.GetComponent<RectTransform>().sizeDelta = destination.sizeDelta;
            item.SetIcon(icon);
            return item;
        }
    }
}