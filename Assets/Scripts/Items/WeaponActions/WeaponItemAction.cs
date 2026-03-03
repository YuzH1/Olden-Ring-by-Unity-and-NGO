using UnityEngine;

namespace SG
{
    public class WeaponItemAction : ScriptableObject
    {
        public int actionID;//动作ID

        public virtual void AttemptToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            //每种武器动作的共同点
            //1.应该一直追踪当前的武器是什么
        }
    }

}
