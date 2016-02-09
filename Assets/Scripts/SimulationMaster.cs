using UnityEngine;
using System.Collections;

public class SimulationMaster : MonoBehaviour {

    private TimeManager timeManager;
    private VariablesManager varManager;
    private EventsManager eventsManager;

	void Start () {
        timeManager = GetComponent<TimeManager>();
        varManager = GetComponent<VariablesManager>();
        eventsManager = GetComponent<EventsManager>();
	}
	
	void Update () {
	    for (int i=0; i < timeManager.timeScale; i++)
        {
            ExecuteOneUpdateStep();
        }
	}

    void ExecuteOneUpdateStep()
    {
        timeManager.UpdateTime();
        eventsManager.UpdateEvents();
    }
}
