using UnityEngine;

namespace Tools.UI
{
    [CreateAssetMenu(fileName = "SpriteDataUI", menuName = "Scriptables/Sprite Changer", order = 0)]
    public class SpriteChangerSettingsUI : ScriptableObject
    {
        [Header("Main Settings")]
        public Sprite Sprite;
        [Header("Additional Settings")]
        public bool SetPreserveAspect = false;
        public bool PreserveAspect = true;
        [Header("Additional Scale Settings")]
        public bool SetScale = false;
        public Vector3 ScaleTransform = new Vector3(1, 1, 1);
    }
}