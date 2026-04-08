using System;
using System.Collections.Generic;
using UnityEngine;


namespace Common.Utils.Utils
{
    public enum SortDirection
    {
        TopToBottom,
        BottomToTop
    }
    
    public enum RowAlignment
    {
        Left,
        Center,
        Right
    }
    
    //[Obsolete("Script has a problem with Alignment logic (Center option isn't really centering), so i do not recommend to use it until someone will not fix it")]
    public class LayoutGrid3D : MonoBehaviour
    {
        [Header("[Need Enable All Gizmos Visibility in Scene Window to work in Editor Mode]")]
        [Tooltip("Number of columns in the grid (X axis)")]
        [SerializeField, Min(1)] private int columns = 3;
        
        [Tooltip("Number of rows in the grid (Y axis)")]
        [SerializeField, Min(1)] private int rows = 3;
        
        [Tooltip("Spacing between objects on X, Y and Z axis")]
        [SerializeField] private Vector3 spacing = Vector3.one;
        
        [Tooltip("Sort direction for arranging children")]
        [SerializeField] private SortDirection sortDirection = SortDirection.TopToBottom;
        
        [Tooltip("Alignment for incomplete rows (when row has fewer elements than columns)")]
        [SerializeField] private RowAlignment rowAlignment = RowAlignment.Center;
        
        [Tooltip("Center the grid around the parent's position")]
        [SerializeField] private bool centerGrid = true;
        
        [Tooltip("Automatically update layout when children are added/removed")]
        [SerializeField] private bool updateAtRuntime;
        
        [Header("Debug")]
        [Tooltip("Draw gizmos showing the grid layout")]
        [SerializeField] private bool showGizmos = true;
        
        [Tooltip("Color for grid gizmos")]
        [SerializeField] private Color gizmoColor = Color.yellow;
        
        [Tooltip("Number of preview elements to show in gizmos (0 = show all children)")]
        [SerializeField, Min(0)] private int previewElementCount;
        
        private List<Transform> _children = new List<Transform>();
        private int _lastChildCount = -1;
        
        
        
        private void Start()
        {
            UpdateLayout();
        }
        
        private void Update()
        {
            if (!updateAtRuntime) return;
            
            int currentChildCount = transform.childCount;
            if (currentChildCount != _lastChildCount)
            {
                UpdateLayout();
                _lastChildCount = currentChildCount;
            }
        }
        
        
        public void UpdateLayout()
        {
            if (!this.enabled) return;
            
            _children.Clear();
            
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (child.gameObject.activeSelf)
                    _children.Add(child);
            }
            
            if (_children.Count == 0) return;
            
            // Calculate actual grid dimensions based on number of children
            int totalElements = _children.Count;
            int elementsPerLayer = columns * rows;
            
            // Calculate how many rows and columns are actually used
            int actualColumns = Mathf.Min(columns, totalElements);
            int actualRows = Mathf.Min(rows, Mathf.CeilToInt((float)totalElements / columns));
            int totalLayers = Mathf.CeilToInt((float)totalElements / elementsPerLayer);
            
            // Adjust Y offset based on sort direction
            float yOffset = 0f;
            if (centerGrid)
            {
                if (sortDirection == SortDirection.TopToBottom)
                {
                    yOffset = -(actualRows - 1) * spacing.y * 0.5f;
                }
                else // BottomToTop
                {
                    yOffset = (actualRows - 1) * spacing.y * 0.5f;
                }
            }
            
            Vector3 offset = centerGrid ? new Vector3(
                -(actualColumns - 1) * spacing.x * 0.5f,
                yOffset,
                -(totalLayers - 1) * spacing.z * 0.5f
            ) : Vector3.zero;
            
