using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PGSauce.Core.PGPhysics
{
    public static class PgPhysics
    {
        public static Collider[] OverlapCapsule(CapsuleCollider col, int layerMask, float radiusFactor = 1)
        {
            var transform = col.transform;
            var center = transform.TransformPoint(col.center);
            var scale = transform.lossyScale;
            var radius = col.radius * scale.x;
            var height = col.height * scale.y;
            var bottom = new Vector3(center.x, center.y - height  / 2 + radius, center.z);
            var top = new Vector3(center.x, center.y + height / 2 - radius, center.z);
            var cols = Physics.OverlapCapsule(top, bottom, radius * radiusFactor, layerMask);
            return cols;
        }
    }
}
