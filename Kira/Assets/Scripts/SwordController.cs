using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordController : MonoBehaviour
{
	[Header("References")]
    public Transform attackOrigin;      // usually the camera or weapon position
    public LayerMask hitMask;           // what counts as a valid target

    [Header("Attack Settings")]
    public float attackDuration = 0.25f;  // total time of slash
    public float attackRange = 3f;        // final max range
    public float attackRadius = 0.3f;     // starting radius of sphere
    public float maxAttackRadius = 1.0f;  // radius at end of swing
	public float attackAngle = 70f;
    public float attackCooldown = 0.4f;   // time between attacks
    public float damage = 25f;
	private HashSet<GameObject> wereHit = new HashSet<GameObject>();

    private bool isAttacking;
    private bool onCooldown;
    public float attackTimer;

	public Animator attacksAnimation;
    private void FixedUpdate()
    {
        if (isAttacking)
            UpdateAttack();

		UpdateParryAndBlock();
    }

    // --- PUBLIC FUNCTIONS CALLED BY INPUT OR ANIMATIONS ---

    public void StartAttack()
    {
        if (onCooldown || isAttacking || isParrying)
            return;

        isAttacking = true;
		gameObject.GetComponent<Collider>().attachedRigidbody.AddForce(transform.forward * (2f * Time.fixedDeltaTime * 60f), ForceMode.Impulse);
        attackTimer = 0f;
        StartCoroutine(AttackCooldown());
    }

    // --- ATTACK LOGIC ---

    private void UpdateAttack()
    {
        attackTimer += Time.deltaTime;
        float t = attackTimer / attackDuration;
        float currentRange = Mathf.Lerp(1f, attackRange, t);
        float currentRadius = Mathf.Lerp(attackRadius, maxAttackRadius, t);
		
		ShowAnimTest(t);

		// Perform multi-hit SphereCast
        RaycastHit[] hits = Physics.SphereCastAll(
            attackOrigin.position,
            currentRadius,
            attackOrigin.forward,
            currentRange,
            hitMask,
            QueryTriggerInteraction.Ignore
        );

        foreach (RaycastHit hit in hits)
        {
			if(hit.collider.gameObject == gameObject || hit.collider.CompareTag("World")) continue;
			
			if (!wereHit.Add(hit.collider.gameObject)) continue;  // Add returns false if already present
			
            // Calculate direction from player to target
            Vector3 toTarget = (hit.point - attackOrigin.position).normalized;

            // Check if target is within allowed attack angle
            float angleToTarget = Vector3.Angle(attackOrigin.forward, toTarget);
            if (angleToTarget > attackAngle) continue; // skip targets outside cone

            // Apply damage if target has a health component
			var targetHealth = hit.collider.GetComponent<HealthController>();
			var targetCombat = hit.collider.GetComponent<SwordController>();
			float targetDamage = Mathf.Lerp(damage / 4f, damage, t);
			float dotProduct = Vector3.Dot((transform.position - hit.transform.position), hit.transform.forward);
			if(dotProduct < -0.5f)
			{
				targetDamage *= 3f;
			 	print("Hit from behind");
			}
            if (targetHealth)
            {
				if (targetCombat && targetCombat.TryParry(gameObject))
				{
					Debug.Log("Attack was parried!");
					
					isAttacking = false; // optional: cancel your attack
					return;              // stop further damage/knockback
				}
				
				if (targetCombat && targetCombat.isBlocking)
				{
					Debug.Log("Attack blocked!");
					// Apply reduced damage instead of full
					float reducedDamage = targetDamage * 0.3f;
					targetHealth.TryKill(reducedDamage);
					return;
				}
				
                targetHealth.TryKill(damage);
                Debug.DrawLine(attackOrigin.position, hit.point, Color.red, 0.1f);
				print("Dealing damage: " + targetDamage);
            }
			
			float force = Mathf.Lerp(10f, 2f, t) * Time.fixedDeltaTime * 60f;
			if(hit.collider.attachedRigidbody != null)
			{
				hit.collider.attachedRigidbody.AddForce(toTarget * force, ForceMode.Impulse);
			}
			
			var aiController = hit.collider.GetComponent<AIController>();
			if(aiController !=null)
			{
				aiController.lastKnownPlayerPosition = aiController.player.position;
				if(aiController.currentState is AIPatrolState || aiController.currentState is AISuspisciousState)
				{
					if(aiController.gameObject.GetComponent<AIFightingState>() == null)
					{
						aiController.SetState(gameObject.AddComponent<AIFightingState>());
					}
					else
					{
						aiController.SetState(gameObject.GetComponent<AIFightingState>());
					}
				}
			}
			
        }

        if (attackTimer >= attackDuration)
        {
            isAttacking = false;
			wereHit.Clear();
        }
    }

    IEnumerator AttackCooldown()
    {
        onCooldown = true;
        yield return new WaitForSeconds(attackCooldown);
        onCooldown = false;
    }

	/*
    // --- PARRY LOGIC ---

    private void UpdateParry()
    {
        parryTimer += Time.deltaTime;
        if (parryTimer >= parryWindow)
        {
            isParrying = false;
        }
    }

    IEnumerator ParryCooldown()
    {
        onCooldown = true;
        yield return new WaitForSeconds(parryCooldown);
        onCooldown = false;
    }
	
    private void OnDrawGizmosSelected()
    {
        if (!attackOrigin) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(attackOrigin.position * attackRange, maxAttackRadius);
    }
	*/
	// Placeholder animation to be used by mannequins.
	
	[Header("Anim Settings")]
    public Transform swordObj;
	void ShowAnimTest(float pos)
	{
        if(!swordObj) return;
		
		float rotationX = Mathf.Lerp(-(attackAngle / 2f), (attackAngle / 2f), pos);
		float rotationY = Mathf.Lerp(-15f, 20f, pos);
		
		swordObj.localRotation = Quaternion.Euler(rotationY, rotationX, 0f);
	}
	
	
	[Header("Defense Settings")]
	public float parryWindow = 0.25f;        // timing window for parry
	public float parryPushForce = 8f;        // how strong the pushback is

	public bool isBlocking = false;
	private bool isParrying = false;
	private bool parryActive = false;
	private float parryTimer = 0f;

	// Called externally from PlayerInput (ex: when right mouse button pressed)
	public void OnBlockPressed()
	{
		// Start parry window
		parryActive = true;
		parryTimer = 0f;
		isBlocking = true; // holding starts blocking
	}

	public void OnBlockReleased()
	{
		isBlocking = false;
	}

	// Called each frame
	private void UpdateParryAndBlock()
	{
		if (parryActive)
		{
			parryTimer += Time.deltaTime;
			if (parryTimer >= parryWindow)
				parryActive = false; // parry window ends, but block can remain
		}

		// Update HealthSystem block flag if you have a Health component on player
		HealthController hp = GetComponent<HealthController>();
		if (hp) hp.isBlocking = isBlocking;
	}
	
	public bool TryParry(GameObject attacker)
	{
		if (parryActive)
		{
			// Successful parry
			Rigidbody rb = attacker.GetComponent<Rigidbody>();
			if (rb)
			{
				Vector3 pushDir = (attacker.transform.position - transform.position).normalized;
				rb.AddForce(pushDir * parryPushForce, ForceMode.Impulse);
			}

			Debug.Log("Parry successful! Enemy pushed back.");
			return true;
		}

		return false;
	}
}
