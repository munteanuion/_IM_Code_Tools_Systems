#if false

using System;
using UnityEngine;

namespace _CodeTools.ShaderCodeTools
{
    public class ShaderFloatAnimatorMonoInstance : MonoBehaviour, IDisposable
    {
        [SerializeField] private ShaderFloatAnimatorInspector m_shaderFloatAnimatorFields;
        [SerializeField] private bool m_autoInitialize = true;

        private ShaderFloatAnimator m_shaderFloatAnimator;
        

        
        public void Init()
        {
            m_shaderFloatAnimator = new ShaderFloatAnimator(m_shaderFloatAnimatorFields); 
            m_shaderFloatAnimator.Init();
        }
        
        public void Dispose()
        {
            m_shaderFloatAnimator.Dispose();
            m_shaderFloatAnimator = null;
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
            if (m_shaderFloatAnimator != null)
            {
                m_shaderFloatAnimator.PlayFullAnimation();
            }
        }
        
        [ContextMenu("(Show)Start Animation To End Float")]
        public void StartAnimation()
        {
            if (m_shaderFloatAnimator != null)
            {
                m_shaderFloatAnimator.StartAnimationToEndFloat();
            }
        }
        
        [ContextMenu("(Hide)Start Animation To Default Float")]
        public void StopAnimation()
        {
            if (m_shaderFloatAnimator != null)
            {
                m_shaderFloatAnimator.StartAnimationToDefaultFloat();
            }
        }
        
        [ContextMenu("Instant Kill All Animation")]
        public void InstantKillAllAnimation()
        {
            if (m_shaderFloatAnimator != null)
            {
                m_shaderFloatAnimator.KillAnimations();
            }
        }
    }
}

#endif