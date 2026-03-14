using UnityEngine;
using Unity.Netcode;

namespace SG
{
    public class FogDoorInteractable : NetworkBehaviour
    {
        [Header("Fog Door")]
        [SerializeField] GameObject[] fogDoorObject; //雾门对象

        [Header("I.D")]
        public int fogDoorID = 0; //雾门ID，可以用来区分不同的雾门对象，在交互时可以根据ID来控制对应的雾门对象的状态
        
        [Header("Active")]
        public NetworkVariable<bool> isActive = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);



        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            OnIsActiveChanged(false, isActive.Value); //在网络生成时根据当前的isActive值设置雾门对象的激活状态
            isActive.OnValueChanged += OnIsActiveChanged;
            WorldObjectManager.instance.AddFogDoorToList(this); //将当前的雾门对象加入到WorldObjectManager的列表中，方便管理和访问
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            isActive.Value = false; //在网络销毁时将isActive设置为false，确保雾门对象被正确地禁用
            isActive.OnValueChanged -= OnIsActiveChanged; //在网络销毁时取消订阅isActive的值变化事件，避免内存泄漏
            WorldObjectManager.instance.RemoveFogDoorFromList(this); //将当前的雾门对象从WorldObjectManager的列表中移除，保持列表的准确性
        }

        private void OnIsActiveChanged(bool oldValue, bool newValue)
        {
            if(isActive.Value)
            {
                foreach (var obj in fogDoorObject)
                {
                    obj.SetActive(true); //根据isActive的值来设置雾门对象的激活状态
                }
                
            }
            else
            {
                foreach (var obj in fogDoorObject)
                {
                    obj.SetActive(false); //根据isActive的值来设置雾门对象的激活状态
                }
            }
        }
    }
}