            for (int i = 0; i < _children.Count; i++)
            {
                int layer = i / elementsPerLayer;
                int indexInLayer = i % elementsPerLayer;
                int row = indexInLayer / columns;
                int col = indexInLayer % columns;
                
                // Calculate how many elements are in this row
                int elementsInCurrentRow = Mathf.Min(columns, _children.Count - (layer * elementsPerLayer + row * columns));
                
                // Apply row alignment for incomplete rows
                float xPosition = col * spacing.x;
                if (elementsInCurrentRow < columns)
                {
                    float rowOffset = 0f;
                    switch (rowAlignment)
                    {
                        case RowAlignment.Left:
                            rowOffset = 0f;
                            break;
                        case RowAlignment.Center:
                            rowOffset = (columns - elementsInCurrentRow) * spacing.x * 0.5f;
                            break;
                        case RowAlignment.Right:
                            rowOffset = (columns - elementsInCurrentRow) * spacing.x;
                            break;
                    }
                    xPosition += rowOffset;
                }
                
                // Apply sort direction
                float yPosition = sortDirection == SortDirection.TopToBottom 
                    ? row * spacing.y 
                    : -row * spacing.y;
                
                Vector3 position = new Vector3(
                    xPosition,
                    yPosition,
                    layer * spacing.z
                ) + offset;
                
                _children[i].localPosition = position;
            }
        }
        
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            UpdateLayout();
            
            if (!showGizmos) return;
            
            Gizmos.color = gizmoColor;
            
            // Determine how many gizmos to draw
            int childCount = Application.isPlaying ? _children.Count : transform.childCount;
            int gizmoCount = previewElementCount > 0 ? previewElementCount : (childCount > 0 ? childCount : columns * rows);
            
            // Calculate offset based on gizmo count (same logic as UpdateLayout)
            int elementsPerLayer = columns * rows;
            int actualColumns = Mathf.Min(columns, gizmoCount);
            int actualRows = Mathf.Min(rows, Mathf.CeilToInt((float)gizmoCount / columns));
            int totalLayers = Mathf.CeilToInt((float)gizmoCount / elementsPerLayer);
            
            // Adjust Y offset based on sort direction
            float yOffset = 0f;
            if (centerGrid)
            {
                if (sortDirection == SortDirection.TopToBottom)
                {
                    yOffset = -(actualRows - 1) * spacing.y * 0.5f;
                }
                else // BottomToTop
                {
                    yOffset = (actualRows - 1) * spacing.y * 0.5f;
                }
            }
            
            Vector3 offset = centerGrid ? new Vector3(
                -(actualColumns - 1) * spacing.x * 0.5f,
                yOffset,
                -(totalLayers - 1) * spacing.z * 0.5f
            ) : Vector3.zero;
            
            for (int i = 0; i < gizmoCount; i++)
            {
                int layer = i / elementsPerLayer;
                int indexInLayer = i % elementsPerLayer;
                int row = indexInLayer / columns;
                int col = indexInLayer % columns;
                
                // Calculate how many elements are in this row
                int elementsInCurrentRow = Mathf.Min(columns, gizmoCount - (layer * elementsPerLayer + row * columns));
                
                // Apply row alignment for incomplete rows
                float xPosition = col * spacing.x;
                if (elementsInCurrentRow < columns)
                {
                    float rowOffset = 0f;
                    switch (rowAlignment)
                    {
                        case RowAlignment.Left:
                            rowOffset = 0f;
                            break;
                        case RowAlignment.Center:
                            rowOffset = (columns - elementsInCurrentRow) * spacing.x * 0.5f;
                            break;
                        case RowAlignment.Right:
                            rowOffset = (columns - elementsInCurrentRow) * spacing.x;
                            break;
                    }
                    xPosition += rowOffset;
                }
                
                // Apply sort direction
                float yPosition = sortDirection == SortDirection.TopToBottom 
                    ? row * spacing.y 
                    : -row * spacing.y;
                
                Vector3 position = new Vector3(
                    xPosition,
                    yPosition,
                    layer * spacing.z
                ) + offset;
                
                Vector3 worldPos = transform.TransformPoint(position);
                Gizmos.DrawWireCube(worldPos, Vector3.one * 0.2f);
            }
        }
#endif
    }
}