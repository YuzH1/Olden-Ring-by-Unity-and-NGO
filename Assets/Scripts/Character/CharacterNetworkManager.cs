using UnityEngine;
using Unity.Netcode;

namespace SG
{
    public class CharacterNetworkManager : NetworkBehaviour
    {
        [Header("Position")]
        //NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner是指允许所有客户端读取，但只有拥有该对象的客户端可以写入
        public NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<Quaternion> networkRotation = new NetworkVariable<Quaternion>(Quaternion.identity, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public Vector3 networkPositionVelocity;//用于平滑位置更新的速度变量
        public float networkPositionSmoothTime = 0.1f;//移动平滑时间参数
        public float networkRotationSmoothTime = 0.1f;//旋转平滑时间参数
    }    
    
}