using UnityEngine;

public class WaterDamage : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    HealthController hp;
    float damage = 5f;
    float damageTimer = 0f;
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(hp == null)
            {
                hp = other.GetComponent<HealthController>();
            }
            damageTimer -= Time.deltaTime;
            if(damageTimer < 0f)
            {
                hp.TryKill(damage);
                damageTimer = 1f;
            }
        }
    }
}
