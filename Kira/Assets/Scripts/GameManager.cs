using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject player;
	
	[Header("HUD")]
	public TMP_Text continues, swordState;
	public Image healthBar, staminaBar, swordBar;
	
    void Start()
    {
        StartGame();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R) || player == null)
		{
			OnPlayerDeath();
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
		HUD();
    }
	
	void HUD()
	{
		var hp = player.GetComponent<HealthController>();
		var sword = player.GetComponent<SwordController>();
		
		if(hp.extraLives == 0)
			continues.text = "";
		else
			continues.text = "(" + hp.extraLives + ")";
		
		healthBar.fillAmount = hp.hpPercent / 100f;
		staminaBar.fillAmount = hp.stamina;
		
		if(sword.isBlocking)
			swordState.text = "Blocking";
		else
			swordState.text = "";
		
		swordBar.fillAmount = sword.attackTimer / sword.attackDuration;
	}
	
	public enum GameState { Playing, BossFight, Victory, GameOver }

    public static GameManager Instance { get; private set; }

    public GameState currentState = GameState.Playing;
    public GameObject boss;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
	
    public void StartGame()
    {
        currentState = GameState.Playing;
        // Initialize player, HUD, etc.
    }

    public void OnBossRoomEntered(GameObject bossRef)
    {
        currentState = GameState.BossFight;
        boss = bossRef;
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
