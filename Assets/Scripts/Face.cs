using System.Collections.Generic;
using UnityEngine;

public enum Face
{
    North,
    South,
    East,
    West,
    Up,
    Down
}

public class FaceUtils
{
    private static readonly Dictionary<Face, Vector3> FaceRotations = new()
    {
        { Face.North, new Vector3(0, 0, 0) },
        { Face.South, new Vector3(0, 180, 0) },
        { Face.East, new Vector3(0, 90, 0) },
        { Face.West, new Vector3(0, 270, 0) },
        { Face.Up, new Vector3(270, 0, 0) },
        { Face.Down, new Vector3(90, 0, 0) }
    };
    
    public static Vector3 GetEulerRotation(Face face) => FaceRotations[face];
    public static Quaternion GetQuaternionRotation(Face face) => Quaternion.Euler(GetEulerRotation(face));
    public static Quaternion GetInverseQuaternionRotation(Face face) => Quaternion.Inverse(GetQuaternionRotation(face));
    
    private static readonly Dictionary<Face, Vector3> FaceNormals = new()
    {
        { Face.North, Vector3.back },
        { Face.South, Vector3.forward },
        { Face.East, Vector3.right },
        { Face.West, Vector3.left },
        { Face.Up, Vector3.up },
        { Face.Down, Vector3.down }
    };
    
    public static Vector3 GetNormal(Face face) => FaceNormals[face];
}