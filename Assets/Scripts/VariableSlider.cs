using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class VariableSlider : MonoBehaviour {

    public GameVariable gameVar;
    public Text titleText;

    private CircularSlider slider;

	public void Setup (GameVariable thisGameVar) {
        gameVar = thisGameVar;
        slider = GetComponentInChildren<CircularSlider>();
        titleText.text = gameVar.name;
	}
	
    /// <summary>
    /// Update the variable to the current slider value
    /// </summary>
	public void UpdateVariable()
    {
        gameVar.ChangeValue((int)slider.targetValue, false);
    }

    /// <summary>
    /// Update the slider knob position to the current variable value (if it's changed somewhere else)
    /// </summary>
    public void UpdateSlider()
    {
        slider.targetValue = gameVar.value;
    }
}
