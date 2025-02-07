using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

[CreateAssetMenu(fileName = "AudioClips", menuName = "Audio/AudioClipCollection", order = 1)]
public class SoundsClipsCollectionSO : ScriptableObject
{
    public AudioClip backgroundMusic;

    [Space(10)] public AudioClip RocketPowerUp;
    [Space(10)] public AudioClip FanPowerUp;
    [Space(10)] public AudioClip TrashItemDeletion;
    [Space(10)] public AudioClip ItemMergeSound;
    [Space(10)] public AudioClip AddingVehiclesToSlots;
    [Space(10)] public AudioClip Confetti;
    
    
    
    [Space(10)] public AudioClip LevelComplete;
    [Space(10)] public AudioClip LevelFail;


}
