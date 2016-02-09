using UnityEngine;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;

public class PlanetEvent
{
    public string name;
    public string description;
    public EventCriteria[] criteria;
    public VariableEffect[] variableEffects;
    public SpeciesEffect[] speciesEffects;

    public bool CheckIfCriteriaFulfilled(Dictionary<string, GameVariable> variables)
    {
        foreach(EventCriteria theCriteria in criteria)
        {
            if (!theCriteria.Check(variables))
            {
                return false; // if any one criteria has not been met, return false
            }
        }
        // all criteria are met: trigger the event
        Debug.Log(string.Format("Event triggered: {0}", name));
        FireVariableEffects(variables);
        return true;
    }

    private void FireVariableEffects(Dictionary<string, GameVariable> variables)
    {
        foreach (VariableEffect effect in variableEffects)
        {
            effect.Fire(variables);
        }
    }

}

public enum CriteriaType { AtLeast, AtMost, Exactly, Roughly };
public class EventCriteria
{
    public CriteriaType criteriaType;
    public string criteriaVariable;
    public int criteriaNumber;

    private int roughVariableLenience = 5;

    /// <summary>
    /// Check if this criteria has been met
    /// </summary>
    public bool Check(Dictionary<string, GameVariable> variables)
    {
        switch (criteriaType)
        {
            case CriteriaType.AtLeast:
                return variables[criteriaVariable].value >= criteriaNumber;
            case CriteriaType.AtMost:
                return variables[criteriaVariable].value <= criteriaNumber;
            case CriteriaType.Exactly:
                return variables[criteriaVariable].value == criteriaNumber;
            case CriteriaType.Roughly: // return true if value is within roughVariableLenience of criteriaNumber
                return variables[criteriaVariable].value <= criteriaNumber + roughVariableLenience &&
                       variables[criteriaVariable].value >= criteriaNumber - roughVariableLenience;
            default:
                break;
        }
        Debug.LogError("Invalid CriteriaType for variable " + criteriaVariable);
        return false;
    }
}

public class VariableEffect
{
    public string effectVariable;
    public int variableChangeAmount;

    /// <summary>
    /// Set the effectVariable's value to its previous value + variableChangeAmounts
    /// </summary>
    public void Fire(Dictionary<string, GameVariable> variables)
    {
        variables[effectVariable].ChangeValue(variables[effectVariable].value + variableChangeAmount);
    }
}

public class SpeciesEffect
{
    // TODO
}



public class EventsManager : MonoBehaviour {

    public PlanetEvent[] planetEvents;

    private VariablesManager varManager;


    void Start()
    {
        varManager = GetComponent<VariablesManager>();
    }

    public void AddPlanetEvent(string jsonString, int eventNum, int numEvents)
    {
        // if this is the first event, initialise the planetEvents array
        if (planetEvents == null)
        {
            planetEvents = new PlanetEvent[numEvents];
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

        planetEvents[eventNum] = newEvent;

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

    public void UpdateEvents()
    {
        CheckForEventTriggers();
    }

    void CheckForEventTriggers()
    {
        foreach (PlanetEvent pEvent in planetEvents)
        {
            pEvent.CheckIfCriteriaFulfilled(varManager.gameVars);
        }
    }
}
