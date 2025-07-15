using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class LevelTransitionManager : MonoBehaviour
{
    public CRTGlitchTester crtGlitchTester;
    public Image fadeImage;
    public float fadeDuration = 1.0f;
    public float blackScreenDuration = 0.5f;
    
    private AudioSource complete;

    [ContextMenu("Test")]
    public void StartTransition(string sceneName)
    {
        Debug.Log("Starting Level Transition!");

        crtGlitchTester.TestPowerOffEffect();

        float totalCRTDur = 0.55f;

        DOVirtual.DelayedCall(totalCRTDur, () =>
        {
            complete = GameObject.FindGameObjectWithTag("TransitionAudio").GetComponent<AudioSource>();
            complete.Play();

            fadeImage.DOFade(1f, fadeDuration).OnComplete(() =>
            {
                DOVirtual.DelayedCall(blackScreenDuration, () =>
                {
                    SceneManager.LoadScene(sceneName);
                });
            });
        });
    }
}
