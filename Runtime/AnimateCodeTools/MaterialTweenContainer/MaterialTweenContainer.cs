using System;
using System.Collections.Generic;
using UnityEngine;

namespace AnimateCodeTools.MaterialTweenContainer
{
    [System.Serializable]
    public class MaterialTweenContainer : IDisposable
    {
        [field: Header("Ref Materials Settings")]
        [field: SerializeField] public string ShaderFieldName { get; private set; }
        [field: SerializeField] public int[] IndexMaterials { get; private set; }
        [field: SerializeField] public Renderer[] MaterialsMeshRenderer { get; private set; }

        public int ShaderFieldNameID { get; private set; }
        public IReadOnlyList<Material> MaterialInstances => _materialInstance;
        private List<Material> _materialInstance;
        

        public MaterialTweenContainer() { }
        public MaterialTweenContainer(string shaderFieldName, int[] indexMaterials, Renderer[] materialsMeshRenderer)
        {
            ShaderFieldName = shaderFieldName;
            IndexMaterials = indexMaterials;
            MaterialsMeshRenderer = materialsMeshRenderer;
        }
        
        
        public void Init()
        {
            ShaderFieldNameID = Shader.PropertyToID(ShaderFieldName);
            _materialInstance ??= new List<Material>();
            _materialInstance.Clear();
            
            foreach (var meshRenderer in MaterialsMeshRenderer)
            {
                foreach (var indexMaterial in IndexMaterials)
                {
                    if (meshRenderer.materials.Length <= indexMaterial || meshRenderer.materials[indexMaterial] == null)
                    {
                        Debug.LogError($"Material index {indexMaterial} is out of range or null for {meshRenderer.name}. Skipping.");
                        continue;
                    }
                    _materialInstance.Add(meshRenderer.materials[indexMaterial]);
                }
            }
        }
        
        public void Dispose()
        {
            _materialInstance.Clear();
        }


#if PRIME_TWEEN_DOTWEEN_ADAPTER
        
#endif
    }
}