using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;


public static class CustomEditorGUI
{
  public static void Button(Object target, MethodInfo methodInfo)
  {
    if (!methodInfo.GetParameters().All(p => p.IsOptional))
    {
      const string warning = nameof(CustomButtonAttribute) + " works only on methods with no parameters.";
      ShowWarningHelpBox(warning, MessageType.Warning, context: target, logToConsole: true);
    }

    var buttonAttribute =
        (CustomButtonAttribute)methodInfo.GetCustomAttribute(typeof(CustomButtonAttribute), true);
    string buttonText = string.IsNullOrEmpty(buttonAttribute.Text)
        ? ObjectNames.NicifyVariableName(methodInfo.Name)
        : buttonAttribute.Text;

    var buttonEnabled = true;
    CustomButtonAttribute.ButtonEnableMode mode = buttonAttribute.EnableMode;

    buttonEnabled &=
        mode == CustomButtonAttribute.ButtonEnableMode.Always ||
        mode == CustomButtonAttribute.ButtonEnableMode.Play && Application.isPlaying ||
        mode == CustomButtonAttribute.ButtonEnableMode.Editor && !Application.isPlaying;

    bool methodIsCoroutine = methodInfo.ReturnType == typeof(IEnumerator);

    if (methodIsCoroutine)
      buttonEnabled &= Application.isPlaying ? true : false;

    EditorGUI.BeginDisabledGroup(!buttonEnabled);

    if (GUILayout.Button(buttonText))
    {
      object[] defaultParams = methodInfo.GetParameters().Select(p => p.DefaultValue).ToArray();

      if (!Application.isPlaying)
      {
        EditorUtility.SetDirty(target);
        PrefabStage stage = PrefabStageUtility.GetCurrentPrefabStage();

        if (stage != null)
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
      else if (methodInfo.Invoke(target, defaultParams) is IEnumerator methodResult &&
               target is MonoBehaviour behaviour)
      {
        behaviour.StartCoroutine(methodResult);
      }
    }

    EditorGUI.EndDisabledGroup();
  }

  public static void ShowWarningHelpBox(string message, MessageType type, Object context = null,
      bool logToConsole = false)
  {
    EditorGUILayout.HelpBox(message, type);

    if (logToConsole)
      Debug.LogWarning(message, context);
  }

  public static GUIStyle GetLabelStyle()
  {
    var style = new GUIStyle(EditorStyles.label)
    {
      fontStyle = FontStyle.Bold,
      alignment = TextAnchor.UpperLeft
    };

    return style;
  }

  public static GUIStyle GetHeaderStyle()
  {
    var style = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
    {
      fontStyle = FontStyle.Bold,
      alignment = TextAnchor.UpperCenter
    };

    return style;
  }

  public static GUIStyle GetTitleStyle(int fontSize, int offset)
  {
    var style = new GUIStyle(EditorStyles.label)
    {
      fontStyle = FontStyle.Bold,
      alignment = TextAnchor.MiddleCenter,
      margin = new RectOffset(0, 0, offset, offset / 2),
      fontSize = fontSize
    };

    return style;
  }

  public static GUIStyle GetButtonStyle(Color textColor = default)
  {
    Color fixedColor = textColor == default ? Color.white : textColor;

    var button = new GUIStyle(EditorStyles.label)
    {
      alignment = TextAnchor.MiddleCenter,
      margin = new RectOffset(0, 0, 12, 6),
      fontSize = 12,
      normal = { textColor = fixedColor }
    };

    return button;
  }

  public static GUIStyle GetDropDownButtonStyle()
  {
    var button = new GUIStyle(EditorStyles.toolbarPopup)
    {
      alignment = TextAnchor.MiddleCenter,
      fontSize = 12
    };

    return button;
  }

  public static void TitleH1(string text)
  {
    GUILayout.Label(text, GetTitleStyle(16, 12));
  }

  public static void TitleH2(string text)
  {
    GUILayout.Label(text, GetTitleStyle(12, 6));
  }

  public static Texture2D CreateTexture(Color color)
  {
    var texture = new Texture2D(1, 1);
    texture.SetPixel(1, 1, color);
    texture.Apply();

    return texture;
  }

  public static string TextField(string label, string text)
  {
    Vector2 textDimensions = GUI.skin.label.CalcSize(new GUIContent(label));
    EditorGUIUtility.labelWidth = textDimensions.x;
    return EditorGUILayout.TextField(label, text);
  }

  public static int Popup(string label, ref int index, ref string[] content)
  {
    Vector2 textDimensions = GUI.skin.label.CalcSize(new GUIContent(label));
    EditorGUIUtility.labelWidth = textDimensions.x;
    return EditorGUILayout.Popup(label, index, content);
  }
}
