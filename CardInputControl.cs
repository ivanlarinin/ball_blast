using UnityEngine;

public class CartInputControl : MonoBehaviour
{
    [SerializeField] private Cart cart;
    [SerializeField] private Turret turret;

    private void Update()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        cart.SetMovementTarget(Camera.main.ScreenToWorldPoint(mouseScreenPosition));

        if (Input.GetMouseButton(0) == true)
        {
            turret.Fire();
        }
    }
}
