using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;

public class HealthController : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] public bool countsAsKill = true;
    [SerializeField] public bool isPlayer = false;
    public float HP, hpPercent, Defense, CritRate;
    float maxHP, originalDef;
    public int extraLives;
    bool lastHit = false;
    public AnimationController animControl;
    public Animator animator;
    [Header("Effects")]
    [SerializeField] GameObject destroyedObject;
    [SerializeField] AudioSource aSource;
    [SerializeField] AudioClip hitSound, deathSound;

    public bool invulnerable;
    [SerializeField] public float invulnerableTimer;
    public bool staminaActivator = true;
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
            if (animator != null)
            {
                animator.SetBool("Impacted", true);
            }
		}
		
        if (!invulnerable)
        {
            if (animControl != null)
            {
                animControl.IsHitted();
            }
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
            else
            {

                Invoke("StopHurt", .2f);
                Invoke("StopImpact", .15f);
                aSource.PlayOneShot(hitSound);
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
		
		if(stamina <= 0f && staminaActivator == true)
		{
			print("out of Stamina");
			isBlocking = false;
            staminaActivator = false;
		}
        if (staminaActivator == false)
        {
            animator.SetBool("Blocking",false);
            isBlocking = false;
        }
        if (stamina >= .3f && staminaActivator == false)
        {
            staminaActivator = true;
        }
        CheckStamina();

        
    }
    void StopHurt()
    {
        if (animControl != null)
        {
            animControl.EndHitted();
        }
    }
    void StopImpact()
    {
        if (animator != null)
        {
            animator.SetBool("Impacted", false);
        }
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
        if(!isPlayer)
        {
            Destroy(gameObject, 10f);
            aSource.PlayOneShot(deathSound);
            DeathAnim();
            return;
        }

        else
        {
            Destroy(gameObject);
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
        invulnerableTimer += 2.5f;
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

    void DeathAnim()
    {
        animControl.IsDead();

        AIController ai = GetComponent<AIController>();
        if (ai != null) Destroy(ai);

        SwordController sw = GetComponent<SwordController>();
        if(sw != null) Destroy(sw);

        CharacterMovement ch = GetComponent<CharacterMovement>();
        if(ch != null) Destroy(ch);

        Destroy(animControl);

        Destroy(this);
    }
}
