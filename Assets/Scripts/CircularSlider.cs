using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// A circular range / slider control for Unity UI
/// By Pietro Polsinelli https://twitter.com/ppolsinelli
/// </summary>
public class CircularSlider : MonoBehaviour
{
    public Transform Origin; //center of rotation
    public Image ImageSelected; //drag here the image of type filled radial top
    public Text Angle; //value textual feedback
    public VariableSlider varSlider;

    public int scale = 360; //value scale to use
    public float lerpSpeed; // how quickly should the slider animate around to the current value?
    public float targetValue;
    public Color defaultSelectedColor;
    public Color justClickedSelectedColor;

    private int CurrentValue;
    public State CircularButtonState = State.NOT_DRAGGING;
    private float lastTargetValue;

    public enum State
    {
        NOT_DRAGGING,
        DRAGGING,
    }

    void Update()
    {
        ImageSelected.fillAmount += (targetValue - ImageSelected.fillAmount) * lerpSpeed * Time.deltaTime;
        Angle.text = (Mathf.RoundToInt(ImageSelected.fillAmount * 100f)).ToString();

        // check if slider value has changed - if so, update the variable
        if (targetValue != lastTargetValue)
        {
            varSlider.UpdateVariable();
        }

        lastTargetValue = targetValue;
    }

    public void DragOnCircularArea(bool isClick)
    {
        //we ignore the click event due to dragging in order to 
        //ignore beyond max set with drag and enable it on click / touch
        if (isClick && CircularButtonState == State.DRAGGING)
        {
            CircularButtonState = State.NOT_DRAGGING;
            return;
        }

        if (isClick)
            CircularButtonState = State.NOT_DRAGGING;
        else
        {
            CircularButtonState = State.DRAGGING;
        }

        float f = Vector3.Angle(Vector3.up, Input.mousePosition - Origin.position);
        bool onTheRight = Input.mousePosition.x > Origin.position.x;
        int detectedValue = onTheRight ? (int)f : 180 + (180 - (int)f);

        if (detectedValue > 350)
            detectedValue = 360;
        else if (CurrentValue == 360 && detectedValue < 20)
            detectedValue = 360;
        else if (CurrentValue == 0 && detectedValue > 340)
            detectedValue = 0;
        else if (detectedValue < 10)
            detectedValue = 0;

        if (!isClick)
        {
            if (detectedValue <= CurrentValue && Mathf.Abs(CurrentValue - detectedValue) > 180)
                detectedValue = CurrentValue;
            else if (CurrentValue == 0 && detectedValue > 270)
                detectedValue = CurrentValue;
        }

        CurrentValue = detectedValue;

        targetValue = CurrentValue / 360f;
    }

    /// do a nice punch scale and colour fade effect when the thing is clicked/dragged
    public void InteractEffects(bool wasClick)
    {
        if (wasClick && transform.localScale == Vector3.one)
        {
            transform.DOPunchScale(new Vector3(0.05f, 0.05f, 0.05f), 0.3f, 3);
        }

        
        ImageSelected.DOColor(justClickedSelectedColor, 0.05f).
            OnComplete(() => { ImageSelected.DOColor(defaultSelectedColor, 0.25f); });
    }


}