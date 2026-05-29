using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;

public class UIJoystick : MonoBehaviour
{
    [SerializeField] private GameObject joystick;
    [SerializeField] private OnScreenStick onScreenStickComponent;
    private bool wasTouchingLastFrame = false;
    
    private void Start()
    {
        joystick.SetActive(false);
    }

    void Update()
    {
#if UNITY_ANDROID
        if (Touchscreen.current == null) return;

        var primaryTouch = Touchscreen.current.primaryTouch;

        if (primaryTouch.press.isPressed)
        {
            Vector2 screenPosition = primaryTouch.position.ReadValue();
            if (!wasTouchingLastFrame)
            {

                joystick.transform.position = screenPosition;
                joystick.SetActive(true);

                if (onScreenStickComponent != null)
                {
                    PointerEventData data = new PointerEventData(EventSystem.current)
                    {
                        position = screenPosition
                    };

                    onScreenStickComponent.OnPointerDown(data);
                }

                wasTouchingLastFrame = true;
            }                

            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = screenPosition
            };

            onScreenStickComponent.OnDrag(pointerData);
        }
        else
        {
            if (wasTouchingLastFrame)
            {
                joystick.SetActive(false);
                wasTouchingLastFrame = false;

            }
        }
#endif
    }
}
