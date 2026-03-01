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

        public void LoadRightWeapon()
        {
            if(player.playerInventoryManager.currentRightHandWeapon != null)
            {
                rightHandWeaponModel = Instantiate(player.playerInventoryManager.currentRightHandWeapon.weaponModel);
                rightHandSlot.LoadWeapon(rightHandWeaponModel);
                rightHandWeaponManager = rightHandWeaponModel.GetComponent<WeaponManager>();//获取武器管理器组件
                //将武器的伤害属性传递给武器管理器组件，以便武器管理器组件可以将伤害属性传递给伤害碰撞器组件，最终实现武器造成伤害的功能
                rightHandWeaponManager.SetWeaponDamage(player.playerInventoryManager.currentRightHandWeapon);
                //
            }
        }

        public void LoadLeftWeapon()
        {
            if(player.playerInventoryManager.currentLeftHandWeapon != null)
            {
                leftHandWeaponModel = Instantiate(player.playerInventoryManager.currentLeftHandWeapon.weaponModel);
                leftHandSlot.LoadWeapon(leftHandWeaponModel);
                leftHandWeaponManager = leftHandWeaponModel.GetComponent<WeaponManager>();//获取武器管理器组件
                //将武器的伤害属性传递给武器管理器组件，以便武器管理器组件可以将伤害属性传递给伤害碰撞器组件，最终实现武器造成伤害的功能
                leftHandWeaponManager.SetWeaponDamage(player.playerInventoryManager.currentLeftHandWeapon);
            }
        }
    

    
    }
}

