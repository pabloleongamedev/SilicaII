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
            var fadeIn = Mathf.InverseLerp(layer.minHeight - blend, layer.minHeight + blend, height);
            var fadeOut = 1f - Mathf.InverseLerp(layer.maxHeight - blend, layer.maxHeight + blend, height);
            return Mathf.Clamp01(fadeIn * fadeOut) * layer.opacity;
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
