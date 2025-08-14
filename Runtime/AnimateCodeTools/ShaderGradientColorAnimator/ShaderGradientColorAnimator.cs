using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace _CodeTools.ShaderCodeTools.ShaderGradientColorAnimator
{
    [System.Serializable]
    public class ShaderGradientColorAnimatorInspector
    {
        [field: Header("Animation Settings")]
        [field: SerializeField] public Gradient GradientColor { get; private set; }
        [field: SerializeField] public float AnimDuration { get; private set; } = 0.1f;
        
        [field: Header("Ref Materials Settings")]
        [field: SerializeField] public string ShaderFieldName { get; private set; }
        [field: SerializeField] public int[] IndexesMaterial { get; private set; }
        [field: SerializeField] public Renderer[] MaterialMeshRenderer { get; private set; }


        
        public ShaderGradientColorAnimatorInspector() { }
        
        public ShaderGradientColorAnimatorInspector(Gradient gradientColor, float animDuration, string shaderFieldName, int[] indexesMaterial, Renderer[] materialMeshRenderer)
        {
            GradientColor = gradientColor;
            AnimDuration = animDuration;
            ShaderFieldName = shaderFieldName;
            IndexesMaterial = indexesMaterial;
            MaterialMeshRenderer = materialMeshRenderer;
        }
    }

    
    
    [System.Serializable]
    public class ShaderGradientColorAnimator : IDisposable
    {
        [SerializeField] private ShaderGradientColorAnimatorInspector m_fields;
        [SerializeField] private bool m_ifAlreadyAnimatedSkipAnimation;
        
        private CancellationTokenSource m_cancelationToken;
        private int m_shaderFieldNameID;
        private List<Material> m_materialInstance;



        public ShaderGradientColorAnimator() { }

        public ShaderGradientColorAnimator(ShaderGradientColorAnimatorInspector shaderValueAnimatorInspector, bool ifAlreadyAnimatedSkipAnimation = false)
        {
            m_fields = shaderValueAnimatorInspector;
            m_ifAlreadyAnimatedSkipAnimation = ifAlreadyAnimatedSkipAnimation;
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
                VARIABLE.DOKill();
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
            await StartAnimationToEndValue();
            await StartAnimationToDefaultValue();
        }
        
        public async UniTask StartAnimationToEndValue()
        {
            await AnimateToCustomValue(m_fields.GradientColor.Evaluate(1));
        }

        public async UniTask StartAnimationToDefaultValue()
        {
            await AnimateToCustomValue(m_fields.GradientColor.Evaluate(0));
        }

        public void KillAnimations()
        {
            foreach (var material in m_materialInstance)
            {
                material.DOKill();
            }
        }
        
        
        public async UniTask PlayAnimationGradientValue(float value)
        {
            if (value < 0 || value > 1)
            {
                Debug.LogError("Value must be between 0 and 1.");
                return;
            }
            
            Color targetColor = m_fields.GradientColor.Evaluate(value);
            await AnimateToCustomValue(targetColor);
        }
        
        
        
        public async UniTask AnimateToCustomValue(Color targetValue)
        {
            Tween tween = null;

            foreach (var material in m_materialInstance)
            {
                if (DOTween.IsTweening(material))
                {
                    Debug.LogError("Tween is already active on this material");
                    if (m_ifAlreadyAnimatedSkipAnimation) continue;
                }
                
                tween = material.DOColor(targetValue, m_shaderFieldNameID, m_fields.AnimDuration);
            }
            
            if (tween != null && tween.active)
            {
                try {
                    while (tween.active && !tween.IsComplete()) {
                        await UniTask.Yield(PlayerLoopTiming.Update, m_cancelationToken.Token);
                    }
                } catch (OperationCanceledException) { }
            }
        }
    }
}