using UnityEngine;

namespace CodeExtensions
{
    public static class GameObjectExtensions
    {
        public static void Activate(this GameObject gameObject)
        {
            if (!gameObject.activeSelf) gameObject.SetActive(true);
        }
        
        public static void Deactivate(this GameObject gameObject)
        {
            if (gameObject.activeSelf) gameObject.SetActive(false);
        }

        
        public static void Destroy(this GameObject gameObject)
        {
            Object.Destroy(gameObject);
        }
        public static void Destroy(this GameObject gameObject, float delay)
        {
            Object.Destroy(gameObject,delay);
        }
    }
}