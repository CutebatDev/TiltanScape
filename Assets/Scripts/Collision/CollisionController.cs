using Events;
using UnityEngine;

namespace Collision
{
    public class CollisionController : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            CollisionSystem.HandleTrigger(other.gameObject, gameObject);
        }
    }
}