using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ChefRecipe))]
public class DebugRandomiseRecipe : MonoBehaviour
{
    [SerializeField] SerialisedGOAPWorldState<ChefWorldStateEnum> m_targets;

    [SerializeField] int m_bunMax = 3;
    [SerializeField] int m_lettuceMax = 3;
    [SerializeField] int m_tomatoMax = 3;
    [SerializeField] int m_beefMax = 3;

    ChefRecipe m_recipe;

    private void Awake()
    {
        m_recipe = GetComponent<ChefRecipe>();
        Randomise();
    }

    public void Randomise()
    {
        m_recipe.plateRecipe.SetValue<int>((int)ChefWorldStateEnum.Plate_BunCount, Random.Range(0, m_bunMax));
        m_recipe.plateRecipe.SetValue<int>((int)ChefWorldStateEnum.Plate_LettuceCount, Random.Range(0, m_lettuceMax));
        m_recipe.plateRecipe.SetValue<int>((int)ChefWorldStateEnum.Plate_TomatoCount, Random.Range(0, m_tomatoMax));
        m_recipe.plateRecipe.SetValue<int>((int)ChefWorldStateEnum.Plate_BeefCount, Random.Range(0, m_beefMax));
    }
}
