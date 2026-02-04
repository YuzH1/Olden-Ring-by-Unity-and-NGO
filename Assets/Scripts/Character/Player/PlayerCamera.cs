using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera instance;//单例模式，确保全局只有一个PlayerCamera实例
    public Camera cameraObject;//存储摄像机组件的引用

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
    }
    private void Start()
    {
        DontDestroyOnLoad(gameObject); //确保在场景切换时不销毁此对象
    }
}
