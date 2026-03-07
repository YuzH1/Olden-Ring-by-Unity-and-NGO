using UnityEngine;

namespace SG
{
    public class Utility_DestroyAfterTime : MonoBehaviour
    {
        [SerializeField] float timeToDestroy = 5f; //多少秒后销毁这个对象

        private void Awake()
        {
            Destroy(gameObject, timeToDestroy); //在指定时间后销毁这个对象
        }   
    }
}