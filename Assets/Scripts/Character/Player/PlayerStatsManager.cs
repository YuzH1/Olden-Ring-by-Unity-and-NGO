using UnityEngine;

namespace SG
{
    public class PlayerStatsManager : CharacterStatsManager
    {
        PlayerManager player;
        protected override void Awake()
        {
            base.Awake();

            player = GetComponent<PlayerManager>();
        }

        protected override void Start()
        {
            base.Start();

            //为什么在这里计算？
            //当创建一个角色时，Start函数会被调用，此时玩家的网络变量vitality和endurance已经被初始化为默认值（例如10），所以在这里计算一次生命值和耐力值，可以确保玩家在生成时就有正确的初始生命值和耐力值，并且UI中的数据条也会显示正确的数值。
            CalculateHealthBasedOnVitalityLevel(player.playerNetworkManager.vitality.Value); //根据体质等级计算生命值
            CalculateStaminaBasedOnEnduranceLevel(player.playerNetworkManager.endurance.Value); //根据耐力等级计算耐力值

        }
    }
    
}
