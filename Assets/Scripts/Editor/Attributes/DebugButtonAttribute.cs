using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using System;

namespace Morphey.Editor.Attributes
{
  [PublicAPI]
  [AttributeUsage(AttributeTargets.Method)]
  [System.Diagnostics.Conditional("UNITY_EDITOR")]
  public class DebugButtonAttribute : Attribute
  {
    public string Text { get; private set; }
    public ButtonEnableMode EnableMode { get; private set; }

    public DebugButtonAttribute([CallerMemberName] string text = null, ButtonEnableMode enableMode = ButtonEnableMode.Always)
    {
      Text = text;
      EnableMode = enableMode;
    }

    public enum ButtonEnableMode
    {
      Always,
      Play,
      Editor
    }
  }
}