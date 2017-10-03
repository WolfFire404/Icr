using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private PlayerAnimation _playerAnimation;
    private Vector2 _velocity = Vector2.zero;
    private float _speedAddedPerSecond = 0.1f;
    private float _lastWallJumpTime;

    public float WantedSpeed { get; private set; }

    public Vector2 Velocity
    {
        get { return _velocity; }
    }

    private const float Gravity = 60;
    private const float StandardSpeed = 5;
    private const float JumpHeight = 17;
    private const float Acceleration = 10;
    private const float BoostSpeed = 1.5f;

    private bool _grounded;
    private float _direction = 1;
    
    public bool Grounded
    {
        get { return _grounded; }
    }
    
    bool facingRight = true;

    public LayerMask CollisionMask {get { return collisionMask; }}
    
    private void Move()
    {
        var pos = transform.position;
        pos.x += _velocity.x * Time.deltaTime;
        pos.y += _velocity.y * Time.deltaTime;
        transform.position = pos;
    }

    private void Start()
    {
        WantedSpeed = StandardSpeed;
        _playerAnimation = GetComponent<PlayerAnimation>();
    }

    private void Update()
    {
        AddHorizontalVelocity();
        AddGravity();
        SideCollision();
        CheckGrounded();
        AddSpeed();
        
        if (_grounded)
            _velocity.y = 0;
        else UpdateJump();
        if (Input.GetKeyDown(KeyCode.Space))
            Jump();    
        
        Move();
    }

    private void AddSpeed()
    {
        WantedSpeed += _speedAddedPerSecond * Time.deltaTime;
    }

    private void AddHorizontalVelocity()
    {
        float sign = Mathf.Sign(transform.localScale.x);
        bool boost = Camera.main.transform.position.x > transform.position.x;


        _velocity.x += Acceleration * Time.deltaTime * sign;
        if (boost && sign == 1) _velocity.x += BoostSpeed * Time.deltaTime;
        if (Mathf.Abs(_velocity.x) > WantedSpeed)
        {
            _velocity.x = boost ? WantedSpeed + BoostSpeed : WantedSpeed;
            _velocity.x *= sign;
        }
    }

    private void UpdateJump()
    {
        Vector3 pos = transform.position;
        pos.y -= 0.2f;
        var info = Physics2D.Raycast(pos, Vector2.right, _velocity.x * Time.deltaTime + Mathf.Sign(transform.localScale.x) * 1.2f, collisionMask);

        _lastWallJumpTime = !info ? -1 : Time.time;
    }

    private void Jump()
    {
        if (_grounded)
        {
            _velocity.y = JumpHeight;
            _playerAnimation.SetAnimation("player_jump");
        }
        else
        {
            if (Time.time > _lastWallJumpTime + 1.5f || _lastWallJumpTime == -1) return;
            _velocity.x = WantedSpeed * 2 * -transform.localScale.x;
            _velocity.y = JumpHeight;
            _lastWallJumpTime = -1;
            Flip();
        }
    }

    private void AddGravity()
    {
        _velocity.y -= Gravity * Time.deltaTime;
    }

    private void SideCollision()
    {
        float sign = Mathf.Sign(_velocity.x);
        Vector2 bottomRight = transform.position;
        bottomRight.x += 0.5f * sign;
        bottomRight.y -= 0.4f;
        var topRight = bottomRight;
        topRight.y += 0.8f;

        for (var i = 0; i < 2; i++)
        {
            var info = Physics2D.Raycast(i == 0 ? bottomRight : topRight, Vector2.right,
                _velocity.x * Time.deltaTime, collisionMask);

            if (!info) continue;
            
            transform.position = new Vector3(info.point.x - 0.5f * Mathf.Sign(_velocity.x), transform.position.y, transform.position.z);
            _velocity.x = 0;
            return;
        }
    }

    private void CheckGrounded()
    {
        Vector2 bottomLeft = transform.position;
        bottomLeft.y += 0.5f * Mathf.Sign(_velocity.y);
        bottomLeft.x -= 0.4f;
        var bottomRight = bottomLeft;
        bottomRight.x += 0.8f;
        
        Debug.DrawLine(bottomRight, bottomRight + (Vector2.down * _velocity.y * Time.deltaTime    ));

        for (var i = 0; i < 2; i++)
        {
            var info = Physics2D.Raycast(i == 0 ? bottomRight : bottomLeft, Vector2.down,
                _velocity.y * Time.deltaTime * -1,
                collisionMask);
            
            if (!info) continue;
            
            // Move the player down.
            transform.position = new Vector3(transform.position.x, info.point.y - 0.5f * Mathf.Sign(_velocity.y), transform.position.z);
            _grounded = true;
            return;
        }

        _grounded = false;
    }

    private void Flip()
    {
        facingRight = !facingRight;
        
        var theScale = transform.localScale;
        theScale.x *= -1; 
        transform.localScale = theScale;
    }
}