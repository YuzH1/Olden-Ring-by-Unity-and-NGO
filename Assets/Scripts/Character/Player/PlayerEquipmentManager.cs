using UnityEngine;

namespace SG
{
    public class PlayerEquipmentManager : CharacterEquipmentManager
    {
        PlayerManager player;

        public WeaponModelInstantiationSlot rightHandSlot;
        public WeaponModelInstantiationSlot leftHandSlot;

        [SerializeField] WeaponManager rightHandWeaponManager;
        [SerializeField] WeaponManager leftHandWeaponManager;

        public GameObject rightHandWeaponModel;
        public GameObject leftHandWeaponModel;

        protected override void Awake()
        {
            base.Awake();

            player = GetComponent<PlayerManager>();
            //获得槽位信息
            InitializeWeaponSlots();

        }

        protected override void Start()
        {
            base.Start();

            LoadWeaponsOnBothHands();
        }

        private void InitializeWeaponSlots()//在Awake里获取槽位信息，避免在Start里获取槽位信息时槽位还没有被初始化
        {
            WeaponModelInstantiationSlot[] weaponSlots = GetComponentsInChildren<WeaponModelInstantiationSlot>();

            foreach(var weaponSlot in weaponSlots)
            {
                if(weaponSlot.weaponSlot == WeaponModelSlot.RightHand)
                {
                    rightHandSlot = weaponSlot;
                }
                else if(weaponSlot.weaponSlot == WeaponModelSlot.LeftHand)
                {
                    leftHandSlot = weaponSlot;
                }
            }
        }

        public void LoadWeaponsOnBothHands()
        {
            LoadRightWeapon();;
            LoadLeftWeapon();
        }

