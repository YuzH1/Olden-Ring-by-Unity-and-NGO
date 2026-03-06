using UnityEngine;

namespace SG
{
    [CreateAssetMenu(menuName = "Character Actions/Weapon Item Action/Test Action")]
    public class WeaponItemAction : ScriptableObject
    {
        public int actionID;//动作ID

        public virtual void AttemptToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            //每种武器动作的共同点
            //1.应该一直追踪当前的武器是什么
            if(playerPerformingAction.IsOwner)
            {
                playerPerformingAction.playerNetworkManager.currentWeaponBeingUsedID.Value = weaponPerformingAction.itemID; //更新网络变量，通知所有客户端当前正在使用的武器发生了变化
            }

            Debug.Log("Attempting to perform weapon action: " + actionID + " with weapon: " + weaponPerformingAction.itemName);
        }
    }

}
