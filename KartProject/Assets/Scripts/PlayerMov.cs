using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMov : MonoBehaviour
{
    public float turnSpeed, maxSpeed, speed, actualMovementSpeed, acceleration, deacceleration, brakeForce;
    public float turningTime, changeTurningAnimationTime;
    private Rigidbody2D _rb;
    private Animator _animator;
    public Vector3 LastFramePos;
    public bool isMoving, movingBackward, secondTurnAnimation, isDrifting, isDriftingLeft, isDriftingRight;
    public float movVer, movHor;
    private float _tempTurnSpeed;
    private SpriteRenderer _spriteRenderer;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _tempTurnSpeed = turnSpeed;
    }
    private void Update()
    {
        movHor = Input.GetAxis("Horizontal");
        movVer = Input.GetAxis("Vertical");
        if (!isDrifting)
        {
            _spriteRenderer.flipX = movHor > 0.01 ? true : false;
        }
        //    turnSpeed = secondTurnAnimation == false ? _tempTurnSpeed : _tempTurnSpeed - _tempTurnSpeed / 4;

        Acceleration();

        if (isMoving)
        {
            if (movVer < 0 && speed > 0)
            {
                Brake();
            }

            Turning();
        }
        else
        {
            turningTime = 0;
            secondTurnAnimation = false;
        }

        CalculateSpeed();
        MoveBackward();
        Drift();
        _animator.SetFloat("Horizontal", Mathf.Abs(movHor));
        _animator.SetBool("isMoving", isMoving);
        _animator.SetBool("SecondTurnAnimation", secondTurnAnimation);


    }
    //se fazer curva por X tempo, dimuinuir a turn speed (drift(?))

    //drift
    private void Acceleration()
    {
        if (speed < maxSpeed - 4)
        {
            secondTurnAnimation = false;
            turningTime = 0;
        }

        if (movVer > 0)
        {
            if (speed < maxSpeed)
            {
                speed += (movVer + acceleration) * Time.deltaTime; //acceleration
            }
        }
        else if (movVer == 0)
        {
            if (speed > 0)
            {
                speed -= (movVer + deacceleration) * Time.deltaTime; //braking
                if (speed <= 0.2)
                {
                    speed = 0;
                }
            }
        }
        transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);
    }

    private void Brake()
    {
        speed -= (movVer + brakeForce) * Time.deltaTime;
        if (speed <= 0.2)
        {
            speed = 0;
        }
        Debug.Log("braking");
    }
    private void Turning()
    {
        if (movHor != 0)
        {
            turningTime += Time.deltaTime;
            if (speed > speed / 2 && secondTurnAnimation)
            {
                if (!isDrifting)
                {
                    speed -= acceleration * 1.5f * Time.deltaTime; //reduce speed by turning
                }
            }
            if (turningTime > changeTurningAnimationTime)
            {
                secondTurnAnimation = true;
            }
        }
        else if (Mathf.Abs(movHor) < 1)
        {
            secondTurnAnimation = false;
            turningTime = 0;
        }
    }

    private void Drift()
    {
        if (Input.GetKey(KeyCode.Space) && (movHor != 0 || isDrifting))
        {
            isDrifting = true;
            if (isDrifting && !isDriftingRight && !isDriftingLeft)
            {
                if (movHor > 0.1)
                {
                    isDriftingRight = true;
                    isDriftingLeft = false;
                }
                if (movHor < -0.1)
                {
                    isDriftingLeft = true;
                    isDriftingRight = false;
                }
            }
            if (isDriftingLeft)
            {
                if (movHor < -0.1f)
                {
                    turnSpeed = _tempTurnSpeed * 0.5f;
                }
                else if (movHor > 0.1f)
                {
                    turnSpeed = 20;
                }
            }
            if (isDriftingRight)
            {
                if (movHor > 0.1f)
                {
                    turnSpeed = _tempTurnSpeed * 0.5f;
                }
                else if (movHor < -0.1f)
                {
                    turnSpeed = 20;
                }
            }
        }
        else
        {
            isDrifting = false;
            isDriftingRight = false;
            isDriftingLeft = false;
            turnSpeed = _tempTurnSpeed;
        }
    }
    private void CalculateSpeed()
    {
        float movementPerFrame = Vector3.Distance(this.transform.position, LastFramePos);
        actualMovementSpeed = movementPerFrame / Time.deltaTime;
        LastFramePos = transform.position;
        isMoving = actualMovementSpeed > 0 ? true : false;
    }
    public void MoveBackward()
    {

        if (isMoving && !movingBackward)
        {
            transform.Rotate(0f, movHor * turnSpeed * Time.deltaTime, 0f, Space.Self);
        }
        else
        {
            if (movVer < -0.1f)
            {
                movingBackward = true;
                transform.Translate(Vector3.forward * Mathf.Lerp(-1, 0, movVer) * Time.deltaTime, Space.Self);
                transform.Rotate(0f, -movHor * turnSpeed * Time.deltaTime, 0f, Space.Self);
            }
            else
            {
                movingBackward = false;
            }
        }
    }
}
