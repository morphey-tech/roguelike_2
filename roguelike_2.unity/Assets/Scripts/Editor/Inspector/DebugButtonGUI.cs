using Morphey.Editor.Attributes;
using UnityEditor.SceneManagement;
using System.Collections;
using System.Reflection;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Morphey.Editor.Inspector
{
  public static class DebugButtonGUI
  {
    public static void Button(Object target, MethodInfo methodInfo)
    {
      if ( !methodInfo.GetParameters().All(p => p.IsOptional) )
      {
        const string warning = nameof(DebugButtonAttribute) + " works only on methods with no parameters.";
        ShowWarningHelpBox(warning, MessageType.Warning, context: target, logToConsole: true);
      }

      var buttonAttribute = (DebugButtonAttribute)methodInfo.GetCustomAttribute(typeof(DebugButtonAttribute), true);
      string buttonText = string.IsNullOrEmpty(buttonAttribute.Text)
          ? ObjectNames.NicifyVariableName(methodInfo.Name)
          : buttonAttribute.Text;

      var buttonEnabled = true;
      DebugButtonAttribute.ButtonEnableMode mode = buttonAttribute.EnableMode;

      buttonEnabled &=
          mode == DebugButtonAttribute.ButtonEnableMode.Always ||
          mode == DebugButtonAttribute.ButtonEnableMode.Play && Application.isPlaying ||
          mode == DebugButtonAttribute.ButtonEnableMode.Editor && Application.isPlaying == false;

      bool methodIsCoroutine = methodInfo.ReturnType == typeof(IEnumerator);

      if ( methodIsCoroutine )
        buttonEnabled &= Application.isPlaying;

      EditorGUI.BeginDisabledGroup(!buttonEnabled);

      if ( GUILayout.Button(buttonText) )
      {
        object [] defaultParams = methodInfo.GetParameters().Select(p => p.DefaultValue).ToArray();

        if ( Application.isPlaying == false )
        {
          EditorUtility.SetDirty(target);
          PrefabStage stage = PrefabStageUtility.GetCurrentPrefabStage();

          if ( stage != null )
          {
            methodInfo.Invoke(target, defaultParams);
            EditorSceneManager.MarkSceneDirty(stage.scene);
          }
          else
          {
            methodInfo.Invoke(target, defaultParams);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
          }
        }
        else if ( methodInfo.Invoke(target, defaultParams) is IEnumerator methodResult &&
                 target is MonoBehaviour behaviour )
        {
          behaviour.StartCoroutine(methodResult);
        }
      }

      EditorGUI.EndDisabledGroup();
    }

    private static void ShowWarningHelpBox(string message, MessageType type, Object context = null, bool logToConsole = false)
    {
      EditorGUILayout.HelpBox(message, type);

      if ( logToConsole )
        Debug.LogWarning(message, context);
    }
  }
}