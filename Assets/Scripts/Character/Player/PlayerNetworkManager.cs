using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

namespace SG
{
    public class PlayerNetworkManager : CharacterNetworkManager
    {
        PlayerManager player;

        public NetworkVariable<FixedString64Bytes> characterName = new NetworkVariable<FixedString64Bytes>("Player", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner); //角色名字，默认值为"Player"，所有客户端可读，只有拥有者可写

        [Header("Equipment")]
        public NetworkVariable<int> currentWeaponBeingUsedID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner); //当前正在使用的武器ID，默认值为0表示没有武器，所有客户端可读，只有拥有者可写
        public NetworkVariable<int> currentRightHandWeaponID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner); //当前右手武器的ID，默认值为0表示没有武器，所有客户端可读，只有拥有者可写
        public NetworkVariable<int> currentLeftHandWeaponID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner); //当前左手武器的ID，默认值为0表示没有武器，所有客户端可读，只有拥有者可写
        public NetworkVariable<bool> isUsingRightHand = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner); //当前是否使用右手武器，默认值为true，所有客户端可读，只有拥有者可写
        public NetworkVariable<bool> isUsingLeftHand = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner); //当前是否使用左手武器，默认值为true，所有客户端可读，只有拥有者可写


        protected override void Awake()
        {
            base.Awake();

            player = GetComponent<PlayerManager>();
        }

        public void SetCharacterActionHand(bool rightHandAction)
        {
            if(rightHandAction)
            {
                isUsingLeftHand.Value = false; //如果选择使用右手动作，确保左手动作状态为false
                isUsingRightHand.Value = true; //设置右手动作状态为true
            }
            else
            {
                isUsingRightHand.Value = false; //如果选择使用左手动作，确保右手动作状态为false
                isUsingLeftHand.Value = true; //设置左手动作状态为true
            }
        }

        public void SetNewMaxHealthValue(int oldVitality, int newVitality)
        {
            maxHealth.Value = player.playerStatsManager.CalculateHealthBasedOnVitalityLevel(newVitality); //根据体质等级计算新的最大生命值，并更新网络变量
            PlayerUIManager.Instance.playerUIHudManager.SetMaxHealthValue(maxHealth.Value); //更新玩家UI中生命值数据条的最大值，确保它显示正确的最大生命值
            currentHealth.Value = maxHealth.Value; //当最大生命值变化时，重置当前生命值为新的最大生命值，确保玩家不会因为体质等级的提升而立即死亡

        }
    
        public void SetNewMaxStaminaValue(int oldEndurance, int newEndurance)
        {
            maxStamina.Value = player.playerStatsManager.CalculateStaminaBasedOnEnduranceLevel(newEndurance); //根据耐力等级计算新的最大耐力值，并更新网络变量
            PlayerUIManager.Instance.playerUIHudManager.SetMaxStaminaValue(maxStamina.Value); //更新玩家UI中耐力值数据条的最大值，确保它显示正确的最大耐力值
            currentStamina.Value = maxStamina.Value; //当最大耐力值变化时，重置当前耐力值为新的最大耐力值，确保玩家不会因为耐力等级的提升而立即无法使用技能
        }

        public void OnCurrentRightHandWeaponChanged(int oldWeaponID, int newWeaponID)
        {
            WeaponItem newWeapon =  Instantiate(WorldItemDatabase.instance.GetWeaponByID(newWeaponID)); //根据新的武器ID从物品数据库中获取对应的武器数据
            player.playerInventoryManager.currentRightHandWeapon = newWeapon; //更新玩家的当前右手武器数据
            player.playerEquipmentManager.LoadRightWeapon(); //加载新的右手武器模型和属性
        }
        
        public void OnCurrentLeftHandWeaponChanged(int oldWeaponID, int newWeaponID)
        {
            WeaponItem newWeapon =  Instantiate(WorldItemDatabase.instance.GetWeaponByID(newWeaponID)); //根据新的武器ID从物品数据库中获取对应的武器数据
            player.playerInventoryManager.currentLeftHandWeapon = newWeapon; //更新玩家的当前左手武器数据
            player.playerEquipmentManager.LoadLeftWeapon(); //加载新的左手武器模型和属性
        }

        public void OnCurrentWeaponBeingUsedChanged(int oldWeaponID, int newWeaponID)
        {
            WeaponItem newWeapon = Instantiate(WorldItemDatabase.instance.GetWeaponByID(newWeaponID)); //根据新的武器ID从物品数据库中获取对应的武器数据
            player.playerCombatManager.currentWeaponBeingUsed = newWeapon; //更新玩家当前正在使用的武器数据
        }
    }
}