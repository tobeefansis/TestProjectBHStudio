using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInfo : NetworkBehaviour
{
    [SerializeField] private TextMeshPro nameTextMesh;
    [SerializeField] private TextMeshPro killsTextMesh;
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private Transform playerTransform;
    [SerializeField] [Range(1, 20)] private int notTangibleCooldown;
    [Space(10)]
    [SerializeField] private string playerName;
    [SerializeField] private int killsCount;
    [SerializeField] private bool isDash;
    [SerializeField] private bool isTangible;
    [Space(10)]
    [SerializeField] private UnityEvent OnTangible;
    [SerializeField] private UnityEvent OnNotTangible;


    [SyncVar(hook = nameof(SyncKillsCount))]
    private int _SyncKillsCount;
    [SyncVar(hook = nameof(SyncTangible))]
    private bool _SyncIsTangible;
    [SyncVar(hook = nameof(SyncPlayerName))]
    private string _SyncPlayerName;


    public bool IsTangible
    {
        get => isTangible; set
        {
            isTangible = value;
            if (IsTangible)
            {
                OnTangible.Invoke();
            }
            else
            {
                OnNotTangible.Invoke();
            }
        }
    }
    public string PlayerName { get => playerName; set => playerName = value; }

  

    public Transform CameraTarget { get => cameraTarget; set => cameraTarget = value; }
    public Transform PlayerTransform { get => playerTransform; set => playerTransform = value; }

    #region MONO

    private void Start()
    {
        if (isClient && isLocalPlayer && hasAuthority)
        {

            if (NetManager.I)
            {
                NetManager.I.SetPlayerInfo(this);
            }
        }
        if (hasAuthority)
        {
            if (isServer)
                ChangePlayerNameValue(playerName);
            else
                CmdChangePlayerNameValue(playerName);
        }
        nameTextMesh.text = playerName;
        killsTextMesh.text = $"{killsCount} kills";

    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerInfo enamyPlayerInfo = other.gameObject.GetComponent<PlayerInfo>();
        print("OnTriggerEnter");
        if (enamyPlayerInfo && isDash && enamyPlayerInfo.IsTangible && IsTangible)
        {
            isDash = false;
            print("AddKillCount");
            AddKillCount();
            enamyPlayerInfo.Kill();
        }
    }
    #endregion

    #region SERVER

    [Server]
    public void ChangeKillsCount(int newValue)
    {
        _SyncKillsCount = newValue;
        if (NetManager.I)
        {
            if (newValue >= NetManager.I.NumberOfKillsToWin)
            {
                RpcEndGame();
            }
        }
        
    }
    [Server]
    public void ChangePlayerNameValue(string newValue)
    {
        _SyncPlayerName = newValue;
    }
    [Server]
    public void ChangeTangibleValue(bool newValue)
    {
        _SyncIsTangible = newValue;
    }
    #endregion

    #region COMMAND

    [Command]
    public void CmdChangeKillsCount(int newValue)
    {
        ChangeKillsCount(newValue);
    }

    [Command(requiresAuthority = false)]
    public void CmdChangeTangibleValue(bool newValue)
    {
        ChangeTangibleValue(newValue);
    }
   
    [Command]
    public void CmdChangePlayerNameValue(string newValue)
    {
        ChangePlayerNameValue(newValue);
    }
    [ClientRpc]
    public void RpcEndGame()
    {
        if (NetManager.I)
        {
            NetManager.I.EndGame(this);
        }
    }
    #endregion

    public void Restart()
    {
        if (isServer)
        {
            NetworkManager.singleton.ServerChangeScene("SampleScene");
        }
    }
    public void Kill()
    {
        StartCoroutine(NotTangiblePlayer());
    }
    public void AddKillCount()
    {
        if (hasAuthority)
        {
            if (isServer)
                ChangeKillsCount(killsCount + 1);
            else
                CmdChangeKillsCount(killsCount + 1);
        }

    }

    public void SetDashStatus(bool isDash)
    {
        this.isDash = isDash;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        SetTangible(true);
    }

    public void SyncTangible(bool oldValue, bool newValue)
    {
        IsTangible = newValue;
    }

    private void SyncKillsCount(int oldValue, int newValue)
    {
        killsCount = newValue;
        killsTextMesh.text = $"{killsCount} kills";

    }


    private IEnumerator NotTangiblePlayer()
    {
        SetTangible(false);

        yield return new WaitForSeconds(notTangibleCooldown);
        SetTangible(true);
    }

    private void SetTangible(bool v)
    {
        if (isServer)
            ChangeTangibleValue(v);
        else
            CmdChangeTangibleValue(v);
    }
   
    private void SyncPlayerName(string oldValue, string newValue)
    {
        playerName = newValue;
        nameTextMesh.text = playerName;
    }
}
