using UnityEngine;
using System.Collections;

public class GameVariable
{
    public string name;
    public string description;
    public int defaultValue;
    public int currentValue;
}


public class VariablesManager : MonoBehaviour {

    public GameVariable[] gameVars;

    /// <summary>
    /// Add a new player controlled variable and create a slider for it
    /// </summary>
    /// <param name="jsonString">
    /// Should have properties: "name", "description", "default"
    /// </param>
    public void AddVariable(string jsonString, int variableNum, int numVariables)
    {
        if (gameVars == null)
        {
            gameVars = new GameVariable[numVariables];
        }

        GameVariable newVar = JsonUtility.FromJson<GameVariable>(jsonString);

        gameVars[variableNum] = newVar;
    }
}
