using System;
using RefClassInParentForColliders;

namespace EXAMPLE_CODE_TOOLS_SYSTEMS
{
    public class HandHealBossRef : RefClassInParentForColliders<HandHealBoss>
    {
        public override void Init(HandHealBoss refClassInParentForColliders, Type componentTypeToAdd = null)
        {
            base.Init(refClassInParentForColliders, typeof(HandHealBossRef));
            
            OnCollisionEnterAction += m_RefClassInParentForColliders.OnCollisionEnter;
            OnCollisionExitAction += m_RefClassInParentForColliders.OnCollisionExit;
            OnTriggerEnterAction += m_RefClassInParentForColliders.OnTriggerEnter;
            OnTriggerExitAction += m_RefClassInParentForColliders.OnTriggerExit;
        }

        public override void Dispose()
        {
            base.Dispose();
            
            OnCollisionEnterAction -= m_RefClassInParentForColliders.OnCollisionEnter;
            OnCollisionExitAction -= m_RefClassInParentForColliders.OnCollisionExit;
            OnTriggerEnterAction -= m_RefClassInParentForColliders.OnTriggerEnter;
            OnTriggerExitAction -= m_RefClassInParentForColliders.OnTriggerExit;
        }
    }
}