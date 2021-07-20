using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PGSauce.Core.Utilities
{
    public static class PGExtensions
    {
        public static bool OutOfRange<T>(this List<T> list, int index)
        {
            return !(index >= 0 && index < list.Count);
        }
        
        public static string GetTransformPath(this Transform current) {
            if (current.parent == null)
                return "/" + current.name;
            return current.parent.GetTransformPath() + "/" + current.name;
        }
        
        public static float Remap(this float value, float inputA, float inputB, float outputA, float outputB)
        {
            return (value - inputA) / (inputB - inputA) * (outputB - outputA) + outputA;
        }

        public static float Remap(this int value, float inputA, float inputB, float outputA, float outputB)
        {
            return Remap((float)value, inputA, inputB, outputA, outputB);
        }

        /// 
        /// Is the object left, right, or in front ?
        /// 
        /// 
        /// 
        /// 
        /// -1 = left, 1 = right, 0 = in front (or behind)
        public static float RelativeOrientation(this Transform transform, Vector3 targetDir)
        {
            var up = transform.up;
            var forward = transform.forward;

            var perp = Vector3.Cross(forward, targetDir);
            var dir = Vector3.Dot(perp, up);
            if (dir > 0f)
            {
                return 1f;
            }
            else if (dir < 0f)
            {
                return -1f;
            }
            else
            {
                return 0f;
            }
        }

        /// <summary>
        /// positive = in front, negative = behind
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static float InFront(this Transform transform, Transform target)
        {
            var toTarget = (target.position - transform.position).normalized;
            return Vector3.Dot(toTarget, transform.forward);
        }
    }
}
