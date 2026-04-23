using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace CollidersInteractionWrapper
{
    [System.Serializable]
    public class InteractionFilterSettings
    {
        public LayerMask LayerMask = ~0;
        public List<string> Tags = new();
    }

    [DisallowMultipleComponent]
    public class ColliderInteractWrapper : MonoBehaviour
    {
        [Header("Filters")]
        [SerializeField] private InteractionFilterSettings _filterSettings = new();

        [Header("Events")]
        public UnityEvent<Collider> OnTrigger_Enter = new();
        public UnityEvent<Collider> OnTrigger_Exit = new();
        public UnityEvent<Collision> OnCollision_Enter = new();
        public UnityEvent<Collision> OnCollision_Exit = new();

        private readonly List<Collider> _activeTriggers = new();
        private readonly List<Collision> _activeCollisions = new();

        public IReadOnlyList<Collider> ActiveTriggers => _activeTriggers;
        public IReadOnlyList<Collision> ActiveCollisions => _activeCollisions;
        
        public bool HasOnTriggers => _activeTriggers.Count > 0;
        public bool HasOnCollisions => _activeCollisions.Count > 0;

        public void SetFilterSettings(InteractionFilterSettings settings)
        {
            _filterSettings = settings;
        }

        private bool IsValid(GameObject obj)
        {
            if (_filterSettings == null) return true;

            if ((_filterSettings.LayerMask.value & (1 << obj.layer)) == 0)
                return false;

            if (_filterSettings.Tags != null && _filterSettings.Tags.Count > 0 && !_filterSettings.Tags.Contains(obj.tag))
                return false;

            return true;
        }
        
        
        private void OnTriggerEnter(Collider other)
        {
            if (!IsValid(other.gameObject)) return;
            
            if (!_activeTriggers.Contains(other))
            {
                _activeTriggers.Add(other);
                OnTrigger_Enter?.Invoke(other);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_activeTriggers.Remove(other))
            {
                OnTrigger_Exit?.Invoke(other);
            }
        }

        
        
        private void OnCollisionEnter(Collision collision)
        {
            if (!IsValid(collision.gameObject)) return;

            if (_activeCollisions.FindIndex(c => c.collider == collision.collider) == -1)
            {
                _activeCollisions.Add(collision);
                OnCollision_Enter?.Invoke(collision);
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            int index = _activeCollisions.FindIndex(c => c.collider == collision.collider);
            if (index != -1)
            {
                _activeCollisions.RemoveAt(index);
                OnCollision_Exit?.Invoke(collision);
            }
        }
    }
}