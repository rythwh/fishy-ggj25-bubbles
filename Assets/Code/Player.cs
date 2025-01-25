using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Vector2 movementInput;
    
    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
        Debug.Log("Moving!");
    }

    private void Update()
    {
        Vector3 moveDirection = new Vector3(movementInput.x, 0f, movementInput.y);
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }
}