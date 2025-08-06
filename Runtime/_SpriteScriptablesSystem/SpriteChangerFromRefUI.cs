using UnityEngine;
using UnityEngine.UI;

namespace Tools.UI
{
    public class SpriteChangerFromRefUI : MonoBehaviour
    {
        public Image Image;
        public SpriteChangerSettingsUI SpriteData;
        public bool RuntimeSetOnStart = true;

        

        private void OnValidate()
        {
            Image ??= GetComponent<Image>();
            
            SetSpriteData();
        } 

        private void SetSpriteData()
        {
            if (SpriteData && Image)
            {
                Image.sprite = SpriteData.Sprite;
                if (SpriteData.SetPreserveAspect) 
                    Image.preserveAspect = SpriteData.PreserveAspect;
                if (SpriteData.SetScale) 
                    Image.transform.localScale = SpriteData.ScaleTransform;
            }
            else
            {
                Debug.LogWarning("SpriteChangerFromRefUI::SetSpriteData: SpriteData is null or Image is null");
            }
        }

        private void Start()
        {
            if (RuntimeSetOnStart)
            {
                SetSpriteData();
            }
        }
    }
}