/*
 * Arquitectura: World/Runtime
 * Script: NightSkyStars
 * Rol: Presenter ambiental de estrellas y banda tipo Via Lactea sobre el skybox.
 * Relaciones: Consume WorldTimeService mediante IWorldTimeSource.
 * Uso como referencia: efectos atmosfericos runtime sin modificar materiales compartidos.
 */
using UnityEngine;

[DisallowMultipleComponent]
public sealed class NightSkyStars : MonoBehaviour
{
    private static readonly int FadeId = Shader.PropertyToID("_Fade");
    private static readonly int TintId = Shader.PropertyToID("_Tint");
    private static readonly int MilkyWayTintId = Shader.PropertyToID("_MilkyWayTint");
    private static readonly int StarIntensityId = Shader.PropertyToID("_StarIntensity");
    private static readonly int MilkyWayIntensityId = Shader.PropertyToID("_MilkyWayIntensity");
    private static readonly int MilkyWayWidthId = Shader.PropertyToID("_MilkyWayWidth");

    [Header("Time Source")]
    [SerializeField] private MonoBehaviour timeSourceBehaviour;

    [Header("Visibility")]
    [SerializeField, Range(0f, 24f)] private float fadeInStartHour = 18f;
    [SerializeField, Range(0f, 24f)] private float fullNightStartHour = 20f;
    [SerializeField, Range(0f, 24f)] private float fullNightEndHour = 4f;
    [SerializeField, Range(0f, 24f)] private float fadeOutEndHour = 6f;

    [Header("Look")]
    [SerializeField] private Shader starShader;
    [SerializeField] private Color starTint = new(0.78f, 0.86f, 1f, 1f);
    [SerializeField] private Color milkyWayTint = new(0.55f, 0.68f, 1f, 1f);
    [SerializeField, Range(0f, 5f)] private float starIntensity = 1.6f;
    [SerializeField, Range(0f, 5f)] private float milkyWayIntensity = 1.25f;
    [SerializeField, Range(0.02f, 0.45f)] private float milkyWayWidth = 0.16f;

    [Header("Dome")]
    [SerializeField] private Camera targetCamera;
    [SerializeField, Min(10f)] private float domeRadius = 750f;
    [SerializeField, Range(12, 96)] private int longitudeSegments = 48;
    [SerializeField, Range(6, 48)] private int latitudeSegments = 24;

    private IWorldTimeSource timeSource;
    private Transform domeTransform;
    private Material material;
    private Mesh mesh;

    private void Awake()
    {
        ResolveTimeSource();
        EnsureDome();
    }

    private void OnEnable()
    {
        ResolveTimeSource();
        EnsureDome();
    }

    private void LateUpdate()
    {
        EnsureDome();
        UpdateDomePosition();
        ApplyMaterialProperties();
    }

    private void OnDisable()
    {
        if (domeTransform != null)
            domeTransform.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (Application.isPlaying)
        {
            if (material != null)
                Destroy(material);
            if (mesh != null)
                Destroy(mesh);
            if (domeTransform != null)
                Destroy(domeTransform.gameObject);
        }
        else
        {
            if (material != null)
                DestroyImmediate(material);
            if (mesh != null)
                DestroyImmediate(mesh);
            if (domeTransform != null)
                DestroyImmediate(domeTransform.gameObject);
        }
    }

    private void EnsureDome()
    {
        if (domeTransform != null)
        {
            domeTransform.gameObject.SetActive(true);
            return;
        }

        if (targetCamera == null)
        {
            Debug.LogWarning("[NightSkyStars] Asigna Target Camera por Inspector.", this);
            return;
        }

        if (starShader == null)
        {
            Debug.LogWarning("[NightSkyStars] Asigna el shader Project/World/Night Milky Way por Inspector.", this);
            return;
        }

        material = new Material(starShader)
        {
            name = "Night Milky Way Runtime",
            hideFlags = HideFlags.DontSave
        };

        mesh = CreateSphereMesh(longitudeSegments, latitudeSegments);
        mesh.name = "Night Sky Dome Runtime";
        mesh.hideFlags = HideFlags.DontSave;

        var dome = new GameObject("Night Sky Stars Runtime")
        {
            hideFlags = HideFlags.DontSave
        };
        domeTransform = dome.transform;
        domeTransform.SetParent(transform, false);

        var meshFilter = dome.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = mesh;

        var meshRenderer = dome.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = material;
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        meshRenderer.receiveShadows = false;
    }

