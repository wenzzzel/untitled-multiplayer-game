using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [Header("References to other scripts")]
    [SerializeField] private HealthBarFill healthBarFillScript;


#region Public methods

    public void UpdateHealthBar(int currentHealth)
    {
        healthBarFillScript.UpdateFill(currentHealth);
    }

#endregion

}
