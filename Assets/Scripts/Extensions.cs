using UnityEngine;

public static class Extensions {
    public static Vector3 Copy(this Vector3 v, float? overrideX = null, float? overrideY = null, float? overrideZ = null) {
        return new Vector3(overrideX ?? v.x, overrideY ?? v.y, overrideZ ?? v.z);
    }
}
