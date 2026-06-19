/*
 * Arquitectura: World/Runtime
 * Script: SkyboxCloudAnimator
 * Rol: Presenter ambiental para dar movimiento al skybox ToonScapes sin modificar el material asset en runtime.
 * Relaciones: Usa RenderSettings.skybox o un material asignado por Inspector.
 * Uso como referencia: controlar movimiento atmosferico desde escena, no desde assets compartidos.
 */
using UnityEngine;

public class SkyboxCloudAnimator : MonoBehaviour
{
    private static readonly int BackgroundRotationSpeedId = Shader.PropertyToID("_BackgroundRotationSpeed");
    private static readonly int MidgroundRotationSpeedId = Shader.PropertyToID("_MidgroundRotationSpeed");
    private static readonly int ForegroundRotationSpeedId = Shader.PropertyToID("_ForegroundRotationSpeed");
    private static readonly int BackgroundDistortionSpeedId = Shader.PropertyToID("_BackgroundDistortionSpeed");
    private static readonly int MidgroundDistortionSpeedId = Shader.PropertyToID("_MidgroundDistortionSpeed");
    private static readonly int ForegroundDistortionSpeedId = Shader.PropertyToID("_ForegroundDistortionSpeed");

    [Header("Skybox")]
    [SerializeField] private Material sourceSkybox;
    [SerializeField] private bool useRenderSettingsSkybox = true;

    [Header("Rotation Speed")]
    [Tooltip("Degrees per second for the far cloud/background layer.")]
    [SerializeField] private float backgroundSpeed = 0.15f;

    [Tooltip("Degrees per second for the middle cloud layer.")]
    [SerializeField] private float midgroundSpeed = 0.35f;

    [Tooltip("Degrees per second for the near cloud layer.")]
    [SerializeField] private float foregroundSpeed = 0.6f;

    [Header("Distortion")]
    [SerializeField] private bool animateDistortion = true;
    [SerializeField] private float backgroundDistortionSpeed = 0.08f;
    [SerializeField] private float midgroundDistortionSpeed = 0.12f;
    [SerializeField] private float foregroundDistortionSpeed = 0.18f;

    private Material runtimeSkybox;
    private Material originalSkybox;

    private void OnEnable()
    {
        ApplySkyboxInstance();
        ApplySpeeds();
    }

    private void OnValidate()
    {
        if (!isActiveAndEnabled)
            return;

        if (!Application.isPlaying && runtimeSkybox == null)
            return;

        ApplySpeeds();
    }

    private void OnDisable()
    {
        RestoreOriginalSkybox();
        DestroyRuntimeSkybox();
    }

    private void ApplySkyboxInstance()
    {
        var material = sourceSkybox != null ? sourceSkybox : RenderSettings.skybox;
        if (material == null)
        {
            Debug.LogWarning("[SkyboxCloudAnimator] Asigna un skybox material o configura RenderSettings.skybox.", this);
            return;
        }

        if (runtimeSkybox != null && runtimeSkybox.shader == material.shader)
            return;

        RestoreOriginalSkybox();
        DestroyRuntimeSkybox();

        originalSkybox = RenderSettings.skybox;
        runtimeSkybox = new Material(material)
        {
            name = $"{material.name} (Cloud Animator Runtime)",
            hideFlags = HideFlags.DontSave
        };

        if (useRenderSettingsSkybox)
            RenderSettings.skybox = runtimeSkybox;
    }

    private void ApplySpeeds()
    {
        var material = runtimeSkybox != null ? runtimeSkybox : sourceSkybox;
        if (material == null)
            return;

        SetFloatIfPresent(material, BackgroundRotationSpeedId, backgroundSpeed);
        SetFloatIfPresent(material, MidgroundRotationSpeedId, midgroundSpeed);
        SetFloatIfPresent(material, ForegroundRotationSpeedId, foregroundSpeed);

        if (!animateDistortion)
            return;

        SetFloatIfPresent(material, BackgroundDistortionSpeedId, backgroundDistortionSpeed);
        SetFloatIfPresent(material, MidgroundDistortionSpeedId, midgroundDistortionSpeed);
        SetFloatIfPresent(material, ForegroundDistortionSpeedId, foregroundDistortionSpeed);
    }

    private void RestoreOriginalSkybox()
    {
        if (useRenderSettingsSkybox && originalSkybox != null && RenderSettings.skybox == runtimeSkybox)
            RenderSettings.skybox = originalSkybox;

        originalSkybox = null;
    }

    private void DestroyRuntimeSkybox()
    {
        if (runtimeSkybox == null)
            return;

        if (Application.isPlaying)
            Destroy(runtimeSkybox);
        else
            DestroyImmediate(runtimeSkybox);

        runtimeSkybox = null;
    }

    private static void SetFloatIfPresent(Material material, int propertyId, float value)
    {
        if (material.HasProperty(propertyId))
            material.SetFloat(propertyId, value);
    }
}
