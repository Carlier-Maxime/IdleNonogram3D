using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public static Player P1;
    public SwipeDetector swipeDetector;
    public InputActionReference posAction;
    public ClickDetector clickDetector;

    private void OnEnable()
    {
        P1 = this;
    }

    public void MoveForShowSouthFace(Bounds bounds)
    {
        if (Camera.main == null) return;
        var fovV = Camera.main.fieldOfView * Math.PI / 180;
        var aspectRatio = Screen.width / (float) Screen.height;
        var fovH = 2 * Math.Atan(Math.Tan(fovV / 2) * aspectRatio);
        var distH = (float) (bounds.extents.x / Math.Tan(fovH / 2));
        var distV = (float) (bounds.extents.y / Math.Tan(fovV / 2));
        var requiredDistance = Math.Max(distH, distV);
        var pos = bounds.center;
        pos.z -= bounds.extents.z + requiredDistance;
        transform.position = pos;
        transform.LookAt(bounds.center - new Vector3(0, 0, bounds.extents.z));
    }
}