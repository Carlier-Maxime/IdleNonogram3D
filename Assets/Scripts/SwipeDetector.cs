using UnityEngine;
using UnityEngine.InputSystem;

public class SwipeDetector : MonoBehaviour
{
    public delegate void Swipe(Vector2 startPos, Vector2 direction, Vector2 delta);
    public event Swipe OnSwipeStarted;
    public event Swipe OnSwipePerformed;
    public event Swipe OnSwipeCanceled;
    [SerializeField]
    private InputActionReference swipeClickAction;
    [SerializeField]
    private float swipeThreshold = 0.05f;
    private Vector2 _startPosition = Vector2.zero;
    private Vector2 _direction = Vector2.zero;
    private Vector2 _delta = Vector2.zero;

    private void OnEnable()
    {
        swipeClickAction.action.started += SwipeStarted;
        swipeClickAction.action.canceled += SwipeCanceled;
        swipeClickAction.action.Enable();
    }

    private void OnDisable()
    {
        swipeClickAction.action.Disable();
        swipeClickAction.action.started -= SwipeStarted;
        swipeClickAction.action.canceled -= SwipeCanceled;
    }
    
    private void SwipeStarted(InputAction.CallbackContext obj)
    {
        _startPosition = Utils.NormalizeSreenCoords(Player.P1.posAction.action.ReadValue<Vector2>());
        Player.P1.posAction.action.performed += SwipePerformed;
    }
    
    private void SwipePerformed(InputAction.CallbackContext obj)
    {
        var direction = Utils.NormalizeSreenCoords(Player.P1.posAction.action.ReadValue<Vector2>()) - _startPosition;
        if (direction.magnitude < swipeThreshold) return;
        if (_direction == Vector2.zero)
        {
            _direction = direction;
            OnSwipeStarted?.Invoke(_startPosition, _direction, _direction);
        }
        _delta = direction - _direction;
        _direction = direction;
        OnSwipePerformed?.Invoke(_startPosition, _direction, _delta);
    }
    
    private void SwipeCanceled(InputAction.CallbackContext obj)
    {
        OnSwipeCanceled?.Invoke(_startPosition, _direction, _delta);
        Player.P1.posAction.action.performed -= SwipePerformed;
        _startPosition = Vector2.zero;
        _direction = Vector2.zero;
        _delta = Vector2.zero;
    }
}