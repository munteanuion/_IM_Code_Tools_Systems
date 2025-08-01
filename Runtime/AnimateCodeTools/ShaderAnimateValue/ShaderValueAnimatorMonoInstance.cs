using System;
using UnityEngine;

namespace _CodeTools.ShaderCodeTools
{
    public class ShaderValueAnimatorMonoInstance : MonoBehaviour, IDisposable
    {
        [SerializeField] private ShaderValueAnimatorInspector m_shaderValueAnimatorFields;
        [SerializeField] private bool m_autoInitialize = true;

        private ShaderValueAnimator m_shaderValueAnimator;
        


        
        public void Init()
        {
            m_shaderValueAnimator = new ShaderValueAnimator(m_shaderValueAnimatorFields); 
            m_shaderValueAnimator.Init();
        }
        
        public void Dispose()
        {
            m_shaderValueAnimator.Dispose();
            m_shaderValueAnimator = null;
        }

        private void OnEnable()
        {
            if (m_autoInitialize)
            {
                Init();
            }
        }
        
        private void OnDisable()
        {
            if (m_autoInitialize)
            {
                Dispose();
            }
        }

        
        

        [ContextMenu("Play Full Animation")]
        public void PlayFullAnimation()
        {
            if (m_shaderValueAnimator != null)
            {
                m_shaderValueAnimator.PlayFullAnimation();
            }
        }
        
        [ContextMenu("(Show)Start Animation To End Value")]
        public void StartAnimation()
        {
            if (m_shaderValueAnimator != null)
            {
                m_shaderValueAnimator.StartAnimationToEndValue();
            }
        }
        
        [ContextMenu("(Hide)Start Animation To Default Value")]
        public void StopAnimation()
        {
            if (m_shaderValueAnimator != null)
            {
                m_shaderValueAnimator.StartAnimationToDefaultValue();
            }
        }
        
        [ContextMenu("Instant Kill All Animation")]
        public void InstantKillAllAnimation()
        {
            if (m_shaderValueAnimator != null)
            {
                m_shaderValueAnimator.KillAnimations();
            }
        }
    }
}