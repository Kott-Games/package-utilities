using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kott.Utilities
{
    [CustomPropertyDrawer(typeof(GenericListAttribute))]
    public class GenericListDrawer : PropertyDrawer
    {
        private SerializedProperty _property;
        private List<SerializedProperty> _items;
        private Type _itemType;

        public VisualTreeAsset m_InspectorUXML;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            if (!property.isArray)
                return base.CreatePropertyGUI(property);

            _property = property;

            Type fieldType = fieldInfo.FieldType;
            _itemType = fieldType.IsArray ? fieldType.GetElementType() : fieldType.GenericTypeArguments[0];

            VisualElement root = new VisualElement();

            _items = new List<SerializedProperty>();
            for (int i = 0; i < property.arraySize; i++)
                _items.Add(property.GetArrayElementAtIndex(i));

            ListView listView = new ListView
            {
                showAddRemoveFooter = true,
                showBorder = true,
                showFoldoutHeader = false,
                reorderable = true,
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
                reorderMode = ListViewReorderMode.Simple,
                bindingPath = property.propertyPath,
                overridingAddButtonBehavior = OnAddItem,
                makeItem = OnMakeItem,
                bindItem = OnBindItem,
            };
            root.Add(listView);

            root.Bind(property.serializedObject);

            return root;
        }

        private VisualElement OnMakeItem()
        {
            VisualElement root = new VisualElement();

            Label label = new Label() { name = "header" };
            root.Add(label);

            VisualElement items = new VisualElement() { name = "items" };
            root.Add(items);

            return m_InspectorUXML.CloneTree();
        }

        private void OnBindItem(VisualElement element, int index)
        {
            SerializedProperty itemProperty = _property.GetArrayElementAtIndex(index);

            Label header = element.Q<Label>("header");
            Type itemType = itemProperty.managedReferenceValue.GetType();
            header.text = itemType.Name;

            VisualElement items = element.Q<VisualElement>("items");
            items.Clear();

            SerializedProperty iter = itemProperty.Copy();
            int depth = iter.depth;
            iter.NextVisible(true);
            while (iter.depth > depth)
            {
                PropertyField propertyField = new PropertyField();
                propertyField.BindProperty(iter.Copy());
                items.Add(propertyField);

                if (!iter.NextVisible(false))
                    break;
            }
        }

        private void OnAddItem(BaseListView view, Button button)
        {
            var menu = new GenericMenu();
            var types = TypeCache.GetTypesDerivedFrom(_itemType);
            foreach (var type in types)
            {
                if (type.IsAbstract)
                    continue;

                menu.AddItem(new GUIContent(type.Name), false, () =>
                {
                    _property.arraySize++;
                    SerializedProperty newItem = _property.GetArrayElementAtIndex(_property.arraySize - 1);
                    newItem.managedReferenceValue = Activator.CreateInstance(type);
                    _property.serializedObject.ApplyModifiedProperties();
                });
            }

            menu.DropDown(button.worldBound);
        }
    }
}
