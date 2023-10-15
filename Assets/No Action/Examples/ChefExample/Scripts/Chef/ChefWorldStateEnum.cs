using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChefWorldStateEnum
{
    Chef_HoldItem,

    Plate_BurgerCount, // burger
    Plate_FriesCount, // fries
    Plate_DrinkCount, // drink
    Plate_CrossCount, // croissant
    Plate_TotalCount,

    Chef_HoldRef,
    Plate_Delivered,
    Plate_ReadyForItems,
}
