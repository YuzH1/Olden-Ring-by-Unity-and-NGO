using UnityEngine;

namespace SG
{
    public class TitleScreenLoadmanager : MonoBehaviour
    {
        PlayerControls playerControls; //玩家输入控制器

        [Header("Title Screen Inputs")]
        [SerializeField] bool deleteCharacterSlot = false;

        private void Update()
        {
            if(deleteCharacterSlot)
            {
                deleteCharacterSlot = false; //重置删除角色槽位的输入状态，避免重复触发删除逻辑
                TitleScreenManager.instance.AttemptToDeleteCharacterSlot(); //尝试删除角色槽位
            }
        }

        private void OnEnable()
        {
            if(playerControls == null)
            {
                playerControls = new PlayerControls(); //创建玩家控制实例
                playerControls.UI.GamePad_X.performed += i => deleteCharacterSlot = true; //绑定删除角色槽位的输入事件
                playerControls.UI.KeyBoard_Backspace.performed += i => deleteCharacterSlot = true; //绑定删除角色槽位的输入事件
            }

            playerControls.Enable();
        }

        private void OnDisable()
        {
            playerControls.Disable();
        }
    }
    
}
