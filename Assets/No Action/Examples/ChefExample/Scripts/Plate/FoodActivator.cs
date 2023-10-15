using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodActivator : MonoBehaviour
{
    [SerializeField] GameObject m_burger;
    [SerializeField] GameObject m_fries;
    [SerializeField] GameObject m_drink;
    [SerializeField] GameObject m_croissant;

    private void Awake()
    {
        HideAll();
    }

    public void ActivateBurger()
    {
        m_burger.SetActive(true);
    }

    public void ActivateFries()
    {
        m_fries.SetActive(true);
    }

    public void ActivateDrink()
    {
        m_drink.SetActive(true);
    }

    public void ActivateCroissant()
    {
        m_croissant.SetActive(true);
    }

    public void HideAll()
    {
        m_burger.SetActive(false);
        m_fries.SetActive(false);
        m_drink.SetActive(false);
        m_croissant.SetActive(false);
    }
}
