using UnityEngine;

public class BossTriggerZone : MonoBehaviour
{
    public GameObject bossToActivate;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
			GameManager manager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();;
            manager.OnBossRoomEntered(bossToActivate);
            bossToActivate.SetActive(true);
            gameObject.SetActive(false); // Disable the trigger after use
        }
    }
}