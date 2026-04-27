using UnityEngine;

public class EnemyHitboxScript : MonoBehaviour
{
    public EnemyCombat owner;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            owner.OnHitPlayer(other);
        }
    }
}
