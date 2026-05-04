using UnityEngine;

public class GoalZone : MonoBehaviour
{
    // when player enters the goal trigger, complete the level
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Level completed!");
            SessionManager.Instance.CompleteLevel();
        }
    }
}