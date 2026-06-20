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
    private static readonly int BackgroundRotationId = Shader.PropertyToID("_BackgroundRotation");
    private static readonly int MidgroundRotationId = Shader.PropertyToID("_MidgroundRotation");
    private static readonly int ForegroundRotationId = Shader.PropertyToID("_ForegroundRotation");
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
    private Material instancedSourceSkybox;
    private float backgroundRotation;
    private float midgroundRotation;
    private float foregroundRotation;

    public Material GetOrCreateAnimatedSkybox()
    {
        return GetOrCreateAnimatedSkybox(sourceSkybox);
    }

    public Material GetOrCreateAnimatedSkybox(Material skybox)
    {
        var material = skybox != null ? skybox : sourceSkybox;
        if (material == null)
            material = RenderSettings.skybox;

        ApplySkyboxInstance(material);
        ApplySpeeds();

        return runtimeSkybox != null ? runtimeSkybox : material;
    }

    private void OnEnable()
    {
        ApplySkyboxInstance();
        ApplySpeeds();
    }

    private void Update()
    {
        if (runtimeSkybox == null)
            return;

        AdvanceRotations(Time.unscaledDeltaTime);
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
        ApplySkyboxInstance(material);
    }

    private void ApplySkyboxInstance(Material material)
    {
        if (material == null)
        {
            Debug.LogWarning("[SkyboxCloudAnimator] Asigna un skybox material o configura RenderSettings.skybox.", this);
            return;
        }

        if (runtimeSkybox != null && instancedSourceSkybox == material)
            return;

        RestoreOriginalSkybox();
        DestroyRuntimeSkybox();

        originalSkybox = RenderSettings.skybox;
        runtimeSkybox = new Material(material)
        {
            name = $"{material.name} (Cloud Animator Runtime)",
            hideFlags = HideFlags.DontSave
        };
        instancedSourceSkybox = material;
        CaptureRotations(runtimeSkybox);

        if (useRenderSettingsSkybox)
        {
            RenderSettings.skybox = runtimeSkybox;
            DynamicGI.UpdateEnvironment();
        }
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
        instancedSourceSkybox = null;
    }

    private void CaptureRotations(Material material)
    {
        backgroundRotation = GetFloatIfPresent(material, BackgroundRotationId, backgroundRotation);
        midgroundRotation = GetFloatIfPresent(material, MidgroundRotationId, midgroundRotation);
        foregroundRotation = GetFloatIfPresent(material, ForegroundRotationId, foregroundRotation);
    }

    private void AdvanceRotations(float deltaTime)
    {
        backgroundRotation = Mathf.Repeat(backgroundRotation + backgroundSpeed * deltaTime, 360f);
        midgroundRotation = Mathf.Repeat(midgroundRotation + midgroundSpeed * deltaTime, 360f);
        foregroundRotation = Mathf.Repeat(foregroundRotation + foregroundSpeed * deltaTime, 360f);

        SetFloatIfPresent(runtimeSkybox, BackgroundRotationId, backgroundRotation);
        SetFloatIfPresent(runtimeSkybox, MidgroundRotationId, midgroundRotation);
        SetFloatIfPresent(runtimeSkybox, ForegroundRotationId, foregroundRotation);
    }

    private static void SetFloatIfPresent(Material material, int propertyId, float value)
    {
        if (material.HasProperty(propertyId))
            material.SetFloat(propertyId, value);
    }

    private static float GetFloatIfPresent(Material material, int propertyId, float fallback)
    {
        return material.HasProperty(propertyId) ? material.GetFloat(propertyId) : fallback;
    }
}
