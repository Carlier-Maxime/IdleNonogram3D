using System.Collections;
using UnityEngine;

public class RotateBySwipe : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeed = 45f;
    [SerializeField]
    private float snapAngle = 90f;
    [SerializeField]
    private float snapDuration = 0.2f;
    private Vector3 _axis = Vector3.zero;
    private Coroutine _snappingCoroutine;
        
    private void Start()
    {
        var swipeDetector = Player.P1.swipeDetector;
        swipeDetector.OnSwipeStarted += RotateStarted;
        swipeDetector.OnSwipePerformed += RotatePerformed;
        swipeDetector.OnSwipeCanceled += RotateCanceled;
    }

    private void RotateStarted(Vector2 startPos, Vector2 direction, Vector2 delta)
    {
        _axis = Mathf.Abs(direction.x) > Mathf.Abs(direction.y) ? (direction.x > 0 ? Vector3.down : Vector3.up) : (direction.y > 0 ? Vector3.right : Vector3.left);
        if (_snappingCoroutine != null) StopCoroutine(_snappingCoroutine);
    }

    private void RotatePerformed(Vector2 startPos, Vector2 direction, Vector2 delta)
    {
        transform.Rotate(_axis, delta.magnitude * rotationSpeed, Space.World);
    }
    
    private void RotateCanceled(Vector2 startPos, Vector2 direction, Vector2 delta)
    {
        _axis = Vector3.zero;
        SnapToNearest90();
    }
    
    private void SnapToNearest90()
    {
        if (_snappingCoroutine != null) StopCoroutine(_snappingCoroutine);
        _snappingCoroutine = StartCoroutine(SmoothSnapRotation(snapAngle, snapDuration));
    }

    private IEnumerator SmoothSnapRotation(float angle, float duration)
    {
        var currentEuler = transform.rotation.eulerAngles;
        var snappedEuler = new Vector3(
            Mathf.Round(currentEuler.x / angle) * angle,
            Mathf.Round(currentEuler.y / angle) * angle,
            Mathf.Round(currentEuler.z / angle) * angle
        );

        var startRotation = transform.rotation;
        var targetRotation = Quaternion.Euler(snappedEuler);
        var elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        transform.rotation = targetRotation; 
    }
}