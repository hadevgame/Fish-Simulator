using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UCExtension
{
    public static class QuaternionExtension
    {
        public static Quaternion Lerp(this Quaternion quaternion, Quaternion target, float t)
        {
            return Quaternion.Lerp(quaternion, target, t);
        }
    }
}