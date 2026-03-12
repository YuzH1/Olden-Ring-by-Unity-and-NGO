using UnityEngine;

namespace SG
{
    public class AIBossCharacterManager : AICharacterManager
    {
        // 给这个AI一个特殊的id
        // 当这个AI生成的时候，检查保存文件（字典)
        // 如果没有这个ID，加入保存文件，并且生成这个AI
        // 如果有这个ID，检查这个ID的状态
        // 如果这个ID的boss被打败了，禁用这个object
        // 如果这个ID的boss没有被打败，允许这个AI继续生成和存在
        
    }
}
