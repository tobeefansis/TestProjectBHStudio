using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class NetManager : NetworkManager
{
    private static NetManager i;

    [SerializeField] private CameraMovement cameraMovement;
    [SerializeField] private PlayerInfo playerInfo;
    [SerializeField] string playerName;
    [SerializeField] [Range(1, 20)] private int numberOfKillsToWin;
    [SerializeField] [Range(1, 20)] private float timeToLoadNextLevel = 5;
    [SerializeField] private UnityEvent<PlayerInfo> onEndGame;

    public static NetManager I { get => i; set => i = value; }
    public int NumberOfKillsToWin { get => numberOfKillsToWin; set => numberOfKillsToWin = value; }
    public UnityEvent<PlayerInfo> OnEndGame { get => onEndGame; set => onEndGame = value; }

    #region MONO
    public override void Awake()
    {
        if (i)
        {
            Destroy(gameObject);
        }
        else
        {
            i = this;
        }
    }

    private void OnGUI()
    {
        if (!NetworkClient.isConnected && !NetworkServer.active)
        {
            GUILayout.BeginArea(new Rect(10, 10, 215, 9999));
            playerName = GUILayout.TextField(playerName);
            GUILayout.EndArea();
        }
    }
    #endregion
    public void SetPlayerInfo(PlayerInfo playerInfo)
    {
        this.playerInfo = playerInfo;
        cameraMovement.SetTargetPlayer(playerInfo);
        playerInfo.PlayerName = playerName;
    }

    public void EndGame(PlayerInfo winner)
    {
        OnEndGame.Invoke(winner);
        StartCoroutine(RestartScene());
    }

    IEnumerator RestartScene()
    {
        yield return new WaitForSeconds(timeToLoadNextLevel);
        playerInfo.Restart();
    }
}
