using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CollidersInteractionWrapper
{
    [Serializable]
    public class CollidersInteractContainer
    {
        [SerializeField] private List<Collider> colliders = new();
        private List<ColliderInteractWrapper> _wrappers = new();

        public IReadOnlyList<ColliderInteractWrapper> Wrappers => _wrappers;



        public CollidersInteractContainer() { }

        public CollidersInteractContainer(List<Collider> colliders)
        {
            this.colliders = colliders;
        }

        
        public void InitializeWrappers()
        {
            _wrappers.Clear();

            foreach (var col in colliders)
            {
                if (col == null)
                    continue;

                if (!col.TryGetComponent(out ColliderInteractWrapper wrapper))
                    wrapper = col.gameObject.AddComponent<ColliderInteractWrapper>();

                _wrappers.Add(wrapper);
            }
        }

        public void UninitializeWrappers()
        {
            foreach (var wrapper in _wrappers)
            {
                UnityEngine.Object.Destroy(wrapper);
            }
            _wrappers.Clear();
        }

        
        
        public void Subscribe(
            UnityAction<Collider> onTriggerEnter = null,
            UnityAction<Collider> onTriggerExit = null,
            UnityAction<Collision> onCollisionEnter = null,
            UnityAction<Collision> onCollisionExit = null)
        {
            foreach (var wrapper in _wrappers)
            {
                if (onTriggerEnter != null)
                    wrapper.OnTrigger_Enter.AddListener(onTriggerEnter);
                
                if (onTriggerExit != null)
                    wrapper.OnTrigger_Exit.AddListener(onTriggerExit);
                
                if (onCollisionEnter != null)
                    wrapper.OnCollision_Enter.AddListener(onCollisionEnter);
                
                if (onCollisionExit != null)
                    wrapper.OnCollision_Exit.AddListener(onCollisionExit); 
            }
        }

        public void Unsubscribe(
            UnityAction<Collider> onTriggerEnter = null,
            UnityAction<Collider> onTriggerExit = null,
            UnityAction<Collision> onCollisionEnter = null,
            UnityAction<Collision> onCollisionExit = null)
        {
            foreach (var wrapper in _wrappers)
            {
                if (onTriggerEnter != null)
                    wrapper.OnTrigger_Enter.RemoveListener(onTriggerEnter);
                
                if (onTriggerExit != null)
                    wrapper.OnTrigger_Exit.RemoveListener(onTriggerExit);
                
                if (onCollisionEnter != null)
                    wrapper.OnCollision_Enter.RemoveListener(onCollisionEnter);
                
                if (onCollisionExit != null)
                    wrapper.OnCollision_Exit.RemoveListener(onCollisionExit);
            }
        }
    }
}