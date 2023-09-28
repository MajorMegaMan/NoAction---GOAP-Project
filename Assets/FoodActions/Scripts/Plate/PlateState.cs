using BBB.GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateState : ChefActionTarget
{
    [SerializeField] ChefRecipe m_recipe;
    [SerializeField] SerialisedGOAPWorldState<ChefWorldStateEnum> m_plateState;

    // The actions to fufil the recipe.
    Queue<RecipePlateAction> m_actionPlan;

    [SerializeField] ActionCollection<IGOAPAction> m_agentActions;
    [SerializeField] PickUpItemAction m_platePickUpAction;
    [SerializeField] DeliverItemAction m_plateDeliverAction;

    HashSet<IGOAPAction> m_deliveryActions;

    public ChefRecipe recipe { get { return m_recipe; } }

    public DeliverItemAction deliverAction { get { return m_plateDeliverAction; } }
    public EatingPosition deliveryLocation { get { return m_plateDeliverAction.deliveryLocation; } }

    private void Awake()
    {
        m_agentActions.ValidateCollection();

        m_deliveryActions = new HashSet<IGOAPAction>();
        m_deliveryActions.Add(m_platePickUpAction);
        m_deliveryActions.Add(m_plateDeliverAction);

        //SetRecipe(m_recipe);
    }

    private void Start()
    {
        SetRecipe(m_recipe);
    }

    public void SetRecipe(ChefRecipe recipe)
    {
        m_recipe = recipe;
        if (m_recipe != null)
        {
            RefreshRecipePlan();
        }
    }

    public void RefreshRecipePlan()
    {
        m_actionPlan = m_recipe.FindRecipePlan(m_plateState);
    }

    public GOAPWorldState GetWorldState()
    {
        return m_plateState;
    }

    public void CopyActionPlan(Queue<RecipePlateAction> fillPlan)
    {
        fillPlan.Clear();
        foreach(var action in m_actionPlan)
        {
            fillPlan.Enqueue(action);
        }
    }

    public bool HasActionsReady()
    {
        return m_actionPlan.Count > 0;
    }

    public RecipePlateAction PeekAction()
    {
        if (m_actionPlan.Count > 0)
        {
            return m_actionPlan.Peek();
        }
        else
        {
            return null;
        }
    }

    public RecipePlateAction PopAction()
    {
        if(m_actionPlan.Count > 0)
        {
            return m_actionPlan.Dequeue();
        }
        else
        {
            return null;
        }
    }

    public HashSet<IGOAPAction> GetAgentPlateActions()
    {
        return m_agentActions.GetBaseActions();
    }

    public bool PlateIsFinished()
    {
        return m_recipe.CheckWorldState(m_plateState);
    }

    public void DeliveryActionsUnion(HashSet<IGOAPAction> runningActions)
    {
        runningActions.UnionWith(m_deliveryActions);
    }

    public void SetDeliveryPoint(EatingPosition deliveryPoint)
    {
        m_plateDeliverAction.SetDeliveryPoint(deliveryPoint);
    }

    public void SetPickUpPoint(EatingPosition pickUpPoint)
    {
        m_platePickUpAction.SetPickUpPoint(pickUpPoint);
    }
}
