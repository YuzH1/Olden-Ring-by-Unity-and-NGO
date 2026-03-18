using UnityEngine;
using Unity.Netcode;
using System.Collections;
using Unity.VisualScripting;
using System.Collections.Generic;
using System;

namespace SG
{
    public class AIBossCharacterManager : AICharacterManager
    {
        // 给这个AI一个特殊的id
        public int bossID = 0; //为这个AI分配一个唯一的ID，基于它在场景中的位置或者其他属性，确保每个Boss都有一个独特的ID


        [Header("Status")]
        public NetworkVariable<bool> bossFightIsActive = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner); //使用NetworkVariable来同步这个状态，确保所有客户端都知道这个Boss是否已经被打败了
        public NetworkVariable<bool> hasBeenDefeated = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner); //使用NetworkVariable来同步这个状态，确保所有客户端都知道这个Boss是否已经被打败了
        public NetworkVariable<bool> hasBeenAwakened = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner); //使用NetworkVariable来同步这个状态，确保所有客户端都知道这个Boss是否已经被唤醒了
        [SerializeField] List<FogDoorInteractable> fogDoors; //与这个Boss相关联的雾门列表，可以在Inspector中手动分配这些雾门对象，确保它们在游戏中正确地响应Boss的状态变化
        [SerializeField] string sleepAniamation = "Sleep_01"; //Boss的睡眠动画名称，可以在Inspector中设置，确保它与Animator中的动画状态名称一致
        [SerializeField] string awakenAnimation = "Awaken_01"; //Boss的唤醒动画名称，可以在Inspector中设置，确保它与Animator中的动画状态名称一致 

        [Header("Phase Shift")]
        public float minimumHealthPrcentageToShift = 50; //Boss进行阶段转换的最低生命值百分比，可以在Inspector中设置，确保它在游戏中正确地触发阶段转换
        [SerializeField] string phaseShiftAnimation = "Phase_Change_01";//Boss的阶段转换动画名称，可以在Inspector中设置，确保它与Animator中的动画状态名称一致
        [SerializeField] AICombatStanceState phase2CombatStanceState; //Boss的第二阶段的战斗姿态状态，可以在Inspector中设置，确保它在游戏中正确地切换到这个状态


        [Header("States")]
        [SerializeField] BossSleepState bossSleepState;

        [Header("Defeat MSG")]
        [SerializeField] string defeatMessage = "LEGEND FELLED"; //击败Boss时显示的消息，可以在Inspector中设置



        // 当这个AI生成的时候，检查保存文件（字典)
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            bossFightIsActive.OnValueChanged += OnBossFightIsActiveChanged; //订阅这个事件，当bossFightIsActive的值发生变化时，调用OnBossFightIsActiveChanged方法，这样我们就可以在所有客户端上同步这个状态了
            OnBossFightIsActiveChanged(bossFightIsActive.Value, bossFightIsActive.Value); 
            //在生成时调用一次这个方法，确保客户端在中途加入游戏时，也能正确地设置这个状态，避免因为错过了这个事件而导致状态不同步的问题

            if(IsOwner)
            {
                bossSleepState = Instantiate(bossSleepState); //创建BossSleepState的实例，确保它在游戏中可以被正确地使用
                currentState = bossSleepState;
            }

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
                    hasBeenDefeated.Value = WorldSaveGameManager.Instance.currentCharacterData.bossesDefeated[bossID];
                    hasBeenAwakened.Value = WorldSaveGameManager.Instance.currentCharacterData.bossesAwakened[bossID];

                }
                //定位雾门
                // 让雾门和boss共享同一个ID，这样当boss被打败时，可以通过这个ID来控制对应的雾门的状态（比如打开或者关闭）
                StartCoroutine(GetFogDoorsFromWorldObjectManager());

                if(hasBeenAwakened.Value)
                {
                    for(int i = 0; i < fogDoors.Count; i++)
                    {
                        fogDoors[i].isActive.Value = true; //如果这个Boss已经被唤醒了，启用雾门
                    }
                }

                if(hasBeenDefeated.Value)
                {
                    for(int i = 0; i < fogDoors.Count; i++)
                    {
                        fogDoors[i].isActive.Value = false; //如果这个Boss已经被打败了，禁用雾门
                    }
                    aiCharacterNetworkManager.isActive.Value = false; //如果这个Boss已经被打败了，就将它设置为不活跃状态，这样它就不会出现在游戏中
                    Destroy(gameObject, 0.1f); //销毁这个Boss对象，确保它从游戏中消失   
                }

                
            }

            if(!hasBeenAwakened.Value)
            {
                characterAnimatorManager.PlayTargetActionAnimation(sleepAniamation, true); //如果这个Boss还没有被唤醒，播放睡眠动画，第二个参数表示是否使用根运动，第三个参数表示是否允许旋转
            }
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            bossFightIsActive.OnValueChanged -= OnBossFightIsActiveChanged; //取消订阅这个事件，确保在这个对象被销毁时，不会再调用这个方法了，避免潜在的错误和内存泄漏
        }

        private IEnumerator GetFogDoorsFromWorldObjectManager()
        {
            while(WorldObjectManager.instance.fogDoors.Count == 0)
                yield return new WaitForEndOfFrame(); //等待WorldObjectManager的雾门列表被填充
            
            fogDoors = new List<FogDoorInteractable>();

            foreach(var fogDoor in WorldObjectManager.instance.fogDoors)
            {
                if(fogDoor.fogDoorID == bossID)
                {
                    fogDoors.Add(fogDoor);
                }
            }

        }

        public override IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
        {
            // 播放死亡弹窗
            PlayerUIManager.Instance.playerUIPopUpManager.SendBossDiedPopUp(defeatMessage); //调用PlayerUIManager中的方法，显示Boss死了的弹窗，确保所有客户端都能看到这个弹窗
            

            if (IsOwner)
            {
                characterNetworkManager.currentHealth.Value = 0; //将当前生命值设置为0，确保所有客户端都知道角色已经死亡
                isDead.Value = true; //将死亡状态设置为true，触发相关的死亡逻辑

                //重置需要重置的flag
                bossFightIsActive.Value = false; //将Boss战斗状态设置为false，触发相关的逻辑

                foreach(var fogDoor in fogDoors)
                {
                    fogDoor.isActive.Value = false; //当Boss被打败时，禁用相关的雾门
                }

                

                //如果不在地面上，播放空中死亡动画
                if (!manuallySelectDeathAnimation)
                {
                    characterAnimatorManager.PlayTargetActionAnimation("Dead_01", true); //播放死亡动画，第二个参数表示是否使用根运动，第三个参数表示是否允许旋转
                }

                hasBeenDefeated.Value = true;

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
            WorldSoundFXManager.instance.PlayBossDefeatedTrack(GetComponent<AIBossDurkSoundFXManager>().bossDefeatedTrack); //调用WorldSoundFXManager中的方法，播放Boss被击败的音乐，确保所有客户端都能听到这个音乐


            yield return new WaitForSeconds(5); //等待5秒，确保死亡动画和音效播放完毕

            //死亡惩罚（掉落卢恩）
            //关闭角色控制，禁用碰撞体等，确保角色无法再进行任何操作
            Destroy(gameObject, 0.1f); //销毁这个Boss对象，确保它从游戏中消失
        }
    
        public void WakeBoss()
        {
            if(IsOwner)
            {
                if(!hasBeenAwakened.Value)
                {
                    characterAnimatorManager.PlayTargetActionAnimation(awakenAnimation, true); //如果这个Boss已经被唤醒了，播放唤醒动画，第二个参数表示是否使用根运动，第三个参数表示是否允许旋转
                }
                
                hasBeenAwakened.Value = true;
                bossFightIsActive.Value = true; //将Boss战斗状态设置为true，触发相关的逻辑
                currentState = idleState; //唤醒Boss后，将它的状态切换到idle状态，这样它就可以开始正常的AI行为了

                if(!WorldSaveGameManager.Instance.currentCharacterData.bossesAwakened.ContainsKey(bossID))
                    {
                        WorldSaveGameManager.Instance.currentCharacterData.bossesAwakened.Add(bossID, true);
                        
                    }
                    // 否则，加载已经存在的这个ID的boss的数据
                    else
                    {
                        WorldSaveGameManager.Instance.currentCharacterData.bossesAwakened.Remove(bossID);
                        WorldSaveGameManager.Instance.currentCharacterData.bossesAwakened.Add(bossID, true);
                        
                    }

                    for(int i = 0; i < fogDoors.Count; i++)
                    {
                        fogDoors[i].isActive.Value = true; //唤醒Boss时，启用雾门
                    }
            }

            
        }
    
        private void OnBossFightIsActiveChanged(bool previousValue, bool newValue)
        {
            if(bossFightIsActive.Value)
            {
                WorldSoundFXManager.instance.PlayBossTrack(GetComponent<AIBossDurkSoundFXManager>().bossIntroTrack, GetComponent<AIBossDurkSoundFXManager>().bossLoopTrack); //当Boss战斗状态变为true时，播放Boss的背景音乐，确保所有客户端都能听到这个音乐
                //在Boss HP Bar的父对象下实例化Boss HP Bar预制体，确保它在UI中正确地显示
                GameObject bossHPBar = Instantiate(PlayerUIManager.Instance.playerUIHudManager.bossHPBarPrefab, PlayerUIManager.Instance.playerUIHudManager.bossHPBarParent); 

                UI_Boss_HP_Bar bossHPBarScript = bossHPBar.GetComponentInChildren<UI_Boss_HP_Bar>();
                bossHPBarScript.EnableBossHPBar(this); //调用Boss HP Bar脚本中的方法，传入这个Boss的引用，这样Boss HP Bar就可以正确地显示这个Boss的生命值了
                
                //当Boss战斗状态变为true时，执行相关的逻辑，比如播放背景音乐、触发环境变化等
            }
            else
            {
                WorldSoundFXManager.instance.StopBossTrack(); //当Boss战斗状态变为false时，停止播放Boss的背景音乐，确保所有客户端都能听到这个变化
            }
        
        }
    
        public void PhaseShift()
        {
            characterAnimatorManager.PlayTargetActionAnimation(phaseShiftAnimation, true); //播放阶段转换动画，第二个参数表示是否使用根运动，第三个参数表示是否允许旋转
            //在阶段转换动画的事件中，切换Boss的攻击模式、调整Boss的属性等，确保阶段转换的效果能够正确地体现出来
            combatStanceState = Instantiate(phase2CombatStanceState); //切换Boss的战斗姿态状态，这样它就会使用新的攻击模式了
            currentState = combatStanceState; //将Boss的当前状态切换到战斗姿态状态，这样它就会开始使用新的攻击模式了
        }

    }
}
