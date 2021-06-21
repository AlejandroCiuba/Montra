using UnityEngine;
using System;
public class HealthSystem : MonoBehaviour, IHealth
{
    // Start is called before the first frame update
    void Start()
    {
        SetHealth();
    }

    void Update()
    {
        if(healthPoints == 0) Death();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.tag == "Enemy") RemoveHealth(1);
        else if(col.gameObject.tag == "Health") AddHealth(1);
    }

    [Header("Health Display")] 
    [SerializeField] private HealthDisplay display;

    [Header("Health Settings")]
    [SerializeField] private int healthPoints = 5;
    [SerializeField] private int maxHealth = 10;
    [SerializeField] public bool invincible = false;

    public event Action<int, bool> OnDeath;
    public int Health{get => healthPoints;}
    public void SetHealth()
    {
        display.maxHealth = maxHealth;
        display.healthPoints = healthPoints;
        display.Initialize();
    }
    public void AddHealth(int increase) 
    {
        if(healthPoints + increase > maxHealth) {increase = maxHealth - healthPoints; healthPoints = maxHealth;}
        else if(invincible) return;
        else healthPoints += increase;
        display.Add(increase);
        UnityEngine.Debug.Log("You Gained Health!!!");
    }
    public void RemoveHealth(int decrease) 
    {
        if(healthPoints - decrease <= 0) {decrease = healthPoints; healthPoints = 0;} //TODO: Add lose condition
        else if(invincible) return;
        else healthPoints -= decrease;
        display.Remove(decrease);
        UnityEngine.Debug.Log("You Lost Health!!!");
    }

    public void Death()
    {
        OnDeath?.Invoke(0, false);
        UnityEngine.Debug.Log("You Lost!!!");
    }
}
