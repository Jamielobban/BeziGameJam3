using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LevelTransitionManager : MonoBehaviour
{
    public CRTGlitchTester crtGlitchTester;
    public Image fadeImage;
    public float fadeDuration = 1.0f;
    public float blackScreenDuration = 0.5f;

    [ContextMenu("Test")]
    public void StartTransition(System.Action onComplete)
    {
        Debug.Log("Starting Level Transition!");

        // Trigger CRT Power Off Effect (collapse + glitch)
        crtGlitchTester.TestPowerOffEffect();

        // Fade to black AFTER CRT effect completes
        float totalCRTDur = 0.55f;  // Same as CRT collapse duration in your glitch script
        DOVirtual.DelayedCall(totalCRTDur, () =>
        {
            fadeImage.DOFade(1f, fadeDuration).OnComplete(() =>
            {
                onComplete?.Invoke();

                DOVirtual.DelayedCall(blackScreenDuration, () =>
                {
                    fadeImage.DOFade(0f, fadeDuration);
                    crtGlitchTester.TestPowerOnEffect();  // Power On after fade back
                });
            });
        });
    }
}
