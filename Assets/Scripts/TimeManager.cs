using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimeManager : MonoBehaviour {

    public float startTimeScale;
    public float baseTimeSpeed;
    public float timeScale;
    public Text yearText;
    public int maxYear;
    public Button button0x;
    public Button button1x;
    public Button button2x;
    public Button button5x;

    private float trueYear = 0; // locally store year as a float for a more consistent timescale
    private GameVariable yearVar;

	void Start () {
        trueYear = 0;
	}

    /// <summary>
    /// Create "year" as a variable. Called by JsonImport after importing other variables
    /// </summary>
    public void AddYearVariable()
    {
        yearVar = GetComponent<VariablesManager>().AddCustomVar("year");
        yearVar.description = "The current in-game year";
        yearVar.value = 0;
        yearVar.maxAmount = maxYear;
        timeScale = startTimeScale;
        yearText.text = "0";
    }
	
	public void UpdateTime()
    {
        trueYear += Time.deltaTime * baseTimeSpeed * timeScale;
        yearVar.value = (int)trueYear;
        yearText.text = yearVar.value.ToString();
    }

    public void SetTimescale(float newTimeScale)
    {
        timeScale = newTimeScale;
    }

    // the followings methods are for the different buttons (buttons can't call methods with parameters :( )

    public void SetTimescale0x()
    {
        SetTimescale(0);
    }

    public void SetTimescale1x()
    {
        SetTimescale(1);
    }

    public void SetTimescale2x()
    {
        SetTimescale(2);
    }

    public void SetTimescale5x()
    {
        SetTimescale(5);
    }
}
