using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UCExtension.Common;
using UCExtension.GUI;
using UnityEngine;

namespace UCExtension.GUI
{
    public class UIFollowPoint<T> : MonoBehaviour where T : TransformFollowerUI
    {
        [SerializeField] T followerPrefab;

        Tween destroyTween = null;

        T spawnedFollower;

        public T Follower
        {
            get
            {
                if (!spawnedFollower)
                {
                    RespawnFollower();
                }
                return spawnedFollower;
            }
        }

        public void RespawnFollower()
        {
            destroyTween?.Kill();
            DestroyFollower();
            spawnedFollower = TransformFollowerManager.Ins.SpawnControlPositionUI(followerPrefab, transform);
        }

        public void DestroyFollower()
        {
            if (spawnedFollower)
            {
                spawnedFollower.Recycle();
                spawnedFollower = null;
            }
        }

        private void OnDestroy()
        {
            if (spawnedFollower)
            {
                Destroy(spawnedFollower.gameObject);
            }
        }

        private void OnEnable()
        {
            if (spawnedFollower)
            {
                spawnedFollower.gameObject.SetActive(true);
            }

        }

        private void OnDisable()
        {
            if (spawnedFollower)
            {
                spawnedFollower.gameObject.SetActive(false);
            }
        }
    }

}