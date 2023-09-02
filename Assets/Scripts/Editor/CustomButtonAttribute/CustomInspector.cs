using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(Object), true)]
public class CustomInspector : UnityEditor.Editor
{
  private IEnumerable<MethodInfo> _methods = null;

  private void OnEnable()
  {
    _methods = ReflectionUtility.GetAllMethods(target,
        m => m.GetCustomAttributes(typeof(CustomButtonAttribute), true).Length > 0);
  }

  public override void OnInspectorGUI()
  {
    DrawDefaultInspector();
    DrawButtons();
  }

  private void DrawButtons()
  {
    if (_methods.Any() == false)
      return;

    EditorGUILayout.Space();

    foreach (MethodInfo method in _methods)
      CustomEditorGUI.Button(serializedObject.targetObject, method);
  }
}