using UnityEngine;
using UnityEngine.TextCore.Text;

namespace SG
{
    public class AIBossDurkSoundFXManager : CharacterSoundFXManager
    {
        [Header("Wooshes")]
        public AudioClip[] clubWhooshes;//棍棒攻击挥动时的音效

        [Header("Club Impacts")]
        public AudioClip[] clubImpacts;//棍棒攻击命中时的音效

        [Header("Stomp Impacts")]
        public AudioClip[] stompImpacts;//践踏攻击命中时的音效

        [Header("Roars")]
        public AudioClip[] roars;//咆哮音效

        public virtual void PlayClubImpactSFX()
        {
            if(clubImpacts.Length == 0)
                return;
            PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(clubImpacts));
        }

        public virtual void PlayStompImpactSFX()
        {
            if(stompImpacts.Length == 0)
                return;
            PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(stompImpacts));
        }

        public virtual void PlayRoarSFX()
        {
            if(roars.Length == 0)
                return;
            PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(roars));
        }

    }
}
