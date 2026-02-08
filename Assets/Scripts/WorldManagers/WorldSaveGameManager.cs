using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SG
{   
    public class WorldSaveGameManager : MonoBehaviour
    {
        public static WorldSaveGameManager Instance;//创建单例，原因：方便其他脚本访问此类中的方法和属性

        [SerializeField] PlayerManager player; //玩家管理器，用于获取玩家数据

        [Header("Save/Load")]
        [SerializeField] bool saveGame;
        [SerializeField] bool loadGame;

        [Header("World Scene Index")]
        [SerializeField] int worldSceneIndex = 1; //场景索引，用于加载世界场景

        [Header("Save File Data Writer")]
        public SaveFileDataWriter saveFileDataWriter; //保存文件数据写入器

        [Header("Current Character Data")]
        public CharacterSlots currentCharacterSlot; //当前角色槽位
        public CharacterSaveData currentCharacterData; //当前角色数据
        private string saveFileName; //保存文件名，根据当前角色槽位决定
        
        [Header("Character Slots")]
        //为什么不用数组或列表来存储角色槽位？
        //因为我们只有10个角色槽位，使用单独的变量可以更清晰地表示每个槽位的角色数据，
        // 避免了数组或列表的复杂性，同时也方便在Inspector面板中直接编辑每个槽位的数据。
        public CharacterSaveData characterSlots01;
        // public CharacterSaveData characterSlots02;
        // public CharacterSaveData characterSlots03;
        // public CharacterSaveData characterSlots04;
        // public CharacterSaveData characterSlots05;
        // public CharacterSaveData characterSlots06;
        // public CharacterSaveData characterSlots07;
        // public CharacterSaveData characterSlots08;
        // public CharacterSaveData characterSlots09;
        // public CharacterSaveData characterSlots10;


        private void Awake()
        {
            if(Instance == null) //如果实例为空，将当前对象赋值给实例，否则销毁当前对象，确保单例模式
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            

        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject); //确保在场景切换时不销毁此对象
        }

        private void Update()
        {
            if(saveGame)
            {
                saveGame = false; //重置保存游戏的标志
                SaveGame(); //调用保存游戏的方法
            }

            if(loadGame)
            {
                loadGame = false; //重置加载游戏的标志
                LoadGame(); //调用加载游戏的方法
            }
        }

        public void DecideCharacterFileNameBasedOnCurrentCharacterSlot()//根据当前角色槽位决定保存文件名
        {
            switch (currentCharacterSlot)
            {
                case CharacterSlots.characterSlot_01:
                    saveFileName = "characterSlot_01.json";
                    break;
                case CharacterSlots.characterSlot_02:
                    saveFileName = "characterSlot_02.json";
                    break;
                case CharacterSlots.characterSlot_03:
                    saveFileName = "characterSlot_03.json";
                    break;
                case CharacterSlots.characterSlot_04:
                    saveFileName = "characterSlot_04.json";
                    break;
                case CharacterSlots.characterSlot_05:
                    saveFileName = "characterSlot_05.json";
                    break;
                case CharacterSlots.characterSlot_06:
                    saveFileName = "characterSlot_06.json";
                    break;
                case CharacterSlots.characterSlot_07:
                    saveFileName = "characterSlot_07.json";
                    break;
                case CharacterSlots.characterSlot_08:
                    saveFileName = "characterSlot_08.json";
                    break;
                case CharacterSlots.characterSlot_09:
                    saveFileName = "characterSlot_09.json";
                    break;
                case CharacterSlots.characterSlot_10:
                    saveFileName = "characterSlot_10.json";
                    break;
            }
        }

        public void CreateNewGame()
        {
            DecideCharacterFileNameBasedOnCurrentCharacterSlot(); //根据当前角色槽位决定保存文件名

            //创建一个新的角色数据对象，并初始化默认值
            currentCharacterData = new CharacterSaveData();
           
        }

        public void LoadGame()
        {
            DecideCharacterFileNameBasedOnCurrentCharacterSlot(); //根据当前角色槽位来加载保存文件中的数据
            //从保存文件中加载角色数据
            saveFileDataWriter = new SaveFileDataWriter(); //创建一个新的保存文件数据写入器对象
            //设置保存数据的目录路径为应用程序的持久数据路径，
            //这是一个特殊的目录，适合存储游戏数据，因为它在不同平台上具有一致的路径，并且不会被用户轻易访问或修改。
            saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath; 
            saveFileDataWriter.dataSaveFileName = saveFileName; //设置保存文件名，根据当前角色槽位决定
            currentCharacterData = saveFileDataWriter.LoadSaveFile(); //从保存文件中加载角色数据，并将其赋值给当前角色数据

            StartCoroutine(LoadWorldScene()); //开始加载世界场景的协程
        }

        public void SaveGame()
        {
            DecideCharacterFileNameBasedOnCurrentCharacterSlot(); //根据当前角色槽位来保存当前角色数据到保存文件中
            //将当前角色数据保存到保存文件中
            saveFileDataWriter = new SaveFileDataWriter(); //创建一个新的保存文件数据写入器对象
            saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath; //设置保存数据的目录路径为应用程序的持久数据路径
            saveFileDataWriter.dataSaveFileName = saveFileName; //设置保存文件名，根据当前角色槽位决定

            //从游戏中传递角色信息
            player.SaveGameDataToCurrentCharacterData(ref currentCharacterData);

            saveFileDataWriter.CreateNewChracterSaveFile(currentCharacterData); //将当前角色数据保存到保存文件中
        }

        public IEnumerator LoadWorldScene()//协程方法，用于异步加载新游戏场景
        {
            //什么是异步加载？为什么要使用异步加载？
            //异步加载是指在加载场景时，不会阻塞主线程，
            //允许游戏继续运行，避免卡顿和冻结的情况发生。
            //使用异步加载可以提供更流畅的游戏体验，尤其是在加载大型场景时。
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(worldSceneIndex); //异步加载新游戏场景，原因：避免卡顿

            yield return null; //等待一帧，确保加载操作开始
        }

        public int GetWorldSceneIndex()//获取世界场景索引
        {
            return worldSceneIndex;
        }
    }
}
