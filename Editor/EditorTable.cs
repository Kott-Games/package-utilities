using System;
using System.Collections.Generic;
using UnityEngine;

namespace ToG.Utility
{
    public class EditorTableColumn
    {
        public float Width;
    }

    public class EditorTableRow
    {
        public bool DrawOnTop;
        public Vector2 PositionOffset;
    }

    [Serializable]
    public class EditorTable<TColumn, TRow>
        where TColumn : EditorTableColumn
        where TRow : EditorTableRow
    {
        private delegate void InternalDrawDelegate(Rect position, int ci);

        public delegate void DrawHeaderDelegate(Rect position, TColumn column);
        public delegate void DrawCellDelegate(Rect position, TColumn column, TRow row);

        private List<TColumn> _columns = new();
        private List<TRow> _rows = new();

        private Vector2 _scrollPosition;
        private int _visibleCellsXMin;
        private int _visibleCellsXMax;
        private int _visibleCellsYMin;
        private int _visibleCellsYMax;

        private DrawHeaderDelegate _drawHeader;
        private DrawCellDelegate _drawCell;

        [SerializeField] public float HeaderHeight = 24f;
        [SerializeField] public float RowHeight = 18f;

        [SerializeField] public float ColumnSpacing = 3f;
        [SerializeField] public float RowSpacing = 3f;

        [SerializeField] public bool LockHeader = true;
        [SerializeField] public bool LockFirstColumn = false;

        [SerializeField] public bool CullCells = true;

        public IList<TColumn> Columns => _columns;
        public IList<TRow> Rows => _rows;

        public void Setup(DrawHeaderDelegate drawHeader, DrawCellDelegate drawCell)
        {
            _drawHeader = drawHeader;
            _drawCell = drawCell;
        }

        public void DrawGUILayoutTable()
        {
            Vector2 tableSize = Vector2.zero;
            for (int ci = 0; ci < _columns.Count; ci++) tableSize.x += _columns[ci].Width;
            tableSize.x += ColumnSpacing * (_columns.Count - 1);
            tableSize.y = HeaderHeight + (RowHeight + RowSpacing) * _rows.Count;

            HackedGUILayout.ScrollViewData data = HackedGUILayout.BeginScrollView(
                _scrollPosition, GUILayout.ExpandHeight(false));
            _scrollPosition = data.ScrollPosition;

            Rect cullRect = data.VisibleRect;
            cullRect.position += _scrollPosition;

            if (LockHeader) cullRect = cullRect.AddPadding(0f, HeaderHeight + RowSpacing, 0f, 0f);
            if (LockFirstColumn) cullRect = cullRect.AddPadding(_columns[0].Width + RowSpacing, 0f, 0f, 0f);

            _visibleCellsXMin = _columns.Count;
            _visibleCellsXMax = -1;
            _visibleCellsYMin = _rows.Count;
            _visibleCellsYMax = -1;
            float xPositionMin = 0.0f;
            for (int ci = 0; ci < _columns.Count; ci++)
            {
                float xPositionMax = xPositionMin + _columns[ci].Width;
                if (xPositionMin <= cullRect.xMax && xPositionMax >= cullRect.xMin)
                {
                    _visibleCellsXMin = Mathf.Min(_visibleCellsXMin, ci);
                    _visibleCellsXMax = Mathf.Max(_visibleCellsXMax, ci);
                }
                xPositionMin = xPositionMax + ColumnSpacing;
            }
            float yPositionMin = HeaderHeight + RowSpacing;
            for (int ri = 0; ri < _rows.Count; ri++)
            {
                float yPositionMax = yPositionMin + RowHeight;
                if (yPositionMin <= cullRect.yMax && yPositionMax >= cullRect.yMin)
                {
                    _visibleCellsYMin = Mathf.Min(_visibleCellsYMin, ri);
                    _visibleCellsYMax = Mathf.Max(_visibleCellsYMax, ri);
                }
                yPositionMin = yPositionMax + RowSpacing;
            }
            bool isHeaderRowVisible = 0f < cullRect.yMax && HeaderHeight > cullRect.yMin;

            Rect tableRect = GUILayoutUtility.GetRect(tableSize.x, tableSize.y);

            Rect headerRowRect = new Rect(tableRect);
            headerRowRect.height = HeaderHeight;
            if (LockHeader) headerRowRect.y += _scrollPosition.y;

            if (LockHeader || !CullCells || isHeaderRowVisible)
                DrawRow(headerRowRect, HeaderHeight, (cellRect, ci) => _drawHeader(cellRect, _columns[ci]));

            if (LockHeader)
            {
                Rect bodyRect = new Rect(tableRect);
                bodyRect.height -= HeaderHeight + RowSpacing;
                bodyRect.y += HeaderHeight + RowSpacing;

                Rect bodyClipRect = new Rect(bodyRect);
                bodyClipRect.x += _scrollPosition.x;
                bodyClipRect.y += _scrollPosition.y;

                GUI.BeginClip(bodyClipRect);

                Rect rowRectIterator = new Rect(
                    -_scrollPosition.x, -_scrollPosition.y, tableRect.width, RowHeight);
                DrawRows(rowRectIterator);

                GUI.EndClip();
            }
            else
            {
                Rect rowRectIterator = new Rect(
                    tableRect.x, tableRect.y + HeaderHeight + RowSpacing, tableRect.width, RowHeight);
                DrawRows(rowRectIterator);
            }

            GUILayout.EndScrollView();
        }

        private void DrawRows(Rect rowRectIterator)
        {
            Rect rowRectIteratorStart = new Rect(rowRectIterator);
            int drawOnTopCount = _rows.Count;

            for (int ri = 0; ri < _rows.Count; ri++)
            {
                if (!_rows[ri].DrawOnTop &&
                    (!CullCells || ri >= _visibleCellsYMin && ri <= _visibleCellsYMax))
                {
                    Rect rowRect = new Rect(rowRectIterator);
                    rowRect.position += _rows[ri].PositionOffset;
                    DrawRow(rowRect, RowHeight, (cellRect, ci) => _drawCell(cellRect, _columns[ci], _rows[ri]));
                    drawOnTopCount--;
                }
                rowRectIterator.y += RowHeight + RowSpacing;
            }

            if (drawOnTopCount > 0)
            {
                rowRectIterator = rowRectIteratorStart;
                for (int ri = 0; ri < _rows.Count; ri++)
                {
                    if (_rows[ri].DrawOnTop &&
                        (!CullCells || ri >= _visibleCellsYMin && ri <= _visibleCellsYMax))
                    {
                        Rect rowRect = new Rect(rowRectIterator);
                        rowRect.position += _rows[ri].PositionOffset;
                        DrawRow(rowRect, RowHeight, (cellRect, ci) => _drawCell(cellRect, _columns[ci], _rows[ri]));
                    }
                    rowRectIterator.y += RowHeight + RowSpacing;
                }
            }
        }

        private void DrawRow(Rect rowRect, float rowHeight, InternalDrawDelegate draw)
        {
            if (LockFirstColumn)
            {
                Rect firstCellRect = new Rect(rowRect);
                firstCellRect.width = _columns[0].Width;
                firstCellRect.x += _scrollPosition.x;

                draw(firstCellRect, 0);

                Rect otherCellsRect = new Rect(rowRect);
                otherCellsRect.width -= firstCellRect.width + ColumnSpacing;
                otherCellsRect.x += firstCellRect.width + ColumnSpacing;

                Rect otherCellsClipRect = new Rect(otherCellsRect);
                otherCellsClipRect.x += _scrollPosition.x;

                GUI.BeginClip(otherCellsClipRect);

                Rect cellRectIterator = new Rect(-_scrollPosition.x, 0f, 0f, rowHeight);
                DrawCells(cellRectIterator, 1, draw);

                GUI.EndClip();
            }
            else
            {
                Rect cellRectIterator = new Rect(rowRect);
                DrawCells(cellRectIterator, 0, draw);
            }
        }

        private void DrawCells(Rect cellRectIterator, int startIndex, InternalDrawDelegate draw)
        {
            for (int ci = startIndex; ci < _columns.Count; ci++)
            {
                Rect cellRect = new Rect(cellRectIterator);
                cellRect.width = _columns[ci].Width;
                if (!CullCells || ci >= _visibleCellsXMin && ci <= _visibleCellsXMax)
                    draw(cellRect, ci);
                cellRectIterator.x += cellRect.width + ColumnSpacing;
            }
        }
    }
}
