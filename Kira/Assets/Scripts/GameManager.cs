using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject player;
	public float missionTimer;
	
	[Header("HUD")]
	public TMP_Text continues, swordState, battleTimer;
	public TMP_Text demoCompleteText;
    public Image staminaBar, swordBar, inmortalityImage;
    //public Image healthBar;
    public Slider healthBar, hitBar;
    public float lerpSpeed = 0.05f;
    
    [Header("animation")]
    public Animator livesAnimation;
    void Start()
    {
        StartGame();
    }

    // Update is called once per frame
    void Update()
    {
    
        if(Input.GetKeyDown(KeyCode.R) || player == null || player.transform.position.y < -10f)
		{
			OnPlayerDeath();
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
		HUD();
		missionTimer += Time.deltaTime;
    }
	
	void HUD()
	{
		var hp = player.GetComponent<HealthController>();
		var sword = player.GetComponent<SwordController>();
		
        livesAnimation.SetInteger("Vidas", hp.extraLives);

        if(hp.invulnerableTimer > 0)
        {
            inmortalityImage.enabled = true;
        }
        else
        {
           inmortalityImage.enabled = false;
        }
        
		if(hp.extraLives == 0)
			continues.text = "";
		else
			continues.text = "(" + hp.extraLives + ")";

        //healthBar.fillAmount = hp.hpPercent / 100f;
        if(healthBar.value != hp.HP)
        {
            healthBar.value = hp.HP;
        }
        staminaBar.fillAmount = hp.stamina;
        if(healthBar.value != hitBar.value)
        {
        hitBar.value = Mathf.Lerp(hitBar.value,hp.HP,lerpSpeed);
            
        }

		if(sword.isBlocking)
			swordState.text = "Blocking";
		else
			swordState.text = "";
		
		swordBar.fillAmount = sword.attackTimer / sword.attackDuration;
		
		battleTimer.text = "Time: " + TimeSpan.FromSeconds(missionTimer).ToString(@"mm\:ss");
	}
	
	public enum GameState { Playing, BossFight, Victory, GameOver }

    public GameState currentState = GameState.Playing;
    public GameObject boss;


	
    public void StartGame()
    {
		player = GameObject.FindWithTag("Player");
        currentState = GameState.Playing;
        // Initialize player, HUD, etc.
    }

    public void OnBossRoomEntered(GameObject bossRef)
    {
        currentState = GameState.BossFight;
        boss = bossRef;
        float timeToFinish = missionTimer;
        string txt = "Finished! Time to Level Complete was " + TimeSpan.FromSeconds(timeToFinish).ToString(@"mm\:ss");
        demoCompleteText.text = txt;
        Debug.Log("Boss fight started!");
        // Trigger boss intro, lock arena doors, etc.
    }

    public void OnBossDefeated()
    {
        currentState = GameState.Victory;
        Debug.Log("Victory!");
        // Trigger cutscene or end screen
    }

    public void OnPlayerDeath()
    {
        currentState = GameState.GameOver;
        Debug.Log("Game Over!");
        // Trigger game over screen or restart
    }
}
