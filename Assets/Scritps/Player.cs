using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    public static Player instance;
    [SerializeField] private float speed = 5.0f;
    public Vector3 moveTargetPos;
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
        float alpha = Mathf.Lerp(0.5f, 1.0f, (Mathf.Sin(Time.time * 2.0f) + 1.0f) / 2.0f);

        Color color = new Color(0.4f, 0.4f, 0.4f, alpha);
        GameManager.instance.SetTileColor(pickUpPos, color);
        rb.MovePosition(Vector3.MoveTowards(rb.position, moveTargetPos, speed * Time.fixedDeltaTime));

        if(currentTile != null && GameManager.instance.GetTileFromMap(pickUpPos) == null)
        {
            GameManager.instance.SetPreviewTile(pickUpPos, color, currentTile);
        }

        if (Vector3.Distance(rb.position, moveTargetPos) < 0.01f)
        {
            rb.position = moveTargetPos;
            TileData currentTile = GameManager.instance.GetTileFromMap(gameObject.transform.position);
            GameManager.instance.TileChoices(currentTile, gameObject);
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        GameManager.instance.ClearPreviewMap();
        if (context.performed)
        {
            if (GameManager.instance.currentGameState != GameManager.GameState.PlayerTurn)
                return;
            Vector3 oldTargetPosition = moveTargetPos;
            Vector2 input = context.ReadValue<Vector2>();
            GameManager.instance.SetTileColor(pickUpPos, new Color(1.0f, 1.0f, 1.0f, 1.0f));
            
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
            if (tile == null) return;
            switch (tile.tileState)
            {
                case TileData.TileState.NormalTile:
                    currentTile = GameManager.instance.GetTileBase(0);
                    break;
                case TileData.TileState.FireTile:
                    currentTile = GameManager.instance.GetTileBase(1);
                    break;
                case TileData.TileState.GrassTile:
                    currentTile = GameManager.instance.GetTileBase(2);
                    break;
                case TileData.TileState.WaterTile:
                    currentTile = GameManager.instance.GetTileBase(3);
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
