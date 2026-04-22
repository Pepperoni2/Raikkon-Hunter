using System.Threading;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpVelocity;
    [SerializeField] private GroundCheck groundCheck;

    [SerializeField] private SkeletonAnimation skeletonAnimation;

    private Rigidbody2D rigidbody2D;
    private float horizontalInput;
    private float originalXScale;
    private bool wasInAir;
    private float airTimeout;


    void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        skeletonAnimation.AnimationState.SetAnimation(0, "idle", true);
        originalXScale = transform.localScale.x;
        skeletonAnimation.AnimationState.End += FigureOutAnimationsStateAfterLanding;
    }

    private void FigureOutAnimationsStateAfterLanding(TrackEntry trackEntry)
    {
        if(trackEntry.Animation.Name != "landing") {return;}
        if(horizontalInput != 0)
        {
            skeletonAnimation.AnimationState.SetAnimation(0, "walk", true);
        }
    }
    
    private void FixedUpdate()
    {
        transform.position += Vector3.right * horizontalInput * speed * Time.deltaTime;

        if (wasInAir)
        {
            airTimeout -= Time.deltaTime;
        }

        if(groundCheck.isGrounded && wasInAir && airTimeout <= 0)
        {
            wasInAir = false;
            skeletonAnimation.AnimationState.SetAnimation(0, "landing", false);
            skeletonAnimation.AnimationState.AddAnimation(0, "idle", true, 0);
        }
    }

    public void OnMoveHorizontal(InputValue value)
    {
        horizontalInput = value.Get<float>();
        if(horizontalInput != 0)
        {
            if(horizontalInput > 0)
            {
                transform.localScale = new Vector3(originalXScale, transform.localScale.y, transform.localScale.z);
            }
            else
            {
                transform.localScale = new Vector3(-originalXScale, transform.localScale.y, transform.localScale.z);
            }

            if (!wasInAir)
            {
                skeletonAnimation.AnimationState.SetAnimation(0, "walk", true);
            }
        }
        else
        {
            if (!wasInAir)
            {
                skeletonAnimation.AnimationState.SetAnimation(0, "idle", true);
            }
        }
    }

    public void OnJump()
    {
        if(!groundCheck.isGrounded) { return; }
        rigidbody2D.linearVelocityY = jumpVelocity;
        skeletonAnimation.AnimationState.SetAnimation(0, "jump", false);
        wasInAir = true;
        airTimeout = 0.2f;
    }
}
