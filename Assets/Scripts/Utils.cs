
using UnityEngine;

public class Utils
{
    public static Vector2 NormalizeSreenCoords(Vector2 coords)
    {
        coords.x /= Screen.width;
        coords.y /= Screen.height;
        return coords;
    }

    public static Vector2 UnNormalizeScreenCoords(Vector2 normalizedCoords)
    {
        normalizedCoords.x *= Screen.width;
        normalizedCoords.y *= Screen.height;
        return normalizedCoords;
    }
}