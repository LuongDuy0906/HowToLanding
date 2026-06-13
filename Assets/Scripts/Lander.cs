using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Lander : MonoBehaviour
{
    public static Lander Instance { get; private set; }



    public event EventHandler OnUpForce;
    public event EventHandler OnRightForce;
    public event EventHandler OnLeftForce;
    public event EventHandler OnBeforeForce;
    public event EventHandler OnCoinPickup;
    public event EventHandler<OnLandedEventArgs> OnLanded;

    public class OnLandedEventArgs: EventArgs
    {
        public int score;
    }

    private Rigidbody2D landerRigibody2D;
    private float fuelAmount;
    private float fuelAmountMax = 10f;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        Instance = this;
        fuelAmount = fuelAmountMax;
        landerRigibody2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        OnBeforeForce?.Invoke(this, EventArgs.Empty);

        if(fuelAmount <= 0f)
        {
            return;
        }

        if(Keyboard.current.upArrowKey.isPressed || Keyboard.current.leftArrowKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
        {
            ConsumeFuel();
        }

        if (Keyboard.current.upArrowKey.isPressed)
        {
            float force = 700f;
            landerRigibody2D.AddForce(force * transform.up * Time.deltaTime);
            OnUpForce?.Invoke(this, EventArgs.Empty);
        }

        if (Keyboard.current.leftArrowKey.isPressed)
        {
            float turnSpeed = +100f;
            landerRigibody2D.AddTorque(turnSpeed * Time.deltaTime);
            OnLeftForce?.Invoke(this, EventArgs.Empty);
        }

        if (Keyboard.current.rightArrowKey.isPressed)
        {
            float turnSpeed = +-100f;
            landerRigibody2D.AddTorque(turnSpeed * Time.deltaTime);
            OnRightForce?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(!collision.gameObject.TryGetComponent(out LandingPad landingPad))
        {
            Debug.Log("Landing pad");
            return;
        }

        float softLandingVelocityMagnitube = 4f;
        float relativeVelocityMagnitube = collision.relativeVelocity.magnitude;
        if (relativeVelocityMagnitube > softLandingVelocityMagnitube)
        {
            Debug.Log("Landing to hard");
            return; 
        }

        float dotVector = Vector2.Dot(Vector2.up, transform.up);
        float minDotVector = .90f;
        if(dotVector < minDotVector)
        {
            Debug.Log("Landing to steep angle");
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
            score = score
        });
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
