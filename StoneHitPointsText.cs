using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Destructable))]
public class StoneHitpointsText : MonoBehaviour
{
    [SerializeField] private Text hitpointText;
    private Destructable destructible;
    private void Awake()
    {
        destructible = GetComponent<Destructable>();
        destructible.ChangeHitPoints.AddListener(OnChangeHitpoint);
    }

    private void OnDestroy()
    {
        if (destructible != null)
        {
            destructible.ChangeHitPoints.RemoveListener(OnChangeHitpoint);
        }
    }

    private void OnChangeHitpoint()
    {

        int hitPoints = destructible.GetHitPoints();


        if (hitPoints >= 1000)
        {
            hitpointText.text = (hitPoints / 1000) + "K";
        }
        else
        {
            hitpointText.text = hitPoints.ToString();
        }
    }
}
