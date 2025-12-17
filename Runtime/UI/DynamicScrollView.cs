using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kott.Utilities.UI
{
    /// <summary>
    /// Provides an abstract base class for a scrollable view that displays a dynamic collection of fixed size elements,
    /// supporting efficient data loading and virtualization for large datasets.
    /// </summary>
    /// <typeparam name="TData">The type of data associated with each element in the scroll view.</typeparam>
    /// <typeparam name="TElement">The type of UI element used to represent each data item.</typeparam>
    [Serializable]
    public class DynamicScrollView<TData, TElement> : IView<IReadOnlyList<TData>>
        where TElement : Component, IView<TData>
    {
        private enum FlexMode
        {
            None,
            Stretch,
            Scale,
        }

        private enum FillMode
        {
            Fixed,
            MinSize,
            MaxSize,
        }

        [Header("References")]
        [SerializeField] private UpdatableScrollRect _scrollRect;
        [SerializeField] private TElement _elementPrefab;

        [Header("Settings")]
        [SerializeField] private GridLayoutGroup.Corner _startCorner = GridLayoutGroup.Corner.UpperLeft;
        [SerializeField] private GridLayoutGroup.Axis _startAxis = GridLayoutGroup.Axis.Horizontal;
        [SerializeField] protected Vector2 _cellSize = new(100f, 100f);
        [SerializeField] protected Vector2 _spacing = new(0f, 0f);
        [SerializeField] protected Vector4 _padding = new(0f, 0f, 0f, 0f);
        [SerializeField] private FlexMode _flexMode = FlexMode.None;
        [SerializeField] private FillMode _fillMode = FillMode.Fixed;
        [SerializeField] private int _fillCount = 3;
        [SerializeField] private float _viewPadding = 100f;

        private PrefabObjectPool<TElement> _elementPool;
        private List<TElement> _visibleElements = new();

        private Vector2 _contentVirtualSize;
        private Vector2 _elementSize;
        private Vector2Int _elementCount;

        private Vector2 _contentVirtualOffset;

        public IReadOnlyList<TData> Data { get; private set; }

        public void Init()
        {
            _elementPool = new(_elementPrefab, _scrollRect.content);
            _scrollRect.onValueChanged.AddListener(OnViewPositionChanged);
        }

        public void UpdateView(IReadOnlyList<TData> data)
        {
            Data = data;

            SetFocusPosition(0);
            _scrollRect.StartCoroutine(DelayUpdateRoutine());
        }

        private void OnViewPositionChanged(Vector2 viewPosition)
        {
            Update();
        }

        private IEnumerator DelayUpdateRoutine()
        {
            Update();
            yield return null;
            Update();
        }

        private void Update()
        {
            UpdateValues();
            UpdateContent();
            UpdateElements();
        }

        private void SetFocusPosition(int index)
        {
            // int axis = (int)_dynamicAxis;
            // float elementSize = _elementSize[axis];
            // float elementSizeSecondary = _elementSize[1 - axis];
            // float elementSpacing = _elementSpacing[axis];
            // float elementSpacingSecondary = _elementSpacing[1 - axis];
            // 
            // Vector2 viewportSizeBoth = TransformImaginaryRect(_scrollRect.viewport).size;
            // float viewportSize = viewportSizeBoth[axis];
            // float viewportSizeSecondary = viewportSizeBoth[1 - axis];
            // float contentPosition = _scrollRect.content.anchoredPosition[axis];
            // 
            // int secondaryCount = Mathf.FloorToInt(
            //     (viewportSizeSecondary + elementSpacingSecondary) /
            //     (elementSizeSecondary + elementSpacingSecondary));
            // int primaryCount = (Data.Count + secondaryCount - 1) / secondaryCount;
            // 
            // int indexPrimary = index / secondaryCount;
            // 
            // _contentVirtualOffset = indexPrimary * (elementSize + elementSpacing) - contentPosition;
        }

        private void UpdateValues()
        {
            Vector2 viewportSize = _scrollRect.viewport.rect.size;

            int axisStatic = (int)_startAxis;
            int axisDynamic = 1 - axisStatic;

            float viewportSizeStatic = viewportSize[axisStatic];
            float cellSizeStatic = _cellSize[axisStatic];
            float spacingStatic = _spacing[axisStatic];

            viewportSizeStatic -= _padding[axisStatic] + _padding[axisStatic + 2];

            int countStatic = _fillCount;
            if (_fillMode != FillMode.Fixed)
            {
                float countFloat = (viewportSizeStatic + spacingStatic) / (cellSizeStatic + spacingStatic);
                countStatic = _fillMode == FillMode.MaxSize && _flexMode != FlexMode.None ?
                    Mathf.FloorToInt(countFloat) :
                    Mathf.CeilToInt(countFloat);
                countStatic = Mathf.Max(countStatic, 1);
            }

            int countDynamic = Mathf.CeilToInt((float)Data.Count / countStatic);

            _elementCount[axisStatic] = countStatic;
            _elementCount[axisDynamic] = countDynamic;

            _elementSize = _cellSize;
            if (_flexMode != FlexMode.None)
            {
                float cellSizeStaticExpanded = (viewportSizeStatic + spacingStatic) / countStatic - spacingStatic;
                _elementSize[axisStatic] = cellSizeStaticExpanded;

                if (_flexMode == FlexMode.Scale)
                {
                    float scale = cellSizeStaticExpanded / _cellSize[axisStatic];
                    _elementSize[axisDynamic] *= scale;
                }
            }

            _contentVirtualSize = _elementCount * (_elementSize + _spacing) - _spacing;
            _contentVirtualSize[axisStatic] += _padding[axisStatic] + _padding[axisStatic + 2];
            _contentVirtualSize[axisDynamic] += _padding[axisDynamic] + _padding[axisDynamic + 2];
        }

        private void UpdateContent()
        {
            UpdateAnchors(_scrollRect.content);

            Vector2 viewportSize = _scrollRect.viewport.rect.size;
            Vector2 contentPosition = _scrollRect.content.anchoredPosition;

            Vector2 minContentPosition = RemapVector(-viewportSize);
            Vector2 maxContentPosition = 2f * viewportSize;

            Vector2 minVirtualOffset = new(0f, 0f);
            Vector2 contentPositionOffsetView = RemapVector(minContentPosition - contentPosition);
            Vector2 contentPositionOffsetVirtual = RemapVector(_contentVirtualOffset - minVirtualOffset);
            Vector2 contentPositionOffset = Vector2.Max(contentPositionOffsetView, contentPositionOffsetVirtual);
            contentPositionOffset = RemapVector(contentPositionOffset);

            contentPosition += contentPositionOffset;
            _contentVirtualOffset -= contentPositionOffset;
            _scrollRect.OffsetContentPosition(contentPositionOffset);

            Vector2 maxVirtualSize = _contentVirtualSize;
            Vector2 contentSizeView = maxContentPosition - RemapVector(contentPosition);
            Vector2 contentSizeVirtual = RemapVector(_contentVirtualOffset) + maxVirtualSize;
            Vector2 contentSize = Vector2.Min(contentSizeView, contentSizeVirtual);

            _scrollRect.SetContentSize(contentSize);
        }

        private void UpdateElements()
        {
            Vector2 viewportSize = _scrollRect.viewport.rect.size;
            Vector2 contentPosition = _scrollRect.content.anchoredPosition;

            Vector2 viewportMin = -_viewPadding * Vector2.one;
            Vector2 viewportMax = viewportSize + _viewPadding * Vector2.one;

            Vector2 stepSize = _elementSize + _spacing;
            Vector2 virtualContentPosition = RemapVector(contentPosition + _contentVirtualOffset);

            Vector2Int minIndex = Vector2Int.FloorToInt((viewportMin - virtualContentPosition) / stepSize);
            Vector2Int maxIndex = Vector2Int.FloorToInt((viewportMax - virtualContentPosition) / stepSize);

            // clamping
            minIndex = Vector2Int.Max(Vector2Int.Min(minIndex, _elementCount - Vector2Int.one), Vector2Int.zero);
            maxIndex = Vector2Int.Max(Vector2Int.Min(maxIndex, _elementCount - Vector2Int.one), Vector2Int.zero);

            _elementPool.ReleaseActive();
            _visibleElements.Clear();

            int axisStatic = (int)_startAxis;
            int axisDynamic = 1 - axisStatic;

            for (int indexDynamic = minIndex[axisDynamic]; indexDynamic <= maxIndex[axisDynamic]; indexDynamic++)
            {
                for (int indexStatic = minIndex[axisStatic]; indexStatic <= maxIndex[axisStatic]; indexStatic++)
                {
                    int index = indexDynamic * _elementCount[axisStatic] + indexStatic;
                    if (index < 0 || index >= Data.Count) continue;

                    TElement element = _elementPool.Get();
                    element.UpdateView(Data[index]);

                    RectTransform rectTransform = element.GetComponent<RectTransform>();
                    UpdateAnchors(rectTransform);

                    Vector2 indexVector = new(0f, 0f);
                    indexVector[axisStatic] = indexStatic;
                    indexVector[axisDynamic] = indexDynamic;

                    Vector2 elementPosition = indexVector * stepSize;
                    elementPosition.x += _padding.x;
                    elementPosition.y += _padding.y;
                    elementPosition = RemapVector(elementPosition);
                    elementPosition += _contentVirtualOffset;

                    rectTransform.sizeDelta = _elementSize;
                    rectTransform.anchoredPosition = elementPosition;
                }
            }
        }

        private void UpdateAnchors(RectTransform rectTransform)
        {
            switch (_startCorner)
            {
                case GridLayoutGroup.Corner.UpperLeft:
                    rectTransform.anchorMin = new(0f, 1f);
                    rectTransform.anchorMax = new(0f, 1f);
                    rectTransform.pivot = new(0f, 1f);
                    break;
                case GridLayoutGroup.Corner.UpperRight:
                    rectTransform.anchorMin = new(1f, 1f);
                    rectTransform.anchorMax = new(1f, 1f);
                    rectTransform.pivot = new(1f, 1f);
                    break;
                case GridLayoutGroup.Corner.LowerLeft:
                    rectTransform.anchorMin = new(0f, 0f);
                    rectTransform.anchorMax = new(0f, 0f);
                    rectTransform.pivot = new(0f, 0f);
                    break;
                case GridLayoutGroup.Corner.LowerRight:
                    rectTransform.anchorMin = new(1f, 0f);
                    rectTransform.anchorMax = new(1f, 0f);
                    rectTransform.pivot = new(1f, 0f);
                    break;
            }
        }

        private Vector2 RemapVector(Vector2 position)
        {
            switch (_startCorner)
            {
                case GridLayoutGroup.Corner.UpperLeft:
                    position.y = -position.y;
                    break;
                case GridLayoutGroup.Corner.UpperRight:
                    position.x = -position.x;
                    position.y = -position.y;
                    break;
                case GridLayoutGroup.Corner.LowerLeft:
                    // no remap
                    break;
                case GridLayoutGroup.Corner.LowerRight:
                    position.x = -position.x;
                    break;
            }

            return position;
        }
    }
}