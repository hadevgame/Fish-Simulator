using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UCExtension
{
    [CreateAssetMenu(fileName = "FindAnimationSettings", menuName = "UCExtension/Find Animation Settings")]
    public class FindAnimationSettings : ScriptableObject
    {
        public List<AnimatorOverrideConfig> Settings;
    }
}