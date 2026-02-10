using UnityEngine;
using TMPro;

namespace SG
{
    public class UI_Character_Save_Slot : MonoBehaviour
    {
        SaveFileDataWriter saveFileDataWriter; //保存文件数据写入器

        [Header("Game Slot")]
        public CharacterSlots characterSlot; //角色槽位

        [Header("Character Info")]
        public TextMeshProUGUI characterNameText; //角色名称文本
        public TextMeshProUGUI timePlayed; //游戏时间文本

        private void OnEnable()
        {
            LoadSaveSlots(); //加载保存槽位数据
        }

        private void LoadSaveSlots()
        {
            saveFileDataWriter = new SaveFileDataWriter(); //创建保存文件数据写入器实例
            saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath; //设置保存数据目录路径

            // if (characterSlot == CharacterSlots.characterSlot_01)
            // {
            //     saveFileDataWriter.dataSaveFileName = WorldSaveGameManager.Instance.DecideCharacterFileNameBasedOnCurrentCharacterSlot(characterSlot); //根据当前角色槽位决定保存文件名

            //     if (saveFileDataWriter.CheckToSeeIfFileExists())
            //     {
            //         characterNameText.text = WorldSaveGameManager.Instance.characterSlots01.characterName; //从当前角色数据中获取角色名称，并显示在UI上
            //     }
            //     else
            //     {
            //         gameObject.SetActive(false); //如果保存文件不存在，隐藏该保存槽位的UI
            //     }

            // }

            switch (characterSlot)
            {
                case CharacterSlots.characterSlot_01:
                    saveFileDataWriter.dataSaveFileName = WorldSaveGameManager.Instance.DecideCharacterFileNameBasedOnCurrentCharacterSlot(characterSlot); //根据当前角色槽位决定保存文件名

                    if (saveFileDataWriter.CheckToSeeIfFileExists())
                    {
                        characterNameText.text = WorldSaveGameManager.Instance.characterSlots01.characterName; //从当前角色数据中获取角色名称，并显示在UI上
                    }
                    else
                    {
                        gameObject.SetActive(false); //如果保存文件不存在，隐藏该保存槽位的UI
                    }
                    break;
                case CharacterSlots.characterSlot_02:
                    saveFileDataWriter.dataSaveFileName = WorldSaveGameManager.Instance.DecideCharacterFileNameBasedOnCurrentCharacterSlot(characterSlot); //根据当前角色槽位决定保存文件名

                    if (saveFileDataWriter.CheckToSeeIfFileExists())
                    {
                        characterNameText.text = WorldSaveGameManager.Instance.characterSlots02.characterName; //从当前角色数据中获取角色名称，并显示在UI上
                    }
                    else
                    {
                        gameObject.SetActive(false); //如果保存文件不存在，隐藏该保存槽位的UI
                    }
                    break;
                case CharacterSlots.characterSlot_03:
                    saveFileDataWriter.dataSaveFileName = WorldSaveGameManager.Instance.DecideCharacterFileNameBasedOnCurrentCharacterSlot(characterSlot); //根据当前角色槽位决定保存文件名

                    if (saveFileDataWriter.CheckToSeeIfFileExists())
                    {
                        characterNameText.text = WorldSaveGameManager.Instance.characterSlots03.characterName; //从当前角色数据中获取角色名称，并显示在UI上
                    }
                    else
                    {
                        gameObject.SetActive(false); //如果保存文件不存在，隐藏该保存槽位的UI
                    }
                    break;
                case CharacterSlots.characterSlot_04:
                    saveFileDataWriter.dataSaveFileName = WorldSaveGameManager.Instance.DecideCharacterFileNameBasedOnCurrentCharacterSlot(characterSlot); //根据当前角色槽位决定保存文件名

                    if (saveFileDataWriter.CheckToSeeIfFileExists())
                    {
                        characterNameText.text = WorldSaveGameManager.Instance.characterSlots04.characterName; //从当前角色数据中获取角色名称，并显示在UI上
                    }
                    else
                    {
                        gameObject.SetActive(false); //如果保存文件不存在，隐藏该保存槽位的UI
                    }
                    break;
                case CharacterSlots.characterSlot_05:
                    saveFileDataWriter.dataSaveFileName = WorldSaveGameManager.Instance.DecideCharacterFileNameBasedOnCurrentCharacterSlot(characterSlot); //根据当前角色槽位决定保存文件名

                    if (saveFileDataWriter.CheckToSeeIfFileExists())
                    {
                        characterNameText.text = WorldSaveGameManager.Instance.characterSlots05.characterName; //从当前角色数据中获取角色名称，并显示在UI上
                    }
                    else
                    {
                        gameObject.SetActive(false); //如果保存文件不存在，隐藏该保存槽位的UI
                    }
                    break;
                case CharacterSlots.characterSlot_06:
                    saveFileDataWriter.dataSaveFileName = WorldSaveGameManager.Instance.DecideCharacterFileNameBasedOnCurrentCharacterSlot(characterSlot); //根据当前角色槽位决定保存文件名

                    if (saveFileDataWriter.CheckToSeeIfFileExists())
                    {
                        characterNameText.text = WorldSaveGameManager.Instance.characterSlots06.characterName; //从当前角色数据中获取角色名称，并显示在UI上
                    }
                    else
                    {
                        gameObject.SetActive(false); //如果保存文件不存在，隐藏该保存槽位的UI
                    }
                    break;
                case CharacterSlots.characterSlot_07:
                    saveFileDataWriter.dataSaveFileName = WorldSaveGameManager.Instance.DecideCharacterFileNameBasedOnCurrentCharacterSlot(characterSlot); //根据当前角色槽位决定保存文件名

                    if (saveFileDataWriter.CheckToSeeIfFileExists())
                    {
                        characterNameText.text = WorldSaveGameManager.Instance.characterSlots07.characterName; //从当前角色数据中获取角色名称，并显示在UI上
                    }
                    else
                    {
                        gameObject.SetActive(false); //如果保存文件不存在，隐藏该保存槽位的UI
                    }
                    break;
                case CharacterSlots.characterSlot_08:
                    saveFileDataWriter.dataSaveFileName = WorldSaveGameManager.Instance.DecideCharacterFileNameBasedOnCurrentCharacterSlot(characterSlot); //根据当前角色槽位决定保存文件名

                    if (saveFileDataWriter.CheckToSeeIfFileExists())
                    {
                        characterNameText.text = WorldSaveGameManager.Instance.characterSlots08.characterName; //从当前角色数据中获取角色名称，并显示在UI上
                    }
                    else
                    {
                        gameObject.SetActive(false); //如果保存文件不存在，隐藏该保存槽位的UI
                    }
                    break;
                case CharacterSlots.characterSlot_09:
                    saveFileDataWriter.dataSaveFileName = WorldSaveGameManager.Instance.DecideCharacterFileNameBasedOnCurrentCharacterSlot(characterSlot); //根据当前角色槽位决定保存文件名

                    if (saveFileDataWriter.CheckToSeeIfFileExists())
                    {
                        characterNameText.text = WorldSaveGameManager.Instance.characterSlots09.characterName; //从当前角色数据中获取角色名称，并显示在UI上
                    }
                    else
                    {
                        gameObject.SetActive(false); //如果保存文件不存在，隐藏该保存槽位的UI
                    }
                    break;
                case CharacterSlots.characterSlot_10:
                    saveFileDataWriter.dataSaveFileName = WorldSaveGameManager.Instance.DecideCharacterFileNameBasedOnCurrentCharacterSlot(characterSlot); //根据当前角色槽位决定保存文件名

                    if (saveFileDataWriter.CheckToSeeIfFileExists())
                    {
                        characterNameText.text = WorldSaveGameManager.Instance.characterSlots10.characterName; //从当前角色数据中获取角色名称，并显示在UI上
                    }
                    else
                    {
                        gameObject.SetActive(false); //如果保存文件不存在，隐藏该保存槽位的UI
                    }
                    break;
                default:
                    break;
            }
        }

        public void LoadGameFromCharacterSlot()
        {
            WorldSaveGameManager.Instance.currentCharacterSlot = characterSlot; //将当前角色槽位设置为玩家选择的槽位
            WorldSaveGameManager.Instance.LoadGame(); //调用世界保存游戏管理器的加载游戏方法，加载玩家选择的角色数据并进入游戏
        }
    
        public void SelectCurrentCharacterSlot()
        {
            TitleScreenManager.instance.SelectCharacterSlot(characterSlot); //调用标题屏幕管理器的选择角色槽位方法，将玩家选择的槽位传递给标题屏幕管理器
        }

        
    }

}
