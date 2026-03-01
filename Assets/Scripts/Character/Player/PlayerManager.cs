using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace SG
{
    public class PlayerManager : CharacterManager
    {
        [Header("DEBUG MENU")]
        [SerializeField] bool respawnCharacter = false; //是否在角色死亡后自动重生
        [SerializeField] bool switchRightWeapon = false; //是否切换右手武器

        [HideInInspector]public PlayerAnimatorManager playerAnimatorManager;
        [HideInInspector]public PlayerLocomotionManager playerLocomotionManager;
        [HideInInspector] public PlayerNetworkManager playerNetworkManager;
        [HideInInspector] public PlayerStatsManager playerStatsManager;
        [HideInInspector] public PlayerInventoryManager playerInventoryManager;
        [HideInInspector] public PlayerEquipmentManager playerEquipmentManager;
        protected override void Awake()
        {
            base.Awake();
            // PlayerManager specific initialization can go here
            playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
            playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
            playerNetworkManager = GetComponent<PlayerNetworkManager>();
            playerStatsManager = GetComponent<PlayerStatsManager>();
            playerInventoryManager = GetComponent<PlayerInventoryManager>();
            playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
        }

        protected override void Update()
        {
            base.Update();
            // Player-specific update logic
            if(!IsOwner)//只有拥有该对象的客户端才处理移动
                return;
            
            //处理移动
            playerLocomotionManager.HandleAllMovement();

            //处理耐力恢复
            playerStatsManager.RegenerateStamina();
        }

        protected override void LateUpdate()
        {
            if(!IsOwner)//只有拥有该对象的客户端才处理摄像机跟随等逻辑
                return;
            base.LateUpdate();

            PlayerCamera.instance.HandleAllCameraActions(); //调用摄像机的处理函数，确保在所有对象更新后执行摄像机相关逻辑

            //debug
            DebugMenu();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            // 如果是拥有者（本地玩家），可以执行一些特定的初始化逻辑，例如设置摄像机跟随等
            if(IsOwner)
            {
                PlayerCamera.instance.player = this; //将玩家管理器的引用传递给摄像机
                PlayerInputManager.instance.player = this; //将玩家管理器的引用传递给输入管理器
                WorldSaveGameManager.Instance.player = this; //将玩家管理器的引用传递给世界保存游戏管理器，方便保存和加载玩家数据

                //更新数据值，当数据变化时，
                //语法：networkVariable.OnValueChanged += YourMethod; //当网络变量的值发生变化时，调用YourMethod方法来处理这个变化
                playerNetworkManager.vitality.OnValueChanged += playerNetworkManager.SetNewMaxHealthValue; //更新UI中的最大生命值
                playerNetworkManager.endurance.OnValueChanged += playerNetworkManager.SetNewMaxStaminaValue; //更新UI中的最大耐力值

                //更新UI中的数据条
                playerNetworkManager.currentHealth.OnValueChanged += PlayerUIManager.Instance.playerUIHudManager.SetNewHealthValue; //更新UI中的生命值
                playerNetworkManager.currentStamina.OnValueChanged += PlayerUIManager.Instance.playerUIHudManager.SetNewStaminaValue; //更新UI中的耐力值
                playerNetworkManager.currentStamina.OnValueChanged += playerStatsManager.ResetStaminaRegenerationTimer; //重置耐力恢复计时器

                
            }
            
            //数据
            playerNetworkManager.currentHealth.OnValueChanged += playerNetworkManager.CheckHP; //检查生命值是否为0，触发死亡事件

            //装备
            playerNetworkManager.currentRightHandWeaponID.OnValueChanged += playerNetworkManager.OnCurrentRightHandWeaponChanged; //当当前右手武器ID变化时，更新右手武器数据和模型
            playerNetworkManager.currentLeftHandWeaponID.OnValueChanged += playerNetworkManager.OnCurrentLeftHandWeaponChanged; //当当前左手武器ID变化时，更新左手武器数据和模型    
        
        }

        public override IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
        {
            if(IsOwner)
            {
                PlayerUIManager.Instance.playerUIPopUpManager.SendYouDiedPopUp();
            }
            return base.ProcessDeathEvent(manuallySelectDeathAnimation);

            //检查是否还有玩家存活，如果没有，触发游戏结束逻辑

        }

        public override void ReviveCharacter()
        {
            base.ReviveCharacter();

            if(IsOwner)
            {
                playerNetworkManager.currentHealth.Value = playerNetworkManager.maxHealth.Value; //将当前生命值设置为最大生命值，确保角色重生时是满血状态
                playerNetworkManager.currentStamina.Value = playerNetworkManager.maxStamina.Value; //将当前耐力值设置为最大耐力值，确保角色重生时是满耐力状态
                
                //重启焦点

                //播放重生动画
                playerAnimatorManager.PlayTargetActionAnimation("Empty", false); //播放重生动画，第二个参数表示是否使用根运动，第三个参数表示是否允许旋转
            }

        }

        //ref是什么？
        //ref是C#中的一个关键字，用于将参数以引用的方式传递给方法。这意味着在方法内部对该参数的修改会影响到调用该方法的外部变量。
        public void SaveGameDataToCurrentCharacterData(ref CharacterSaveData currentCharacterSaveData)
        {
            currentCharacterSaveData.sceneIndex = SceneManager.GetActiveScene().buildIndex; //保存当前场景索引
            currentCharacterSaveData.characterName = playerNetworkManager.characterName.Value.ToString(); //保存角色名字
            currentCharacterSaveData.secondsPlayed = Time.timeSinceLevelLoad; //保存游戏时间，单位为秒
            currentCharacterSaveData.xPos = transform.position.x; //保存角色在世界中的X
            currentCharacterSaveData.yPos = transform.position.y; //保存角色在世界中的Y
            currentCharacterSaveData.zPos = transform.position.z; //保存角色在世界中的Z

            currentCharacterSaveData.currentHealth = playerNetworkManager.currentHealth.Value; //保存当前生命值
            currentCharacterSaveData.currentStamina = playerNetworkManager.currentStamina.Value; //保存当前耐力值

            currentCharacterSaveData.vitality = playerNetworkManager.vitality.Value; //保存角色的体质等级
            currentCharacterSaveData.endurance = playerNetworkManager.endurance.Value; //保存角色的耐力等级

        }

        public void LoadGameDataFromCurrentCharacterData(ref CharacterSaveData currentCharacterSaveData)
        {
            playerNetworkManager.characterName.Value = currentCharacterSaveData.characterName; //加载角色名字
            
            Vector3 myPos = new Vector3(currentCharacterSaveData.xPos, currentCharacterSaveData.yPos, currentCharacterSaveData.zPos); //从保存数据中获取角色在世界中的坐标
            transform.position = myPos; //将角色移动到保存数据中的位置

            playerNetworkManager.vitality.Value = currentCharacterSaveData.vitality; //加载角色的体质等级
            playerNetworkManager.endurance.Value = currentCharacterSaveData.endurance; //加载角色的耐力等级

            playerNetworkManager.maxHealth.Value = playerStatsManager.CalculateHealthBasedOnVitalityLevel(playerNetworkManager.vitality.Value); //在玩家生成时中的最大生命值
            playerNetworkManager.maxStamina.Value = playerStatsManager.CalculateStaminaBasedOnEnduranceLevel(playerNetworkManager.endurance.Value); //在玩家生成时中的最大耐力值
            
            //如果存档中的生命值为0（新游戏），则设置为满血；否则从存档加载
            playerNetworkManager.currentHealth.Value = currentCharacterSaveData.currentHealth > 0 
                ? currentCharacterSaveData.currentHealth 
                : playerNetworkManager.maxHealth.Value;
            //如果存档中的耐力值为0（新游戏），则设置为满耐力；否则从存档加载
            playerNetworkManager.currentStamina.Value = currentCharacterSaveData.currentStamina > 0 
                ? currentCharacterSaveData.currentStamina 
                : playerNetworkManager.maxStamina.Value;
            
            PlayerUIManager.Instance.playerUIHudManager.SetMaxHealthValue(playerNetworkManager.maxHealth.Value); //在玩家生成时更新UI中的最大生命值
            PlayerUIManager.Instance.playerUIHudManager.SetMaxStaminaValue(playerNetworkManager.maxStamina.Value); //在玩家生成时更新UI中的最大耐力值
           
        }

        //Debug, 之后删除
        private void DebugMenu()
        {
            if(respawnCharacter)
            {
                respawnCharacter = false; //重置flag，确保只在一次按键事件中触发重生
                ReviveCharacter(); //调用重生函数，重生角色
            }
            if(switchRightWeapon)
            {
                switchRightWeapon = false; //重置flag，确保只在一次按键事件中触发切换武器
                playerEquipmentManager.SwitchRightWeapon(); //调用切换右手武器函数，切换武器
            }
        }

    }
}