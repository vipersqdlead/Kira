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
    public GameObject controlsHelp;
    //public Image healthBar;
    public Slider healthBar, hitBar;
    public float lerpSpeed = 0.05f;
    public DeathCamera deathCam;
    public FadeInOut fadeinOut;

    [Header("animation")]
    public Animator livesAnimation;
    void Start()
    {
        StartGame();
    }

    // Update is called once per frame
    void Update()
    {
        if(player != null)
        {
            deathCam.gameObject.transform.position = Camera.main.transform.position;
            deathCam.gameObject.transform.rotation = Camera.main.transform.rotation;
        }

        if(Input.GetKeyDown(KeyCode.R) || player == null || player.transform.position.y < -10f)
		{
			OnPlayerDeath();
            Invoke("RestartLevel", 1f);
		}
        if(Input.GetKeyDown(KeyCode.F1))
        {
            controlsHelp.SetActive(!controlsHelp.activeSelf);
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

        healthBar.value = hp.hpPercent / 100f;
        staminaBar.fillAmount = hp.stamina;
        if(healthBar.value != hitBar.value)
        {
            hitBar.value = Mathf.Lerp(hitBar.value, healthBar.value, lerpSpeed);
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
        fadeinOut.ActivateFadeOut = true;
        Time.timeScale = 0.2f;
        Debug.Log("Boss fight started!");
        Invoke("ReturnToMenu", 1.5f);
        // Trigger boss intro, lock arena doors, etc.
    }

    void ReturnToMenu()
    {
        Cursor.lockState = CursorLockMode.None; // lock cursor in the center
        Cursor.visible = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnBossDefeated()
    {
        currentState = GameState.Victory;
        Debug.Log("Victory!");
        // Trigger cutscene or end screen
    }

    public void OnPlayerDeath()
    {
        if(player != null)
        {
            Destroy(player);
        }
        livesAnimation.SetInteger("Vidas", 0);
        deathCam.gameObject.SetActive(true);
        fadeinOut.ActivateFadeOut = true;
        Time.timeScale = 0.2f;
        currentState = GameState.GameOver;
        Debug.Log("Game Over!");
        // Trigger game over screen or restart
    }
}
