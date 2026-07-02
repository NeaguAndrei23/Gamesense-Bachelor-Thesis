using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardZone : MonoBehaviour
{
    // when player enters the hazard trigger, notify the SessionManager of a death
    public string zoneName;
    [Tooltip("Optional: Describe details about the zone, like name or other notes")]
    public string zoneDescription;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SessionManager.Instance.RegisterDeath(zoneName);
            other.GetComponent<PlayerControls>().Respawn(new Vector3(-10, -2.5f, 0));
        }
    }
}