using UnityEngine;
using UnityEngine.TextCore.Text;

namespace SG
{
    public class AIBossDurkSoundFXManager : CharacterSoundFXManager
    {
        [Header("BGM")]
        public AudioClip bossIntroTrack; //Boss战斗的引入曲目，可以在Inspector中设置，确保它在游戏中正确地播放
        public AudioClip bossLoopTrack; //Boss战斗的循环曲目
        public AudioClip bossDefeatedTrack; //Boss被击败的曲目

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
