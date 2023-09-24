using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class RecipeActionCollection : ScriptableObject, IActionCollectionHolder<RecipePlateAction>
{
    [SerializeField] ActionCollection<RecipePlateAction> m_recipeActions;

    public ActionCollection<RecipePlateAction> GetActionCollection()
    {
        return m_recipeActions;
    }

    private void OnValidate()
    {
        m_recipeActions.ValidateCollection();
    }
}
