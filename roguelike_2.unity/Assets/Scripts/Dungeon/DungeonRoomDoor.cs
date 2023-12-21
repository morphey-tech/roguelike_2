using UnityEngine;

public class DungeonRoomDoor : MonoBehaviour
{
  public bool IsOpen => gameObject.activeSelf == false;

  public void Open()
  {
    gameObject.SetActive(false);
  }

  public void Close()
  {
    gameObject.SetActive(true);
  }
}