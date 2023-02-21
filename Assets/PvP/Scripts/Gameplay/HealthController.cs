using Mirror;
using System;

public class HealthController : NetworkBehaviour
{
    [SyncVar] public int maxHealth = 3;
    public int NumOfHeart { get; private set; }

    public event Action HealthChange;

    private void Awake()
    {
        NumOfHeart = maxHealth;
    }

    public void ChangingHealthValue(int value)
    {
        maxHealth = value;
        NumOfHeart = maxHealth;

        HealthChange?.Invoke();
    }
}
