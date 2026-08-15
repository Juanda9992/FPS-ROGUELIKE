using UnityEngine;

public static class CursorManager
{
    public static void SetCursorVisible(bool visible)
    {
        Debug.Log("Cursor is visible: " + visible);
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
