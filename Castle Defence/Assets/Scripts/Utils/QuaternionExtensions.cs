using UnityEngine;

namespace Utils.Extensions
{
    public static class QuaternionExtensions
    {

        public static bool EqualsApproximately(this Quaternion a, Quaternion b, float acceptableRange)
        {
            return 1 - Mathf.Abs(Quaternion.Dot(a, b)) < acceptableRange;
        }
    }
}
