using UnityEngine;
using System.Collections;

namespace Gem
{
    public interface IMovingPlatform
    {
        Vector3 GetSurfaceAngularVelocity(Vector3 aPoint);
        Vector3 GetSurfaceVelocity(Vector3 aPoint);
    }
}
