using UnityEngine;

namespace SG
{   
    public class CharacterManager : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject); //确保在场景切换时不销毁此对象
        }
    }
}
