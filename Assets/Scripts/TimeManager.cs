using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
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
    public Color buttonSelected;
    public Color buttonNotSelected;

    private float trueYear = 0; // locally store year as a float for a more consistent timescale
    private GameVariable yearVar;
    private Image[] buttonImgs;

	void Start () {
        trueYear = 0;

        buttonImgs = new Image[4];
        Button[] buttons = new Button[4] { button0x, button1x, button2x, button5x };
        for (int i= 0; i < 4; i++)
        {
            buttonImgs[i] = buttons[i].GetComponent<Image>();
        }
	}

    /// <summary>
    /// Create "year" as a variable. Called by JsonImport after importing other variables
    /// </summary>
    public void AddYearVariable()
    {
        yearVar = GetComponent<VariablesManager>().AddCustomVar("Year");
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

    // the followings methods are for the different buttons cos buttons can't call methods with parameters >:(

    public void SetTimescale0x()
    {
        SetTimescale(0);
        ResetAllButtonColours();
        buttonImgs[0].DOColor(buttonSelected, 1f);
    }

    public void SetTimescale1x()
    {
        SetTimescale(1);
        ResetAllButtonColours();
        buttonImgs[1].DOColor(buttonSelected, 1f);
    }

    public void SetTimescale2x()
    {
        SetTimescale(2);
        ResetAllButtonColours();
        buttonImgs[2].DOColor(buttonSelected, 1f);
    }

    public void SetTimescale5x()
    {
        SetTimescale(5);
        ResetAllButtonColours();
        buttonImgs[3].DOColor(buttonSelected, 1f);
    }

    void ResetAllButtonColours()
    {
        foreach(Image img in buttonImgs)
        {
            img.DOColor(buttonNotSelected, 1f);
        }
    }
}
