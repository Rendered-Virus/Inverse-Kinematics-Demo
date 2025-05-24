using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
     [SerializeField] private float _speed;
     [SerializeField] private float _rotationSpeed;
     [SerializeField] private Transform _body;
     private Rigidbody _rigidbody;

     [SerializeField] private Transform[] _legs;
     [SerializeField] private float _bodyHeight;
     [SerializeField] private float _turnSpeed;
     [SerializeField] private float _offsetTime;
     private Vector3 _offset;
    
     private Vector3 _midpoint;
     private Vector3 _movement;

     private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        CalculateMidpoint();
        
        _offset = transform.position - _midpoint;
        //_offset = transform.worldToLocalMatrix.MultiplyVector(_offset);
    }

    private void FixedUpdate()
    {
        if (Input.GetAxisRaw("Vertical") != 0)
            _movement += _speed * Input.GetAxisRaw("Vertical") * Time.fixedDeltaTime * Vector3.forward;
        
        _rigidbody.linearVelocity = _body.forward * (_speed * Input.GetAxisRaw("Vertical"));
        
        CalculateMidpoint();
        BodyMovement();
        BodyRotation();
    }

    private void BodyMovement()
    {
       
    }
    private void BodyRotation()
    {
        Physics.Raycast(_body.position + _body.up * 2, -_body.up, out RaycastHit hit,10,LayerMask.GetMask("Ground"));
       
        //Rotating
        var rot  = Quaternion.LookRotation(Vector3.Cross(transform.right,hit.normal),hit.normal) * 
                             Quaternion.AngleAxis(Input.GetAxisRaw("Horizontal") * _rotationSpeed, hit.normal);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * _turnSpeed);
        
        //Positioning
        var pos = hit.point + _body.up * _bodyHeight;
        var dir =  pos - transform.position;
        
        _body.localPosition = Vector3.Lerp(_body.localPosition,_body.InverseTransformDirection(dir),_offsetTime * Time.deltaTime);
        print(transform.up);
    }

    private void CalculateMidpoint()
    {
        var left = Vector3.Lerp(_legs[0].position,_legs[1].position, 0.5f);
        var right = Vector3.Lerp(_legs[2].position,_legs[3].position, 0.5f);
        _midpoint = Vector3.Lerp(left, right, 0.5f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        //Gizmos.DrawWireSphere(_midpoint, 0.3f);
        Gizmos.DrawLine(_midpoint,_midpoint + transform.localToWorldMatrix.MultiplyVector(_offset));
    }
}
