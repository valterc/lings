using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class MenuButton : MonoBehaviour
{
     
    public enum ButtonState
    {
        Pressed,
        Normal
    }

    public delegate void ButtonAction(MenuButton button, ButtonState newState);

    public event ButtonAction OnAction;
    public bool useMouseInput = true;
    public bool useHapticFeedback = true;
    public bool disabled;
    public Sprite disabledSprite;
    public Camera renderCamera;
    public bool handleBackKey;

    private SpriteRenderer spriteRenderer;
    private Sprite normalSprite;
    private ButtonState internalState;
    private bool wasDisabled;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        normalSprite = spriteRenderer.sprite;
        if (disabledSprite == null)
        {
            disabledSprite = normalSprite;
        }

        internalState = ButtonState.Normal;
    }

    void Update()
    {
        UpdateDisabledSettings();

        if (disabled)
        {
            return;
        }
        Input.simulateMouseWithTouches = false;

        Rect buttonBounds = BoundsToScreenRect(spriteRenderer.bounds);

        foreach (var touch in Input.touches)
        {
            Vector2 touchPosition = new Vector2(touch.position.x, Screen.height - touch.position.y);

            if (touch.phase == TouchPhase.Began)
            {
                if (buttonBounds.Contains(touchPosition))
                {
                    InvokeOnAction(ButtonState.Pressed);
                }
            }
            else
            {
                if (touch.phase == TouchPhase.Ended || !buttonBounds.Contains(touchPosition))
                {
                    InvokeOnAction(ButtonState.Normal);
                }
            }
        }

#if UNITY_EDITOR || UNITY_STANDALONE
        if (useMouseInput)
        {

            Vector2 mousePosition = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);

            if (Input.GetMouseButtonDown(0))
            {
                if (buttonBounds.Contains(mousePosition))
                {
                    InvokeOnAction(ButtonState.Pressed);
                }
            }
            else if (Input.GetMouseButton(0))
            {
                if (!buttonBounds.Contains(mousePosition))
                {
                    InvokeOnAction(ButtonState.Normal);
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                InvokeOnAction(ButtonState.Normal);
            }
        }
#endif

        if (handleBackKey)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                InvokeOnAction(ButtonState.Pressed);
            }
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                InvokeOnAction(ButtonState.Normal);
            }
        }

        Input.simulateMouseWithTouches = true;
    }

    private void InvokeOnAction(ButtonState buttonState)
    {
        if (internalState == buttonState)
        {
            return;
        }

        internalState = buttonState;

        if (useHapticFeedback)
        {
            DoHapticFeedback();
        }

        if (OnAction != null)
        {
            OnAction(this, buttonState);
        }
    }

    private void DoHapticFeedback()
    {
        if (internalState == MenuButton.ButtonState.Pressed)
        {
            transform.localScale = Vector3.one * .9f;
        }
        else
        {
            transform.localScale = Vector3.one;
        }
    }

    private void UpdateDisabledSettings()
    {
        if (!wasDisabled && disabled)
        {
            wasDisabled = true;
            spriteRenderer.sprite = disabledSprite;
        }
        else if (wasDisabled && !disabled)
        {
            wasDisabled = false;
            spriteRenderer.sprite = normalSprite;
        }
    }

    public Rect BoundsToScreenRect(Bounds bounds)
    {
        Camera camera = this.renderCamera != null ? this.renderCamera : Camera.main;
        Vector3 origin = camera.WorldToScreenPoint(new Vector3(bounds.min.x, bounds.max.y, 0f));
        Vector3 extent = camera.WorldToScreenPoint(new Vector3(bounds.max.x, bounds.min.y, 0f));

        return new Rect(origin.x, Screen.height - origin.y, extent.x - origin.x, origin.y - extent.y);
    }

}
