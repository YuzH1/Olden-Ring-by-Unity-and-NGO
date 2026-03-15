using UnityEngine;
using Unity.Netcode;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

namespace SG
{
    public class CharacterManager : NetworkBehaviour
    {
        [Header("Status")]
        //角色是否死亡的网络变量，所有客户端都可以访问和修改这个变量，
        //当角色死亡时，isDead会被设置为true，触发相关的死亡逻辑，比如播放死亡动画、禁用角色控制等
        //为什么这个变量放在这个文件而不是NetworkManager文件里？
        // 因为这个变量是角色的状态，直接放在角色管理器里更方便管理和访问，而不是放在网络管理器里，
        // 这样可以避免网络管理器过于臃肿，同时也更符合面向对象的设计原则，将角色相关的状态和逻辑封装在角色管理器中
        public NetworkVariable<bool> isDead = new NetworkVariable<bool>(false,NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [HideInInspector] public CharacterController characterController;
        [HideInInspector] public Animator animator;
        [HideInInspector] public CharacterNetworkManager characterNetworkManager;
        [HideInInspector] public CharacterEffectsManager characterEffectsManager;
        [HideInInspector] public CharacterAnimatorManager characterAnimatorManager;
        [HideInInspector] public CharacterCombatManager characterCombatManager;
        [HideInInspector] public CharacterSoundFXManager characterSoundFXManager;
        [HideInInspector] public CharacterLocomotionManager characterLocomotionManager;

        [Header("Character Group")]
        public CharacterGroup characterGroup;

        [Header("Flags")]
        public bool isPerformingAction = false;//这个标志可以用来控制角色在执行动作时不能移动或攻击等，确保动作的完整性和连贯性
        

        protected virtual void Awake()
        {
            DontDestroyOnLoad(this); //确保在场景切换时不销毁此对象
            characterController = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            characterNetworkManager = GetComponent<CharacterNetworkManager>();
            characterEffectsManager = GetComponent<CharacterEffectsManager>();
            characterAnimatorManager = GetComponent<CharacterAnimatorManager>();
            characterCombatManager = GetComponent<CharacterCombatManager>();
            characterSoundFXManager = GetComponent<CharacterSoundFXManager>();
            characterLocomotionManager = GetComponent<CharacterLocomotionManager>();
        }

        protected virtual void Start()
        {
            IgnoreMyOwnColliders(); //在Start方法中调用这个方法，确保在角色的碰撞体被启用之前就设置好忽略碰撞的关系，避免角色在游戏开始时就发生不必要的碰撞
        }

        protected virtual void Update()
        {
            animator.SetBool("isGrounded", characterLocomotionManager.isGrounded);//将是否在地面上的状态传递给动画参数，可以用于实现一些基于地面状态的动画过渡，比如从空中到地面的过渡
            // Base character update logic can go here
            if (IsOwner)
            {
                //只有拥有该对象的客户端才更新位置
                characterNetworkManager.networkPosition.Value = transform.position;
                characterNetworkManager.networkRotation.Value = transform.rotation;
            }
            else
            {
                //客户端插值更新位置和旋转
                transform.position = Vector3.SmoothDamp//使用平滑阻尼函数来平滑位置更新，避免瞬移，这是客户端插值
                    (transform.position,
                    characterNetworkManager.networkPosition.Value,
                    ref characterNetworkManager.networkPositionVelocity,
                    characterNetworkManager.networkPositionSmoothTime);

                transform.rotation = Quaternion.Slerp //使用球形插值来平滑旋转
                    (transform.rotation,
                    characterNetworkManager.networkRotation.Value,
                    characterNetworkManager.networkRotationSmoothTime);//旋转插值
            }

        }

        protected virtual void FixedUpdate()//在物理更新周期调用，适合处理物理相关的逻辑，比如角色移动、碰撞检测等
        {
            // Base character fixed update logic can go here
        }

        protected virtual void LateUpdate()//在Update之后调用，适合处理摄像机跟随等需要在所有对象更新后执行的逻辑
        {
            // Base character late update logic can go here
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            characterNetworkManager.OnIsMovingChanged(false, characterNetworkManager.isMoving.Value); //在网络对象生成时，手动调用一次OnIsMovingChanged方法，确保动画状态正确设置，即使isMoving的值没有发生变化
            characterNetworkManager.OnIsActiveChanged(false, characterNetworkManager.isActive.Value); //在网络对象生成时，手动调用一次OnIsActiveChanged方法，确保角色的激活状态正确设置，即使isActive的值没有发生变化
            
            characterNetworkManager.isMoving.OnValueChanged += characterNetworkManager.OnIsMovingChanged; //订阅isMoving网络变量的值变化事件，当isMoving的值发生变化时，调用OnIsMovingChanged方法来处理相关逻辑，比如切换动画状态等
            characterNetworkManager.isActive.OnValueChanged += characterNetworkManager.OnIsActiveChanged; //订阅isActive网络变量的值变化事件，当isActive的值发生变化时，调用OnIsActiveChanged方法来处理相关逻辑，比如启用或禁用角色等
            characterNetworkManager.currentHealth.OnValueChanged += characterNetworkManager.CheckHP; //任何角色血量变为0时都需要触发死亡
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            characterNetworkManager.isMoving.OnValueChanged -= characterNetworkManager.OnIsMovingChanged;
            characterNetworkManager.isActive.OnValueChanged -= characterNetworkManager.OnIsActiveChanged;
            characterNetworkManager.currentHealth.OnValueChanged -= characterNetworkManager.CheckHP;
        }

        public virtual IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
        {
            if (IsOwner)
            {
                characterNetworkManager.currentHealth.Value = 0; //将当前生命值设置为0，确保所有客户端都知道角色已经死亡
                isDead.Value = true; //将死亡状态设置为true，触发相关的死亡逻辑

                //重置需要重置的flag

                //如果不在地面上，播放空中死亡动画
                if (!manuallySelectDeathAnimation)
                {
                    characterAnimatorManager.PlayTargetActionAnimation("Dead_01", true); //播放死亡动画，第二个参数表示是否使用根运动，第三个参数表示是否允许旋转
                }
            }

            //播放死亡音效

            yield return new WaitForSeconds(5); //等待5秒，确保死亡动画和音效播放完毕

            //死亡惩罚（掉落卢恩）
            //关闭角色控制，禁用碰撞体等，确保角色无法再进行任何操作
        }

        public virtual void ReviveCharacter()
        {

        }

        protected virtual void IgnoreMyOwnColliders()
        {
            Collider characterControllerCollider = GetComponent<Collider>(); //获取角色控制器的碰撞体组件
            Collider[] damagableCharacterColliders = GetComponentsInChildren<Collider>();//获取角色身上所有的碰撞体组件，包括子对象上的碰撞体
            List<Collider> ignoreColliders = new List<Collider>();

            foreach (var col in damagableCharacterColliders)
            {
                ignoreColliders.Add(col); //将角色身上所有的碰撞体添加到忽略列表中
            }

            ignoreColliders.Add(characterControllerCollider); //将角色控制器的碰撞体添加到忽略列表中

            foreach (var col in ignoreColliders)
            {
                foreach (var otherCol in ignoreColliders)
                {

                    Physics.IgnoreCollision(col, otherCol, true); //让角色身上所有的碰撞体之间互相忽略碰撞，这样角色就不会和自己发生碰撞了

                }
            }
        }
    }
}
