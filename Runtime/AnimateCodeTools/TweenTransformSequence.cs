#if PRIME_TWEEN_DOTWEEN_ADAPTER
        
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using PrimeTween;

namespace PrimeTweenExtensions
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class TweenTransformSequence : MonoBehaviour
    {
        [Serializable]
        public enum TweenAnimType
        {
            LocalPosition,
            LocalRotation,
            LocalScale,
            Position,
            Rotation,
            Scale
        }

        [Serializable]
        public class TweenItem
        {
            public TweenAnimType animType = TweenAnimType.LocalPosition;
            public Transform targetTransform; 
            public TweenSettings<Vector3> vector3Settings;
            public bool animateAsync = true;
        }

        [Header("Debug. Don't forget to disable after preview!")]
        [SerializeField] private bool useEditorPreview = false;
        
        [Header("Tween Sequence Settings")]
        [SerializeField] private bool playOnEnable = false;
        
        [Tooltip("List of animations to play in sequence or individually.")]
        [SerializeField] private  List<TweenItem> tweenSequence = new List<TweenItem>();
        
        private bool _isPlaying = false;



        private void OnEnable()
        {
            if (!Application.isPlaying) return;

            if (playOnEnable) PlaySequence();
        }

        private void OnDisable()
        {
            if (!Application.isPlaying) return;

            StopSequence();
        }

        private void OnValidate()
        {
            if (useEditorPreview && !Application.isPlaying && !_isPlaying)
                PlaySequence();
            else if (!useEditorPreview && !Application.isPlaying && _isPlaying)
                StopSequence();
        }

        
        
        public async void PlaySequence()
        {
            _isPlaying = true;
            foreach (var tween in tweenSequence)
            {
                if (!tween.animateAsync)
                    _ = PlayTween(tween);
            }
            foreach (var tween in tweenSequence)
            {
                if (tween.animateAsync)
                    await PlayTween(tween);
            }
        }

        
        private async Task PlayTween(TweenItem item)
        {
            if (item.targetTransform == null)
            {
                Debug.LogWarning($"Tween '{item}' has no target component assigned!");
                return;
            }

            Transform target = item.targetTransform;

            switch (item.animType)
            {
                case TweenAnimType.LocalPosition:
                    await Tween.LocalPosition(target, item.vector3Settings).AsyncWaitForCompletion();
                    break;
                case TweenAnimType.LocalRotation:
                    await Tween.LocalRotation(target, item.vector3Settings).AsyncWaitForCompletion();
                    break;
                case TweenAnimType.LocalScale:
                    await Tween.LocalScale(target, item.vector3Settings).AsyncWaitForCompletion();
                    break;
                case TweenAnimType.Position:
                    await Tween.Position(target, item.vector3Settings).AsyncWaitForCompletion();
                    break;
                case TweenAnimType.Rotation:
                    await Tween.Rotation(target, item.vector3Settings).AsyncWaitForCompletion();
                    break;
                case TweenAnimType.Scale:
                    await Tween.Scale(target, item.vector3Settings).AsyncWaitForCompletion();
                    break;
                default:
                    return;
            }
        }

        
        public void StopSequence()
        {
            for (var i = tweenSequence.Count -1; i >= 0; i--)
            {
                var tween = tweenSequence[i];
                var transform1 = tween.targetTransform;
                if (!transform1) continue;

                transform1.DOKill();

                switch (tween.animType)
                {
                    case TweenAnimType.LocalPosition:
                        transform1.localPosition = tween.vector3Settings.startValue;
                        break;
                    case TweenAnimType.LocalRotation:
                        transform1.localRotation = Quaternion.Euler(tween.vector3Settings.startValue);
                        break;
                    case TweenAnimType.LocalScale:
                        transform1.localScale = tween.vector3Settings.startValue;
                        break;
                    case TweenAnimType.Position:
                        transform1.position = tween.vector3Settings.startValue;
                        break;
                    case TweenAnimType.Rotation:
                        transform1.rotation = Quaternion.Euler(tween.vector3Settings.startValue);
                        break;
                    case TweenAnimType.Scale:
                        transform1.localScale = tween.vector3Settings.startValue;
                        break;
                }
            }

            _isPlaying = false;
        }
    }
}

#endif