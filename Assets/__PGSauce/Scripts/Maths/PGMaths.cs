using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PGSauce.Core.Maths
{
    public static class PGMaths
    {
        public static Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
        {
            Func<float, float> f = x => -4 * height * x * x + 4 * height * x;
            var mid = Vector3.Lerp(start, end, t);
            return new Vector3(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
        }
        public static Vector2 Parabola(Vector2 start, Vector2 end, float height, float t)
        {
            Func<float, float> f = x => -4 * height * x * x + 4 * height * x;
            var mid = Vector2.Lerp(start, end, t);
            return new Vector2(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t));
        }

        public static float Sign(this float x, bool allowZero = false)
        {
            if (!allowZero) return Mathf.Sign(x);
            if (x < 0)
            {
                return -1;
            }
            return x > 0 ? 1 : 0;
        }

        public static float Abs(this float x)
        {
            return Mathf.Abs(x);
        }
    }
}
