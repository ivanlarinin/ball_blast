using UnityEngine;

public class CartInputControl : MonoBehaviour
{
    [SerializeField] private Cart cart;

    private void Update()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        cart.SetMovementTarget(Camera.main.ScreenToWorldPoint(mouseScreenPosition));
    }
}