        #region 右手武器
        public void SwitchRightWeapon()
        {
            if(!player.IsOwner)
            {
                return;
            }

            player.playerAnimatorManager.PlayTargetActionAnimation("Swap_Right_Weapon_01", false, true, true, true);


            //艾尔登法环切换武器逻辑
            //1.如果除了主武器之外还有其他武器，则切换到其他武器，而不是切换回空手状态
            //2.如果只有主武器，切换到空手状态，然后跳过空槽位直接切换回主武器状态，不要在有多个空槽位时遍历所有空槽位才切换回主武器

            WeaponItem selectedWeapon = null;

            //禁用双持模式如果没有双持武器
            //检查武器索引

            //切换到下一个武器时，索引增加1
            player.playerInventoryManager.rightHandWeaponIndex += 1;

            //如果武器索引超过了武器槽位的数量，则重置为0
            if(player.playerInventoryManager.rightHandWeaponIndex < 0 || player.playerInventoryManager.rightHandWeaponIndex > 2)
            {
                player.playerInventoryManager.rightHandWeaponIndex = 0;

                //如果找到了下一个武器，或者已经检查了所有武器槽位，则切换到选中的武器（可能是空手状态）
                float weaponCount = 0;
                WeaponItem firstWeapon = null;
                int firstWeaponPosition = 0;

                for(int i = 0; i < player.playerInventoryManager.weaponsInRightHandSlots.Length; i++)
                {
                    if(player.playerInventoryManager.weaponsInRightHandSlots[i].itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
                    {
                        weaponCount += 1;//统计武器数量

                        if(firstWeapon == null)
                        {
                            firstWeapon = player.playerInventoryManager.weaponsInRightHandSlots[i];//记录第一个武器槽位的信息，以便在没有其他武器可切换时切换回这个武器
                            firstWeaponPosition = i;
                        }
                    }
                }
                if(weaponCount <= 1)
                {
                    player.playerInventoryManager.rightHandWeaponIndex = -1;//如果没有其他武器可切换了，则重置索引为-1，这样下次切换时就会切换回第一个武器槽位的武器
                    selectedWeapon = WorldItemDatabase.instance.unarmedWeapon;//切换回空手状态
                    player.playerNetworkManager.currentRightHandWeaponID.Value = selectedWeapon.itemID;//为所有已连接的客户端发送切换武器的ID
                }
                else
                {
                    player.playerInventoryManager.rightHandWeaponIndex = firstWeaponPosition;//如果没有其他武器可切换了，则重置索引为第一个武器槽位的位置，这样下次切换时就会切换回第一个武器槽位的武器
                    player.playerNetworkManager.currentRightHandWeaponID.Value = firstWeapon.itemID;//为所有已连接的客户端发送切换武器的ID
                }

                return;
            }

            //根据新的武器索引获取新的武器数据
            foreach(WeaponItem weapon in player.playerInventoryManager.weaponsInRightHandSlots)
            {
                //检查是不是空武器槽
                if(player.playerInventoryManager.weaponsInRightHandSlots[player.playerInventoryManager.rightHandWeaponIndex].itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
                {
                    //如果不是空武器槽，则切换到该武器
                    selectedWeapon = player.playerInventoryManager.weaponsInRightHandSlots[player.playerInventoryManager.rightHandWeaponIndex];
                    
                    //为所有已连接的客户端发送切换武器的ID
                    player.playerNetworkManager.currentRightHandWeaponID.Value = player.playerInventoryManager.weaponsInRightHandSlots[player.playerInventoryManager.rightHandWeaponIndex].itemID;

                    return;
                }

            }

            if(selectedWeapon == null && player.playerInventoryManager.rightHandWeaponIndex <= 2)
            {
                SwitchRightWeapon(); //如果没有找到下一个武器，并且当前索引还没有超过武器槽位的数量，则继续切换到下一个武器
            }
            


        }

        public void LoadRightWeapon()
        {
            if(player.playerInventoryManager.currentRightHandWeapon != null)
            {
                rightHandSlot.UnloadWeapon(); //在加载新武器之前先卸载当前武器，确保不会有多个武器模型同时存在于场景中
                rightHandWeaponModel = Instantiate(player.playerInventoryManager.currentRightHandWeapon.weaponModel);
                rightHandSlot.LoadWeapon(rightHandWeaponModel);
                rightHandWeaponManager = rightHandWeaponModel.GetComponent<WeaponManager>();//获取武器管理器组件
                //将武器的伤害属性传递给武器管理器组件，以便武器管理器组件可以将伤害属性传递给伤害碰撞器组件，最终实现武器造成伤害的功能
                rightHandWeaponManager.SetWeaponDamage(player, player.playerInventoryManager.currentRightHandWeapon);
                //
            }
        }

        
        #endregion
       
        #region 左手武器
        public void SwitchLeftWeapon()
        {
            if(!player.IsOwner)
            {
                return;
            }

            player.playerAnimatorManager.PlayTargetActionAnimation("Swap_Left_Weapon_01", false);
        }
        public void LoadLeftWeapon()
        {
            if(player.playerInventoryManager.currentLeftHandWeapon != null)
            {
                leftHandSlot.UnloadWeapon(); //在加载新武器之前先卸载当前武器，确保不会有多个武器模型同时存在于场景中
                leftHandWeaponModel = Instantiate(player.playerInventoryManager.currentLeftHandWeapon.weaponModel);
                leftHandSlot.LoadWeapon(leftHandWeaponModel);
                leftHandWeaponManager = leftHandWeaponModel.GetComponent<WeaponManager>();//获取武器管理器组件
                //将武器的伤害属性传递给武器管理器组件，以便武器管理器组件可以将伤害属性传递给伤害碰撞器组件，最终实现武器造成伤害的功能
                leftHandWeaponManager.SetWeaponDamage(player, player.playerInventoryManager.currentLeftHandWeapon);
            }
        }
        


        #endregion

        #region 伤害碰撞
            
        public void OpenDamageCollider()//动画事件调用
        {
            if(player.playerNetworkManager.isUsingRightHand.Value)
            {
                rightHandWeaponManager.meleeDamageCollider.EnableDamageCollider();
            }
            else if(player.playerNetworkManager.isUsingLeftHand.Value)
            {
                leftHandWeaponManager.meleeDamageCollider.EnableDamageCollider();
            }
        } 

        public void CloseDamageCollider()//动画事件调用
        {
            if(player.playerNetworkManager.isUsingRightHand.Value)
            {
                rightHandWeaponManager.meleeDamageCollider.DisableDamageCollider();
            }
            else if(player.playerNetworkManager.isUsingLeftHand.Value)
            {
                leftHandWeaponManager.meleeDamageCollider.DisableDamageCollider();
            }
        } 




        #endregion

    
    }
}

