using Morphey.Editor.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Morphey.Editor.Inspector
{
  [CanEditMultipleObjects]
  [CustomEditor(typeof(Object), true)]
  public class DebugButtonInspector : UnityEditor.Editor
  {
    private IEnumerable<MethodInfo> _methods = null;

    private void OnEnable()
    {
      _methods = ReflectionUtility.GetAllMethods(target,
          m => m.GetCustomAttributes(typeof(DebugButtonAttribute), true).Length > 0);
    }

    public override void OnInspectorGUI()
    {
      DrawDefaultInspector();
      DrawButtons();
    }

    private void DrawButtons()
    {
      if ( _methods.Any() == false )
        return;

      EditorGUILayout.Space();

      foreach ( MethodInfo method in _methods )
        DebugButtonGUI.Button(serializedObject.targetObject, method);
    }
  }
}