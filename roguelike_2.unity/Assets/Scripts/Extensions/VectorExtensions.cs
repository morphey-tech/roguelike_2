using UnityEngine;

public static class VectorExtensions
{
  public static float SqrMagnitudeXY(this Vector3 vector) => vector.x * vector.x + vector.y * vector.y;
  public static float SqrMagnitudeXZ(this Vector3 vector) => vector.x * vector.x + vector.z * vector.z;
  public static float SqrMagnitudeYZ(this Vector3 vector) => vector.y * vector.y + vector.z * vector.z;

  public static float MagnitudeXY(this Vector3 vector) => Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y);
  public static float MagnitudeXZ(this Vector3 vector) => Mathf.Sqrt(vector.x * vector.x + vector.z * vector.z);
  public static float MagnitudeYZ(this Vector3 vector) => Mathf.Sqrt(vector.y * vector.y + vector.z * vector.z);

  public static int CompareMagnitude(this Vector3 vector, Vector3 other) =>
    vector.magnitude.CompareTo(other.magnitude);

  public static int CompareMagnitudeXY(this Vector3 vector, Vector3 other) =>
    vector.MagnitudeXY().CompareTo(other.MagnitudeXY());

  public static int CompareMagnitudeXZ(this Vector3 vector, Vector3 other) =>
    vector.MagnitudeXZ().CompareTo(other.MagnitudeXZ());

  public static int CompareMagnitudeYZ(this Vector3 vector, Vector3 other) =>
    vector.MagnitudeYZ().CompareTo(other.MagnitudeYZ());

  public static Vector3 NormalizeXY(this Vector3 vector) => new Vector2(vector.x, vector.y).normalized;
  public static Vector3 NormalizeXZ(this Vector3 vector) => new Vector2(vector.x, vector.z).normalized;
  public static Vector3 NormalizeYZ(this Vector3 vector) => new Vector2(vector.y, vector.z).normalized;

  public static Vector3 ApplyRotateXYZ(this ref Vector3 vector, float eulerX, float eulerY, float eulerZ)
  {
    vector = RotateXYZ(vector, eulerX, eulerY, eulerZ);
    return vector;
  }

  public static Vector3 ApplyRotateXY(this ref Vector3 vector, float eulerX, float eulerY) =>
    ApplyRotateXYZ(ref vector, eulerX, eulerY, 0);

  public static Vector3 ApplyRotateXZ(this ref Vector3 vector, float eulerX, float eulerZ) =>
    ApplyRotateXYZ(ref vector, eulerX, 0, eulerZ);

  public static Vector3 ApplyRotateYZ(this ref Vector3 vector, float eulerY, float eulerZ) =>
    ApplyRotateXYZ(ref vector, 0, eulerY, eulerZ);

  public static Vector3 ApplyRotateX(this ref Vector3 vector, float eulerX) =>
    ApplyRotateXYZ(ref vector, eulerX, 0, 0);

  public static Vector3 ApplyRotateY(this ref Vector3 vector, float eulerY) =>
    ApplyRotateXYZ(ref vector, 0, eulerY, 0);

  public static Vector3 ApplyRotateZ(this ref Vector3 vector, float eulerZ) =>
    ApplyRotateXYZ(ref vector, 0, 0, eulerZ);

  public static Vector3 RotateXYZ(this Vector3 vector, float eulerX, float eulerY, float eulerZ) =>
    Quaternion.Euler(eulerX, eulerY, eulerZ) * vector;

  public static Vector3 RotateXY(this Vector3 vector, float eulerX, float eulerY) =>
    RotateXYZ(vector, eulerX, eulerY, 0);

  public static Vector3 RotateXZ(this Vector3 vector, float eulerX, float eulerZ) =>
    RotateXYZ(vector, eulerX, 0, eulerZ);

  public static Vector3 RotateYZ(this Vector3 vector, float eulerY, float eulerZ) =>
    RotateXYZ(vector, 0, eulerY, eulerZ);

  public static Vector3 RotateX(this Vector3 vector, float eulerX) => RotateXYZ(vector, eulerX, 0, 0);
  public static Vector3 RotateY(this Vector3 vector, float eulerY) => RotateXYZ(vector, 0, eulerY, 0);
  public static Vector3 RotateZ(this Vector3 vector, float eulerZ) => RotateXYZ(vector, 0, 0, eulerZ);

  public static Vector3 RotateAround(this Vector3 vector, float angle, Vector3 axis) => Quaternion.AngleAxis(angle, axis) * vector;
}
