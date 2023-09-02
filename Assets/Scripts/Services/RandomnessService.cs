using System;

public class RandomnessService
{
  private readonly Random _random;

  public RandomnessService()
  {
    _random = new Random();
  }

  public RandomnessService(int seed)
  {
    _random = new(seed);
  }

  public int RandomInt()
  {
    return _random.Next();
  }

  public int RandomInt(int min, int max)
  {
    return _random.Next(min, max);
  }

  public double RandomDouble()
  {
    return _random.NextDouble();
  }

  public double RandomDouble(double min, double max)
  {
    return _random.NextDouble() * (max  - min) + min;
  }
}
