using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimeManager : MonoBehaviour {

    public float year;
    public float baseTimeSpeed;
    public float timeScale;
    public Text yearText;

	void Start () {
        year = 0;
	}
	
	public void UpdateTime()
    {
        year += Time.deltaTime * baseTimeSpeed * timeScale;
        yearText.text = ((int)year).ToString();
    }

    public void SetTimescale(float newTimeScale)
    {
        timeScale = newTimeScale;
    }

    // the followings methods are for the different buttons (they can't call methods with parameters :/ )

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
