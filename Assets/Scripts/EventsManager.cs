using UnityEngine;
using LitJson;
using System;
using System.Collections;

public class PlanetEvent
{
    public string name;
    public string description;
    public EventCriteria[] criteria;
    public VariableEffect[] variableEffects;
    public SpeciesEffect[] speciesEffects;

}

public enum CriteriaType { AtLeast, AtMost, Exactly, Roughly };
public class EventCriteria
{
    public CriteriaType criteriaType;
    public string criteriaVariable;
    public int criteriaNumber;
}

public class VariableEffect
{
    public string effectVariable;
    public int variableChangeAmount;
}

public class SpeciesEffect
{
    // TODO
}



public class EventsManager : MonoBehaviour {

    public PlanetEvent[] planetEvents;

    public void AddPlanetEvent(string jsonString, int variableNum, int numVariables)
    {
        // if this is the first event, initialise the planetEvents array
        if (planetEvents == null)
        {
            planetEvents = new PlanetEvent[numVariables];
        }

        // generate a PlanetEvent and its EventCriteria and effects based on this
        JsonData jsonEvent = JsonMapper.ToObject(jsonString);

        PlanetEvent newEvent = new PlanetEvent();
        newEvent.name = jsonEvent["name"].ToString();
        newEvent.description = jsonEvent["description"].ToString();

        // generate EventCriteria objects
        newEvent.criteria = new EventCriteria[jsonEvent["criteria"].Count];
        for (int i = 0; i < jsonEvent["criteria"].Count; i++)
        {
            newEvent.criteria[i] = GenerateEventCriteria(jsonEvent["criteria"][i]);
        }

        // generate variable effects
        newEvent.variableEffects = new VariableEffect[jsonEvent["variableEffects"].Count];
        for (int i = 0; i < jsonEvent["variableEffects"].Count; i++)
        {
            newEvent.variableEffects[i] = GenerateVariableEffect(jsonEvent["variableEffects"][i]);
        }

    }

    EventCriteria GenerateEventCriteria(JsonData jsonData)
    {
        EventCriteria criteria = new EventCriteria();
        criteria.criteriaVariable = jsonData["criteriaVariable"].ToString();
        criteria.criteriaNumber = Convert.ToInt32(jsonData["criteriaNumber"].ToString());
        
        switch (jsonData["criteriaType"].ToString())
        {
            case "AtLeast":
                criteria.criteriaType = CriteriaType.AtLeast;
                break;
            case "AtMost":
                criteria.criteriaType = CriteriaType.AtMost;
                break;
            case "Exactly":
                criteria.criteriaType = CriteriaType.Exactly;
                break;
            case "Roughly":
                criteria.criteriaType = CriteriaType.Roughly;
                break;
            default:
                Debug.LogError(string.Format("Invalid criteriaType! Json data: {0}", jsonData));
                break;
        }

        return criteria;
    }

    VariableEffect GenerateVariableEffect(JsonData jsonData)
    {
        VariableEffect newEffect = new VariableEffect();
        newEffect.effectVariable = jsonData["effectVariable"].ToString();
        newEffect.variableChangeAmount = Convert.ToInt32(jsonData["variableChangeAmount"].ToString());

        return newEffect;
    }
}
