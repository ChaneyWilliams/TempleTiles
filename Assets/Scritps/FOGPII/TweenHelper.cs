using UnityEngine;
using DG.Tweening;

public static class TweenHelper
{
     public static Sequence PunchUp(Transform transform)
    {
       Sequence sq = DOTween.Sequence();

        sq.Append(transform.DOLocalMoveY(transform.position.y + 1.0f, 1.5f));
        sq.Join(transform.DOPunchScale(new Vector3(1.5f, 1.5f, 1.5f),0.7f));

        return sq;
    }

    public static Sequence Move(Transform transform, Vector3 where)
    {
        Sequence sq = DOTween.Sequence();

        sq.Append(transform.DOLocalMove(new Vector3(transform.position.x + where.x,
                                        transform.position.y + where.y,
                                        transform.position.z + where.z), 1.0f));
        sq.Join(transform.DOPunchScale(new Vector3(1.0f, 0.5f, 1.0f), 1.0f));
        return sq;
    }

    public static Sequence PopAndSpin(Transform transform)
    {
        Sequence sq = DOTween.Sequence();

        sq.Append(transform.DOPunchRotation(new Vector3(0.0f, 0.0f, 15.0f), 0.5f));
        sq.Join(transform.DOPunchScale(new Vector3(0.5f, 0.5f, 0.5f), 0.5f));

        return sq;
    }
}
