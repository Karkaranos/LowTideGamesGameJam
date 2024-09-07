using UnityEngine;
using UnityEngine.InputSystem;

public class LightMovement : MonoBehaviour
{
    [SerializeField] private float maxMouseSpeed;
    Vector2 mousePosition;
    void Start()
    {
        mousePosition = Vector2.zero;
    }

    public void FixedUpdate()
    {
        //Moves light to mouse position
        mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        transform.position = Vector2.MoveTowards(transform.position, mousePosition, maxMouseSpeed);
        //transform.position = mousePosition;
    }
}