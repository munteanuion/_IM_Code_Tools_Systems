using UnityEngine;

namespace UnityIteratorComponentContainers
{
    public static class ParticleSystemExtensions
    {
        public static void PlayAll(this ParticleSystem[] systems, bool withChildren = true)
        {
            foreach (var ps in systems)
            {
                if (ps != null)
                    ps.Play(withChildren);
            }
        }

        public static void StopAll(this ParticleSystem[] systems, bool withChildren = true)
        {
            foreach (var ps in systems)
            {
                if (ps != null)
                    ps.Stop(withChildren);
            }
        }

        public static void ClearAll(this ParticleSystem[] systems, bool withChildren = true)
        {
            foreach (var ps in systems)
            {
                if (ps != null)
                    ps.Clear(withChildren);
            }
        }

        public static bool AnyPlaying(this ParticleSystem[] systems)
        {
            foreach (var ps in systems)
            {
                if (ps != null && ps.isPlaying)
                    return true;
            }
            return false;
        }
    }
}