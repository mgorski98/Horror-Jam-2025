using UnityEngine;

public static class Extensions {
    public static Vector3 Copy(this Vector3 v, float? overrideX = null, float? overrideY = null, float? overrideZ = null) {
        return new Vector3(overrideX ?? v.x, overrideY ?? v.y, overrideZ ?? v.z);
    }

    public static Vector3 ToFlatXZ(this Vector3 v) => new(v.x, 0, v.z);
}
