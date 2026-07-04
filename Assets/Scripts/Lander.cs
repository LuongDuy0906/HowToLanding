using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Lander : MonoBehaviour
{
    private const float GRAVITY_NORMAL = 0.7f;

    public static Lander Instance { get; private set; }

    public event EventHandler OnUpForce;
    public event EventHandler OnRightForce;
    public event EventHandler OnLeftForce;
    public event EventHandler OnBeforeForce;
    public event EventHandler OnCoinPickup;
    public event EventHandler<OnStateChangeEventArgs> OnStateChange;
    public event EventHandler<OnLandedEventArgs> OnLanded;

    public enum LandingType
    {
        Success,
        WrongLandingArea,
        TooSteepAngle,
        TooFastLanding
    }

    public enum State
    {
        WaitingToStart,
        Normal,
        GameOver
    }

    public class OnLandedEventArgs: EventArgs
    {
        public LandingType type;
        public int score;
        public float dotVector;
        public float landingSpeed;
        public float scoreMultiplier;
    }

    public class OnStateChangeEventArgs: EventArgs
    {
        public State state;
    }

    private Rigidbody2D landerRigibody2D;
    private float fuelAmount;
    private float fuelAmountMax = 10f;
    private State state;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        Instance = this;
        fuelAmount = fuelAmountMax;
        state = State.WaitingToStart;
        landerRigibody2D = GetComponent<Rigidbody2D>();
        landerRigibody2D.gravityScale = 0f;
    }

    private void FixedUpdate()
    {
        OnBeforeForce?.Invoke(this, EventArgs.Empty);

        switch (state)
        {
            default:
            case State.WaitingToStart:
                if (GameInput.Instance.IsUpActionPressed() || GameInput.Instance.IsLeftActionPressed() || GameInput.Instance.IsRightActionPressed())
                {
                    landerRigibody2D.gravityScale = GRAVITY_NORMAL;
                    SetState(State.Normal);
                }
                break;
            case State.Normal:
                if (fuelAmount <= 0f)
                {
                    return;
                }

                if (GameInput.Instance.IsUpActionPressed() || GameInput.Instance.IsLeftActionPressed() || GameInput.Instance.IsRightActionPressed())
                {
                    ConsumeFuel();
                }

                if (GameInput.Instance.IsUpActionPressed())
                {
                    float force = 700f;
                    landerRigibody2D.AddForce(force * transform.up * Time.deltaTime);
                    OnUpForce?.Invoke(this, EventArgs.Empty);
                }

                if (GameInput.Instance.IsLeftActionPressed())
                {
                    float turnSpeed = +100f;
                    landerRigibody2D.AddTorque(turnSpeed * Time.deltaTime);
                    OnLeftForce?.Invoke(this, EventArgs.Empty);
                }

                if (GameInput.Instance.IsRightActionPressed())
                {
                    float turnSpeed = +-100f;
                    landerRigibody2D.AddTorque(turnSpeed * Time.deltaTime);
                    OnRightForce?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.GameOver:
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(!collision.gameObject.TryGetComponent(out LandingPad landingPad))
        {
            Debug.Log("Landing pad");
            OnLanded?.Invoke(this, new OnLandedEventArgs
            {
                type = LandingType.WrongLandingArea,
                score = 0,
                dotVector = 0f,
                landingSpeed = 0,
                scoreMultiplier = 0
            });
            SetState(State.GameOver);
            return;
        }

        float softLandingVelocityMagnitube = 4f;
        float relativeVelocityMagnitube = collision.relativeVelocity.magnitude;
        if (relativeVelocityMagnitube > softLandingVelocityMagnitube)
        {
            Debug.Log("Landing to hard");
            OnLanded?.Invoke(this, new OnLandedEventArgs
            {
                type = LandingType.TooFastLanding,
                score = 0,
                dotVector = 0f,
                landingSpeed = relativeVelocityMagnitube,
                scoreMultiplier = 0
            });
            SetState(State.GameOver);
            return; 
        }

        float dotVector = Vector2.Dot(Vector2.up, transform.up);
        float minDotVector = .90f;
        if(dotVector < minDotVector)
        {
            Debug.Log("Landing to steep angle");
            OnLanded?.Invoke(this, new OnLandedEventArgs
            {
                type = LandingType.TooSteepAngle,
                score = 0,
                dotVector = dotVector,
                landingSpeed = relativeVelocityMagnitube,
                scoreMultiplier = 0
            });
            SetState(State.GameOver);
            return;
        }

        Debug.Log("Landing successful");

        float maxScoreAmountLandingAngle = 100;
        float scoreDotVectorMultiplier = 10f;
        float landingAngleScore = maxScoreAmountLandingAngle - Mathf.Abs(dotVector - 1f) * scoreDotVectorMultiplier * maxScoreAmountLandingAngle;

        float maxScoreAmountLandingSpeed = 100;
        float landingSpeedScore = (softLandingVelocityMagnitube - relativeVelocityMagnitube) * maxScoreAmountLandingSpeed;

        Debug.Log("Landing Angle Score: " + landingAngleScore);
        Debug.Log("Landing Speed Score: " + landingSpeedScore);

        int score = Mathf.RoundToInt((landingAngleScore + landingSpeedScore) * landingPad.GetScoreMultiplier());

        Debug.Log("Score: " +  score);

        OnLanded?.Invoke(this, new OnLandedEventArgs
        {
            type = LandingType.Success,
            score = score,
            dotVector = dotVector,
            landingSpeed = relativeVelocityMagnitube,
            scoreMultiplier = landingPad.GetScoreMultiplier()
        });
        SetState(State.GameOver);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.TryGetComponent(out FuelPickup fuelPickup)){
            float addFuelAmount = 10f;
            fuelAmount += addFuelAmount;

            if(fuelAmount > fuelAmountMax)
            {
                fuelAmount = fuelAmountMax;
            }

            fuelPickup.DestroySelf();
        }

        if (collision.gameObject.TryGetComponent(out CoinPickup coinPickup))
        {
            OnCoinPickup?.Invoke(this, EventArgs.Empty);
            coinPickup.DestroySelf();
        }
    }

    private void SetState(State state)
    {
        this.state = state;
        OnStateChange?.Invoke(this, new OnStateChangeEventArgs
        {
            state = state
        });
    }

    private void ConsumeFuel()
    {
        float fuelComsumptionAmount = 1f;
        fuelAmount -= fuelComsumptionAmount * Time.deltaTime;
    }

    public float GetFuelAmount()
    {
        return fuelAmount;
    }

    public float GetFuelAmountNormalized()
    {
        return fuelAmount / fuelAmountMax;
    }

    public float GetSpeedX()
    {
        return landerRigibody2D.linearVelocityX;
    }

    public float GetSpeedY()
    {
        return landerRigibody2D.linearVelocityY;
    }
}
