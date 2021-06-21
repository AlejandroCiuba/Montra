using UnityEngine;
public interface IEnemy
{
    IHealth health();
    void OnCollisionEnter2D(Collision2D col);
    int Damage{get; set;}
}