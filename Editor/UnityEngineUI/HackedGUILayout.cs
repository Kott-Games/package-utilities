namespace UnityEngine
{
    public class HackedGUILayout
    {
        public struct ScrollViewData
        {
            public Vector2 ScrollPosition;
            public Rect ViewRect;
            public Rect VisibleRect;
        }

        private static readonly int s_BoxHash;
        private static readonly int s_ButonHash;
        private static readonly int s_RepeatButtonHash;
        private static readonly int s_ToggleHash;
        private static readonly int s_ButtonGridHash;
        private static readonly int s_SliderHash;
        private static readonly int s_BeginGroupHash;
        private static readonly int s_ScrollviewHash;

        static HackedGUILayout()
        {
            s_BoxHash = "Box".GetHashCode();
            s_ButonHash = "Button".GetHashCode();
            s_RepeatButtonHash = "repeatButton".GetHashCode();
            s_ToggleHash = "Toggle".GetHashCode();
            s_ButtonGridHash = "ButtonGrid".GetHashCode();
            s_SliderHash = "Slider".GetHashCode();
            s_BeginGroupHash = "BeginGroup".GetHashCode();
            s_ScrollviewHash = "scrollView".GetHashCode();
        }

        public static ScrollViewData BeginScrollView(Vector2 scrollPosition,
            params GUILayoutOption[] options)
        {
            return BeginScrollView(scrollPosition,
                alwaysShowHorizontal: false, alwaysShowVertical: false,
                GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar, GUI.skin.scrollView,
                options);
        }

        internal static ScrollViewData BeginScrollView(Vector2 scrollPosition,
            bool alwaysShowHorizontal, bool alwaysShowVertical,
            GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, GUIStyle background,
            params GUILayoutOption[] options)
        {
            GUIUtility.CheckOnGUI();
            GUIScrollGroup gUIScrollGroup =
                (GUIScrollGroup)GUILayoutUtility.BeginLayoutGroup(background, null, typeof(GUIScrollGroup));
            EventType type = Event.current.type;
            EventType eventType = type;
            if (eventType == EventType.Layout)
            {
                gUIScrollGroup.resetCoords = true;
                gUIScrollGroup.isVertical = true;
                gUIScrollGroup.stretchWidth = 1;
                gUIScrollGroup.stretchHeight = 1;
                gUIScrollGroup.verticalScrollbar = verticalScrollbar;
                gUIScrollGroup.horizontalScrollbar = horizontalScrollbar;
                gUIScrollGroup.needsVerticalScrollbar = alwaysShowVertical;
                gUIScrollGroup.needsHorizontalScrollbar = alwaysShowHorizontal;
                gUIScrollGroup.ApplyOptions(options);
            }

            Rect viewRect = new Rect(0f, 0f, gUIScrollGroup.clientWidth, gUIScrollGroup.clientHeight);
            return BeginScrollView(gUIScrollGroup.rect, scrollPosition,
                viewRect, alwaysShowHorizontal,
                alwaysShowVertical, horizontalScrollbar, verticalScrollbar, background);
        }

        internal static ScrollViewData BeginScrollView(Rect position, Vector2 scrollPosition,
            Rect viewRect, bool alwaysShowHorizontal, bool alwaysShowVertical,
            GUIStyle horizontalScrollbar, GUIStyle verticalScrollbar, GUIStyle background)
        {
            GUIUtility.CheckOnGUI();
            if (Event.current.type == EventType.DragUpdated && position.Contains(Event.current.mousePosition))
            {
                if (Mathf.Abs(Event.current.mousePosition.y - position.y) < 8f)
                {
                    scrollPosition.y -= 16f;
                    GUI.InternalRepaintEditorWindow();
                }
                else if (Mathf.Abs(Event.current.mousePosition.y - position.yMax) < 8f)
                {
                    scrollPosition.y += 16f;
                    GUI.InternalRepaintEditorWindow();
                }
            }

            int controlID = GUIUtility.GetControlID(s_ScrollviewHash, FocusType.Passive);
            ScrollViewState scrollViewState =
                (ScrollViewState)GUIUtility.GetStateObject(typeof(ScrollViewState), controlID);
            if (scrollViewState.apply)
            {
                scrollPosition = scrollViewState.scrollPosition;
                scrollViewState.apply = false;
            }

            scrollViewState.position = position;
            scrollViewState.scrollPosition = scrollPosition;
            scrollViewState.visibleRect = (scrollViewState.viewRect = viewRect);
            scrollViewState.visibleRect.width = position.width;
            scrollViewState.visibleRect.height = position.height;
            GUI.scrollViewStates.Push(scrollViewState);
            Rect screenRect = new Rect(position);
            switch (Event.current.type)
            {
                case EventType.Layout:
                    GUIUtility.GetControlID(s_SliderHash, FocusType.Passive);
                    GUIUtility.GetControlID(s_RepeatButtonHash, FocusType.Passive);
                    GUIUtility.GetControlID(s_RepeatButtonHash, FocusType.Passive);
                    GUIUtility.GetControlID(s_SliderHash, FocusType.Passive);
                    GUIUtility.GetControlID(s_RepeatButtonHash, FocusType.Passive);
                    GUIUtility.GetControlID(s_RepeatButtonHash, FocusType.Passive);
                    break;
                default:
                {
                    bool flag = alwaysShowVertical;
                    bool flag2 = alwaysShowHorizontal;
                    if (flag2 || viewRect.width > screenRect.width)
                    {
                        scrollViewState.visibleRect.height = position.height - horizontalScrollbar.fixedHeight +
                                                             (float)horizontalScrollbar.margin.top;
                        screenRect.height -= horizontalScrollbar.fixedHeight + (float)horizontalScrollbar.margin.top;
                        flag2 = true;
                    }

                    if (flag || viewRect.height > screenRect.height)
                    {
                        scrollViewState.visibleRect.width = position.width - verticalScrollbar.fixedWidth +
                                                            (float)verticalScrollbar.margin.left;
                        screenRect.width -= verticalScrollbar.fixedWidth + (float)verticalScrollbar.margin.left;
                        flag = true;
                        if (!flag2 && viewRect.width > screenRect.width)
                        {
                            scrollViewState.visibleRect.height = position.height - horizontalScrollbar.fixedHeight +
                                                                 (float)horizontalScrollbar.margin.top;
                            screenRect.height -=
                                horizontalScrollbar.fixedHeight + (float)horizontalScrollbar.margin.top;
                            flag2 = true;
                        }
                    }

                    if (Event.current.type == EventType.Repaint && background != GUIStyle.none)
                    {
                        background.Draw(position, position.Contains(Event.current.mousePosition), isActive: false,
                            flag2 && flag, hasKeyboardFocus: false);
                    }

                    if (flag2 && horizontalScrollbar != GUIStyle.none)
                    {
                        scrollPosition.x = GUI.HorizontalScrollbar(
                            new Rect(position.x, position.yMax - horizontalScrollbar.fixedHeight, screenRect.width,
                                horizontalScrollbar.fixedHeight), scrollPosition.x,
                            Mathf.Min(screenRect.width, viewRect.width), 0f, viewRect.width, horizontalScrollbar);
                    }
                    else
                    {
                        GUIUtility.GetControlID(s_SliderHash, FocusType.Passive);
                        GUIUtility.GetControlID(s_RepeatButtonHash, FocusType.Passive);
                        GUIUtility.GetControlID(s_RepeatButtonHash, FocusType.Passive);
                        scrollPosition.x = ((horizontalScrollbar != GUIStyle.none)
                            ? 0f
                            : Mathf.Clamp(scrollPosition.x, 0f, Mathf.Max(viewRect.width - position.width, 0f)));
                    }

                    if (flag && verticalScrollbar != GUIStyle.none)
                    {
                        scrollPosition.y = GUI.VerticalScrollbar(
                            new Rect(screenRect.xMax + (float)verticalScrollbar.margin.left, screenRect.y,
                                verticalScrollbar.fixedWidth, screenRect.height), scrollPosition.y,
                            Mathf.Min(screenRect.height, viewRect.height), 0f, viewRect.height, verticalScrollbar);
                        break;
                    }

                    GUIUtility.GetControlID(s_SliderHash, FocusType.Passive);
                    GUIUtility.GetControlID(s_RepeatButtonHash, FocusType.Passive);
                    GUIUtility.GetControlID(s_RepeatButtonHash, FocusType.Passive);
                    scrollPosition.y = ((verticalScrollbar != GUIStyle.none)
                        ? 0f
                        : Mathf.Clamp(scrollPosition.y, 0f, Mathf.Max(viewRect.height - position.height, 0f)));
                    break;
                }
                case EventType.Used:
                    break;
            }

            Vector2 scrollOffset = new Vector2(Mathf.Round(0f - scrollPosition.x - viewRect.x),
                Mathf.Round(0f - scrollPosition.y - viewRect.y));
            GUIClip.Push(screenRect, scrollOffset, Vector2.zero, resetOffset: false);

            Rect visibleRect = scrollViewState.visibleRect;
            // visibleRect.position -= scrollOffset;

            return new ScrollViewData()
            {
                ScrollPosition = scrollPosition,
                ViewRect = viewRect,
                VisibleRect = visibleRect,
            };
        }
    }
}
