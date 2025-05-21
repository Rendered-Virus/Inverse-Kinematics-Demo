using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[Serializable]
public class LegData
{
    public Transform leg;
    [HideInInspector]public Vector3 point;
    public float distance;
    [HideInInspector]public bool moving;
    public Vector3 bodyPointOffset;
}

public class LegMovement : MonoBehaviour
{
    [SerializeField] private Transform _body;
    
    [SerializeField] private LegData[] _legs;
    
    [SerializeField] private float _maxDistance;
    [SerializeField] private float _legMoveTime;
    [SerializeField] private float _legMovementY;
    [SerializeField] private float _footPlacementOffset;
    [SerializeField] private float _backFootDelay;

    private void Start()
    {
        foreach (var leg in _legs)
        {
            leg.bodyPointOffset = leg.leg.position - _body.position;
        }
    }

    private void FixedUpdate()
    {
        WalkCycle();
    }

    private void WalkCycle()
    {
        for (var i = 0; i < _legs.Length; i++)
        {
            var leg = _legs[i];

            if (i == 0 || i == 4 || i == 2)
            {
                if (!_legs[1].moving && !_legs[3].moving)
                {
                    CastPoint(leg);
                    DistanceCheck(leg);
                }
            }
            else
            {
                if (!_legs[0].moving && !_legs[4].moving)
                {
                    CastPoint(leg);
                    DistanceCheck(leg);
                }
            }

        }
    }
    private void DistanceCheck(LegData leg)
    {
        if (leg.distance > _maxDistance && !leg.moving)
            MoveLeg(leg);
    }

    private void MoveLeg(LegData leg)
    {
        
        var legTransform = leg.leg;
        var point = leg.point + _footPlacementOffset * -_body.forward;
        
        leg.moving = true;
        
        var halfPoint = legTransform.position + (point - legTransform.position) * 0.5f;
        halfPoint += transform.up * _legMovementY;
        legTransform.DOMove(halfPoint, _legMoveTime * 0.5f).OnComplete(() =>
        {
            legTransform.DOMove(point, _legMoveTime * 0.5f).OnComplete(() => leg.moving = false);
        });
    }
    private void CastPoint(LegData leg)
    {
       /* var point = _body.position;
        point += transform.right * leg.leg.localPosition.x;*/

        var point = _body.position + leg.bodyPointOffset;
        
        Physics.Raycast(point + _body.up, -_body.up, out RaycastHit hit, 5, LayerMask.GetMask("Ground"));
        
        leg.point = hit.point;
        leg.distance = Vector3.Distance(hit.point, leg.leg.position);
    }

    private void OnDrawGizmos()
    {
        Draw(_legs[0]);
    }

    private void Draw(LegData leg)
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(leg.point, .4f);
    }
}