    private void UpdateDomePosition()
    {
        if (domeTransform == null)
            return;

        var center = targetCamera != null ? targetCamera.transform.position : Vector3.zero;
        var radius = targetCamera != null ? Mathf.Min(domeRadius, targetCamera.farClipPlane * 0.9f) : domeRadius;
        domeTransform.position = center;
        domeTransform.localScale = Vector3.one * radius;
    }

    private void ApplyMaterialProperties()
    {
        if (material == null)
            return;

        if (timeSource == null)
            return;

        material.SetFloat(FadeId, GetNightFade(timeSource.CurrentHour));
        material.SetColor(TintId, starTint);
        material.SetColor(MilkyWayTintId, milkyWayTint);
        material.SetFloat(StarIntensityId, starIntensity);
        material.SetFloat(MilkyWayIntensityId, milkyWayIntensity);
        material.SetFloat(MilkyWayWidthId, milkyWayWidth);
    }

    private float GetNightFade(float hour)
    {
        if (hour >= fullNightStartHour || hour <= fullNightEndHour)
            return 1f;

        if (hour >= fadeInStartHour && hour < fullNightStartHour)
            return Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(fadeInStartHour, fullNightStartHour, hour));

        if (hour > fullNightEndHour && hour <= fadeOutEndHour)
            return Mathf.SmoothStep(1f, 0f, Mathf.InverseLerp(fullNightEndHour, fadeOutEndHour, hour));

        return 0f;
    }

    private void ResolveTimeSource()
    {
        timeSource = timeSourceBehaviour as IWorldTimeSource;

        if (timeSourceBehaviour != null && timeSource == null)
            Debug.LogWarning("[NightSkyStars] El Time Source asignado no implementa IWorldTimeSource.", this);

        if (timeSourceBehaviour == null)
            Debug.LogWarning("[NightSkyStars] Asigna WorldTimeService por Inspector.", this);
    }

    private static Mesh CreateSphereMesh(int longitudeCount, int latitudeCount)
    {
        longitudeCount = Mathf.Max(12, longitudeCount);
        latitudeCount = Mathf.Max(6, latitudeCount);

        var vertices = new Vector3[(longitudeCount + 1) * (latitudeCount + 1)];
        var triangles = new int[longitudeCount * latitudeCount * 6];
        var vertexIndex = 0;

        for (var lat = 0; lat <= latitudeCount; lat++)
        {
            var v = lat / (float)latitudeCount;
            var phi = Mathf.PI * v;
            var y = Mathf.Cos(phi);
            var ringRadius = Mathf.Sin(phi);

            for (var lon = 0; lon <= longitudeCount; lon++)
            {
                var u = lon / (float)longitudeCount;
                var theta = u * Mathf.PI * 2f;
                vertices[vertexIndex++] = new Vector3(Mathf.Cos(theta) * ringRadius, y, Mathf.Sin(theta) * ringRadius);
            }
        }

        var triangleIndex = 0;
        for (var lat = 0; lat < latitudeCount; lat++)
        {
            for (var lon = 0; lon < longitudeCount; lon++)
            {
                var current = lat * (longitudeCount + 1) + lon;
                var next = current + longitudeCount + 1;

                triangles[triangleIndex++] = current;
                triangles[triangleIndex++] = next;
                triangles[triangleIndex++] = current + 1;

                triangles[triangleIndex++] = current + 1;
                triangles[triangleIndex++] = next;
                triangles[triangleIndex++] = next + 1;
            }
        }

        var sphereMesh = new Mesh
        {
            vertices = vertices,
            triangles = triangles
        };
        sphereMesh.RecalculateBounds();
        return sphereMesh;
    }
}
