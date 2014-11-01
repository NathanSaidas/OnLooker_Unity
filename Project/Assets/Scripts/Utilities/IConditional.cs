using UnityEngine;
namespace EndevGame
{
    public interface IConditional
    {
        bool condition(Transform aPlayer, Transform aObject);
    }
}