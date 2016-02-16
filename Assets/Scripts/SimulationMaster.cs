using UnityEngine;
using System.Collections;

public class SimulationMaster : MonoBehaviour {

    private TimeManager timeManager;
    private EventsManager eventsManager;

	void Start () {
        timeManager = GetComponent<TimeManager>();
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
