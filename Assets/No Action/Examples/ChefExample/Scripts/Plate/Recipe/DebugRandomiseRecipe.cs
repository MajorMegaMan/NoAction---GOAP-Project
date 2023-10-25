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
        int total = 0;
        int rand = Random.Range(0, m_bunMax);
        total += rand;
        m_recipe.plateRecipe.SetValue<int>((int)ChefWorldStateEnum.Plate_BurgerCount, rand);

        rand = Random.Range(0, m_lettuceMax);
        total += rand;
        m_recipe.plateRecipe.SetValue<int>((int)ChefWorldStateEnum.Plate_FriesCount, rand);

        rand = Random.Range(0, m_tomatoMax);
        total += rand;
        m_recipe.plateRecipe.SetValue<int>((int)ChefWorldStateEnum.Plate_DrinkCount, rand);

        rand = Random.Range(0, m_beefMax);
        total += rand;
        m_recipe.plateRecipe.SetValue<int>((int)ChefWorldStateEnum.Plate_CrossCount, rand);

        if (total == 0)
        {
            rand = Random.Range(0, 4);
            switch(rand)
            {
                case 0:
                    {
                        m_recipe.plateRecipe.SetValue<int>((int)ChefWorldStateEnum.Plate_BurgerCount, 1);
                        break;
                    }
                case 1:
                    {
                        m_recipe.plateRecipe.SetValue<int>((int)ChefWorldStateEnum.Plate_FriesCount, 1);
                        break;
                    }
                case 2:
                    {
                        m_recipe.plateRecipe.SetValue<int>((int)ChefWorldStateEnum.Plate_DrinkCount, 1);
                        break;
                    }
                case 3:
                    {
                        m_recipe.plateRecipe.SetValue<int>((int)ChefWorldStateEnum.Plate_CrossCount, 1);
                        break;
                    }
            }
        }
    }

    public static void RandomiseRecipe(BBB.GOAP.GOAPWorldState recipe, int maxCounts)
    {
        recipe.SetValue<int>((int)ChefWorldStateEnum.Plate_BurgerCount, Random.Range(0, maxCounts));
        recipe.SetValue<int>((int)ChefWorldStateEnum.Plate_FriesCount, Random.Range(0, maxCounts));
        recipe.SetValue<int>((int)ChefWorldStateEnum.Plate_DrinkCount, Random.Range(0, maxCounts));
        recipe.SetValue<int>((int)ChefWorldStateEnum.Plate_CrossCount, Random.Range(0, maxCounts));
    }
}
