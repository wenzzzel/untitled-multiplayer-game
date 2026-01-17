using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [Header("References to other scripts")]
    [SerializeField] private HealthBarFill healthBarFillScript;


#region Public methods

    public void UpdateHealthBar(int currentHealth) //TODO: This one needs to update on the server and all clients
    {
        healthBarFillScript.UpdateFill(currentHealth);
    }

#endregion

}
