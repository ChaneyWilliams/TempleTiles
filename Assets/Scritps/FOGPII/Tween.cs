using UnityEngine;
using DG.Tweening;

public class Tween : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //transform.DOLocalMoveY(transform.position.y + 1.0f, 1.0f, true);
        //transform.DOLocalMoveY(5.0f, 1.0f, true);

        TweenHelper.PunchUp(transform);
    }

}
