using UnityEngine;

public static class MathfHelper
{
  public static bool QuadraticSolver(float a, float b, float c, out float t1, out float t2)
  {
    float D = Mathf.Pow(b, 2f) - 4f * a * c;
    if (D > 0 || D == 0)
    {
      t1 = (-b + Mathf.Sqrt(D)) / (2 * a);
      t2 = (-b - Mathf.Sqrt(D)) / (2 * a);

      return true;
    }

    t1 = -1;
    t2 = -1;
    return false;
  }

  public static Vector3 CalculateTargetPositionForCatchUp(Vector3 startTargetPosition, Vector3 targetMoveDirection,
    float targetMoveSpeed, Vector3 hauntingPoint, float haunterSpeed)
  {
    float time = TimeForCatchUpToThePoint(startTargetPosition, targetMoveDirection, targetMoveSpeed, hauntingPoint,
    haunterSpeed);
    return startTargetPosition + targetMoveDirection * (targetMoveSpeed * time);
    ;
  }

  public static Vector3 CalculateMoveDirectionForCatchUp(Vector3 startTargetPosition, Vector3 targetMoveDirection,
    float targetMoveSpeed, Vector3 hauntingPoint, float haunterSpeed)
  {
    return CalculateTargetPositionForCatchUp(startTargetPosition, targetMoveDirection, targetMoveSpeed,
    hauntingPoint,
    haunterSpeed) - hauntingPoint;
  }

  public static bool CanCatchUpToThePoint(Vector3 targetPosition, Vector3 targetMoveDirection, float targetMoveSpeed,
    Vector3 hauntingPoint, float haunterSpeed)
  {
    return CanCatchUpToThePoint(targetPosition, targetMoveDirection, targetMoveSpeed, hauntingPoint, haunterSpeed,
    out float time);
  }

  public static bool CanCatchUpToThePoint(Vector3 targetPosition, Vector3 targetMoveDirection, float targetMoveSpeed,
    Vector3 hauntingPoint, float haunterSpeed, out float time)
  {
    time = TimeForCatchUpToThePoint(targetPosition, targetMoveDirection, targetMoveSpeed, hauntingPoint,
    haunterSpeed);
    return time >= 0;
  }

  public static bool CanCatchUpToThePoint(Vector3 targetPosition, Vector3 targetMoveDirection,
    float targetMoveSpeed, Vector3 hauntingPoint, float haunterSpeed, out Vector3 hitPoint)
  {
    CanCatchUpToThePoint(targetPosition, targetMoveDirection, targetMoveSpeed, hauntingPoint, haunterSpeed,
    out float time);
    hitPoint = (targetPosition + targetMoveDirection.normalized * time);
    return time >= 0;
  }

  public static float TimeForCatchUpToThePoint(Vector3 startTargetPosition, Vector3 targetMoveDirection,
    float targetMoveSpeed, Vector3 hauntingPoint, float haunterSpeed)
  {
    Vector3 targetToHaunter = hauntingPoint - startTargetPosition;
    //  float d = targetToHaunter.magnitude;

    float a = haunterSpeed * haunterSpeed - targetMoveSpeed * targetMoveSpeed;
    float b = 2 * Vector3.Dot(targetToHaunter, targetMoveDirection * targetMoveSpeed);
    float c = -targetToHaunter.sqrMagnitude;

    float time;
    if (QuadraticSolver(a, b, c, out float t1, out float t2))
    {
      if (t1 < 0 && t2 < 0)
        return -1;

      if (t1 > 0 && t2 > 0)
        time = Mathf.Min(t1, t2);
      else
        time = Mathf.Max(t1, t2);
    }
    else
      return -1;

    return time;
  }

  public static Vector3 NearestPointInRay(Vector3 position, Vector3 origin, Vector3 direction) =>
    origin + direction * GetUForIntersectPoint(position, origin, direction);

  public static Vector3 NearestPointInLineSegment(Vector3 position, Vector3 startPoint, Vector3 endPoint)
  {
    Vector3 startToEnd = endPoint - startPoint;
    float u = GetUForIntersectPoint(position, startPoint, endPoint - startPoint);

    return u < 0 ? startPoint : u > 1 ? endPoint : startPoint + startToEnd * u;
  }

  public static bool HasPerpendicularToLine(Vector3 position, Vector3 startPoint, Vector3 endPoint)
  {
    return TryGetPerpendicularPointInLineFromPosition(position, startPoint, endPoint, out Vector3 result);
  }

  public static bool TryGetPerpendicularPointInLineFromPosition(Vector3 position, Vector3 startPoint, Vector3 endPoint, out Vector3 result)
  {
    Vector3 startToEnd = endPoint - startPoint;
    float u = GetUForIntersectPoint(position, startPoint, startToEnd);

    bool isHas = IsCorrectUForPerpendicular(u);
    result = isHas ? startPoint + startToEnd * u : Vector3.zero;
    return isHas;
  }

  private static float GetUForIntersectPoint(Vector3 position, Vector3 origin, Vector3 direction) => Vector3.Dot(position - origin, direction) / direction.sqrMagnitude;
  private static bool IsCorrectUForPerpendicular(float u) => u >= 0 && u <= 1;
}