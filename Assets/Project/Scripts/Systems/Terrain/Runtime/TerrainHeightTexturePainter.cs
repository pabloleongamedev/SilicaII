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

        [Tooltip("Optional explicit terrains. If empty, the component uses the Terrain on this GameObject and all child Terrains.")]
        [SerializeField] private List<UnityEngine.Terrain> targetTerrains = new();

        [SerializeField] private List<HeightTextureLayer> layers = new();

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

        private static TerrainLayer[] GetTerrainLayers(IReadOnlyList<HeightTextureLayer> validLayers)
        {
            var terrainLayers = new TerrainLayer[validLayers.Count];
            for (var i = 0; i < validLayers.Count; i++)
            {
                terrainLayers[i] = validLayers[i].terrainLayer;
            }

            return terrainLayers;
        }

        private static float CalculateHeightWeight(HeightTextureLayer layer, float height)
        {
            var blend = Mathf.Max(0.001f, layer.blendDistance);
            var fadeIn = ApplyBlendCurve(Mathf.InverseLerp(layer.minHeight - blend, layer.minHeight + blend, height), layer.blendCurve);
            var fadeOut = 1f - ApplyBlendCurve(Mathf.InverseLerp(layer.maxHeight - blend, layer.maxHeight + blend, height), layer.blendCurve);
            return Mathf.Clamp01(fadeIn * fadeOut) * layer.opacity;
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
