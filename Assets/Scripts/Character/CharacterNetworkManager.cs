using UnityEngine;
using Unity.Netcode;

namespace SG
{
    public class CharacterNetworkManager : NetworkBehaviour
    {
        CharacterManager character;

        [Header("Position")]
        //NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner是指允许所有客户端读取，但只有拥有该对象的客户端可以写入
        public NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<Quaternion> networkRotation = new NetworkVariable<Quaternion>(Quaternion.identity, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public Vector3 networkPositionVelocity;//用于平滑位置更新的速度变量
        public float networkPositionSmoothTime = 0.1f;//移动平滑时间参数
        public float networkRotationSmoothTime = 0.1f;//旋转平滑时间参数

        [Header("Animation")]
        public NetworkVariable<float> horizontalMovement = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<float> verticalMovement = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<float> moveAmount = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [Header("Flags")]
        public NetworkVariable<bool> isSprinting = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        //ServerRpc是指这个方法只能由客户端调用，并且会在服务器上执行，
        // 这样可以确保只有拥有该对象的客户端才能更新网络变量，其他客户端只能读取网络变量，
        // 从而实现了基本的权限控制，防止了其他客户端恶意修改网络变量导致游戏状态不一致的问题
        [ServerRpc]
        public void NotifyActionAnimationServerRpc(
            ulong clientID, 
            string animationID, 
            bool applyRootMotion)
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
        public void PlayActionAnimationForClientRpc(
            ulong clientID,
            string animationID,
            bool applyRootMotion)
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
            character.applyRootMotion = applyRootMotion;
            character.animator.CrossFade(animationID, 0.2f);

        }
    
    }    
    
}