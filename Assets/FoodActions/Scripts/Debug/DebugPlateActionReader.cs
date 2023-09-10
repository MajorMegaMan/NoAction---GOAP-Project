using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlateState))]
public class DebugPlateActionReader : MonoBehaviour
{
    PlateState plate;
    public string actionPlan;

    Queue<RecipePlateAction> platePlan = new Queue<RecipePlateAction>();

    private void Awake()
    {
        plate = GetComponent<PlateState>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        plate.CopyActionPlan(platePlan);
        actionPlan = "";
        foreach (var action in platePlan)
        {
            actionPlan += action.actionName + ", ";
        }
    }
}
