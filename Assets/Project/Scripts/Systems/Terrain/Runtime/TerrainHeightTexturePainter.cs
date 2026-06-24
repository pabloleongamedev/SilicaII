using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Project.Systems.Terrain.Runtime
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public sealed class TerrainHeightTexturePainter : MonoBehaviour
    {
        public enum BlendCurveMode
        {
            Linear,
            SmoothStep,
            SmootherStep,
            EaseIn,
            EaseOut
        }

        public enum GrassAssetMode
        {
            Auto,
            Texture,
            Prefab
        }

        [Serializable]
        public sealed class HeightTextureLayer
        {
            [Tooltip("Terrain layer/texture to paint for this height band.")]
            public TerrainLayer terrainLayer;

            [Tooltip("World-space height where this layer starts becoming visible.")]
            public float minHeight;

            [Tooltip("World-space height where this layer stops being visible.")]
            public float maxHeight = 10f;

            [Min(0.001f)]
            [Tooltip("Soft blend distance around min/max height.")]
            public float blendDistance = 1f;

            [Tooltip("Controls how soft or sharp the height transition feels inside Blend Distance.")]
            public BlendCurveMode blendCurve = BlendCurveMode.SmootherStep;

            [Range(0f, 1f)]
            [Tooltip("Layer strength after height blending.")]
            public float opacity = 1f;
        }

        [Serializable]
        public sealed class HeightGrassLayer
        {
            [Tooltip("Auto prefers Detail Prefab when assigned, otherwise Detail Texture.")]
            public GrassAssetMode assetMode = GrassAssetMode.Auto;

            [Tooltip("Optional grass texture detail. Use either this or Detail Prefab.")]
            public Texture2D detailTexture;

            [Tooltip("Optional grass prefab detail. Use either this or Detail Texture.")]
            public GameObject detailPrefab;

            [Tooltip("World-space height where this grass starts appearing.")]
            public float minHeight;

            [Tooltip("World-space height where this grass stops appearing.")]
            public float maxHeight = 10f;

            [Min(0.001f)]
            [Tooltip("Soft blend distance around min/max height. Density fades out near these borders.")]
            public float blendDistance = 1f;

            [Tooltip("Controls how soft or sharp the grass density transition feels inside Blend Distance.")]
            public BlendCurveMode blendCurve = BlendCurveMode.SmootherStep;

            [Range(0f, 1f)]
            [Tooltip("Overall density multiplier for this grass layer.")]
            public float density = 1f;

            [Range(0, 512)]
            [Tooltip("Maximum detail density written to each Terrain detail cell.")]
            public int maxDensityPerCell = 8;

            [Range(0f, 1f)]
            [Tooltip("Cells below this height weight are left empty. Lower values paint a fuller band.")]
            public float minimumPaintWeight = 0.01f;

            [Header("Appearance")]
            public Color healthyColor = Color.white;
            public Color dryColor = Color.white;

            [Min(0.01f)] public float minWidth = 0.5f;
            [Min(0.01f)] public float maxWidth = 1f;
            [Min(0.01f)] public float minHeightScale = 0.5f;
            [Min(0.01f)] public float maxHeightScale = 1f;

            public DetailRenderMode renderMode = DetailRenderMode.GrassBillboard;
        }

        [Tooltip("Optional explicit terrains. If empty, the component uses the Terrain on this GameObject and all child Terrains.")]
        [SerializeField] private List<UnityEngine.Terrain> targetTerrains = new();

        [SerializeField] private List<HeightTextureLayer> layers = new();

        [Header("Grass Painting")]
        [SerializeField] private List<HeightGrassLayer> grassLayers = new();

        [Min(0)]
        [SerializeField] private int detailResolutionOverride;

        [Min(8)]
        [SerializeField] private int detailResolutionPerPatch = 32;

        [Header("Grass Visibility")]
        [Min(1f)]
        [Tooltip("How far Terrain detail grass remains visible in the Scene/Game view.")]
        [SerializeField] private float detailDrawDistance = 500f;

        [Range(0f, 1f)]
        [Tooltip("Global Terrain detail density multiplier applied when grass is painted.")]
        [SerializeField] private float detailObjectDensity = 1f;

        [Header("Sampling")]
        [SerializeField] private bool includeTerrainWorldY = true;

        [Min(1)]
        [SerializeField] private int alphamapResolutionOverride;

        [Header("Smoothing")]
        [Min(0)]
        [Tooltip("Applies a box blur to the final alphamap. Use this to remove jagged height-band edges.")]
        [SerializeField] private int blurIterations = 1;

        [Range(1, 8)]
        [Tooltip("How many alphamap pixels each blur step samples around every point.")]
        [SerializeField] private int blurRadius = 2;

        [Range(0f, 1f)]
        [Tooltip("0 keeps the raw height paint, 1 uses the fully blurred result.")]
        [SerializeField] private float blurStrength = 0.75f;

        public IReadOnlyList<HeightTextureLayer> Layers => layers;
        public IReadOnlyList<HeightGrassLayer> GrassLayers => grassLayers;

        [ContextMenu("Apply Height Texture Painting")]
        public void ApplyHeightTexturePainting()
        {
            var validLayers = GetValidLayers();
            if (validLayers.Count == 0)
            {
                Debug.LogWarning($"{nameof(TerrainHeightTexturePainter)} has no valid terrain layers to paint.", this);
                return;
            }

            var terrains = GetTargetTerrains();
            if (terrains.Count == 0)
            {
                Debug.LogWarning($"{nameof(TerrainHeightTexturePainter)} could not find any Terrain with TerrainData.", this);
                return;
            }

            foreach (var terrain in terrains)
            {
                PaintTerrain(terrain, validLayers);
            }

            Debug.Log($"{nameof(TerrainHeightTexturePainter)} painted {terrains.Count} terrain(s) with {validLayers.Count} layer(s).", this);
        }

        [ContextMenu("Apply Grass Painting")]
        public void ApplyGrassPainting()
        {
            var validGrassLayers = GetValidGrassLayers();
            if (validGrassLayers.Count == 0)
            {
                Debug.LogWarning($"{nameof(TerrainHeightTexturePainter)} has no valid grass layers to paint.", this);
                return;
            }

            var terrains = GetTargetTerrains();
            if (terrains.Count == 0)
            {
                Debug.LogWarning($"{nameof(TerrainHeightTexturePainter)} could not find any Terrain with TerrainData.", this);
                return;
            }

            foreach (var terrain in terrains)
            {
                PaintGrass(terrain, validGrassLayers);
            }

            Debug.Log($"{nameof(TerrainHeightTexturePainter)} painted grass on {terrains.Count} terrain(s) with {validGrassLayers.Count} detail layer(s).", this);
        }

        [ContextMenu("Apply Textures And Grass")]
        public void ApplyTexturesAndGrass()
        {
            ApplyHeightTexturePainting();
            ApplyGrassPainting();
        }

        private void PaintTerrain(UnityEngine.Terrain terrain, IReadOnlyList<HeightTextureLayer> validLayers)
        {
            var terrainData = terrain.terrainData;
            terrainData.terrainLayers = GetTerrainLayers(validLayers);

            if (alphamapResolutionOverride > 0 &&
                terrainData.alphamapResolution != alphamapResolutionOverride)
            {
                terrainData.alphamapResolution = alphamapResolutionOverride;
            }

            var width = terrainData.alphamapWidth;
            var height = terrainData.alphamapHeight;
            var layerCount = validLayers.Count;
            var alphamaps = new float[height, width, layerCount];
            var terrainPosition = terrain.transform.position;

            for (var y = 0; y < height; y++)
            {
                var normalizedY = height <= 1 ? 0f : y / (float)(height - 1);

                for (var x = 0; x < width; x++)
                {
                    var normalizedX = width <= 1 ? 0f : x / (float)(width - 1);
                    var terrainHeight = terrainData.GetInterpolatedHeight(normalizedX, normalizedY);
                    var worldHeight = includeTerrainWorldY ? terrainHeight + terrainPosition.y : terrainHeight;

                    var totalWeight = 0f;
                    for (var layerIndex = 0; layerIndex < layerCount; layerIndex++)
                    {
                        var weight = CalculateHeightWeight(validLayers[layerIndex], worldHeight);
                        alphamaps[y, x, layerIndex] = weight;
                        totalWeight += weight;
                    }

                    if (totalWeight <= 0f)
                    {
                        alphamaps[y, x, 0] = 1f;
                        continue;
                    }

                    for (var layerIndex = 0; layerIndex < layerCount; layerIndex++)
                    {
                        alphamaps[y, x, layerIndex] /= totalWeight;
                    }
                }
            }

            SmoothAlphamaps(alphamaps, width, height, layerCount);
            terrainData.SetAlphamaps(0, 0, alphamaps);

#if UNITY_EDITOR
            EditorUtility.SetDirty(terrainData);
            EditorUtility.SetDirty(terrain);
#endif
        }

        private void PaintGrass(UnityEngine.Terrain terrain, IReadOnlyList<HeightGrassLayer> validGrassLayers)
        {
            var terrainData = terrain.terrainData;
            ApplyGrassVisibilitySettings(terrain);

            var detailResolution = GetValidDetailResolution();
            if (detailResolution > 0 && terrainData.detailResolution != detailResolution)
            {
                terrainData.SetDetailResolution(detailResolution, detailResolutionPerPatch);
            }

            terrainData.detailPrototypes = GetDetailPrototypes(validGrassLayers);
            terrainData.RefreshPrototypes();

            var width = terrainData.detailWidth;
            var height = terrainData.detailHeight;
            var terrainPosition = terrain.transform.position;

            for (var layerIndex = 0; layerIndex < validGrassLayers.Count; layerIndex++)
            {
                var grassLayer = validGrassLayers[layerIndex];
                var detailMap = new int[height, width];
                var paintedCells = 0;
                var maxWrittenDensity = 0;
                var minSampledHeight = float.PositiveInfinity;
                var maxSampledHeight = float.NegativeInfinity;

                for (var y = 0; y < height; y++)
                {
                    var normalizedY = height <= 1 ? 0f : y / (float)(height - 1);

                    for (var x = 0; x < width; x++)
                    {
                        var normalizedX = width <= 1 ? 0f : x / (float)(width - 1);
                        var terrainHeight = terrainData.GetInterpolatedHeight(normalizedX, normalizedY);
                        var worldHeight = includeTerrainWorldY ? terrainHeight + terrainPosition.y : terrainHeight;
                        minSampledHeight = Mathf.Min(minSampledHeight, worldHeight);
                        maxSampledHeight = Mathf.Max(maxSampledHeight, worldHeight);
                        var weight = CalculateGrassWeight(grassLayer, worldHeight);
                        var writtenDensity = GetGrassDensity(grassLayer, weight);
                        detailMap[y, x] = writtenDensity;

                        if (writtenDensity > 0)
                        {
                            paintedCells++;
                            maxWrittenDensity = Mathf.Max(maxWrittenDensity, writtenDensity);
                        }
                    }
                }

                terrainData.SetDetailLayer(0, 0, layerIndex, detailMap);
                Debug.Log(
                    $"{nameof(TerrainHeightTexturePainter)} grass layer {layerIndex} '{GetGrassLayerName(grassLayer)}': " +
                    $"painted cells {paintedCells}/{width * height}, max density {maxWrittenDensity}, " +
                    $"terrain height range {minSampledHeight:0.###}-{maxSampledHeight:0.###}, " +
                    $"layer range {grassLayer.minHeight:0.###}-{grassLayer.maxHeight:0.###}, " +
                    $"density {grassLayer.density:0.##}, max cell {grassLayer.maxDensityPerCell}, " +
                    $"{GetGrassPrototypeDebug(grassLayer)}.",
                    this);
            }

            terrain.Flush();
            Debug.Log(
                $"{nameof(TerrainHeightTexturePainter)} grass visibility on '{terrain.name}': " +
                $"detail distance {terrain.detailObjectDistance:0.##}, detail density {terrain.detailObjectDensity:0.##}, " +
                $"detail resolution {terrainData.detailResolution}x{terrainData.detailResolution}, prototypes {terrainData.detailPrototypes.Length}.",
                this);

#if UNITY_EDITOR
            EditorUtility.SetDirty(terrainData);
            EditorUtility.SetDirty(terrain);
#endif
        }

        private void ApplyGrassVisibilitySettings(UnityEngine.Terrain terrain)
        {
            terrain.detailObjectDistance = Mathf.Max(1f, detailDrawDistance);
            terrain.detailObjectDensity = detailObjectDensity;
        }

        [ContextMenu("Sort Layers By Height")]
        public void SortLayersByHeight()
        {
            layers.Sort((a, b) => a.minHeight.CompareTo(b.minHeight));
        }

        [ContextMenu("Use Current Terrain Layers")]
        public void UseCurrentTerrainLayers()
        {
            var terrain = GetTargetTerrains().FirstOrDefault();
            if (terrain == null)
            {
                Debug.LogWarning($"{nameof(TerrainHeightTexturePainter)} could not find any Terrain with TerrainData.", this);
                return;
            }

            layers.Clear();
            var terrainLayers = terrain.terrainData.terrainLayers;
            var terrainSize = terrain.terrainData.size;
            var step = terrainLayers.Length == 0 ? 0f : terrainSize.y / terrainLayers.Length;

            for (var i = 0; i < terrainLayers.Length; i++)
            {
                layers.Add(new HeightTextureLayer
                {
                    terrainLayer = terrainLayers[i],
                    minHeight = i * step,
                    maxHeight = i == terrainLayers.Length - 1 ? terrainSize.y : (i + 1) * step,
                    blendDistance = Mathf.Max(1f, step * 0.1f),
                    opacity = 1f
                });
            }
        }

        private List<UnityEngine.Terrain> GetTargetTerrains()
        {
            var terrains = targetTerrains
                .Where(terrain => terrain != null && terrain.terrainData != null)
                .Distinct()
                .ToList();

            if (terrains.Count > 0)
            {
                return terrains;
            }

            return GetComponentsInChildren<UnityEngine.Terrain>(true)
                .Where(terrain => terrain != null && terrain.terrainData != null)
                .Distinct()
                .ToList();
        }

        private int GetValidDetailResolution()
        {
            if (detailResolutionOverride <= 0)
            {
                return 0;
            }

            var patchSize = Mathf.Max(8, detailResolutionPerPatch);
            var resolution = Mathf.Max(patchSize, detailResolutionOverride);
            var roundedResolution = Mathf.CeilToInt(resolution / (float)patchSize) * patchSize;

            if (roundedResolution != detailResolutionOverride)
            {
                Debug.LogWarning(
                    $"{nameof(TerrainHeightTexturePainter)} adjusted Detail Resolution Override from {detailResolutionOverride} to {roundedResolution} so it is compatible with patch size {patchSize}.",
                    this);
            }

            return roundedResolution;
        }

        private List<HeightTextureLayer> GetValidLayers()
        {
            var validLayers = new List<HeightTextureLayer>();
            foreach (var layer in layers)
            {
                if (layer?.terrainLayer == null)
                {
                    continue;
                }

                if (layer.maxHeight < layer.minHeight)
                {
                    (layer.minHeight, layer.maxHeight) = (layer.maxHeight, layer.minHeight);
                }

                validLayers.Add(layer);
            }

            return validLayers;
        }

        private List<HeightGrassLayer> GetValidGrassLayers()
        {
            var validGrassLayers = new List<HeightGrassLayer>();
            foreach (var layer in grassLayers)
            {
                if (layer == null || (layer.detailTexture == null && layer.detailPrefab == null))
                {
                    continue;
                }

                if (GetGrassUsePrefab(layer) && layer.detailPrefab == null)
                {
                    continue;
                }

                if (!GetGrassUsePrefab(layer) && ResolveGrassTexture(layer) == null)
                {
                    continue;
                }

                if (layer.maxHeight < layer.minHeight)
                {
                    (layer.minHeight, layer.maxHeight) = (layer.maxHeight, layer.minHeight);
                }

                if (layer.maxWidth < layer.minWidth)
                {
                    (layer.minWidth, layer.maxWidth) = (layer.maxWidth, layer.minWidth);
                }

                if (layer.maxHeightScale < layer.minHeightScale)
                {
                    (layer.minHeightScale, layer.maxHeightScale) = (layer.maxHeightScale, layer.minHeightScale);
                }

                layer.maxDensityPerCell = Mathf.Max(0, layer.maxDensityPerCell);
                layer.minimumPaintWeight = Mathf.Clamp01(layer.minimumPaintWeight);
                layer.healthyColor = GetVisibleGrassColor(layer.healthyColor);
                layer.dryColor = GetVisibleGrassColor(layer.dryColor);

                validGrassLayers.Add(layer);
            }

            return validGrassLayers;
        }

        private static TerrainLayer[] GetTerrainLayers(IReadOnlyList<HeightTextureLayer> validLayers)
        {
            var terrainLayers = new TerrainLayer[validLayers.Count];
            for (var i = 0; i < validLayers.Count; i++)
            {
                terrainLayers[i] = validLayers[i].terrainLayer;
            }

            return terrainLayers;
        }

        private static DetailPrototype[] GetDetailPrototypes(IReadOnlyList<HeightGrassLayer> validGrassLayers)
        {
            var prototypes = new DetailPrototype[validGrassLayers.Count];
            for (var i = 0; i < validGrassLayers.Count; i++)
            {
                var layer = validGrassLayers[i];
                var usePrefab = GetGrassUsePrefab(layer);
                var detailTexture = ResolveGrassTexture(layer);
                var prototype = new DetailPrototype
                {
                    prototypeTexture = usePrefab ? null : detailTexture,
                    prototype = usePrefab ? layer.detailPrefab : null,
                    healthyColor = layer.healthyColor,
                    dryColor = layer.dryColor,
                    minWidth = layer.minWidth,
                    maxWidth = layer.maxWidth,
                    minHeight = layer.minHeightScale,
                    maxHeight = layer.maxHeightScale,
                    renderMode = usePrefab ? DetailRenderMode.VertexLit : layer.renderMode,
                    usePrototypeMesh = usePrefab,
                    useInstancing = usePrefab,
                    useDensityScaling = true,
                    density = 1f,
                    targetCoverage = 1f,
                    alignToGround = 1f,
                    positionJitter = 1f,
                    noiseSpread = 0.1f
                };

                if (!prototype.Validate(out var errorMessage))
                {
                    Debug.LogWarning(
                        $"{nameof(TerrainHeightTexturePainter)} detail prototype '{GetGrassLayerName(layer)}' is invalid: {errorMessage}",
                        layer.detailPrefab != null ? layer.detailPrefab : layer.detailTexture);
                }

                prototypes[i] = prototype;
            }

            return prototypes;
        }

        private static string GetGrassLayerName(HeightGrassLayer layer)
        {
            return GetGrassUsePrefab(layer)
                ? layer.detailPrefab != null ? layer.detailPrefab.name : "Missing Prefab"
                : ResolveGrassTexture(layer) != null ? ResolveGrassTexture(layer).name : "Missing Texture";
        }

        private static string GetGrassPrototypeDebug(HeightGrassLayer layer)
        {
            if (!GetGrassUsePrefab(layer))
            {
                return $"mode Texture, size {layer.minWidth:0.##}-{layer.maxWidth:0.##}w x {layer.minHeightScale:0.##}-{layer.maxHeightScale:0.##}h";
            }

            var meshFilter = layer.detailPrefab != null ? layer.detailPrefab.GetComponentInChildren<MeshFilter>() : null;
            var meshSize = meshFilter != null && meshFilter.sharedMesh != null
                ? Vector3.Scale(meshFilter.sharedMesh.bounds.size, meshFilter.transform.lossyScale)
                : Vector3.zero;

            return $"mode Prefab, mesh size {meshSize.x:0.###}x{meshSize.y:0.###}x{meshSize.z:0.###}, " +
                $"size {layer.minWidth:0.##}-{layer.maxWidth:0.##}w x {layer.minHeightScale:0.##}-{layer.maxHeightScale:0.##}h";
        }

        private static bool GetGrassUsePrefab(HeightGrassLayer layer)
        {
            var hasUsablePrefab = HasUsablePrefabMesh(layer);
            var hasUsableTexture = ResolveGrassTexture(layer) != null;
            return layer.assetMode switch
            {
                GrassAssetMode.Prefab => hasUsablePrefab || !hasUsableTexture,
                GrassAssetMode.Texture => false,
                _ => !hasUsableTexture && hasUsablePrefab
            };
        }

        private static Texture2D ResolveGrassTexture(HeightGrassLayer layer)
        {
            if (layer.detailTexture != null)
            {
                return layer.detailTexture;
            }

            var renderer = layer.detailPrefab != null ? layer.detailPrefab.GetComponentInChildren<Renderer>() : null;
            var material = renderer != null ? renderer.sharedMaterial : null;
            if (material == null)
            {
                return null;
            }

            var texture = GetTextureFromMaterial(material, "_BaseMap")
                ?? GetTextureFromMaterial(material, "_MainTex")
                ?? GetTextureFromMaterial(material, "_BaseTexture");

            return texture as Texture2D;
        }

        private static Texture GetTextureFromMaterial(Material material, string propertyName)
        {
            return material.HasProperty(propertyName) ? material.GetTexture(propertyName) : null;
        }

        private static bool HasUsablePrefabMesh(HeightGrassLayer layer)
        {
            var meshFilter = layer.detailPrefab != null ? layer.detailPrefab.GetComponentInChildren<MeshFilter>() : null;
            return meshFilter != null &&
                meshFilter.sharedMesh != null &&
                meshFilter.sharedMesh.bounds.size.sqrMagnitude > 0f;
        }

        private static Color GetVisibleGrassColor(Color color)
        {
            if (color.a > 0f && color.grayscale > 0.02f)
            {
                return color;
            }

            return Color.white;
        }

        private static float CalculateHeightWeight(HeightTextureLayer layer, float height)
        {
            var blend = Mathf.Max(0.001f, layer.blendDistance);
            var fadeIn = ApplyBlendCurve(Mathf.InverseLerp(layer.minHeight - blend, layer.minHeight + blend, height), layer.blendCurve);
            var fadeOut = 1f - ApplyBlendCurve(Mathf.InverseLerp(layer.maxHeight - blend, layer.maxHeight + blend, height), layer.blendCurve);
            return Mathf.Clamp01(fadeIn * fadeOut) * layer.opacity;
        }

        private static float CalculateGrassWeight(HeightGrassLayer layer, float height)
        {
            var blend = Mathf.Max(0.001f, layer.blendDistance);
            var fadeIn = ApplyBlendCurve(Mathf.InverseLerp(layer.minHeight - blend, layer.minHeight + blend, height), layer.blendCurve);
            var fadeOut = 1f - ApplyBlendCurve(Mathf.InverseLerp(layer.maxHeight - blend, layer.maxHeight + blend, height), layer.blendCurve);
            return Mathf.Clamp01(fadeIn * fadeOut);
        }

        private static int GetGrassDensity(HeightGrassLayer layer, float weight)
        {
            if (weight < layer.minimumPaintWeight || layer.maxDensityPerCell <= 0 || layer.density <= 0f)
            {
                return 0;
            }

            var weightedDensity = Mathf.RoundToInt(layer.maxDensityPerCell * layer.density * weight);
            return Mathf.Clamp(weightedDensity, 0, layer.maxDensityPerCell);
        }

        private void SmoothAlphamaps(float[,,] alphamaps, int width, int height, int layerCount)
        {
            if (blurIterations <= 0 || blurStrength <= 0f || width <= 1 || height <= 1)
            {
                return;
            }

            var source = alphamaps;
            var working = new float[height, width, layerCount];
            var radius = Mathf.Max(1, blurRadius);

            for (var iteration = 0; iteration < blurIterations; iteration++)
            {
                for (var y = 0; y < height; y++)
                {
                    for (var x = 0; x < width; x++)
                    {
                        var sampleCount = 0;
                        for (var sampleY = Mathf.Max(0, y - radius); sampleY <= Mathf.Min(height - 1, y + radius); sampleY++)
                        {
                            for (var sampleX = Mathf.Max(0, x - radius); sampleX <= Mathf.Min(width - 1, x + radius); sampleX++)
                            {
                                for (var layerIndex = 0; layerIndex < layerCount; layerIndex++)
                                {
                                    working[y, x, layerIndex] += source[sampleY, sampleX, layerIndex];
                                }

                                sampleCount++;
                            }
                        }

                        var totalWeight = 0f;
                        for (var layerIndex = 0; layerIndex < layerCount; layerIndex++)
                        {
                            var blurred = working[y, x, layerIndex] / sampleCount;
                            var blended = Mathf.Lerp(source[y, x, layerIndex], blurred, blurStrength);
                            working[y, x, layerIndex] = blended;
                            totalWeight += blended;
                        }

                        NormalizeWeights(working, y, x, layerCount, totalWeight);
                    }
                }

                if (iteration < blurIterations - 1)
                {
                    Array.Clear(source, 0, source.Length);
                    (source, working) = (working, source);
                }
            }

            if (!ReferenceEquals(working, alphamaps))
            {
                for (var y = 0; y < height; y++)
                {
                    for (var x = 0; x < width; x++)
                    {
                        for (var layerIndex = 0; layerIndex < layerCount; layerIndex++)
                        {
                            alphamaps[y, x, layerIndex] = working[y, x, layerIndex];
                        }
                    }
                }
            }
        }

        private static void NormalizeWeights(float[,,] alphamaps, int y, int x, int layerCount, float totalWeight)
        {
            if (totalWeight <= 0f)
            {
                alphamaps[y, x, 0] = 1f;
                for (var layerIndex = 1; layerIndex < layerCount; layerIndex++)
                {
                    alphamaps[y, x, layerIndex] = 0f;
                }

                return;
            }

            for (var layerIndex = 0; layerIndex < layerCount; layerIndex++)
            {
                alphamaps[y, x, layerIndex] /= totalWeight;
            }
        }

        private static float ApplyBlendCurve(float value, BlendCurveMode mode)
        {
            var t = Mathf.Clamp01(value);
            return mode switch
            {
                BlendCurveMode.SmoothStep => t * t * (3f - 2f * t),
                BlendCurveMode.SmootherStep => t * t * t * (t * (6f * t - 15f) + 10f),
                BlendCurveMode.EaseIn => t * t,
                BlendCurveMode.EaseOut => 1f - (1f - t) * (1f - t),
                _ => t
            };
        }

        private void Reset()
        {
            targetTerrains = GetComponentsInChildren<UnityEngine.Terrain>(true)
                .Where(terrain => terrain != null && terrain.terrainData != null)
                .Distinct()
                .ToList();
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(TerrainHeightTexturePainter))]
    public sealed class TerrainHeightTexturePainterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space(8f);
            var painter = (TerrainHeightTexturePainter)target;

            if (GUILayout.Button("Apply To Terrains"))
            {
                painter.ApplyHeightTexturePainting();
            }

            if (GUILayout.Button("Apply Grass"))
            {
                painter.ApplyGrassPainting();
            }

            if (GUILayout.Button("Apply Textures And Grass"))
            {
                painter.ApplyTexturesAndGrass();
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Use Current Terrain Layers"))
                {
                    painter.UseCurrentTerrainLayers();
                }

                if (GUILayout.Button("Sort Layers"))
                {
                    painter.SortLayersByHeight();
                }
            }
        }
    }
#endif
}
