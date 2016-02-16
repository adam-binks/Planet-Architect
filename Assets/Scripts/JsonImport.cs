using UnityEngine;
using LitJson;
using System.Collections;
using System;

public class JsonImport : MonoBehaviour {

    public TextAsset variablesText;
    public TextAsset eventsText;

    private VariablesManager varManager;
    private EventsManager eventsManager;

	void Start () {
        varManager = GetComponent<VariablesManager>();
        eventsManager = GetComponent<EventsManager>();

        ImportVariables();
        ImportEvents();
        GetComponent<TimeManager>().AddYearVariable();
    }

    /// <summary>
    /// Read "Content/variables.txt" json file and create GameVariable objects and sliders for each of them
    /// </summary>
	void ImportVariables()
    {
        JsonData gameVariables = JsonMapper.ToObject(variablesText.text);

        for (int i = 0; i < gameVariables["variables"].Count; i++)
        {
            // create a new GameVariable and slider
            varManager.AddVariable(gameVariables["variables"][i].ToJson(), i, gameVariables["variables"].Count);
        }
    }

    void ImportEvents()
    {
        JsonData planetEvents = JsonMapper.ToObject(eventsText.text);

        for (int i = 0; i < planetEvents["events"].Count; i++)
        {
            // create a new PlanetEvent with EventCriteria and effects
            eventsManager.AddPlanetEvent(planetEvents["events"][i].ToJson(), i, planetEvents["events"].Count);
        }
    }
}
