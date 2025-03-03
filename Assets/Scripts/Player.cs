using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player P1;
    public SwipeDetector swipeDetector;

    private void OnEnable()
    {
        P1 = this;
    }
}