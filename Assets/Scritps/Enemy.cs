using UnityEngine;

public class Enemy : MonoBehaviour
{
    bool hasMoved = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasMoved)
        {
            hasMoved = true;
            MoveLeft();
        }
    }

    public void MoveLeft()
    {
        Vector3 target = new Vector3(transform.position.x - 0.5f, transform.position.y, transform.position.z);
        //TweenHelper.Move(transform, target);
        hasMoved = false;
    }
}
