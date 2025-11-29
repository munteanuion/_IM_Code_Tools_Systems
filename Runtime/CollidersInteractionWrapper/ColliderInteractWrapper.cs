using UnityEngine;
using UnityEngine.Events;

namespace _IM_Code_Tools_Systems.Plugins._IM_Code_Tools_Systems.Runtime.CollidersInteractionWrapper
{
    [DisallowMultipleComponent]
    public class ColliderInteractWrapper : MonoBehaviour
    {
        public UnityEvent<Collider> OnTrigger_Enter = new();
        public UnityEvent<Collider> OnTrigger_Exit = new();
        public UnityEvent<Collision> OnCollision_Enter = new();
        public UnityEvent<Collision> OnCollision_Exit = new();

        
        private void OnTriggerEnter(Collider other)
        {
            OnTrigger_Enter?.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            OnTrigger_Exit?.Invoke(other);
        }

        private void OnCollisionEnter(Collision collision)
        {
            OnCollision_Enter?.Invoke(collision);
        }

        private void OnCollisionExit(Collision collision)
        {
            OnCollision_Exit?.Invoke(collision);
        }
    }
}