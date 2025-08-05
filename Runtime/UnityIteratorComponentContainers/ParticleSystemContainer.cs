using System;
using UnityEngine;

namespace UnityIteratorComponentContainers
{
    [Serializable]
    public class ParticleSystemContainer
    {
        [field: SerializeField] public ParticleSystem[] ParticleSystems { get; private set; }
        
        
        
        public void PlayAll(bool withChildren = true)
        {
            ParticleSystems.PlayAll(withChildren);
        }

        public void StopAll(bool withChildren = true)
        {
            ParticleSystems.StopAll(withChildren);
        }

        public void ClearAll(bool withChildren = true)
        {
            ParticleSystems.ClearAll(withChildren);
        }

        public bool AnyPlaying()
        {
            return ParticleSystems.AnyPlaying();
        }
    }
}