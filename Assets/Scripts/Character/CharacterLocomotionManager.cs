using UnityEngine;

namespace SG
{
    public class CharacterLocomotionManager : MonoBehaviour
    {
        CharacterManager character;
        [Header("Ground Check & Jumping")]
        [SerializeField] protected float gravityForce = -5.5f;//重力加速度，决定了角色下落的速度和跳跃的高度
        [SerializeField] LayerMask groundLayer;//地面层，用于地面检测，确保角色只能与地面发生碰撞，而不会与其他物体发生碰撞
        [SerializeField] protected float groundCheckRadius = 1;//地面检测的半径，决定了角色与地面接触的范围
        [SerializeField] protected Vector3 yVelocity;//用于处理角色的垂直速度，特别是在跳跃和重力影响下
        [SerializeField] protected float groundedYVelocity = -20f;//一个向下的速度，确保角色在地面上时有一个稳定的接触，防止角色悬空或浮空
        [SerializeField] protected float fallStartYVelocity = -5f;//一个向下的速度阈值，当角色的垂直速度低于这个值时，认为角色开始下落
        protected bool fallingVelocityAsBeenSet = false;//一个标志，表示是否已经设置了下落速度，防止在每帧都设置下落速度，导致角色无法正常跳跃或落地
        protected float inAirTimer = 0;//一个计时器，用于记录角色在空中的时间，可以用于实现一些基于空中时间的逻辑，比如二段跳、长按跳跃等

        protected virtual void Awake()
        {
            // Base locomotion initialization can go here
            character = GetComponent<CharacterManager>();
        }

        protected virtual void Update()
        {
            // Base locomotion update logic can go here
            HandleGroundCheck();
            HandleInAirYVelocity();
            
        }

        protected void HandleGroundCheck()
        {
            character.isGrounded = Physics.CheckSphere(character.transform.position, groundCheckRadius, groundLayer);
        }

        protected void HandleInAirYVelocity()
        {
            if(character.isGrounded)
            {
                if(yVelocity.y < 0)
                {
                    inAirTimer = 0;
                    fallingVelocityAsBeenSet = false;
                    yVelocity.y = groundedYVelocity;//当角色在地面上时，设置一个稳定的向下速度，确保角色与地面保持接触
                }
            }
            else
            {
                if(!character.isJumping && fallingVelocityAsBeenSet == false)
                {
                    fallingVelocityAsBeenSet = true;//设置标志，表示已经设置了下落速度
                    yVelocity.y = fallStartYVelocity;//当角色开始下落时，设置一个向下的速度，确保角色能够正常下落
                }

                inAirTimer += Time.deltaTime;//增加空中时间计时器
                character.animator.SetFloat("inAirTimer", inAirTimer);//将空中时间传递给动画参数，可以用于实现一些基于空中时间的动画过渡，比如从跳跃到下落的过渡

                yVelocity.y += gravityForce * Time.deltaTime;//应用重力，增加垂直速度，确保角色能够正常下落

            }
            //需要一直应用垂直速度，无论角色是在地面上还是在空中，这样可以确保角色能够正常跳跃和落地
            character.characterController.Move(yVelocity * Time.deltaTime);//移动角色，应用垂直速度，确保角色能够正常下落
        }

        protected void OnDrawGizmosSelected()//在编辑器中可视化地面检测范围，帮助调试和调整地面检测的半径和位置
        {

            if (character == null)
                return;

            Gizmos.DrawSphere(character.transform.position, groundCheckRadius);
        }
    }
}