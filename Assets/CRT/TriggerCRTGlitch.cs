#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using DG.Tweening;

public class CRTGlitchTester : MonoBehaviour
{
    [SerializeField] private Material crtMaterial;
    [SerializeField] private Transform crtQuad;

    private const float ResetDistortionStrength = 0.03f;
    private const float ResetStaticStrength = 0.19f;
    private const float ResetImageBrightness = 2f;
    private static readonly Vector2 ResetCRTWarpStrength = new Vector2(0.5f, 1f);
    private static readonly Vector3 ResetQuadScale = new Vector3(9.823153f, 6.893441f, 6.893441f);
    public bool isStart;

    private void Start()
    {
        if (!isStart)
        {
            SoftResetDie();
        }
        else
        {
            SoftResetDie();
        }
    }

    [ContextMenu("CRT Glitch")]
    private void TestCRTGlitchEffect(float duration)
    {
        DOTween.To(() => crtMaterial.GetFloat("_DistortionStrength"),
                   x => crtMaterial.SetFloat("_DistortionStrength", x),
                   -13.28f, duration);

        DOTween.To(() => crtMaterial.GetFloat("_StaticStrength"),
                   x => crtMaterial.SetFloat("_StaticStrength", x),
                   1.9f, duration).SetDelay(0.3f);

        DOTween.To(() => crtMaterial.GetFloat("_ImageBrightness"),
                   x => crtMaterial.SetFloat("_ImageBrightness", x),
                   1.24f, duration);
    }

    [ContextMenu("Reset CRT Settings")]
    public void ResetCRTSettings()
    {
        DOTween.To(() => crtMaterial.GetFloat("_DistortionStrength"),
                   x => crtMaterial.SetFloat("_DistortionStrength", x),
                   ResetDistortionStrength, 0.6f);

        DOTween.To(() => crtMaterial.GetFloat("_StaticStrength"),
                   x => crtMaterial.SetFloat("_StaticStrength", x),
                   ResetStaticStrength, 0.6f);

        DOTween.To(() => crtMaterial.GetFloat("_ImageBrightness"),
                   x => crtMaterial.SetFloat("_ImageBrightness", x),
                   ResetImageBrightness, 0.6f);

        crtMaterial.SetVector("_CRTWarpStrength", new Vector4(ResetCRTWarpStrength.x, ResetCRTWarpStrength.y, 0f, 0f));
    }

    [ContextMenu("Test Power Off Effect")]
    public void TestPowerOffEffect()
    {
        float duration = 0.45f;  // Sync duration for glitch and collapse

        var seq = DOTween.Sequence();

        seq.AppendCallback(() => TestCRTGlitchEffect(duration * 0.75f));

        seq.Join(crtQuad.DOScaleY(0.1f, duration * 1.55f).SetEase(Ease.InQuad));

        seq.Join(DOTween.To(() => crtMaterial.GetVector("_CRTWarpStrength"),
                            v => crtMaterial.SetVector("_CRTWarpStrength", new Vector4(0.5f, v.y, 0f, 0f)),
                            new Vector4(0.5f, 5, 0f, 0f), duration * 2));
    }

    [ContextMenu("Test Power On Effect")]
    public void TestPowerOnEffect()
    {
        GameObject.FindGameObjectWithTag("TurnOnAudio").GetComponent<AudioSource>().Play();
        Debug.Log("Now");
        ResetCRTSettings();

        crtQuad.DOScale(ResetQuadScale, 0.7f).SetEase(Ease.OutQuad);

        DOTween.To(() => crtMaterial.GetVector("_CRTWarpStrength"),
                   v => crtMaterial.SetVector("_CRTWarpStrength", new Vector4(v.x, v.y, 0f, 0f)),
                   new Vector4(ResetCRTWarpStrength.x, ResetCRTWarpStrength.y, 0f, 0f), 0.7f);
    }

    public void SoftResetDie()
    {

        crtQuad.localScale = new Vector3(10f, 5f, 5f); 
        crtMaterial.SetVector("_CRTWarpStrength", new Vector4(5f, 5f, 0f, 0f));

        crtMaterial.SetFloat("_DistortionStrength", -13.28f);
        crtMaterial.SetFloat("_StaticStrength", 1.9f);
        crtMaterial.SetFloat("_ImageBrightness", 1.24f);

        if (!isStart)
        {
            TestPowerOnEffect();
        }
    }
}
