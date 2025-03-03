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
}