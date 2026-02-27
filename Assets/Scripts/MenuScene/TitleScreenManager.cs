using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

namespace SG //命名空间：组织代码，防止命名冲突，命名：SG代表项目名称
{
    public class TitleScreenManager : MonoBehaviour
    {
        public static TitleScreenManager instance; //单例实例
        [Header("Menus")]
        [SerializeField] GameObject titleScreenMainMenu; //标题屏幕主菜单对象
        [SerializeField] GameObject titleScreenLoadMenu; //标题屏幕加载菜单对象
        [SerializeField] ScrollRect loadMenuScrollRect; //加载菜单的 ScrollRect，用于滚动到顶部

        [Header("Buttons")]
        [SerializeField] Button mainMenuNewGameButton; //新游戏按钮对象
        [SerializeField] Button loadMenuReturnButton; //返回按钮对象
        [SerializeField] Button mainMenuLoadGameButton; //主菜单加载游戏按钮对象
        [SerializeField] Button firstCharacterSlotButton; //第一个存档槽位按钮，用于自动选择
        [SerializeField] Button noFreeSlotsOkayButton; //没有空余槽位弹出窗口的确认按钮对象
        [SerializeField] Button deleteCharacterSlotConfirmButton; //删除角色槽位确认弹出窗口的确认按钮对象

        [Header("Pop Ups")]
        [SerializeField] GameObject noFreeSlotsPopup; //没有空余槽位的弹出窗口对象
        [SerializeField] GameObject deleteCharacterSlotPopUp; //删除角色槽位的确认弹出窗口对象


        [Header("Character Slots")]
        public CharacterSlots currentCharacterSlot = CharacterSlots.No_Slot; //当前角色槽位对象

        

        private void Awake()
        {
            if (instance == null)
            {
                instance = this; //设置单例实例
            }
            else
            {
                Destroy(gameObject); //如果已经存在实例，销毁当前对象，确保只有一个实例存在
            }
        }

        public void StartNetworkAsHost()
        {
            // Code to start the network as host
            NetworkManager.Singleton.StartHost();
        }

        public void StartNewGame()
        {
            WorldSaveGameManager.Instance.AttemptToCreateNewGame(); //创建一个新的游戏
            //这里为什么能调用WOrldSaveGameManager的协程方法？
            //因为WorldSaveGameManager是单例模式，可以通过Instance访问其公共方法
        }

        public void OpenLoadGameMenu()
        {
            titleScreenMainMenu.SetActive(false); //隐藏主菜单
            titleScreenLoadMenu.SetActive(true); //显示加载菜单

            //将 ScrollView 滚动到顶部，确保看到第一个存档
            if(loadMenuScrollRect != null)
            {
                loadMenuScrollRect.verticalNormalizedPosition = 1f; //1.0 = 顶部，0.0 = 底部
            }

            //自动选择第一个存档槽位
            if(firstCharacterSlotButton != null)
            {
                firstCharacterSlotButton.Select();
            }
            else
            {
                //若未设置，则选择返回按钮
                loadMenuReturnButton.Select();
            }
        }

        

        public void CloseLoadGameMenu()
        {
            titleScreenLoadMenu.SetActive(false); //隐藏加载菜单
            titleScreenMainMenu.SetActive(true); //显示主菜单

            //自动选择加载游戏按键
            mainMenuLoadGameButton.Select();
            currentCharacterSlot = CharacterSlots.No_Slot;
        }

        public void DisplayNoFreeSlotsPopup()
        {
            noFreeSlotsPopup.SetActive(true); //显示没有空余槽位的弹出窗口
            noFreeSlotsOkayButton.Select(); //自动选择确认按钮
        }

        public void CloseNoFreeSlotsPopup()
        {
            noFreeSlotsPopup.SetActive(false); //隐藏没有空余槽位的弹出窗口
            mainMenuNewGameButton.Select(); //自动选择新游戏按键
        }
    
        public void SelectCharacterSlot(CharacterSlots characterSlot)
        {
            currentCharacterSlot = characterSlot; //设置当前角色槽位为选择的槽位
        }

        public void SelectNoSlot()
        {
            currentCharacterSlot = CharacterSlots.No_Slot; //设置当前角色槽位为无槽位
        }
    
        public void AttemptToDeleteCharacterSlot()
        {
            if(currentCharacterSlot != CharacterSlots.No_Slot)
            {
                deleteCharacterSlotPopUp.SetActive(true); //显示删除角色槽位的确认弹出窗口
                deleteCharacterSlotConfirmButton.Select(); //自动选择确认按钮
            }
        }

        public void DeleteCharacterSlot()
        {
            deleteCharacterSlotPopUp.SetActive(false); //隐藏删除角色槽位的确认弹出窗口
            WorldSaveGameManager.Instance.DeleteGame(currentCharacterSlot); //调用世界保存游戏管理器的删除角色槽位方法，删除玩家选择的角色槽位数据
            
            titleScreenLoadMenu.SetActive(false); //隐藏加载菜单
            titleScreenLoadMenu.SetActive(true); //显示主菜单
            loadMenuReturnButton.Select(); //自动选择返回按钮
        }

        public void CloseDeleteCharacterSlotPopUp()
        {
            deleteCharacterSlotPopUp.SetActive(false); //隐藏删除角色槽位的确认弹出窗口
            loadMenuReturnButton.Select(); //自动选择返回按钮
        }
    }
}
