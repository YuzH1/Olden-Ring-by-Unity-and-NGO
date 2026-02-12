using SG;
using UnityEngine;

public class InstantCharacterEffect : ScriptableObject
{
    [Header("Effect ID")]
    public int instantEffectID; //即时效果ID

    public virtual void ProcessEffect(CharacterManager character)
    {
        
    }
}
