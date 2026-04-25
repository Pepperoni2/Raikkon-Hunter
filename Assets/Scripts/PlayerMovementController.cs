using System.Threading;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpVelocity;
    [SerializeField] private float boostVelocity; 
    [SerializeField] private GroundCheck groundCheck;
    [SerializeField] private SkeletonAnimation skeletonAnimation;

    [Header("Boost Meter Settings")]
    [SerializeField] private float maxBoostAmount = 2f; // Total seconds of continuous boost
    [SerializeField] private float boostRegenRate = 1f; // How fast it regains per second
    [SerializeField] private float boostRegenDelay = 1f; // Wait time after boosting before regen starts
    [SerializeField] private Slider boostMeterUI; // Reference to your UI Slider

    private Rigidbody2D rigidbody2D;
    private float horizontalInput;
    private float originalXScale;
    private bool wasInAir;
    private float airTimeout;
    
    // Boost State Fields
    private float currentBoostAmount;
    private float regenTimer;
    private bool isBoosting;

    void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        skeletonAnimation.AnimationState.SetAnimation(0, "idle", true);
        originalXScale = transform.localScale.x;
        skeletonAnimation.AnimationState.End += FigureOutAnimationsStateAfterLanding;

        // Initialize our boost meter
        currentBoostAmount = maxBoostAmount;
        if (boostMeterUI != null)
        {
            boostMeterUI.maxValue = maxBoostAmount;
            boostMeterUI.value = currentBoostAmount;
        }
    }

    private void FigureOutAnimationsStateAfterLanding(TrackEntry trackEntry)
    {
        if(trackEntry.Animation.Name != "landing") { return; }
        if(isBoosting) { return; } 

        if(horizontalInput != 0)
        {
            skeletonAnimation.AnimationState.SetAnimation(0, "walk", true);
        }
        else 
        {
            skeletonAnimation.AnimationState.SetAnimation(0, "idle", true);
        }
    }
    
    private void FixedUpdate()
    {
        // --- Boost Meter Logic ---
        if (isBoosting)
        {
            // 1. Depletion Phase
            currentBoostAmount -= Time.deltaTime;
            regenTimer = boostRegenDelay; // Keep resetting the delay while boosting

            float facingDirection = Mathf.Sign(transform.localScale.x);
            transform.position += Vector3.right * facingDirection * boostVelocity * Time.deltaTime;

            // When the tank is empty, force the boost to stop
            if (currentBoostAmount <= 0)
            {
                currentBoostAmount = 0;
                StopBoost();
            }
        }
        else
        {
            // Standard Movement
            transform.position += Vector3.right * horizontalInput * speed * Time.deltaTime;

            // 2 & 3. Delay and Regeneration Phase
            if (regenTimer > 0)
            {
                regenTimer -= Time.deltaTime;
            }
            else if (currentBoostAmount < maxBoostAmount)
            {
                currentBoostAmount += boostRegenRate * Time.deltaTime;
                currentBoostAmount = Mathf.Clamp(currentBoostAmount, 0, maxBoostAmount);
            }
        }

        // Update the UI Slider visually
        if (boostMeterUI != null)
        {
            boostMeterUI.value = currentBoostAmount;
        }

        // --- Air & Landing Logic ---
        if (wasInAir)
        {
            airTimeout -= Time.deltaTime;
        }

        if(groundCheck.isGrounded && wasInAir && airTimeout <= 0)
        {
            wasInAir = false;
            
            if (!isBoosting)
            {
                skeletonAnimation.AnimationState.SetAnimation(0, "landing", false);
                skeletonAnimation.AnimationState.AddAnimation(0, "idle", true, 0);
            }
        }
    }

    private void StopBoost()
    {
        isBoosting = false;
        skeletonAnimation.AnimationState.SetAnimation(0, "boost_end", false);
        
        string nextAnim = "idle";
        if (wasInAir) nextAnim = "jump";
        else if (horizontalInput != 0) nextAnim = "walk";
        
        skeletonAnimation.AnimationState.AddAnimation(0, nextAnim, true, 0f);
    }

    public void OnMoveHorizontal(InputValue value)
    {
        horizontalInput = value.Get<float>();
        if(horizontalInput != 0)
        {
            float sign = Mathf.Sign(horizontalInput);
            transform.localScale = new Vector3(sign * originalXScale, transform.localScale.y, transform.localScale.z);

            if (!wasInAir && !isBoosting)
            {
                skeletonAnimation.AnimationState.SetAnimation(0, "walk", true);
            }
        }
        else
        {
            if (!wasInAir && !isBoosting)
            {
                skeletonAnimation.AnimationState.SetAnimation(0, "idle", true);
            }
        }
    }

    public void OnJump()
    {
        if(!groundCheck.isGrounded) { return; }
        
        float currentJumpVelocity = jumpVelocity;

        if (isBoosting)
        {
            currentJumpVelocity = boostVelocity; 
        }

        rigidbody2D.linearVelocityY = currentJumpVelocity;
        
        if (!isBoosting)
        {
            skeletonAnimation.AnimationState.SetAnimation(0, "jump", false);
        }
        
        wasInAir = true;
        airTimeout = 0.2f;
    }

    public void OnBoost(InputValue value)
    {
        if (value.isPressed){
            // Require at least 10% to start a new boost to prevent stuttering
            if (isBoosting || currentBoostAmount < (maxBoostAmount * 0.1f)) { return; }

            isBoosting = true;

            skeletonAnimation.AnimationState.SetAnimation(0, "boost_start", false);
            skeletonAnimation.AnimationState.AddAnimation(0, "boost_loop", true, 0f);
        }
        else{
            // Key released = STOP BOOSTING
            if (isBoosting){
                StopBoost();
            }
        }
    }
}