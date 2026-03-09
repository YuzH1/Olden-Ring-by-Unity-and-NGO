using UnityEngine;
using System.Linq;
//linq是一个强大的查询工具，可以让我们更方便地操作集合数据，比如数组、列表等。
//在这个代码中，我们使用了LINQ的FirstOrDefault方法来从weaponItemActions数组中查找一个匹配指定ID的WeaponItemAction对象。
//如果找到了，就返回这个对象；如果没有找到，就返回null。这种方式比传统的循环查找更简洁和高效。

namespace SG
{
    public class WorldActionManager : MonoBehaviour
    {
        public static WorldActionManager instance;

        

        [Header("Weapon Item Actions")]
        public WeaponItemAction[] weaponItemActions; 

        

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
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            for(int i = 0; i < weaponItemActions.Length; i++)
            {
                weaponItemActions[i].actionID = i; //为每个武器动作分配一个唯一的ID，基于它在数组中的索引
            }
        }

        public WeaponItemAction GetWeaponItemActionByID(int ID)
        {
            return weaponItemActions.FirstOrDefault(action => action.actionID == ID); //使用LINQ查询数组，返回第一个匹配ID的武器动作，如果没有找到则返回null
        }
    
    }
}