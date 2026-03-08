using System.Xml.Serialization;
using UnityEngine;
using Unity.Netcode;

namespace SG
{
    public class PlayerCombatManager : CharacterCombatManager
    {
        PlayerManager player;
        public WeaponItem currentWeaponBeingUsed;//当前正在使用的武器，可以是近战武器或远程武器

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
                //可以根据需要添加其他攻击类型的体力消耗计算
                default:
                    break;
            }

            // 扣除体力
            Debug.Log("Draining stamina: " + staminaDeducted);
            player.playerNetworkManager.currentStamina.Value -= Mathf.RoundToInt(staminaDeducted);
        }

        public override void SetTarget(CharacterManager newTarget)
        {
            base.SetTarget(newTarget);

            if(player.IsOwner)
            {
                PlayerCamera.instance.SetLockCameraHeight();            }
        }

    }
}
