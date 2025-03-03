
using UnityEngine;

public class Utils
{
    public static Vector2 NormalizeSreenCoords(Vector2 coords)
    {
        coords.x /= Screen.width;
        coords.y /= Screen.height;
        return coords;
    }
}