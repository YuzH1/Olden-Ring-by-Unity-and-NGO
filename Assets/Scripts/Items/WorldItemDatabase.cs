using System.Collections.Generic;
using UnityEngine;
using System.Linq;//使用System.Linq命名空间以便在WorldItemDatabase中使用LINQ方法来查找物品

namespace SG
{
    public class WorldItemDatabase : MonoBehaviour
    {
        public static WorldItemDatabase instance;

        public WeaponItem unarmedWeapon; //空手武器

        [Header("Weapons")]
        [SerializeField] List<WeaponItem> weapons = new List<WeaponItem>();

        [Header("Items")]
        //游戏中所有物品
        private List<Item> items = new List<Item>();

        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            
            //将所有武器添加到物品列表中，并为每个物品分配一个唯一的ID
            foreach(var weapon in weapons)
            {
                items.Add(weapon);
            }

            for(int i = 0; i < items.Count; i++)
            {
                items[i].itemID = i;
            }
        }

        public WeaponItem GetWeaponByID(int id)
        {
            //使用LINQ方法FirstOrDefault来查找ID匹配的武器，如果没有找到则返回null
            return weapons.FirstOrDefault(weapon => weapon.itemID == id);
        }
    }
}
