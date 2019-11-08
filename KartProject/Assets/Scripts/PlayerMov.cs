using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMov : MonoBehaviour
{
    public float turnSpeed, maxSpeed, speed, actualMovementSpeed, acceleration, deacceleration, brakeForce, driftSpeed;
    public float turningTime, changeTurningAnimationTime;
    private Rigidbody2D _rb;
    private Animator _animator;
    public Vector3 LastFramePos;
    public bool isMoving, movingBackward, secondTurnAnimation, isDrifting, isDriftingLeft, isDriftingRight;
    public float movVer, movHor;
    private float _tempTurnSpeed;
    private SpriteRenderer _spriteRenderer;
    public int driftBoost;
    public float timeToDriftBoost, actualTimeToDriftBoost;
    private GameManager _gameManager;
    public KartData kartInfo;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _gameManager = FindObjectOfType<GameManager>();
        turnSpeed = kartInfo.turnSpeed;
        maxSpeed = kartInfo.maxSpeed;
        acceleration = kartInfo.acceleration;
        deacceleration = kartInfo.deacceleration;
        brakeForce = kartInfo.brakeForce;
        driftSpeed = kartInfo.driftSpeed;
        _tempTurnSpeed = turnSpeed;
    }
    private void Start()
    {
        actualTimeToDriftBoost = 0;
    }
    private void Update()
    {
        movHor = Input.GetAxis("Horizontal");
        movVer = Input.GetAxis("Vertical");
        _gameManager.SetSpeedUI(speed);
        if (!isDrifting)
        {
            _spriteRenderer.flipX = movHor > 0.001 ? true : false;
        }
        if (speed >= maxSpeed + driftSpeed * 2)
        {
            speed = maxSpeed + driftSpeed * 2;
        }
        if (speed >= maxSpeed)
        {
            speed -= (movVer + deacceleration / 2) * Time.deltaTime; //deacceleration
        }

        Acceleration();

        if (isMoving || isDrifting)
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

        if (speed >= maxSpeed - 2)
        {
            Drift();
        }
        //  else
        //  {
        //   isDrifting = false;
        //   driftBoost = 0;
        //  FinishDrift();
        //}
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
                speed -= (movVer + deacceleration) * Time.deltaTime; //deacceleration
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
        speed -= (movVer + brakeForce) * Time.deltaTime; // braking
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
                    speed -= acceleration * 1.25f * Time.deltaTime; //reduce speed by turning without drift
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
        if (Input.GetKeyDown(KeyCode.Space) || !isDrifting)
        {
            isDrifting = true;
            _animator.SetBool("isDrifting", isDrifting);
        }
        if (Input.GetKeyUp(KeyCode.Space) && actualTimeToDriftBoost < 1 && isDrifting)
        {
            FinishDrift();
            speed -= 5;
            //fumaça preta sinalizando falha
        }
        if (Input.GetKey(KeyCode.Space) && (/*movHor != 0 ||*/ isDrifting))
        {
            //boost
            actualTimeToDriftBoost += Time.deltaTime;
            if (actualTimeToDriftBoost >= timeToDriftBoost && actualTimeToDriftBoost <= timeToDriftBoost * 2)
            {
                driftBoost = 1;
            }
            else if (actualTimeToDriftBoost >= timeToDriftBoost * 2)
            {
                driftBoost = 2;
            }
            else if (actualTimeToDriftBoost < 1)
            {
                driftBoost = 0;
            }
            //end boost

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
                    turnSpeed = _tempTurnSpeed * 0.65f;
                }
                else if (movHor > 0.1f)
                {
                    turnSpeed = _tempTurnSpeed * 0.1f;
                }
            }
            if (isDriftingRight)
            {
                if (movHor > 0.1f)
                {
                    turnSpeed = _tempTurnSpeed * 0.65f;
                }
                else if (movHor < -0.1f)
                {
                    turnSpeed = _tempTurnSpeed * 0.1f;
                }
            }
        }
        else
        {
            if (isDrifting)
            {
                FinishDrift();
            }
        }
    }

    private void FinishDrift()
    {
        isDrifting = false;
        isDriftingRight = false;
        isDriftingLeft = false;
        turnSpeed = _tempTurnSpeed;
        _animator.SetBool("isDrifting", isDrifting);

        if (driftBoost >= 1 && driftBoost < 2)
        {
            speed += driftSpeed;
            driftBoost = 0;
            actualTimeToDriftBoost = 0;
            Debug.Log("boost1");
        }
        else if (driftBoost >= 2)
        {
            speed += driftSpeed * 1.5f;
            driftBoost = 0;
            actualTimeToDriftBoost = 0;
            Debug.Log("boost2");
        }
        else if (driftBoost < 1)
        {
            driftBoost = 0;
            actualTimeToDriftBoost = 0;
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
