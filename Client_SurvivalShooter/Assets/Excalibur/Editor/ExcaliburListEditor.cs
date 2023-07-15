using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Excalibur
{
    [CustomEditor(typeof(ExcaliburList))]
    public class ExcaliburListEditor : Editor
    {
        private SerializedProperty
            prefab, listType,
            scrolllAxis, startCorner, scrollType,
            padding, spacing,
            defaultSelected, isLoop, allowMultiSelect, fixedRow, fixedColumn, rowCount, columnCount,
            exceedOffsetFactor, dampTime, endDragDampFactor, elasticFactor, scrollAccelerationSpeed, scrollMaxSpeed,
            horizontalBar, verticalBar;

        private ExcaliburList _list;
        private Color originColor;
        private ListType selectedlistType;
        private ScrolllAxis selectedAxis;
        private const float
            leftExceedOffsetFactor = 0.1f, rightExceedOffsetFactor = 1.5f,
            leftDampTime = 0.1f, rightDampTime = 1.5f,
            leftElasticFactor = 0.01f, rightElasticFactor = 1f,
            leftEndDragDampFactor = 0.01f, rightEndDragDampFactor = 1f,
            leftScrollAccelerationSpeed = 0.01f, rightScrollAccelerationSpeed = 1f,
            leftScrollMaxSpeed = 1f, rightScrollMaxSpeed = 20f;

        private void OnEnable()
        {
            _list = target as ExcaliburList;

            prefab = serializedObject.FindProperty("_prefab");
            listType = serializedObject.FindProperty("_params.listType");
            scrolllAxis = serializedObject.FindProperty("_params.scrolllAxis");
            startCorner = serializedObject.FindProperty("_params.startCorner");
            scrollType = serializedObject.FindProperty("_params.scrollType");
            padding = serializedObject.FindProperty("_params.padding");
            spacing = serializedObject.FindProperty("_params.spacing");
            defaultSelected = serializedObject.FindProperty("_params.defaultSelected");
            isLoop = serializedObject.FindProperty("_params.isLoop");
            allowMultiSelect = serializedObject.FindProperty("_params.allowMultiSelect");
            fixedRow = serializedObject.FindProperty("_params.fixedRow");
            fixedColumn = serializedObject.FindProperty("_params.fixedColumn");
            rowCount = serializedObject.FindProperty("_params.rowCount");
            columnCount = serializedObject.FindProperty("_params.columnCount");
            exceedOffsetFactor = serializedObject.FindProperty("_params.exceedOffsetFactor");
            dampTime = serializedObject.FindProperty("_params.dampTime");
            endDragDampFactor = serializedObject.FindProperty("_params.endDragDampFactor");
            elasticFactor = serializedObject.FindProperty("_params.elasticFactor");
            scrollAccelerationSpeed = serializedObject.FindProperty("_params.scrollAccelerationSpeed");
            scrollMaxSpeed = serializedObject.FindProperty("_params.scrollMaxSpeed");
            horizontalBar = serializedObject.FindProperty("_horizontalBar");
            verticalBar = serializedObject.FindProperty("_verticalBar");

            originColor = GUI.backgroundColor;

            _list.AttachChilds();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.BeginVertical();
            if (!EditorApplication.isPlaying)
            {
                EditorGUILayout.PropertyField(prefab);
                EditorGUILayout.PropertyField(listType);
                selectedlistType = (ListType)listType.enumValueIndex;
                EditorGUILayout.PropertyField(scrollType);
                if ((ScrollType)scrollType.enumValueIndex != ScrollType.Nothing)
                {
                    EditorGUILayout.PropertyField(scrolllAxis);
                    EditorGUILayout.PropertyField(defaultSelected);
                    if (selectedlistType != ListType.Content)
                    {
                        EditorGUILayout.PropertyField(startCorner);
                        EditorGUILayout.PropertyField(padding);
                        EditorGUILayout.PropertyField(spacing);
                        EditorGUILayout.PropertyField(isLoop);
                        EditorGUILayout.PropertyField(allowMultiSelect);
                        selectedAxis = (ScrolllAxis)scrolllAxis.enumValueIndex;
                        switch (selectedAxis)
                        {
                            case ScrolllAxis.Horizontal:
                                fixedColumn.boolValue = false;
                                EditorGUILayout.PropertyField(fixedRow);
                                if (fixedRow.boolValue)
                                {
                                    EditorGUILayout.PropertyField(rowCount);
                                    rowCount.intValue = Mathf.Max(rowCount.intValue, 1);
                                }
                                break;
                            case ScrolllAxis.Vertical:
                                fixedRow.boolValue = false;
                                EditorGUILayout.PropertyField(fixedColumn);
                                if (fixedColumn.boolValue)
                                {
                                    EditorGUILayout.PropertyField(columnCount);
                                    columnCount.intValue = Mathf.Max(columnCount.intValue, 1);
                                }
                                break;
                            case ScrolllAxis.Arbitrary:
                                {
                                    if ((ListType)listType.enumValueIndex != ListType.Content)
                                    {
                                        scrolllAxis.enumValueIndex = (int)ScrolllAxis.Vertical;
                                        Debug.LogError("бɻ");
                                    }
                                }
                                break;
                        }
                    }
                    else
                    {
                        if (isLoop.boolValue) { isLoop.boolValue = false; }
                    }
                }
            }
            
            exceedOffsetFactor.floatValue =
                EditorGUILayout.Slider("ExceedFactor", exceedOffsetFactor.floatValue, leftExceedOffsetFactor, rightExceedOffsetFactor);
            dampTime.floatValue =
                EditorGUILayout.Slider("DampTime", dampTime.floatValue, leftDampTime, rightDampTime);
            endDragDampFactor.floatValue =
                EditorGUILayout.Slider("EndDragDampFactor", endDragDampFactor.floatValue, leftEndDragDampFactor, rightEndDragDampFactor);
            elasticFactor.floatValue =
                EditorGUILayout.Slider("ElasticFactor", elasticFactor.floatValue, leftElasticFactor, rightElasticFactor);
            scrollAccelerationSpeed.floatValue =
                EditorGUILayout.Slider("ScrollAccelerationSpeed", scrollAccelerationSpeed.floatValue, leftScrollAccelerationSpeed, rightScrollAccelerationSpeed);
            scrollMaxSpeed.floatValue =
                EditorGUILayout.Slider("ScrollMaxSpeed", scrollMaxSpeed.floatValue, leftScrollMaxSpeed, rightScrollMaxSpeed);

            if (scrollType.enumValueIndex != (int)ScrollType.Nothing) // !EditorApplication.isPlaying && 
            {
                switch (selectedAxis)
                {
                    case ScrolllAxis.Horizontal:
                        EditorGUILayout.PropertyField(horizontalBar);
                        if (verticalBar.objectReferenceValue) { verticalBar.objectReferenceValue = null; }
                        break;
                    case ScrolllAxis.Vertical:
                        EditorGUILayout.PropertyField(verticalBar);
                        if (horizontalBar.objectReferenceValue) { horizontalBar.objectReferenceValue = null; }
                        break;
                    case ScrolllAxis.Arbitrary:
                        EditorGUILayout.PropertyField(horizontalBar);
                        EditorGUILayout.PropertyField(verticalBar);
                        break;
                }
            }
            else
            {
                if (horizontalBar.objectReferenceValue) { horizontalBar.objectReferenceValue = null; }
                if (verticalBar.objectReferenceValue) { verticalBar.objectReferenceValue = null; }
            }

            if (!EditorApplication.isPlaying && 
                selectedlistType != ListType.Content && selectedAxis != ScrolllAxis.Arbitrary && prefab.objectReferenceValue != null)
            {
                GUI.backgroundColor = Color.green;
                if (GUILayout.Button("Create Items")) { _list.CreateItems(); }
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("Destroy Children")) { _list.DestroyItemsOnEdit(); }
            }
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();

            if (!EditorApplication.isPlaying) { _list.LayoutItems(); }

            if (GUI.backgroundColor != originColor) { GUI.backgroundColor = originColor; }
        }
    }
}
