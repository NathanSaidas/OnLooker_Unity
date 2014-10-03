using UnityEngine;
using System;
using System.Collections;


namespace EndevGame
{
    [Flags()]
    public enum PlantType
    {
        NONE = 0,
        ROCK = 1,
        SUNFLOWER = 2,
        VINE = 4, 
        MUSHROOM = 8,
        MIRROR = 16,
        PROPELLER = 32
    }
}