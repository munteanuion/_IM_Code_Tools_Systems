#if false

using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

#if DOTWEEN
using DG.Tweening;
#endif

namespace _CodeTools.ShaderCodeTools
{
    [System.Serializable]
    public class ShaderFloatAnimatorInspector
    {
        [field: Header("Animation Settings")]
        [field: SerializeField] public float DefaultStartValue { get; private set; } = 0.1f;
        [field: SerializeField] public float EndAnimValue { get; private set; } = 1f;
        [field: SerializeField] public float AnimDuration { get; private set; } = 0.1f;
        
        [field: Header("Ref Materials Settings")]
        [field: SerializeField] public string ShaderFieldName { get; private set; }
        [field: SerializeField] public int[] IndexesMaterial { get; private set; }
        [field: SerializeField] public Renderer[] MaterialMeshRenderer { get; private set; }


        public ShaderFloatAnimatorInspector() { }
        
        public ShaderFloatAnimatorInspector(float defaultStartValue, float endAnimValue, float animDuration, string shaderFieldName, int[] indexesMaterial, Renderer[] materialMeshRenderer)
        {
            DefaultStartValue = defaultStartValue;
            EndAnimValue = endAnimValue;
            AnimDuration = animDuration;
            ShaderFieldName = shaderFieldName;
            IndexesMaterial = indexesMaterial;
            MaterialMeshRenderer = materialMeshRenderer;
        }
    }
    
    
    
    [System.Serializable]
    public class ShaderFloatAnimator : IDisposable
    {
        [field: SerializeField] private ShaderFloatAnimatorInspector m_fields;
        
        private CancellationTokenSource m_cancelationToken;
        private int m_shaderFieldNameID;
        private List<Material> m_materialInstance;




        public ShaderFloatAnimator() { }
        public ShaderFloatAnimator(ShaderFloatAnimatorInspector shaderFloatAnimatorInspector)
        {
            m_fields = shaderFloatAnimatorInspector;
        }
        
        public void Init()
        {
            m_shaderFieldNameID = Shader.PropertyToID(m_fields.ShaderFieldName);
            m_materialInstance = new List<Material>(16);
            m_cancelationToken = new CancellationTokenSource();
            
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
            if (m_cancelationToken != null && !m_cancelationToken.IsCancellationRequested)
            {
                m_cancelationToken.Cancel();
                m_cancelationToken.Dispose();
            }
        }
        
        
        
        
        public async UniTask PlayFullAnimation()
        {
            await StartAnimationToEndFloat();
            await StartAnimationToDefaultFloat();
        }
        
        public async UniTask StartAnimationToEndFloat()
        {
            await AnimateToCustomFloat(m_fields.EndAnimValue);
        }

        public async UniTask StartAnimationToDefaultFloat()
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
            
            if (tween != null && tween.active)
            {
                try {
                    while (tween.active && !tween.IsComplete()) {
                        await UniTask.Yield(PlayerLoopTiming.Update, m_cancelationToken.Token);
                    }
                } catch (OperationCanceledException) { }
            }
#endif
        }
    }
}

#endif