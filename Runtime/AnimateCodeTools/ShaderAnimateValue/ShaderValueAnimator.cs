using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

#if DOTWEEN
using DG.Tweening;
#endif

namespace _CodeTools.ShaderCodeTools
{
    [System.Serializable]
    public class ShaderValueAnimatorInspector
    {
        [field: Header("Animation Settings")]
        [field: SerializeField] public float DefaultStartValue { get; private set; } = 0.1f;
        [field: SerializeField] public float EndAnimValue { get; private set; } = 1f;
        [field: SerializeField] public float AnimDuration { get; private set; } = 0.1f;
        
        [field: Header("Ref Materials Settings")]
        [field: SerializeField] public string ShaderFieldName { get; private set; }
        [field: SerializeField] public int[] IndexesMaterial { get; private set; }
        [field: SerializeField] public Renderer[] MaterialMeshRenderer { get; private set; }


        public ShaderValueAnimatorInspector() { }
        
        public ShaderValueAnimatorInspector(float defaultStartValue, float endAnimValue, float animDuration, string shaderFieldName, int[] indexesMaterial, Renderer[] materialMeshRenderer)
        {
            DefaultStartValue = defaultStartValue;
            EndAnimValue = endAnimValue;
            AnimDuration = animDuration;
            ShaderFieldName = shaderFieldName;
            IndexesMaterial = indexesMaterial;
            MaterialMeshRenderer = materialMeshRenderer;
        }
    }
    
    
    
    public class ShaderValueAnimator : IDisposable
    {
        private ShaderValueAnimatorInspector m_fields;
        
        private int m_shaderFieldNameID;
        private List<Material> m_materialInstance;


        

        public ShaderValueAnimator(ShaderValueAnimatorInspector shaderValueAnimatorInspector)
        {
            m_fields = shaderValueAnimatorInspector;
        }
        
        public void Init()
        {
            m_shaderFieldNameID = Shader.PropertyToID(m_fields.ShaderFieldName);
            m_materialInstance = new List<Material>(16);
            
            foreach (var VARIABLE in m_fields.MaterialMeshRenderer)
            {
                foreach (var item2 in m_fields.IndexesMaterial)
                {
                    if (VARIABLE.materials.Length <= item2 || VARIABLE.materials[item2] == null)
                    {
                        Debug.LogError($"Material index {item2} is out of range or null for {VARIABLE.name}. Skipping.");
                        continue;
                    }
                    m_materialInstance.Add(VARIABLE.materials[item2]);
                }
            }
        }
        
        public void Dispose()
        {
            foreach (var VARIABLE in m_materialInstance)
            {
#if DOTWEEN
                VARIABLE.DOKill();
#endif
            }
            m_materialInstance.Clear();
        }
        
        
        
        
        public async UniTask PlayFullAnimation()
        {
            await StartAnimationToEndValue();
            await StartAnimationToDefaultValue();
        }
        
        public async UniTask StartAnimationToEndValue()
        {
            await AnimateToCustomFloat(m_fields.EndAnimValue);
        }

        public async UniTask StartAnimationToDefaultValue()
        {
            await AnimateToCustomFloat(m_fields.DefaultStartValue);
        }

        public void KillAnimations()
        {
            foreach (var material in m_materialInstance)
            {
#if DOTWEEN
                material.DOKill();
#endif
            }
        }
        
        private async UniTask AnimateToCustomFloat(float targetValue)
        {
#if DOTWEEN
            Tween tween = null;
            
            foreach (var material in m_materialInstance)
            {
                material.DOKill();
                tween = material.DOFloat(targetValue, m_shaderFieldNameID, m_fields.AnimDuration);
            }
            
            //if (tween != null)
                //await tween.AsyncWaitForCompletion();
#endif
        }
    }
}