#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



namespace _IM_Code_Tools_Systems
{
    [ExecuteAlways]
    public class PrefabLightmapData : MonoBehaviour
    {
        [Tooltip("Reassigns shaders when applying the baked lightmaps. Might conflict with some shaders like transparent HDRP.")]
        public bool releaseShaders = true;
        
        [SerializeField] RendererInfo[] m_RendererInfo;
        [SerializeField] Texture2D[] m_Lightmaps;
        [SerializeField] Texture2D[] m_LightmapsDir;
        [SerializeField] Texture2D[] m_ShadowMasks;
        [SerializeField] LightInfo[] m_LightInfo;
    
        
        [System.Serializable]
        struct RendererInfo
        {
            public Renderer renderer;
            public int lightmapIndex;
            public Vector4 lightmapOffsetScale;
        }
        
        [System.Serializable]
        struct LightInfo
        {
            public Light light;
            public int lightmapBaketype;
            public int mixedLightingMode;
        }



        private void Awake() => Init();
        private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) => Init();
        private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

        
        private void Init()
        {
            if (m_RendererInfo == null || m_RendererInfo.Length == 0)
                return;

            var lightmaps = LightmapSettings.lightmaps;
            int[] offsetsindexes = new int[m_Lightmaps.Length];
            int counttotal = lightmaps.Length;
            List<LightmapData> combinedLightmaps = new List<LightmapData>();

            for (int i = 0; i < m_Lightmaps.Length; i++)
            {
                bool exists = false;
                for (int j = 0; j < lightmaps.Length; j++)
                {
                    if (m_Lightmaps[i] == lightmaps[j].lightmapColor)
                    {
                        exists = true;
                        offsetsindexes[i] = j;
                    }
                }
                if (!exists)
                {
                    offsetsindexes[i] = counttotal;
                    var newlightmapdata = new LightmapData
                    {
                        lightmapColor = m_Lightmaps[i],
                        lightmapDir = m_LightmapsDir.Length == m_Lightmaps.Length ? m_LightmapsDir[i] : default(Texture2D),
                        shadowMask = m_ShadowMasks.Length == m_Lightmaps.Length  ? m_ShadowMasks[i] : default(Texture2D),
                    };
                    combinedLightmaps.Add(newlightmapdata);
                    counttotal += 1;
                }
            }

            var combinedLightmaps2 = new LightmapData[counttotal];

            lightmaps.CopyTo(combinedLightmaps2, 0);
            combinedLightmaps.ToArray().CopyTo(combinedLightmaps2, lightmaps.Length);

            bool directional=true;

            foreach(Texture2D t in m_LightmapsDir)
            {
                if (t == null)
                {
                    directional = false;
                    break;
                }
            }

            LightmapSettings.lightmapsMode = (m_LightmapsDir.Length == m_Lightmaps.Length && directional) ? LightmapsMode.CombinedDirectional : LightmapsMode.NonDirectional;
            ApplyRendererInfo(m_RendererInfo, offsetsindexes, m_LightInfo);
            LightmapSettings.lightmaps = combinedLightmaps2;
        }
        
        
        private void ApplyRendererInfo(RendererInfo[] infos, int[] lightmapOffsetIndex, LightInfo[] lightsInfo)
        {
            for (int i = 0; i < infos.Length; i++)
            {
                var info = infos[i];

                info.renderer.lightmapIndex = lightmapOffsetIndex[info.lightmapIndex];
                info.renderer.lightmapScaleOffset = info.lightmapOffsetScale;

                if (releaseShaders)
                {
                    Material[] mat = info.renderer.sharedMaterials;
                    for (int j = 0; j < mat.Length; j++)
                    {
                        if (mat[j] != null && Shader.Find(mat[j].shader.name) != null)
                            mat[j].shader = Shader.Find(mat[j].shader.name);
                    }
                }
            }

            for (int i = 0; i < lightsInfo.Length; i++)
            {
                if (lightsInfo[i].light == null)
                    continue;
            
                var bakingOutput = new LightBakingOutput
                {
                    isBaked = true,
                    lightmapBakeType = (LightmapBakeType)lightsInfo[i].lightmapBaketype,
                    mixedLightingMode = (MixedLightingMode)lightsInfo[i].mixedLightingMode
                };

                lightsInfo[i].light.bakingOutput = bakingOutput;
            }
        }

        
        
#if UNITY_EDITOR
        [MenuItem("Tools/Prefab Lightning/Prepare Renderers For Light Mapping", false, 0)]
        [Obsolete("Obsolete")]
        public static void PrepareRenderersForLightMapping()
        {
            if (Lightmapping.giWorkflowMode != Lightmapping.GIWorkflowMode.OnDemand)
            {
                Debug.LogError("ExtractLightmapData requires that you have baked you lightmaps and Auto mode is disabled.");
                return;
            }

            PrefabLightmapData[] prefabs = FindObjectsByType<PrefabLightmapData>(FindObjectsSortMode.None);

            foreach (var instance in prefabs)
                PrepareSingleRendererForLightMapping(instance);
        }

        
        public static void PrepareSingleRendererForLightMapping(PrefabLightmapData prefab)
        {
            var gameObject = prefab.gameObject;
            var renderers = gameObject.GetComponentsInChildren<MeshRenderer>();
            
            foreach (MeshRenderer renderer in renderers)
            {
                var flags = GameObjectUtility.GetStaticEditorFlags(renderer.gameObject);
                flags |= StaticEditorFlags.ContributeGI;
                GameObjectUtility.SetStaticEditorFlags(renderer.gameObject, flags);
            } 
        }

        
        [MenuItem("Tools/Prefab Lightning/Generate Lightmap Info", false, 1)]
        [Obsolete("Obsolete")]
        public static void GenerateLightmapInfo()
        {
            if (Lightmapping.giWorkflowMode != Lightmapping.GIWorkflowMode.OnDemand)
            {
                Debug.LogError("ExtractLightmapData requires that you have baked you lightmaps and Auto mode is disabled.");
                return;
            }

            PrefabLightmapData[] prefabs = FindObjectsByType<PrefabLightmapData>(FindObjectsSortMode.None);
        
            foreach (var instance in prefabs)
                GenerateSingleLightmapInfo(instance);
        }
    
        
        public static void GenerateSingleLightmapInfo(PrefabLightmapData prefab)
        {
            var gameObject = prefab.gameObject;
            var rendererInfos = new List<RendererInfo>();
            var lightmaps = new List<Texture2D>();
            var lightmapsDir = new List<Texture2D>();
            var shadowMasks = new List<Texture2D>();
            var lightsInfos = new List<LightInfo>();

            GenerateLightmapInfo(gameObject, rendererInfos, lightmaps, lightmapsDir, shadowMasks, lightsInfos);

            prefab.m_RendererInfo = rendererInfos.ToArray();
            prefab.m_Lightmaps = lightmaps.ToArray();
            prefab.m_LightmapsDir = lightmapsDir.ToArray();
            prefab.m_LightInfo = lightsInfos.ToArray();
            prefab.m_ShadowMasks = shadowMasks.ToArray();
#if UNITY_2018_3_OR_NEWER
            var targetPrefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource(prefab.gameObject) as GameObject;
            
            if (targetPrefab != null)
            {
                GameObject root = PrefabUtility.GetOutermostPrefabInstanceRoot(prefab.gameObject);
                if (root != null)
                {
                    GameObject rootPrefab = PrefabUtility.GetCorrespondingObjectFromSource(prefab.gameObject);
                    string rootPath = AssetDatabase.GetAssetPath(rootPrefab);
                    PrefabUtility.UnpackPrefabInstanceAndReturnNewOutermostRoots(root, PrefabUnpackMode.OutermostRoot);
                    
                    try { PrefabUtility.ApplyPrefabInstance(prefab.gameObject, InteractionMode.AutomatedAction); }
                    catch { ;}
                    finally { PrefabUtility.SaveAsPrefabAssetAndConnect(root, rootPath, InteractionMode.AutomatedAction); }
                }
                else
                    PrefabUtility.ApplyPrefabInstance(prefab.gameObject, InteractionMode.AutomatedAction);
            }
#else
            var targetPrefab = UnityEditor.PrefabUtility.GetPrefabParent(gameObject) as GameObject;
            if (targetPrefab != null)
                UnityEditor.PrefabUtility.ReplacePrefab(gameObject, targetPrefab);
#endif
        }

        
        private static void GenerateLightmapInfo(GameObject root, List<RendererInfo> rendererInfos, List<Texture2D> lightmaps, List<Texture2D> lightmapsDir, List<Texture2D> shadowMasks, List<LightInfo> lightsInfo)
        {
            var renderers = root.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer renderer in renderers)
            {
                if (renderer.lightmapIndex != -1)
                {
                    RendererInfo info = new RendererInfo();
                    info.renderer = renderer;

                    if (renderer.lightmapScaleOffset != Vector4.zero)
                    {
                        if (renderer.lightmapIndex is < 0 or 0xFFFE) continue;
                        info.lightmapOffsetScale = renderer.lightmapScaleOffset;

                        Texture2D lightmap = LightmapSettings.lightmaps[renderer.lightmapIndex].lightmapColor;
                        Texture2D lightmapDir = LightmapSettings.lightmaps[renderer.lightmapIndex].lightmapDir;
                        Texture2D shadowMask = LightmapSettings.lightmaps[renderer.lightmapIndex].shadowMask;

                        info.lightmapIndex = lightmaps.IndexOf(lightmap);
                        if (info.lightmapIndex == -1)
                        {
                            info.lightmapIndex = lightmaps.Count;
                            lightmaps.Add(lightmap);
                            lightmapsDir.Add(lightmapDir);
                            shadowMasks.Add(shadowMask);
                        }

                        rendererInfos.Add(info);
                    }
                }
            }

            var lights = root.GetComponentsInChildren<Light>(true);

            foreach (Light l in lights)
            {
                LightInfo lightInfo = new LightInfo();
                lightInfo.light = l;
                lightInfo.lightmapBaketype = (int)l.lightmapBakeType;
#if UNITY_2020_1_OR_NEWER
                lightInfo.mixedLightingMode = (int)UnityEditor.Lightmapping.lightingSettings.mixedBakeMode;            
#elif UNITY_2018_1_OR_NEWER
            lightInfo.mixedLightingMode = (int)UnityEditor.LightmapEditorSettings.mixedBakeMode;
#else
            lightInfo.mixedLightingMode = (int)l.bakingOutput.lightmapBakeType;            
#endif
                lightsInfo.Add(lightInfo);
            }
        }
#endif
    }
}
