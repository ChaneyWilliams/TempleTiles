using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    public static Player instance;
    [SerializeField] private float speed = 5.0f;
    private Vector3 moveTargetPos;
    public Vector3 pickUpPos;
    Rigidbody2D rb;
    TileBase currentTile;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        rb = GetComponent<Rigidbody2D>();
        moveTargetPos = transform.position;
        pickUpPos = gameObject.transform.position + Vector3.right;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.MovePosition(Vector3.MoveTowards(rb.position, moveTargetPos, speed * Time.fixedDeltaTime));

        if (Vector3.Distance(rb.position, moveTargetPos) < 0.01f)
        {
            rb.position = moveTargetPos;
            TileData currentTile = GameManager.instance.GetTileFromMap(gameObject.transform.position);
            GameManager.instance.TileChoices(currentTile, gameObject);
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (GameManager.instance.currentGameState != GameManager.GameState.PlayerTurn)
                return;
            Vector3 oldTargetPosition = moveTargetPos;
            Vector2 input = context.ReadValue<Vector2>();

            if (Mathf.Abs(input.x) == 1.0f)
            {
                moveTargetPos = transform.position + new Vector3(input.x, 0.0f, 0.0f);
                pickUpPos = moveTargetPos + new Vector3(input.x, 0.0f, 0.0f);
                TileData futureTile = GameManager.instance.GetTileFromMap(moveTargetPos);
                if (futureTile == null)
                {
                    moveTargetPos = oldTargetPosition;
                    pickUpPos = moveTargetPos + new Vector3(0.0f, input.y, 0.0f);
                    return;
                }
            }
            else if (Mathf.Abs(input.y) == 1.0f)
            {
                moveTargetPos = transform.position + new Vector3(0f, input.y, 0f);
                pickUpPos = moveTargetPos + new Vector3(0f, input.y, 0f);
                if (GameManager.instance.GetTileFromMap(moveTargetPos) == null || GameManager.instance.GetTileFromMap(moveTargetPos).tileState == TileData.TileState.WallTile)
                {
                    moveTargetPos = oldTargetPosition;
                    pickUpPos = moveTargetPos + new Vector3(0f, input.y, 0f);
                    return;
                }
            }
            GameManager.instance.ChangeGameState(GameManager.GameState.LevelTurn);
        }
    }

    public void PickupTile(InputAction.CallbackContext context)
    {
        TileData tile = GameManager.instance.GetTileFromMap(pickUpPos);
        if (currentTile == null)
        {
            if(tile == null)return;
            switch (tile.tileState)
            {
                case TileData.TileState.NormalTile:
                    currentTile = GameManager.instance.GetTileBase(0);
                    break;
                default:
                    return;
            }
            GameManager.instance.map.SetTile(Vector3Int.FloorToInt(pickUpPos), null);
        }
        else
        {
            if (tile == null)
            {
                GameManager.instance.map.SetTile(Vector3Int.FloorToInt(pickUpPos), currentTile);
                currentTile = null;
            }
        }
    }


}
