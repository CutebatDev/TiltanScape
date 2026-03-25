using Events;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerUIController : MonoBehaviour
    {
        public void TogglePauseMenu(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed)
                return;

            EventsManager.Instance.OnTogglePauseMenu?.Invoke();
        }
    }
}