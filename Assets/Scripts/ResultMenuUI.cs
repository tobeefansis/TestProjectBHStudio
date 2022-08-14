using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResultMenuUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gameResult;
    #region MONO
    private void Start()
    {
        if (NetManager.I)
        {
            NetManager.I.OnEndGame.AddListener(ShowResultMenu);
        }
        gameObject.SetActive(false);
    }
    #endregion

    public void ShowResultMenu(PlayerInfo winner)
    {
        gameResult.text = $"{winner.PlayerName} Win";
        gameObject.SetActive(true);
    }
}
