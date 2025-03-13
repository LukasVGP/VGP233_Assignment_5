using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Leaderboards;
using Newtonsoft.Json;

public class UGS_Manager : MonoBehaviour
{
    private static UGS_Manager _instance;
    public static UGS_Manager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("UGS_Manager instance is null!");
            }
            return _instance;
        }
    }

    [SerializeField] private string environmentId = "production";
    [SerializeField] private string leaderboardId = "main_leaderboard";

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private async void Start()
    {
        await InitializeUGS();
    }

    private async Task InitializeUGS()
    {
        try
        {
            var options = new InitializationOptions()
                .SetEnvironmentName(environmentId);

            await UnityServices.InitializeAsync(options);

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            Debug.Log($"UGS initialized. Player ID: {AuthenticationService.Instance.PlayerId}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to initialize UGS: {e.Message}");
        }
    }

    public async Task<bool> SubmitScore(int score)
    {
        try
        {
            var scoreResponse = await LeaderboardsService.Instance.AddPlayerScoreAsync(leaderboardId, score);
            Debug.Log($"Score submitted: {scoreResponse.Score}");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error submitting score: {e.Message}");
            return false;
        }
    }

    public async Task<List<LeaderboardEntry>> GetTopScores(int count = 10)
    {
        try
        {
            var scoresResponse = await LeaderboardsService.Instance.GetScoresAsync(leaderboardId, new GetScoresOptions { Limit = count });

            var entries = new List<LeaderboardEntry>();
            foreach (var score in scoresResponse.Results)
            {
                entries.Add(new LeaderboardEntry
                {
                    Rank = score.Rank,
                    PlayerName = score.PlayerName,
                    Score = (long)score.Score // Added explicit cast from double to long
                });
            }

            return entries;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error getting leaderboard: {e.Message}");
            return new List<LeaderboardEntry>();
        }
    }

    public async Task<LeaderboardEntry> GetPlayerScore()
    {
        try
        {
            var scoreResponse = await LeaderboardsService.Instance.GetPlayerScoreAsync(leaderboardId);

            return new LeaderboardEntry
            {
                Rank = scoreResponse.Rank,
                PlayerName = AuthenticationService.Instance.PlayerName ?? AuthenticationService.Instance.PlayerId,
                Score = (long)scoreResponse.Score // Added explicit cast from double to long
            };
        }
        catch (Exception e)
        {
            Debug.LogError($"Error getting player score: {e.Message}");
            return null;
        }
    }
}

[Serializable]
public class LeaderboardEntry
{
    public int Rank;
    public string PlayerName;
    public long Score;
}