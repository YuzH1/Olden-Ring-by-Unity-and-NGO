using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using System;
using JetBrains.Annotations;
using Unity.VisualScripting;

namespace SG
{
    public class CharacterAnimatorManager : MonoBehaviour
    {
        CharacterManager character;


        int verticalParameterHash;//Animator参数的哈希值，使用哈希值可以提高性能，因为在运行时直接使用字符串会比较慢，而使用哈希值可以更快地访问Animator参数
        int horizontalParameterHash;//Animator参数的哈希值，使用哈希值可以提高性能，因为在运行时直接使用字符串会比较慢，而使用哈希值可以更快地访问Animator参数
        
        [Header("Flags")]
        public bool applyRootMotion = false;//这个标志可以用来控制角色是否应用根运动，根运动是指动画本身带有的位移和旋转

        [Header("Damage Animations")]
        public string lastDamageAnimationPlayed;//上一次播放的受击动画的名称，用于避免重复播放同一个受击动画

        [SerializeField] string hit_Forward_Medium_01 = "Hit_Forward_Medium_01";
        [SerializeField] string hit_Forward_Medium_02 = "Hit_Forward_Medium_02";

        [SerializeField] string hit_Backward_Medium_01 = "Hit_Backward_Medium_01";
        [SerializeField] string hit_Backward_Medium_02 = "Hit_Backward_Medium_02";

        [SerializeField] string hit_Left_Medium_01 = "Hit_Left_Medium_01";
        [SerializeField] string hit_Left_Medium_02 = "Hit_Left_Medium_02";

        [SerializeField] string hit_Right_Medium_01 = "Hit_Right_Medium_01";
        [SerializeField] string hit_Right_Medium_02 = "Hit_Right_Medium_02";

        public List<string> forward_Medium_Damage {get; private set;} = new List<string>();
        public List<string> backward_Medium_Damage {get; private set;} = new List<string>();
        public List<string> left_Medium_Damage {get; private set;} = new List<string>();
        public List<string> right_Medium_Damage {get; private set;} = new List<string>();

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();

            verticalParameterHash = Animator.StringToHash("Vertical");
            horizontalParameterHash = Animator.StringToHash("Horizontal");
        }

        protected virtual void Start()
        {
            forward_Medium_Damage.Add(hit_Forward_Medium_01);
            forward_Medium_Damage.Add(hit_Forward_Medium_02);

            backward_Medium_Damage.Add(hit_Backward_Medium_01);
            backward_Medium_Damage.Add(hit_Backward_Medium_02);

            left_Medium_Damage.Add(hit_Left_Medium_01);
            left_Medium_Damage.Add(hit_Left_Medium_02);

            right_Medium_Damage.Add(hit_Right_Medium_01);
            right_Medium_Damage.Add(hit_Right_Medium_02);
        }

        public string GetRandomAnimationFromList(List<string> animationList)
        {
            if(animationList.Count == 0)
            {
                Debug.LogWarning("动画列表为空，无法获取随机动画");
                return null;
            }

            List<String> finalList = new List<string>();

            foreach(string animation in animationList)
            {
                finalList.Add(animation);
            }

            //检查我们是否已经播放过这个动画，如果播放过不让他重复
            finalList.Remove(lastDamageAnimationPlayed);

            for(int i = finalList.Count - 1; i > -1; i--)
            {
                if(finalList[i] == null)
                {
                    finalList.RemoveAt(i);
                }
            }

            int randomVlaue = UnityEngine.Random.Range(0, finalList.Count);
            return finalList[randomVlaue];
        }

        public void UpdateAnimatorMovementParameters(float horizontalValue, float verticalValue)//tuto中用了issprinting为第三参数，这里不影响
        {
            // Implementation for updating animator parameters
            //方法1：直接设置Animator参数，这种方法简单直接，但可能会导致动画切换不够平滑，特别是在输入值变化较大时
            //0.1f是阻尼时间，Time.deltaTime确保每帧都平滑过渡
            float snappedHorizontal;
            float SnappedVertical;
            
            if(horizontalValue > 0 && horizontalValue <= 0.5f)
            {
                snappedHorizontal = 0.5f;//如果水平输入值在0到0.5之间，将其设为0.5，触发轻微移动动画
            }
            else if(horizontalValue > 0.5f && horizontalValue <= 1f)
            {
                snappedHorizontal = 1f;//如果水平输入值大于0.5，将其设为1，触发快速移动动画
            }
            else if(horizontalValue < 0 && horizontalValue >= -0.5f)
            {
                snappedHorizontal = -0.5f;//如果水平输入值在-0.5到0之间，将其设为-0.5，触发轻微移动动画
            }
            else if(horizontalValue < -0.5f && horizontalValue >= -1f)
            {
                snappedHorizontal = -1f;//如果水平输入值小于-0.5，将其设为-1，触发快速移动动画
            }
            else
            {
                snappedHorizontal = 0;
            }

            if(verticalValue > 0 && verticalValue <= 0.5f)
            {
                SnappedVertical = 0.5f;//如果垂直输入值在0到0.5之间，将其设为0.5，触发轻微移动动画
            }
            else if(verticalValue > 0.5f && verticalValue <= 1f)
            {
                SnappedVertical = 1f;//如果垂直输入值大于0.5，将其设为1，触发快速移动动画
            }
            else if(verticalValue < 0 && verticalValue >= -0.5f)
            {
                SnappedVertical = -0.5f;//如果垂直输入值在-0.5到0之间，将其设为-0.5，触发轻微移动动画
            }
            else if(verticalValue < -0.5f && verticalValue >= -1f)
            {
                SnappedVertical = -1f;//如果垂直输入值小于-0.5，将其设为-1，触发快速移动动画
            }
            else
            {
                SnappedVertical = 0;
            }



            if(character.characterNetworkManager.isSprinting.Value)
            {
                SnappedVertical = 2f;//如果正在冲刺，将垂直输入值设为2，触发冲刺动画
            }


            character.animator.SetFloat(horizontalParameterHash, snappedHorizontal, 0.1f, Time.deltaTime);
            character.animator.SetFloat(verticalParameterHash, SnappedVertical, 0.1f, Time.deltaTime);



        }

