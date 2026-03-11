using UnityEngine;

namespace SG
{
    [CreateAssetMenu(menuName = "A.I/Actions/Attack Action")]
    public class AICharacterAttackAction : ScriptableObject
    {
        [Header("Attack")]
        [SerializeField] private string attackAnimation;

        [Header("Combo Action")]
        public AICharacterAttackAction comboAction;//如果这个攻击动作可以连击，那么这个字段就会被赋值为下一个攻击动作，这样AI角色在执行完当前攻击动作后，就可以根据这个字段来决定是否继续执行连击动作，从而实现更复杂和多样化的攻击

        [Header("Attack Values")]
        [SerializeField] AttackType attackType;//攻击类型，可能是轻攻击，重攻击，特殊攻击等等，这个字段可以用来区分不同类型的攻击动作，并且在AI角色选择攻击动作时，可以根据这个字段来决定选择哪个攻击动作
        public int attackWeight = 50;//攻击权重，决定了在选择攻击动作时的概率，权重越高的攻击动作被选择的概率就越大
        //攻击可以重复
        public float actionRecoveryTime = 1.5f;//执行了此次攻击之后的恢复时间，这个时间可以用来控制AI角色在执行攻击动作后需要等待多久才能进行下一次行动，不影响连击的执行
        public float minimumAttackAngle = -35f;//攻击动作的最小角度，这个角度是相对于AI角色当前朝向和目标位置的角度，如果目标在这个角度范围内，那么这个攻击动作就有可能被选择
        public float maximumAttackAngle = 35f;//攻击动作的最大角度，这个角度是相对于AI角色当前朝向和目标位置的角度，如果目标在这个角度范围内，那么这个攻击动作就有可能被选择
        public float minimumAttackDistance = 0f;//攻击动作的最小距离，这个距离是AI角色与目标之间的距离，如果目标在这个距离范围内，那么这个攻击动作就有可能被选择
        public float maximumAttackDistance = 3f;//攻击动作的最大距离，这个距离是AI角色与目标之间的距离，如果目标在这个距离范围内，那么这个攻击动作就有可能被选择

        public void AttemptToPerformAttack(AICharacterManager aiCharacter)
        {
            aiCharacter.characterAnimatorManager.PlayTargetAttackActionAnimation(attackType,attackAnimation, true);

        }
    }
}
