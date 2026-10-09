using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

/// <summary>
/// Dev only: Visually show inputs in UI
/// </summary>
public class Dev_ShowKeysOnScreen : MonoBehaviour
{
    public TMP_Text wKey;
    public TMP_Text aKey;
    public TMP_Text sKey;
    public TMP_Text dKey;
    public TMP_Text spaceKey;
    public TMP_Text escKey;
    public TMP_Text shiftKey;
    public TMP_Text ctrlKey;
    public TMP_Text lmbKey;
    public TMP_Text rmbKey;

    // Update is called once per frame
    void Update()
    {
        Keyboard keyboard = Keyboard.current;
        Mouse mouse = Mouse.current;

        // W Key
        if (keyboard.wKey.wasPressedThisFrame)
        {
            wKey.color = Color.red;
        }

        if (keyboard.wKey.wasReleasedThisFrame)
        {
            wKey.color = Color.black;
        }
        
        // A Key
        if (keyboard.aKey.wasPressedThisFrame)
        {
            aKey.color = Color.red;
        }

        if (keyboard.aKey.wasReleasedThisFrame)
        {
            aKey.color = Color.black;
        }
        
        // S Key
        if (keyboard.sKey.wasPressedThisFrame)
        {
            sKey.color = Color.red;
        }

        if (keyboard.sKey.wasReleasedThisFrame)
        {
            sKey.color = Color.black;
        }
        
        // D Key
        if (keyboard.dKey.wasPressedThisFrame)
        {
            dKey.color = Color.red;
        }

        if (keyboard.dKey.wasReleasedThisFrame)
        {
            dKey.color = Color.black;
        }
        
        // Space Key
        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            spaceKey.color = Color.red;
        }

        if (keyboard.spaceKey.wasReleasedThisFrame)
        {
            spaceKey.color = Color.black;
        }
        
        // Esc Key
        if (keyboard.escapeKey.wasPressedThisFrame)
        {
            escKey.color = Color.red;
        }

        if (keyboard.escapeKey.wasReleasedThisFrame)
        {
            escKey.color = Color.black;
        }
        
        // Shift Key
        if (keyboard.shiftKey.wasPressedThisFrame)
        {
            shiftKey.color = Color.red;
        }

        if (keyboard.shiftKey.wasReleasedThisFrame)
        {
            shiftKey.color = Color.black;
        }
        
        // CTRL Key
        if (keyboard.ctrlKey.wasPressedThisFrame)
        {
            ctrlKey.color = Color.red;
        }

        if (keyboard.ctrlKey.wasReleasedThisFrame)
        {
            ctrlKey.color = Color.black;
        }

        //LMB Key
        if (mouse.leftButton.wasPressedThisFrame)
        {
            lmbKey.color = Color.red;
        }

        if (mouse.leftButton.wasReleasedThisFrame)
        {
            lmbKey.color = Color.black;
        }

        //RMB Key
        if (mouse.rightButton.wasPressedThisFrame)
        {
            rmbKey.color = Color.red;
        }

        if (mouse.rightButton.wasReleasedThisFrame)
        {
            rmbKey.color = Color.black;
        }
    }
}
