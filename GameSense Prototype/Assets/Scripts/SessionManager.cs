using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class SessionManager : MonoBehaviour
{
    public static SessionManager Instance; // singleton pattern

    [Header("Level Id Configuration")]
    [SerializeField] private string levelId = "Level_Unity";
    private string apiBaseUrl = "https://localhost:7146";
    private string playerId = "UnityPlayer_1";

    private DateTime sessionStart;
    private int deathCount = 0;
    private List<string> deathZones = new List<string>();

    void Awake()
    {
        // singleton setup 
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        if (string.IsNullOrWhiteSpace(levelId))
        {
            Debug.LogWarning("SessionManager: levelId is empty! Make sure to set it in the SessionManager object");
        }
        // record when the session started
        sessionStart = DateTime.UtcNow;
        
    }

    // called by HazardZone when player dies
    public void RegisterDeath(string zoneName)
    {
        deathCount++;
        deathZones.Add(zoneName);
        Debug.Log($"Death registered in zone: {zoneName}. Total deaths: {deathCount}");
    }

    // called when player completes the level
    public void CompleteLevel()
    {
        StartCoroutine(SendSessionData(true));
    }

    // called when player quits the level
    public void QuitLevel()
    {
        StartCoroutine(SendSessionData(false));
    }

    private IEnumerator SendSessionData(bool isCompleted)
    {
        double duration = (DateTime.UtcNow - sessionStart).TotalSeconds;
        Debug.Log($"Sending session data - Deaths: {deathCount}, Completed: {isCompleted}, Duration: {duration}");
        // build session JSON
        string sessionJson = $@"{{
            ""playerId"": ""{playerId}"",
            ""levelId"": ""{levelId}"",
            ""startTime"": ""{sessionStart:yyyy-MM-ddTHH:mm:ss}"",
            ""durationSeconds"": {duration:F1},
            ""deaths"": {deathCount},
            ""attempts"": {deathCount + 1},
            ""isCompleted"": {isCompleted.ToString().ToLower()}
        }}";

        // send session to API
        yield return StartCoroutine(PostJson(apiBaseUrl + "/api/sessions", sessionJson, (sessionId) =>
        {
            // send each death event
            foreach (var zone in deathZones)
            {
                string deathJson = $@"{{""sessionId"": {sessionId}, ""zoneId"": ""{zone}""}}";
                StartCoroutine(PostJson(apiBaseUrl + "/api/deathevents", deathJson, null));
            }
        }));
    }

    private IEnumerator PostJson(string url, string json, Action<int> onSuccess)
    {
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"POST success: {url}");
            if (onSuccess != null)
            {
                // parse the session id from response
                string response = request.downloadHandler.text;
                var sessionData = JsonUtility.FromJson<SessionResponse>(response);
                onSuccess(sessionData.id);
            }
        }
        else
        {
            Debug.LogError($"POST failed: {request.error}");
        }
        request.Dispose(); //avoid memory leak
    }
}

[Serializable]
public class SessionResponse
{
    public int id;
}