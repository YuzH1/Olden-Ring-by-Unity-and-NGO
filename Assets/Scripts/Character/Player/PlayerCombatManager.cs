using System.Xml.Serialization;
using UnityEngine;

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
            //执行武器动作，传入动作和正在执行动作的武器数据
            weaponAction.AttemptToPerformAction(player, weaponPerformingAction); //调用武器动作的执行函数，传入玩家和正在执行动作的武器数据

            //通知服务器执行了武器动作，服务器可以根据需要进行验证和处理，例如广播给其他客户端、应用伤害等
       }
    }
}
