using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;
    private Vector3 moveTargetPosition;
    Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        moveTargetPosition = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.MovePosition(Vector3.MoveTowards(rb.position, moveTargetPosition, speed * Time.fixedDeltaTime));

        if(Vector3.Distance(rb.position, moveTargetPosition) < 0.01f)
        {
            rb.position = moveTargetPosition;
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        Vector3 oldTargetPosition = moveTargetPosition;
        Vector2 input = context.ReadValue<Vector2>();

        if(Mathf.Abs(input.x) == 1.0f)
        {
            moveTargetPosition = transform.position +new Vector3(input.x, 0.0f, 0.0f);
        }
        else if(Mathf.Abs(input.y) == 1.0f)
        {
            moveTargetPosition = transform.position + new Vector3(0.0f, input.y, 0.0f);
        }
    }


}
