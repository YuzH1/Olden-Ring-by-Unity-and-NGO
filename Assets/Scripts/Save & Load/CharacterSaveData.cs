using UnityEngine;

namespace SG
{
    [System.Serializable] //这个是为了让这个类可以被序列化，以便保存和加载数据
    public class CharacterSaveData
    {
        [Header("Scene Index")]
        public int sceneIndex = 1; //场景索引

        [Header("Character Info")]
        public string characterName = "player"; //角色名字

        [Header("Time Played")]
        public float secondsPlayed; //游戏时间，单位为秒

        [Header("World Coordinates")]
        //为什么不用Vector3？
        //因为Vector3是Unity特有的类型，可能在序列化时会有问题，所以我们分开存储X、Y、Z坐标
        //只保存Float，Int，String，Bool等基本类型的数据，方便序列化
        public float xPos; //角色在世界中的X坐标
        public float yPos; //角色在世界中的Y坐标
        public float zPos; //角色在世界中的Z坐标

        [Header("Resources")]
        public int currentHealth; //当前生命值
        public float currentStamina; //当前耐力值

        [Header("Character Stats")]
        public int vitality = 10; //角色的体质等级
        public int endurance = 10; //角色的耐力等级


    }
    
}
