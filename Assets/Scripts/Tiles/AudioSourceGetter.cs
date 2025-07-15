using MoreMountains.Feedbacks;
using UnityEngine;

public class AudioSourceGetter : MonoBehaviour
{
    private MMF_Player player;
    private MMF_AudioSource audioSource;
    private AudioSource og;

    void Start()
    {
        og = FindFirstObjectByType<AudioSource>();
        player = GetComponent<MMF_Player>();
        audioSource = player.GetFeedbackOfType<MMF_AudioSource>();

        audioSource.TargetAudioSource = og;
    }

}
