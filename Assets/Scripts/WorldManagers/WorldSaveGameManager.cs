using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SG
{
    public class WorldSaveGameManager : MonoBehaviour
    {
        public static WorldSaveGameManager Instance;//创建单例，原因：方便其他脚本访问此类中的方法和属性

        public PlayerManager player; //玩家管理器，用于获取玩家数据

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
        public CharacterSaveData characterSlots02;
        public CharacterSaveData characterSlots03;
        public CharacterSaveData characterSlots04;
        public CharacterSaveData characterSlots05;
        public CharacterSaveData characterSlots06;
        public CharacterSaveData characterSlots07;
        public CharacterSaveData characterSlots08;
        public CharacterSaveData characterSlots09;
        public CharacterSaveData characterSlots10;


        private void Awake()
        {
            if (Instance == null) //如果实例为空，将当前对象赋值给实例，否则销毁当前对象，确保单例模式
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
            LoadAllChracterProfiles(); //当游戏开始时在设备中加载所有角色资料
        }

        private void Update()
        {
            if (saveGame)
            {
                saveGame = false; //重置保存游戏的标志
                SaveGame(); //调用保存游戏的方法
            }

            if (loadGame)
            {
                loadGame = false; //重置加载游戏的标志
                LoadGame(); //调用加载游戏的方法
            }
        }

        public string DecideCharacterFileNameBasedOnCurrentCharacterSlot(CharacterSlots characterSlot)//根据当前角色槽位决定保存文件名
        {
            string fileName = "";
            switch (characterSlot)
            {
                case CharacterSlots.characterSlot_01:
                    fileName = "characterSlot_01.json";
                    break;
                case CharacterSlots.characterSlot_02:
                    fileName = "characterSlot_02.json";
                    break;
                case CharacterSlots.characterSlot_03:
                    fileName = "characterSlot_03.json";
                    break;
                case CharacterSlots.characterSlot_04:
                    fileName = "characterSlot_04.json";
                    break;
                case CharacterSlots.characterSlot_05:
                    fileName = "characterSlot_05.json";
                    break;
                case CharacterSlots.characterSlot_06:
                    fileName = "characterSlot_06.json";
                    break;
                case CharacterSlots.characterSlot_07:
                    fileName = "characterSlot_07.json";
                    break;
                case CharacterSlots.characterSlot_08:
                    fileName = "characterSlot_08.json";
                    break;
                case CharacterSlots.characterSlot_09:
                    fileName = "characterSlot_09.json";
                    break;
                case CharacterSlots.characterSlot_10:
                    fileName = "characterSlot_10.json";
                    break;
            }

            return fileName;
        }

        public void AttemptToCreateNewGame()
        {
            saveFileDataWriter = new SaveFileDataWriter(); //创建一个新的保存文件数据写入器对象
            saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath; //设置保存数据的目录路径为应用程序的持久数据路径

            //检查是否能创建保存文件
            saveFileDataWriter.dataSaveFileName = DecideCharacterFileNameBasedOnCurrentCharacterSlot(CharacterSlots.characterSlot_01); //根据当前角色槽位决定保存文件名 //根据当前角色槽位决定保存文件名
            if (!saveFileDataWriter.CheckToSeeIfFileExists())
            {
                //如果不存在保存文件，使用这个槽位来创建一个新的游戏，
                currentCharacterSlot = CharacterSlots.characterSlot_01; //将当前角色槽位设置为角色槽位01
                currentCharacterData = new CharacterSaveData(); //创建一个新的角色数据对象，并初始化默认值
                NewGame(); //调用新游戏的方法，开始加载世界场景的协程
                return;
            }

            //检查是否能创建保存文件
            saveFileDataWriter.dataSaveFileName = DecideCharacterFileNameBasedOnCurrentCharacterSlot(CharacterSlots.characterSlot_02); //根据当前角色槽位决定保存文件名
            if (!saveFileDataWriter.CheckToSeeIfFileExists())
            {
                //如果不存在保存文件，使用这个槽位来创建一个新的游戏，
                currentCharacterSlot = CharacterSlots.characterSlot_02; //将当前角色槽位设置为角色槽位01
                currentCharacterData = new CharacterSaveData(); //创建一个新的角色数据对象，并初始化默认值
                NewGame(); //调用新游戏的方法，开始加载世界场景的协程
                return;
            }

            //检查是否能创建保存文件
            saveFileDataWriter.dataSaveFileName = DecideCharacterFileNameBasedOnCurrentCharacterSlot(CharacterSlots.characterSlot_03); //根据当前角色槽位决定保存文件名
            if (!saveFileDataWriter.CheckToSeeIfFileExists())
            {
                //如果不存在保存文件，使用这个槽位来创建一个新的游戏，
                currentCharacterSlot = CharacterSlots.characterSlot_03; //将当前角色槽位设置为角色槽位01
                currentCharacterData = new CharacterSaveData(); //创建一个新的角色数据对象，并初始化默认值
                NewGame(); //调用新游戏的方法，开始加载世界场景的协程
                return;
            }

            //检查是否能创建保存文件
            saveFileDataWriter.dataSaveFileName = DecideCharacterFileNameBasedOnCurrentCharacterSlot(CharacterSlots.characterSlot_04); //根据当前角色槽位决定保存文件名
            if (!saveFileDataWriter.CheckToSeeIfFileExists())
            {
                //如果不存在保存文件，使用这个槽位来创建一个新的游戏，
                currentCharacterSlot = CharacterSlots.characterSlot_04; //将当前角色槽位设置为角色槽位01
                currentCharacterData = new CharacterSaveData(); //创建一个新的角色数据对象，并初始化默认值
                NewGame(); //调用新游戏的方法，开始加载世界场景的协程
                return;
            }
            
            //检查是否能创建保存文件
            saveFileDataWriter.dataSaveFileName = DecideCharacterFileNameBasedOnCurrentCharacterSlot(CharacterSlots.characterSlot_05); //根据当前角色槽位决定保存文件名
            if (!saveFileDataWriter.CheckToSeeIfFileExists())
            {
                //如果不存在保存文件，使用这个槽位来创建一个新的游戏，
                currentCharacterSlot = CharacterSlots.characterSlot_05; //将当前角色槽位设置为角色槽位01
                currentCharacterData = new CharacterSaveData(); //创建一个新的角色数据对象，并初始化默认值
                NewGame(); //调用新游戏的方法，开始加载世界场景的协程
                return;
            }

            //检查是否能创建保存文件
            saveFileDataWriter.dataSaveFileName = DecideCharacterFileNameBasedOnCurrentCharacterSlot(CharacterSlots.characterSlot_06); //根据当前角色槽位决定保存文件名
            if (!saveFileDataWriter.CheckToSeeIfFileExists())
            {
                //如果不存在保存文件，使用这个槽位来创建一个新的游戏，
                currentCharacterSlot = CharacterSlots.characterSlot_06; //将当前角色槽位设置为角色槽位01
                currentCharacterData = new CharacterSaveData(); //创建一个新的角色数据对象，并初始化默认值
                NewGame(); //调用新游戏的方法，开始加载世界场景的协程
                return;
            }

            //检查是否能创建保存文件
            saveFileDataWriter.dataSaveFileName = DecideCharacterFileNameBasedOnCurrentCharacterSlot(CharacterSlots.characterSlot_07); //根据当前角色槽位决定保存文件名
            if (!saveFileDataWriter.CheckToSeeIfFileExists())
            {
                //如果不存在保存文件，使用这个槽位来创建一个新的游戏，
                currentCharacterSlot = CharacterSlots.characterSlot_07; //将当前角色槽位设置为角色槽位01
                currentCharacterData = new CharacterSaveData(); //创建一个新的角色数据对象，并初始化默认值
                NewGame(); //调用新游戏的方法，开始加载世界场景的协程
                return;
            }

            //检查是否能创建保存文件
            saveFileDataWriter.dataSaveFileName = DecideCharacterFileNameBasedOnCurrentCharacterSlot(CharacterSlots.characterSlot_08); //根据当前角色槽位决定保存文件名
            if (!saveFileDataWriter.CheckToSeeIfFileExists())
            {
                //如果不存在保存文件，使用这个槽位来创建一个新的游戏，
                currentCharacterSlot = CharacterSlots.characterSlot_08; //将当前角色槽位设置为角色槽位01
                currentCharacterData = new CharacterSaveData(); //创建一个新的角色数据对象，并初始化默认值
                NewGame(); //调用新游戏的方法，开始加载世界场景的协程
                return;
            }

            //检查是否能创建保存文件
            saveFileDataWriter.dataSaveFileName = DecideCharacterFileNameBasedOnCurrentCharacterSlot(CharacterSlots.characterSlot_09); //根据当前角色槽位决定保存文件名
            if (!saveFileDataWriter.CheckToSeeIfFileExists())
            {
                //如果不存在保存文件，使用这个槽位来创建一个新的游戏，
                currentCharacterSlot = CharacterSlots.characterSlot_09; //将当前角色槽位设置为角色槽位01
                currentCharacterData = new CharacterSaveData(); //创建一个新的角色数据对象，并初始化默认值
                NewGame(); //调用新游戏的方法，开始加载世界场景的协程
                return;
            }

            //检查是否能创建保存文件
            saveFileDataWriter.dataSaveFileName = DecideCharacterFileNameBasedOnCurrentCharacterSlot(CharacterSlots.characterSlot_10); //根据当前角色槽位决定保存文件名
            if (!saveFileDataWriter.CheckToSeeIfFileExists())
            {
                //如果不存在保存文件，使用这个槽位来创建一个新的游戏，
                currentCharacterSlot = CharacterSlots.characterSlot_10; //将当前角色槽位设置为角色槽位01
                currentCharacterData = new CharacterSaveData(); //创建一个新的角色数据对象，并初始化默认值
                NewGame(); //调用新游戏的方法，开始加载世界场景的协程
                return;
            }
            
            
            //如果没有空余的槽位，通知玩家
            TitleScreenManager.instance.DisplayNoFreeSlotsPopup(); //调用标题屏幕管理器的显示没有空余槽位弹出窗口的方法，通知玩家没有空余的槽位可以创建新游戏
            

        }

        public void NewGame()
        {
            StartCoroutine(LoadWorldScene(true)); //开始加载世界场景的协程，参数true表示新游戏需要在场景加载完成后保存
        }

        public void LoadGame()
        {
            saveFileName = DecideCharacterFileNameBasedOnCurrentCharacterSlot(currentCharacterSlot); //根据当前角色槽位来加载保存文件中的数据
            //从保存文件中加载角色数据
            saveFileDataWriter = new SaveFileDataWriter(); //创建一个新的保存文件数据写入器对象
            //设置保存数据的目录路径为应用程序的持久数据路径，
            //这是一个特殊的目录，适合存储游戏数据，因为它在不同平台上具有一致的路径，并且不会被用户轻易访问或修改。
            saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
            saveFileDataWriter.dataSaveFileName = saveFileName; //设置保存文件名，根据当前角色槽位决定
            currentCharacterData = saveFileDataWriter.LoadSaveFile(); //从保存文件中加载角色数据，并将其赋值给当前角色数据

            StartCoroutine(LoadWorldScene(false)); //开始加载世界场景的协程，false表示是加载存档而非新游戏
        }

        public void SaveGame()
        {
            saveFileName = DecideCharacterFileNameBasedOnCurrentCharacterSlot(currentCharacterSlot); //根据当前角色槽位来保存当前角色数据到保存文件中
            //将当前角色数据保存到保存文件中
            saveFileDataWriter = new SaveFileDataWriter(); //创建一个新的保存文件数据写入器对象
            saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath; //设置保存数据的目录路径为应用程序的持久数据路径
            saveFileDataWriter.dataSaveFileName = saveFileName; //设置保存文件名，根据当前角色槽位决定

            //从游戏中传递角色信息
            player.SaveGameDataToCurrentCharacterData(ref currentCharacterData);

            saveFileDataWriter.CreateNewChracterSaveFile(currentCharacterData); //将当前角色数据保存到保存文件中
        }

        public void DeleteGame(CharacterSlots characterSlot)
        {
            //根据当前角色槽位来选择删除的文件
            saveFileDataWriter = new SaveFileDataWriter(); //创建一个新的保存文件数据写入器对象
            saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
           saveFileDataWriter.dataSaveFileName = DecideCharacterFileNameBasedOnCurrentCharacterSlot(characterSlot); 

            saveFileDataWriter.DeleteSaveFile(); //删除保存文件
        }
        
        //当游戏开始时在设备中加载所有角色资料
        public void LoadAllChracterProfiles()
        {
            saveFileDataWriter = new SaveFileDataWriter(); //创建一个新的保存文件数据写入器对象
            saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath; //设置保存数据的目录路径为应用程序的持久数据路径

            saveFileDataWriter.dataSaveFileName = DecideCharacterFileNameBasedOnCurrentCharacterSlot(CharacterSlots.characterSlot_01); //根据当前角色槽位来加载保存文件中的数据
            characterSlots01 = saveFileDataWriter.LoadSaveFile(); //从保存文件中加载角色数据，并将其赋值给角色槽位01的数据

            saveFileDataWriter.dataSaveFileName = DecideCharacterFileNameBasedOnCurrentCharacterSlot(CharacterSlots.characterSlot_02); //根据当前角色槽位来加载保存文件中的数据
            characterSlots02 = saveFileDataWriter.LoadSaveFile(); //从保存文件中加载角色数据，并将其赋值给角色槽位02的数据

            saveFileDataWriter.dataSaveFileName = DecideCharacterFileNameBasedOnCurrentCharacterSlot(CharacterSlots.characterSlot_03); //根据当前角色槽位来加载保存文件中的数据
            characterSlots03 = saveFileDataWriter.LoadSaveFile(); //从保存文件中加载角色数据，并将其赋值给角色槽位03的数据

            saveFileDataWriter.dataSaveFileName = DecideCharacterFileNameBasedOnCurrentCharacterSlot(CharacterSlots.characterSlot_04); //根据当前角色槽位来加载保存文件中的数据
            characterSlots04 = saveFileDataWriter.LoadSaveFile(); //从保存文件中加载角色数据，并将其赋值给角色槽位04的数据

            saveFileDataWriter.dataSaveFileName = DecideCharacterFileNameBasedOnCurrentCharacterSlot(CharacterSlots.characterSlot_05); //根据当前角色槽位来加载保存文件中的数据
            characterSlots05 = saveFileDataWriter.LoadSaveFile(); //从保存文件中加载角色数据，并将其赋值给角色槽位05的数据

            saveFileDataWriter.dataSaveFileName = DecideCharacterFileNameBasedOnCurrentCharacterSlot(CharacterSlots.characterSlot_06); //根据当前角色槽位来加载保存文件中的数据
            characterSlots06 = saveFileDataWriter.LoadSaveFile(); //从保存文件中加载角色数据，并将其赋值给角色槽位06的数据

            saveFileDataWriter.dataSaveFileName = DecideCharacterFileNameBasedOnCurrentCharacterSlot(CharacterSlots.characterSlot_07); //根据当前角色槽位来加载保存文件中的数据
            characterSlots07 = saveFileDataWriter.LoadSaveFile(); //从保存文件中加载角色数据，并将其赋值给角色槽位07的数据

            saveFileDataWriter.dataSaveFileName = DecideCharacterFileNameBasedOnCurrentCharacterSlot(CharacterSlots.characterSlot_08); //根据当前角色槽位来加载保存文件中的数据
            characterSlots08 = saveFileDataWriter.LoadSaveFile(); //从保存文件中加载角色数据，并将其赋值给角色槽位08的数据

            saveFileDataWriter.dataSaveFileName = DecideCharacterFileNameBasedOnCurrentCharacterSlot(CharacterSlots.characterSlot_09); //根据当前角色槽位来加载保存文件中的数据
            characterSlots09 = saveFileDataWriter.LoadSaveFile(); //从保存文件中加载角色数据，并将其赋值给角色槽位09的数据

            saveFileDataWriter.dataSaveFileName = DecideCharacterFileNameBasedOnCurrentCharacterSlot(CharacterSlots.characterSlot_10); //根据当前角色槽位来加载保存文件中的数据
            characterSlots10 = saveFileDataWriter.LoadSaveFile(); //从保存文件中加载角色数据，并将其赋值给角色槽位10的数据


        }

        public IEnumerator LoadWorldScene(bool isNewGame = false)//协程方法，用于异步加载新游戏场景
        {
            //如果是新游戏，直接使用worldSceneIndex加载世界场景
            //如果是加载存档，使用存档中保存的sceneIndex
            int sceneToLoad = isNewGame ? worldSceneIndex : currentCharacterData.sceneIndex;

            //异步加载场景，避免卡顿
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneToLoad);

            //等待场景加载完成
            yield return loadOperation;

            //从当前角色数据中加载游戏数据到玩家管理器中，
            //原因：在场景加载完成后将玩家数据加载到玩家管理器中，确保玩家数据在进入世界场景时已经准备好
            player.LoadGameDataFromCurrentCharacterData(ref currentCharacterData);

            //如果是新游戏，等场景加载完成后再保存，此时sceneIndex会正确记录为世界场景索引
            if(isNewGame)
            {
                SaveGame();
            }
        }

        public int GetWorldSceneIndex()//获取世界场景索引
        {
            return worldSceneIndex;
        }
    }
}
