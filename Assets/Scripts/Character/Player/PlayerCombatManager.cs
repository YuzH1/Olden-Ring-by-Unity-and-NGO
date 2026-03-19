using System.Xml.Serialization;
using UnityEngine;
using Unity.Netcode;

namespace SG
{
    public class PlayerCombatManager : CharacterCombatManager
    {
        PlayerManager player;
        public WeaponItem currentWeaponBeingUsed;//当前正在使用的武器，可以是近战武器或远程武器

        [Header("Flags")]
        public bool canComboWithMainHandWeapon = false;//是否可以使用主手武器进行战斗，默认为false，只有当玩家装备了主手武器时才会设置为true
        //public bool canComboWithOffHandWeapon = false;//两种情况：主副手同类武器，不同类武器，不同攻击模组

        protected override void Awake()
        {
            base.Awake();

            player = GetComponent<PlayerManager>();
        }
        
        public void PerformWeaponBasedAction(WeaponItemAction weaponAction, WeaponItem weaponPerformingAction)
        {
            if (player.IsOwner)
            {
                //执行武器动作，传入动作和正在执行动作的武器数据
                weaponAction.AttemptToPerformAction(player, weaponPerformingAction); //调用武器动作的执行函数，传入玩家和正在执行动作的武器数据

                //通知服务器执行了武器动作，服务器可以根据需要进行验证和处理，例如广播给其他客户端、应用伤害等
                player.playerNetworkManager.NotifyServerWeaponActionServerRPC(NetworkManager.Singleton.LocalClientId, weaponAction.actionID, weaponPerformingAction.itemID); //调用玩家网络管理器的服务器RPC函数，传入本地客户端ID、武器动作ID和正在执行动作的武器ID
            }
        }



        public override void SetTarget(CharacterManager newTarget)
        {
            base.SetTarget(newTarget);

            if(player.IsOwner)
            {
                PlayerCamera.instance.SetLockCameraHeight();            
            }
        }


        #region 动画事件调用

        public virtual void DrainStaminaBasedOnAttack()//动画事件调用
        {
            if(!player.IsOwner)
            {
                return;
            }
            if(currentWeaponBeingUsed == null)
            {
                return;
            }

            float staminaDeducted = 0;

            switch(currentAttackType)
            {
                case AttackType.LightAttack01:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.lightAttackStaminaCostMultiplier;
                    break;
                case AttackType.LightAttack02:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.lightAttackStaminaCostMultiplier;
                    break;
                case AttackType.HeavyAttack01:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.heavyAttackStaminaCostMultiplier;
                    break;
                case AttackType.ChargeHeavyAttack01:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.chargeHeavyAttackStaminaCostMultiplier;
                    break;
                case AttackType.HeavyAttack02:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.heavyAttackStaminaCostMultiplier;
                    break;
                case AttackType.ChargeHeavyAttack02:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.chargeHeavyAttackStaminaCostMultiplier;
                    break;
                case AttackType.RunningAttack01:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.runningAttackStaminaCostMultiplier;
                    break;
                case AttackType.RollingAttack01:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.rollingAttackStaminaCostMultiplier;
                    break;
                case AttackType.BackStepAttack01:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.backStepAttackStaminaCostMultiplier;
                    break;
                case AttackType.BackStepAttack02:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.backStepAttackStaminaCostMultiplier;
                    break;
                //可以根据需要添加其他攻击类型的体力消耗计算
                default:
                    break;
            }

            // 扣除体力
            // Debug.Log("Draining stamina: " + staminaDeducted);
            player.playerNetworkManager.currentStamina.Value -= Mathf.RoundToInt(staminaDeducted);
        }

        public override void EnableCanDoCombo()//动画事件调用
        {
            if (player.playerNetworkManager.isUsingRightHand.Value)
            {
                player.playerCombatManager.canComboWithMainHandWeapon = true;
            }
            else
            {
                //启用副手武器的连击能力，前提是玩家装备了副手武器
            }

        }

        public override void DisableCanDoCombo()//动画事件调用
        {
            player.playerCombatManager.canComboWithMainHandWeapon = false;
            //禁用副手武器的连击能力
            //player.playerCombatManager.canComboWithOffHandWeapon = false;
        }

        #endregion
        

    }
}
