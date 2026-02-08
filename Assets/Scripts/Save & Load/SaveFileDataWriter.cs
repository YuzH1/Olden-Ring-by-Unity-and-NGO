using UnityEngine;
using System;
using System.IO;
using System.Linq.Expressions;

namespace SG
{
    public class SaveFileDataWriter
    {
        public string saveDataDirectoryPath = "";//保存数据的目录路径
        public string dataSaveFileName = "";//保存数据的文件名

        //在创建一个新的保存文件时，必须检查该角色的保存文件是否已经存在
        public bool CheckToSeeIfFileExists()
        {
            if(File.Exists(Path.Combine(saveDataDirectoryPath, dataSaveFileName)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //删除保存文件
        public void DeleteSaveFile()
        {
            File.Delete(Path.Combine(saveDataDirectoryPath, dataSaveFileName));
        }
        
        //用于在开始新游戏时创建一个新的保存文件
        public void CreateNewChracterSaveFile(CharacterSaveData characterSaveData)
        {
            //创建一个路径来保存数据
            string savePath = Path.Combine(saveDataDirectoryPath, dataSaveFileName);

            try
            {
                //创建一个文件被写入的目录，如果目录不存在，则创建目录
                Directory.CreateDirectory(Path.GetDirectoryName(savePath));
                Debug.Log("在以下路径创建保存文件中：" + savePath);

                //序列化C#游戏数据为JSON格式
                string dataToStore = JsonUtility.ToJson(characterSaveData, true);

                //将文件写入系统IO中
                //语法解释：
                //使用FileStream创建一个新的文件流，指定路径和文件模式为Create（如果文件已存在，则覆盖）。
                //然后使用StreamWriter将数据写入文件流中。最后关闭文件流和StreamWriter。
                using (FileStream stream = new FileStream(savePath, FileMode.Create))
                {
                    using (StreamWriter fileWriter = new StreamWriter(stream))
                    {
                        fileWriter.Write(dataToStore);
                    }
                }
                Debug.Log("保存角色数据文件成功！");
            }
            catch (Exception ex)
            {
                Debug.LogError("保存角色数据文件时发生错误：" + ex.Message +"\n" + "游戏未保存！");
            }
        }
            
        //用于在加载一个已存在的游戏时，加载保存文件中的数据    
        public CharacterSaveData LoadSaveFile()
        {
            CharacterSaveData characterData = null;
            //加载保存数据的路径
            string loadPath = Path.Combine(saveDataDirectoryPath, dataSaveFileName);

            if(File.Exists(loadPath))
            {
                try
                {
                    string dataToLoad = "";

                    using (FileStream stream = new FileStream(loadPath, FileMode.Open))
                    {
                        using (StreamReader fileReader = new StreamReader(stream))
                        {
                            dataToLoad = fileReader.ReadToEnd();
                        }
                    }
                    //反序列化JSON数据为C#对象，传回Unity中使用
                    characterData = JsonUtility.FromJson<CharacterSaveData>(dataToLoad);
                    Debug.Log("加载角色数据文件成功！");
                    
                }
                catch(Exception ex)
                {
                    Debug.LogError("加载角色数据文件时发生错误：" + ex.Message + "\n" + "游戏未加载！");
                }
            }

            return characterData;
        }
    }
}