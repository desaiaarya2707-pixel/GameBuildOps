using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System;

public class Lander : MonoBehaviour
{
    private const float GRAVITY_NORMAL = 0.7f;
    public static Lander Instance {  get; private set; }

    public event EventHandler OnUpForce;
    public event EventHandler OnRightForce;
    public event EventHandler OnLeftForce;
    public event EventHandler OnBeforeForce;
    public event EventHandler OnCoinPick;
    public event EventHandler <OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }

    public event EventHandler<OnLandedEventArgs> OnLanded;
    public class OnLandedEventArgs : EventArgs{
        public LandingType landingType;
        public int score;
        public float dotvector;
        public float landingSpeed;
        public float scoreMultiplier;


    }
    public enum LandingType 
    {
        Success,
        WrongLandingArea,
        TooSteepAngle,
        TooFastLanding,

    }

    public enum State
    {
        WaitingToStart,
        Normal,
        GameOver,
    }

    private Rigidbody2D landerRigidbody2D;
    float fuel;
    float fuelmax = 10f;
    private State state;
    private void Awake()
    {
        Instance = this;

        landerRigidbody2D = GetComponent<Rigidbody2D>();
        landerRigidbody2D.gravityScale = 0f;

        fuel = fuelmax;
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case State.WaitingToStart:
                if (Keyboard.current.upArrowKey.isPressed ||
                    Keyboard.current.leftArrowKey.isPressed ||
                    Keyboard.current.rightArrowKey.isPressed)
                {
                    SetState(State.Normal);
                    landerRigidbody2D.gravityScale = GRAVITY_NORMAL;
                   
                }
                break;

            case State.Normal:

                 OnBeforeForce?.Invoke(this,EventArgs.Empty);

                 Debug.Log("Fuel : " + fuel);

                if (fuel <= 0f)
                {
                    //No fuel
                    return;
                }

                if (Keyboard.current.upArrowKey.isPressed ||
                   Keyboard.current.leftArrowKey.isPressed ||
                   Keyboard.current.rightArrowKey.isPressed)
                {
                    ConsumeFule();

                }

                if (Keyboard.current.upArrowKey.isPressed)
                {
                    float force = 700f;
                    landerRigidbody2D.AddForce(force * transform.up * Time.deltaTime);
                    OnUpForce?.Invoke(this, EventArgs.Empty);

                }
                if (Keyboard.current.leftArrowKey.isPressed)
                {
                    float turnspeed = +100f;
                    landerRigidbody2D.AddTorque(turnspeed * Time.deltaTime);
                    OnLeftForce?.Invoke(this, EventArgs.Empty);

                }
                if (Keyboard.current.rightArrowKey.isPressed)
                {
                    float tunspeed = -100f;
                    landerRigidbody2D.AddTorque(tunspeed * Time.deltaTime);
                    OnRightForce?.Invoke(this, EventArgs.Empty);

                }
                break;

            case State.GameOver:
                break;

        }

    }


    private void OnCollisionEnter2D(Collision2D collision2D)
    {
        

        if (!collision2D.gameObject.TryGetComponent(out LandingPad landingpad))
        {
            OnLanded?.Invoke(this, new OnLandedEventArgs
            {
                landingType = LandingType.WrongLandingArea,
                dotvector = 0f,
                landingSpeed = 0f,
                scoreMultiplier = 0,
                score = 0

            });
            SetState(State.GameOver);
            Debug.Log("Crashed on Terrain!");
            return;
            

        }

        float softlanding = 4f;
        float relativeVelocityMagnitude = collision2D.relativeVelocity.magnitude;
        if (relativeVelocityMagnitude > softlanding)
        {
            Debug.Log("Landed to hard!");
            OnLanded?.Invoke(this, new OnLandedEventArgs
            {
                landingType = LandingType.TooFastLanding,
                dotvector = 0f,
                landingSpeed = relativeVelocityMagnitude,
                scoreMultiplier = 0,
                score = 0

            });
            SetState(State.GameOver);
            Debug.Log(collision2D.relativeVelocity.magnitude);
            return;
        }

        float dotvector = Vector2.Dot(Vector2.up, transform.up);
        float mindotvactor = 0.90f;
        if (mindotvactor > dotvector)
        {
            Debug.Log("Angle too steap");
            OnLanded?.Invoke(this, new OnLandedEventArgs
            {
                landingType = LandingType.TooSteepAngle,
                dotvector = dotvector,
                landingSpeed = relativeVelocityMagnitude,
                scoreMultiplier = 0,
                score = 0

            });
            SetState(State.GameOver);
            Debug.Log(dotvector);
            return;
        }
        Debug.Log("Successful Landing");
        

        float landingAngleMaxScore = 100;
        float landingAngleScore = landingAngleMaxScore - Mathf.Abs(dotvector - 1f);
        Debug.Log("Landing Angle Score : " + landingAngleScore);

        float landingSpeedMaxScore = 100;
        float landingSpeedScore = softlanding - relativeVelocityMagnitude;
        Debug.Log("Landing Speed Score : " + landingSpeedScore * landingSpeedMaxScore);

        int score = Mathf.RoundToInt((landingAngleScore + landingSpeedScore) * landingpad.getscoreMultiplier());
        Debug.Log("Score : " + score);
        OnLanded?.Invoke(this, new OnLandedEventArgs
        {
            landingType = LandingType.Success,
            dotvector = dotvector,
            landingSpeed = relativeVelocityMagnitude,
            scoreMultiplier = landingpad.getscoreMultiplier(),
            score = score

        });

        SetState(State.GameOver);
    }
    private void SetState(State state)
    {
        this.state = state;
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
        {
            state = state
        });
    }

    private void ConsumeFule()
    {
        float fuleConsumptionAmout = 1f;
        fuel -= fuleConsumptionAmout * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collider2D)
    {
        if(collider2D.gameObject.TryGetComponent(out FuelPickup fuelpickup))
        {
            float addFuel = 10f;
            fuel += addFuel;
            if (fuel>fuelmax)
            {
                fuel = fuelmax;
            }
            fuelpickup.Destroyself();
        }

        if(collider2D.gameObject.TryGetComponent(out CoinPickup coinpickup))
        { 
            OnCoinPick?.Invoke(this,EventArgs.Empty);
            coinpickup.Destroyself();
        }

    }
    public float GetSpeedX()
    {
        return landerRigidbody2D.linearVelocityX;
    }

    public float GetSpeedY()
    {
        return landerRigidbody2D.linearVelocityY;
    }

    public float GetFuel()
    {
        return fuel;
    }

    public float GetNormalizedFuel()
    {
        return fuel / fuelmax;
     
    }
}
