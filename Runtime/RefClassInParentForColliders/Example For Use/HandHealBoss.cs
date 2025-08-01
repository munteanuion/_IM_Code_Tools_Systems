using System;
using UnityEngine;

namespace EXAMPLE_CODE_TOOLS_SYSTEMS
{
    public class HandHealBoss : HandHealBossRef, IDisposable
    {
        public void Init()
        {
            base.Init(this);
        }
        
        public override void OnCollisionEnter(Collision collision)
        {
            base.OnCollisionEnter(collision);
        }
    }
}