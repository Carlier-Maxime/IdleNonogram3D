using UnityEngine;

public class RotateBySwipe : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeed = 1f;
    private Vector3 _axis = Vector3.zero;
    private void Start()
    {
        var swipeDetector = SwipeDetector.Instance;
        swipeDetector.OnSwipeStarted += RotateStarted;
        swipeDetector.OnSwipePerformed += RotatePerformed;
        swipeDetector.OnSwipeCanceled += RotateCanceled;
    }

    private void RotateStarted(Vector2 startPos, Vector2 direction, Vector2 delta)
    {
        _axis = Mathf.Abs(direction.x) > Mathf.Abs(direction.y) ? (direction.x > 0 ? Vector3.down : Vector3.up) : (direction.y > 0 ? Vector3.right : Vector3.left);
    }

    private void RotatePerformed(Vector2 startPos, Vector2 direction, Vector2 delta)
    {
        transform.Rotate(_axis, delta.magnitude * rotationSpeed, Space.World);
    }
    
    private void RotateCanceled(Vector2 startPos, Vector2 direction, Vector2 delta)
    {
        _axis = Vector3.zero;
    }
}