using UnityEngine;
using Unity.Netcode;
using NUnit.Framework;

namespace SG
{   
    public class CharacterManager : NetworkBehaviour
    {
        public CharacterController characterController;
        CharacterNetworkManager characterNetworkManager;

        protected virtual void Awake()
        {
            DontDestroyOnLoad(this); //确保在场景切换时不销毁此对象
            characterController = GetComponent<CharacterController>();
            characterNetworkManager = GetComponent<CharacterNetworkManager>();
        }

        protected virtual void Update()
        {
            // Base character update logic can go here
            if(IsOwner)
            {
                //只有拥有该对象的客户端才更新位置
                characterNetworkManager.networkPosition.Value = transform.position;
                characterNetworkManager.networkRotation.Value = transform.rotation;
            }
            else
            {
                //客户端插值更新位置和旋转
                transform.position = Vector3.SmoothDamp//使用平滑阻尼函数来平滑位置更新，避免瞬移，这是客户端插值
                    (transform.position, 
                    characterNetworkManager.networkPosition.Value, 
                    ref characterNetworkManager.networkPositionVelocity, 
                    characterNetworkManager.networkPositionSmoothTime);

                transform.rotation = Quaternion.Slerp //使用球形插值来平滑旋转
                    (transform.rotation, 
                    characterNetworkManager.networkRotation.Value, 
                    characterNetworkManager.networkRotationSmoothTime);//旋转插值
            }

        }

        protected virtual void LateUpdate()//在Update之后调用，适合处理摄像机跟随等需要在所有对象更新后执行的逻辑
        {
            // Base character late update logic can go here
        }
    }
}
