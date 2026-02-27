using System.Collections.Generic;
using SG;
using UnityEngine;

public class WorldCharacterEffectsManager : MonoBehaviour
{
    public static WorldCharacterEffectsManager Instance;

    [Header("Damage")]
    public TakeDamageEffect takeDamageEffect;
    [SerializeField] List<InstantCharacterEffect> instantEffectsList; //即时效果列表

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        GenerateEffectIDs(); //生成效果ID
    }

    private void GenerateEffectIDs()
    {
        for(int i = 0; i < instantEffectsList.Count; i++)
        {
            instantEffectsList[i].instantEffectID = i; //从0开始分配ID
        }
    }

}