        public virtual void PlayTargetActionAnimation(
            string targetAnimation, //要播放的目标动画的名称
            bool isPerformingAction, //这个参数可以用来控制角色在执行动作时不能移动或攻击等，确保动作的完整性和连贯性
            bool applyRootMotion = true, //是否启用根运动，默认为true，表示动画控制角色移动；如果为false，则允许代码控制角色移动
            bool canRotate = false, //这个参数可以用来控制角色在执行动作时是否可以旋转，默认为false，表示在执行动作时不允许旋转；如果为true，则允许旋转
            bool canMove = false) //这个参数可以用来控制角色在执行动作时是否可以移动，默认为false，表示在执行动作时不允许移动；如果为true，则允许移动
        {
            // Debug.Log("正在播放动作动画: " + targetAnimation );
            this.applyRootMotion = applyRootMotion;//如果正在执行动作，启用根运动，让动画控制角色移动；否则禁用根运动，允许代码控制角色移动
            character.animator.CrossFade(targetAnimation, 0.2f);//平滑过渡到目标动画，0.2f是过渡时间，可以根据需要调整
            //可以用于停止角色尝试移动或攻击等，确保动作的完整性和连贯性
            //比如在受到伤害时，角色会播放一个受击动画，这时我们不希望角色在动画播放过程中还能移动或攻击，
            //所以可以设置isPerformingAction为true，来禁止其他动作的执行，直到动画结束后再将isPerformingAction设置为false
            character.isPerformingAction = isPerformingAction;//更新角色的动作状态标志
            character.characterLocomotionManager.canRotate = canRotate;//更新角色的旋转能力标志
            character.characterLocomotionManager.canMove = canMove;//更新角色的移动能力标志

            // 只有拥有者可以调用默认 RequireOwnership 的 ServerRpc。
            if(character.IsOwner)
            {
                //告诉server或host我们播放了动画，这样其他客户端就可以同步动画状态
                character.characterNetworkManager.NotifyActionAnimationServerRpc(
                    NetworkManager.Singleton.LocalClientId,
                    targetAnimation,
                    applyRootMotion);
            }
        }

        public virtual void PlayTargetAttackActionAnimation(AttackType attackType,
            string targetAnimation, //要播放的目标动画的名称
            bool isPerformingAction, //这个参数可以用来控制角色在执行动作时不能移动或攻击等，确保动作的完整性和连贯性
            bool applyRootMotion = true, //是否启用根运动，默认为true，表示动画控制角色移动；如果为false，则允许代码控制角色移动
            bool canRotate = false, //这个参数可以用来控制角色在执行动作时是否可以旋转，默认为false，表示在执行动作时不允许旋转；如果为true，则允许旋转
            bool canMove = false) //这个参数可以用来控制角色在执行动作时是否可以移动，默认为false，表示在执行动作时不允许移动；如果为true，则允许移动
        {
            //连击：追踪最后一个攻击是否播放
            //追踪当前的攻击类型（轻攻击、重攻击等）
            //根据武器类型更新当前武器动画
            //决定，如果我们的攻击能被格挡
            //告诉网络“Attacking”标志被激活（为了反击和格挡等）
            character.characterCombatManager.currentAttackType = attackType;//更新当前攻击类型，这样其他系统就可以根据这个信息来决定伤害、格挡等逻辑
            character.characterCombatManager.lastAttackAnimationPerformed = targetAnimation;//更新最后一次播放的攻击动画，这样我们就可以在需要的时候避免重复播放同一个攻击动画
            this.applyRootMotion = applyRootMotion;//如果正在执行动作，启用根运动，让动画控制角色移动；否则禁用根运动，允许代码控制角色移动
            character.animator.CrossFade(targetAnimation, 0.2f);//平滑过渡到目标动画，0.2f是过渡时间，可以根据需要调整
            character.isPerformingAction = isPerformingAction;//更新角色的动作状态标志
            character.characterLocomotionManager.canRotate = canRotate;//更新角色的旋转能力标志
            character.characterLocomotionManager.canMove = canMove;//更新角色的移动能力标志

            // 只有拥有者可以调用默认 RequireOwnership 的 ServerRpc。
            if(character.IsOwner)
            {
                //告诉server或host我们播放了动画，这样其他客户端就可以同步动画状态
                character.characterNetworkManager.NotifyServerAttackActionAnimationServerRpc(
                    NetworkManager.Singleton.LocalClientId,
                    targetAnimation,
                    applyRootMotion);
            }
        }

        
    }
}