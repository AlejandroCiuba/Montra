using System;
public interface IHealth
{
    int Health{get;}
    void SetHealth();
    void AddHealth(int increaseHealth);
    void RemoveHealth(int decreaseHealth);
    void Death();
    event Action<int, bool> OnDeath;
}
