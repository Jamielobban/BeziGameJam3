using MoreMountains.Feedbacks;
using UnityEngine;

public class AudioSourceGetter : MonoBehaviour
{
    private MMF_Player player;
    private MMF_AudioSource audioSource;
    private AudioSource og;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        og = FindFirstObjectByType<AudioSource>();
        player = GetComponent<MMF_Player>();
        audioSource = player.GetFeedbackOfType<MMF_AudioSource>();

        audioSource.TargetAudioSource = og;
    }

}
