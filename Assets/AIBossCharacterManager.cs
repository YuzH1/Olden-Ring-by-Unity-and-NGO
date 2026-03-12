using UnityEngine;
using Unity.Netcode;
using System.Collections;

namespace SG
{
    public class AIBossCharacterManager : AICharacterManager
    {
        // 给这个AI一个特殊的id
        public int bossID; //为这个AI分配一个唯一的ID，基于它在场景中的位置或者其他属性，确保每个Boss都有一个独特的ID
        [SerializeField] bool hasBeenDefeated = false;


        


        // 当这个AI生成的时候，检查保存文件（字典)
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if(IsServer)
            {
                //如果保存文件中没有这个ID，加入这个ID，并且设置这个ID的状态为未唤醒和未打败
                if(!WorldSaveGameManager.Instance.currentCharacterData.bossesAwakened.ContainsKey(bossID))
                {
                    WorldSaveGameManager.Instance.currentCharacterData.bossesAwakened.Add(bossID, false);
                    WorldSaveGameManager.Instance.currentCharacterData.bossesDefeated.Add(bossID, false);
                }
                // 否则，加载已经存在的这个ID的boss的数据
                else
                {
                    hasBeenDefeated = WorldSaveGameManager.Instance.currentCharacterData.bossesDefeated[bossID];

                    if(hasBeenDefeated)
                    {
                        aiCharacterNetworkManager.isActive.Value = false; //如果这个Boss已经被打败了，就将它设置为不活跃状态，这样它就不会出现在游戏中   
                    }
                }
            }
        }

        
        public override IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
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

                hasBeenDefeated = true;

                if(!WorldSaveGameManager.Instance.currentCharacterData.bossesAwakened.ContainsKey(bossID))
                {
                    WorldSaveGameManager.Instance.currentCharacterData.bossesAwakened.Add(bossID, true);
                    WorldSaveGameManager.Instance.currentCharacterData.bossesDefeated.Add(bossID, true);
                }
                // 否则，加载已经存在的这个ID的boss的数据
                else
                {
                    WorldSaveGameManager.Instance.currentCharacterData.bossesAwakened.Remove(bossID);
                    WorldSaveGameManager.Instance.currentCharacterData.bossesDefeated.Remove(bossID);
                    WorldSaveGameManager.Instance.currentCharacterData.bossesAwakened.Add(bossID, true);
                    WorldSaveGameManager.Instance.currentCharacterData.bossesDefeated.Add(bossID, true);
                }

                WorldSaveGameManager.Instance.SaveGame(); //保存游戏，确保这个Boss的状态被记录下来
            }


            //播放死亡音效

            yield return new WaitForSeconds(5); //等待5秒，确保死亡动画和音效播放完毕

            //死亡惩罚（掉落卢恩）
            //关闭角色控制，禁用碰撞体等，确保角色无法再进行任何操作
        }
    }
}
