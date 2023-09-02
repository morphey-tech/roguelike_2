using System;

[AttributeUsage(AttributeTargets.Method), System.Diagnostics.Conditional("UNITY_EDITOR")]
public class CustomButtonAttribute : Attribute
{
  public enum ButtonEnableMode
  {
    Always,
    Play,
    Editor
  }

  public string Text { get; private set; }
  public ButtonEnableMode EnableMode { get; private set; }

  public CustomButtonAttribute(string text = null, ButtonEnableMode enableMode = ButtonEnableMode.Always)
  {
    Text = text;
    EnableMode = enableMode;
  }
}
