using System;
using UnityEngine;

namespace RefClassInParentForColliders
{
    public abstract class RefClassInParentForColliders<T> : MonoBehaviour, IDisposable
    {
        public T _RefC => m_RefClassInParentForColliders;
        protected T m_RefClassInParentForColliders;
        
        protected Action<Collider> OnTriggerEnterAction;
        protected Action<Collider> OnTriggerExitAction;
        
        protected Action<Collision> OnCollisionEnterAction;
        protected Action<Collision> OnCollisionExitAction;
        
        [SerializeField] protected Collider[] m_collidersForAddRefComponent;



        public virtual void Init(T refClassInParentForColliders, Type componentTypeToAdd)
        {
            m_RefClassInParentForColliders = refClassInParentForColliders;
            
            if (m_collidersForAddRefComponent == null 
                || m_collidersForAddRefComponent?.Length == 0
                || componentTypeToAdd == null
                ) return;
            
            var componentType = componentTypeToAdd; 
            foreach (var variable in m_collidersForAddRefComponent)
            {
                if (variable.gameObject.GetComponent(componentType) == null)
                {
                    var addedComponent = variable.gameObject.AddComponent(componentType) as RefClassInParentForColliders<T>;
                    addedComponent?.Init(refClassInParentForColliders, null);
                }
            }
        }

        public virtual void Dispose()
        {
            OnTriggerEnterAction = null;
            OnTriggerExitAction = null;
            OnCollisionEnterAction = null;
            OnCollisionExitAction = null;
            
            if (m_collidersForAddRefComponent.Length == 0) return;
            
            var componentType = this.GetType();
            foreach (var variable in m_collidersForAddRefComponent)
            {
                Destroy(variable.gameObject.GetComponent(componentType));
            }
        }


        
        
        protected void HandleCollisionOrTrigger<K>(string methodName, K arg)
        {
            if (Equals(m_RefClassInParentForColliders, this)) return;
        
            /*Debug.LogError($"In {methodName} call from abstract class RefClassInParentForColliders<T> " +
                           $"with type {typeof(T)} and ref class {m_RefClassInParentForColliders}");*/
        
            switch (arg)
            {
                case Collision collision:
                    if (methodName == nameof(OnCollisionEnter))
                        OnCollisionEnterAction?.Invoke(collision);
                    else if (methodName == nameof(OnCollisionExit))
                        OnCollisionExitAction?.Invoke(collision);
                    break;
                case Collider collider1:
                    if (methodName == nameof(OnTriggerEnter))
                        OnTriggerEnterAction?.Invoke(collider1);
                    else if (methodName == nameof(OnTriggerExit))
                        OnTriggerExitAction?.Invoke(collider1);
                    break;
            }
        }
        
        public virtual void OnCollisionEnter(Collision collision)
        {
            HandleCollisionOrTrigger(nameof(OnCollisionEnter), collision);
        }
        
        public virtual void OnCollisionExit(Collision collision)
        {
            HandleCollisionOrTrigger(nameof(OnCollisionExit), collision);
        }
        
        public virtual void OnTriggerEnter(Collider other)
        {
            HandleCollisionOrTrigger(nameof(OnTriggerEnter), other);
        }
        
        public virtual void OnTriggerExit(Collider other)
        {
            HandleCollisionOrTrigger(nameof(OnTriggerExit), other);
        }

        
        

        protected virtual void OnValidate()
        {
            if (!Application.isPlaying && m_collidersForAddRefComponent?.Length == 0) 
                m_collidersForAddRefComponent = GetComponentsInChildren<Collider>();
        }
    }
}