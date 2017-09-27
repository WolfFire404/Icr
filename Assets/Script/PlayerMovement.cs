using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private PlayerAnimation _playerAnimation;
    private Vector2 _velocity = Vector2.zero;
    private float _speedAddedPerSecond = 0.1f;

    public float WantedSpeed { get; private set; }

    public Vector2 Velocity
    {
        get { return _velocity; }
    }

    private const float Gravity = 50;
    private const float StandardSpeed = 5;
    private const float JumpHeight = 15;
    private const float Acceleration = 20;
    private const float BoostSpeed = 0.5f;

    private bool _grounded;
    
    public bool Grounded
    {
        get { return _grounded; }
    }
    
    bool facingRight = true;

    private bool grounded;
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
        bool boost = Camera.main.transform.position.x > transform.position.x;
        
        _velocity.x += Acceleration * Time.deltaTime;
        if (boost) _velocity.x += BoostSpeed * Time.deltaTime;
        if (_velocity.x > WantedSpeed)
            _velocity.x = boost ? WantedSpeed + BoostSpeed : WantedSpeed;
    }

    private void Jump()
    {
        if (grounded)
        {
            _playerAnimation.SetAnimation("player_jump");
            _velocity.y = JumpHeight;
        }
        else
        {
            Vector3 direction = Vector2.right;
            if (_velocity.x < 0) direction *= -1;
            var info = Physics2D.Raycast(transform.position, direction, _velocity.x * Time.deltaTime, collisionMask);

            if (info)
            {
                Flip();
                _velocity.y = JumpHeight;
            }

        }
    }

    private void AddGravity()
    {
        _velocity.y -= Gravity * Time.deltaTime;
    }

    private void SideCollision()
    {
        Vector2 bottomRight = transform.position;
        bottomRight.x += 0.5f;
        bottomRight.y -= 0.4f;
        var topRight = bottomRight;
        topRight.y += 0.8f;

        for (var i = 0; i < 2; i++)
        {
            var info = Physics2D.Raycast(i == 0 ? bottomRight : topRight, Vector2.right,
                _velocity.x * Time.deltaTime, collisionMask);

            if (!info) continue;
            
            transform.position = new Vector3(info.point.x - 0.5f, transform.position.y, transform.position.z);
            _velocity.x = 0;
            return;
        }
    }

    private void CheckGrounded()
    {
        

        Vector2 bottomRight,
            bottomLeft = transform.position;
        bottomLeft.y -= 0.5f;
        bottomLeft.x -= 0.4f;
        bottomRight = bottomLeft;
        bottomRight.x += 0.8f;
        
        Debug.DrawLine(bottomRight, bottomRight + (Vector2.down * _velocity.y * Time.deltaTime    ));

        for (var i = 0; i < 2; i++)
        {
            var info = Physics2D.Raycast(i == 0 ? bottomRight : bottomLeft, Vector2.down,
                _velocity.y * Time.deltaTime * -1,
                collisionMask);
            
            if (!info) continue;
            
            // Move the player down.
            transform.position = new Vector3(transform.position.x, info.point.y + 0.5f, transform.position.z);
            _grounded = true;
            return;
        }

        _grounded = false;
    }

    void Flip()
    {

        // Switch the way the player is labelled as facing
        facingRight = !facingRight;

        //Multiply the player's x local cale by -1
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}