using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    public delegate void InputSpaceEventHandler(float space);

    private InputSpaceEventHandler inputSpaceEventHandler;

    public event InputSpaceEventHandler OnInputSpace
    {
        add
        {
            inputSpaceEventHandler += value;
        }
        remove
        {
            inputSpaceEventHandler -= value;
        }
    }


    public event Action<float, float> OnInputHorizontalOrVertical;

    private void Update()
    {
        if (inputSpaceEventHandler != null)
        {
            inputSpaceEventHandler(Input.GetAxisRaw("Space"));
        }

        if (OnInputHorizontalOrVertical != null)
        {
            OnInputHorizontalOrVertical(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }
    }
}
