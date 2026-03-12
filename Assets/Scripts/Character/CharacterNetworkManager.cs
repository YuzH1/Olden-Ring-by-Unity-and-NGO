using UnityEngine;
using Unity.Netcode;

namespace SG
{
    public class CharacterNetworkManager : NetworkBehaviour
    {
        CharacterManager character;

        [Header("Active")]
        public NetworkVariable<bool> isActive = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


        [Header("Position")]
        //NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner是指允许所有客户端读取，但只有拥有该对象的客户端可以写入
        public NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<Quaternion> networkRotation = new NetworkVariable<Quaternion>(Quaternion.identity, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public Vector3 networkPositionVelocity;//用于平滑位置更新的速度变量
        public float networkPositionSmoothTime = 0.1f;//移动平滑时间参数
        public float networkRotationSmoothTime = 0.1f;//旋转平滑时间参数

        [Header("Animation")]
        public NetworkVariable<bool> isMoving = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<float> horizontalMovement = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<float> verticalMovement = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<float> moveAmount = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [Header("Target")]
        public NetworkVariable<ulong> currentTargetNetworkObjectID = new NetworkVariable<ulong>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);//存储当前锁定目标的网络ID，初始值为0表示没有目标

        [Header("Flags")]
        public NetworkVariable<bool> isSprinting = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> isJumping = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> isLockedOn = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> isChargingAttack = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [Header("Stats")]
        public NetworkVariable<int> endurance = new NetworkVariable<int>(10, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> vitality = new NetworkVariable<int>(10, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        

        [Header("Resource Values")]
        public NetworkVariable<int> maxStamina = new NetworkVariable<int>(100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<float> currentStamina = new NetworkVariable<float>(100f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> maxHealth = new NetworkVariable<int>(100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> currentHealth = new NetworkVariable<int>(100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        
        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        public void CheckHP(int oldValue, int newValue)
        {
            // 使用 newValue 参数而非 currentHealth.Value，避免潜在的同步问题
            if(newValue <= 0)
            {
                StartCoroutine(character.ProcessDeathEvent());
            }

            if(character.IsOwner)
            {
                if(currentHealth.Value > maxHealth.Value)
                {
                    currentHealth.Value = maxHealth.Value; //确保当前生命值不会超过最大生命值
                }
            }
        }

        public void OnLockOnTargetIDChanged(ulong oldID, ulong newID)
        {
            if(!IsOwner)
            {
                //当锁定目标发生变化时，如果当前客户端不是拥有者，那么就根据新的网络ID来更新当前锁定的目标
                character.characterCombatManager.currentTarget = NetworkManager.Singleton.SpawnManager.SpawnedObjects[newID].GetComponent<CharacterManager>();
            }

        }

        public void OnIsLockedOnChanged(bool oldValue, bool isLockedOn)
        {
            if(!isLockedOn)
            {
                character.characterCombatManager.currentTarget = null;//如果锁定状态变为false，清除当前锁定目标
            }
        }

        public void OnIsChargingAttackChanged(bool oldStatus, bool newStatus)
        {
            //当充能攻击状态发生变化时，如果当前客户端是拥有者，那么就根据新的状态来更新角色的动画状态
            
            character.animator.SetBool("isChargingAttack", newStatus);

            // //当锁定状态改变时（无论是锁定还是取消锁定），调整摄像机高度
            // if(character.IsOwner)
            // {
            //     PlayerCamera.instance.SetLockCameraHeight();
            // }
            
        }

        public void OnIsMovingChanged(bool oldValue, bool newValue)
        {
            //当移动状态发生变化时，如果当前客户端是拥有者，那么就根据新的状态来更新角色的动画状态
            character.animator.SetBool("isMoving", isMoving.Value);
        }

        public virtual void OnIsActiveChanged(bool oldValue, bool newValue)
        {
            gameObject.SetActive(isActive.Value);
        }

        //ServerRpc是指这个方法只能由客户端调用，并且会在服务器上执行，
        // 这样可以确保只有拥有该对象的客户端才能更新网络变量，其他客户端只能读取网络变量，
        // 从而实现了基本的权限控制，防止了其他客户端恶意修改网络变量导致游戏状态不一致的问题
        [ServerRpc]
        public void NotifyActionAnimationServerRpc(ulong clientID, string animationID, bool applyRootMotion)
        {
            //如果这个角色是服务器或主机，那么就处理动画通知，可以根据clientID来确定是哪个客户端发送的通知
            if(IsServer)
            {
                //在服务器上处理动画通知，可以根据clientID来确定是哪个客户端发送的通知
                //然后可以在服务器上执行一些逻辑，比如验证动画ID是否合法，或者广播给其他客户端等
                PlayActionAnimationForClientRpc(clientID, animationID, applyRootMotion);
            }
        }

        //ClientRpc是指这个方法只能由服务器调用，并且会在所有客户端上执行，
        // 这样可以确保服务器可以通知所有客户端某个事件的发生,
        // 比如某个玩家播放了一个动画，其他客户端需要同步这个动画状态
        [ClientRpc]
        public void PlayActionAnimationForClientRpc(ulong clientID,string animationID,bool applyRootMotion)
        {
            //确认不是本地客户端再执行动画播放逻辑
            if(clientID != NetworkManager.Singleton.LocalClientId)
            {
                PerformActionAnimationFromServer(animationID, applyRootMotion);
            }
        }

        private void PerformActionAnimationFromServer(string animationID, bool applyRootMotion)
        {
            //在本地客户端执行动画播放逻辑，可以根据animationID来确定要播放哪个动画
            //然后调用角色的动画管理器来播放动画，并根据applyRootMotion参数来控制是否启用根运动
            character.characterAnimatorManager.applyRootMotion = applyRootMotion;
            character.animator.CrossFade(animationID, 0.2f);

        }
    

        #region ATTACK ANIMATION
            
        [ServerRpc]
        public void NotifyServerAttackActionAnimationServerRpc(ulong clientID, string animationID, bool applyRootMotion)
        {
            //如果这个角色是服务器或主机，那么就处理动画通知，可以根据clientID来确定是哪个客户端发送的通知
            if(IsServer)
            {
                //在服务器上处理动画通知，可以根据clientID来确定是哪个客户端发送的通知
                //然后可以在服务器上执行一些逻辑，比如验证动画ID是否合法，或者广播给其他客户端等
                PlayAttackActionAnimationForClientRpc(clientID, animationID, applyRootMotion);
            }
        }

        [ClientRpc]
        public void PlayAttackActionAnimationForClientRpc(ulong clientID,string animationID,bool applyRootMotion)
        {
            //确认不是本地客户端再执行动画播放逻辑
            if(clientID != NetworkManager.Singleton.LocalClientId)
            {
                PerformAttackActionAnimationFromServer(animationID, applyRootMotion);
            }
        }

        private void PerformAttackActionAnimationFromServer(string animationID, bool applyRootMotion)
        {
            //在本地客户端执行动画播放逻辑，可以根据animationID来确定要播放哪个动画
            //然后调用角色的动画管理器来播放动画，并根据applyRootMotion参数来控制是否启用根运动
            character.characterAnimatorManager.applyRootMotion = applyRootMotion;
            character.animator.CrossFade(animationID, 0.2f);

        }
    
        #endregion

        #region DAMAGE
            
        // [ServerRpc(RequireOwnership = false)]//过时了
        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        public void NotifyServerCharacterDamageServerRpc(
            ulong damagedCharacterID,
            ulong characterCausingDamageID,
            float physicalDamage,
            float magicDamage,
            float fireDamage,
            float lightningDamage,
            float holyDamage,
            float poiseDamage,
            float angleHitFrom,
            float contactPointX,
            float contactPointY,
            float contactPointZ)
        {
            if(IsServer)
            {
                NotifyServerCharacterDamageClientRpc(damagedCharacterID, characterCausingDamageID, physicalDamage, magicDamage, fireDamage, lightningDamage, holyDamage, poiseDamage, angleHitFrom, contactPointX, contactPointY, contactPointZ);
            }
        }

        [ClientRpc]
        public void NotifyServerCharacterDamageClientRpc(
            ulong damagedCharacterID,
            ulong characterCausingDamageID,
            float physicalDamage,
            float magicDamage,
            float fireDamage,
            float lightningDamage,
            float holyDamage,
            float poiseDamage,
            float angleHitFrom,
            float contactPointX,
            float contactPointY,
            float contactPointZ)      
        {
            ProcessCharacterDamageFromServer(damagedCharacterID, characterCausingDamageID, physicalDamage, magicDamage, fireDamage, lightningDamage, holyDamage, poiseDamage, angleHitFrom, contactPointX, contactPointY, contactPointZ);
        }

        public void ProcessCharacterDamageFromServer(
            ulong damagedCharacterID,
            ulong characterCausingDamageID,
            float physicalDamage,
            float magicDamage,
            float fireDamage,
            float lightningDamage,
            float holyDamage,
            float poiseDamage,
            float angleHitFrom,
            float contactPointX,
            float contactPointY,
            float contactPointZ)      
        {
            CharacterManager damagedCharacter = NetworkManager.Singleton.SpawnManager.SpawnedObjects[damagedCharacterID].GetComponent<CharacterManager>();
            CharacterManager characterCausingDamage = NetworkManager.Singleton.SpawnManager.SpawnedObjects[characterCausingDamageID].GetComponent<CharacterManager>();

            TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.Instance.takeDamageEffect); //创建一个伤害效果实例
            
            damageEffect.physicalDamage = physicalDamage;
            damageEffect.magicDamage = magicDamage;
            damageEffect.fireDamage = fireDamage;
            damageEffect.lightningDamage = lightningDamage;
            damageEffect.holyDamage = holyDamage;
            damageEffect.poiseDamage = poiseDamage;
            damageEffect.angleHitFrom = angleHitFrom;
            damageEffect.contactPoint = new Vector3(contactPointX, contactPointY, contactPointZ); //将伤害接触点的位置传递给伤害效果
            damageEffect.characterCausingDamage = characterCausingDamage; //将造成伤害的角色传递给伤害效果

            damagedCharacter.characterEffectsManager.ProcessInstantEffect(damageEffect); //将伤害效果应用到目标角色身上
        }


        #endregion
    
    }    
    
}