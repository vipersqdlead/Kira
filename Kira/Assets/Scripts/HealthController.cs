using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;

public class HealthController : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] public bool countsAsKill = true;
    public float HP, hpPercent, Defense, CritRate;
    float maxHP, originalDef;
    public int extraLives;
    bool lastHit = false;

    [Header("Effects")]
    [SerializeField] GameObject destroyedObject;

    public bool invulnerable;
    [SerializeField] float invulnerableTimer;
	
	public bool isBlocking = false;
	public float stamina = 1f;
	
	void Awake()
	{

	}

    private void Start()
    {
        maxHP = HP;
        originalDef = Defense;
        hpPercent = maxHP * 100 / HP;
    }

    public bool TryKill(float _dmg)
    {
		if(isBlocking)
		{
			stamina -= 0.2f;
		}
		
        if (!invulnerable)
        {
			float dmg = _dmg / Defense;
            HP -= dmg;
            hpPercent = HP * 100f / maxHP;

            if (HP <= 0)
            {
                if (lastHit == false)
                {
                    if (extraLives <= 0)
                    {
                        lastHit = true;
                        Kill();
                        return true;
                    }
                    else
                    {
                        ContinueUsed();
                    }
                }
            }
            return false;
        }
        else
        {
            return false;
        }
    }

    private void Update()
    {
        if (invulnerable)
        {
            invulnerableTimer -= Time.deltaTime;
            if(invulnerableTimer <= 0f)
            {
                invulnerable = false;
            }
        }
		
		if(stamina <= 0f)
		{
			print("out of Stamina");
			isBlocking = false;
		}
		
		CheckStamina();
    }

    float dpsTimer;
    public void DealExternalDamagePerSecond()
    {
        dpsTimer += Time.deltaTime;
        if(dpsTimer > 1f)
        {
            TryKill(5f);
            dpsTimer = 0f;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

    }

    void Kill()
    {
        InstantiateDestroyedObject();
        Destroy(gameObject);
        return;
    }
	
	void KillNoExplosion()
	{
        InstantiateDestroyedObject();
        Destroy(gameObject);
        return;
	}

    void InstantiateDestroyedObject()
    {
        if(destroyedObject != null)
        {
            GameObject destroyedObj = Instantiate(destroyedObject, transform.position, transform.rotation);
			Rigidbody destroyedObjRb = destroyedObj.GetComponent<Rigidbody>();
            Rigidbody ownRb = GetComponent<Rigidbody>();
            destroyedObjRb.AddForce(ownRb.linearVelocity, ForceMode.VelocityChange);
        }
    }

    public void HealMaxHP()
    {
        HP += maxHP;
        hpPercent = HP * 100 / maxHP;

        return;
    }

    public void HealHPAmmount(float heal)
    {
        HP += heal;
        hpPercent = HP * 100 / maxHP;

        return;
    }

    public void EnableInvulerability()
    {
        invulnerable = true;
        invulnerableTimer += 180f;
        return;
    }

    void ContinueUsed()
    {
        HP = maxHP;
        extraLives--;
        hpPercent = HP * 100 / maxHP;
        invulnerable = true;
        invulnerableTimer += 10f;
		CureStatusEffects();
        return;
    }

    public void GrantExtraLife()
    {
        extraLives++;
        return;
    }
	
	public void CureStatusEffects()
	{
		
	}
	
	public void CheckStamina()
	{
		if(isBlocking)
		{
			stamina -= (0.025f * Time.deltaTime);
		}
		
		else
		{
			if(stamina < 1f) 
			{	
				stamina += (0.04f * Time.deltaTime);
			}
		}
	}
}
