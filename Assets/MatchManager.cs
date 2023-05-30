using BaseTemplate.Behaviours;
using Nakama;
using Nakama.TinyJson;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

public enum MATCH_STATE { WAIT, PLAYER_TURN, OPONENT_TURN, ENDED }

public class MatchManager : MonoSingleton<MatchManager>
{
    NakamaManager _nakamaManager;

    MATCH_STATE _matchState;

    IMatch _match;
    IUserPresence _localPresence, _oponentPresence, _hostPresence;

    List<CardData> _myDeck;

    public void Init()
    {
        _nakamaManager = NakamaManager.Instance;

        _nakamaManager.Socket.ReceivedMatchState += m => UnityMainThreadDispatcher.Instance.Enqueue(() => OnReceivedMatchState(m));
    }

    public void StartNewMatch(IMatch newMatch, IUserPresence localPresence, IUserPresence oponentPresence, IUserPresence hostPresence)
    {
        _match = newMatch;
        _localPresence = localPresence;
        _oponentPresence = oponentPresence;
        _hostPresence = hostPresence;

        UIManager.Instance.GamePanel.DisablePlayerCards();

    }

    private void OnReceivedMatchState(IMatchState matchState)
    {
        string messageJson = Encoding.UTF8.GetString(matchState.State);

        Debug.Log(messageJson);
        Debug.Log(matchState.OpCode);

        switch (matchState.OpCode)
        {
            case OPCodes.UPDATE_TURN:
                UpdateTurn(messageJson);
                break;
            case OPCodes.PLAYER_DROP_CARD:
                var positionState = JsonParser.FromJson<PositionState>(messageJson);
                UpdateBoard(positionState);
                break;
            default:
                break;
        }
    }

    public bool DropCard(int indexPos, int indexOfCard)
    {
        if (_matchState != MATCH_STATE.PLAYER_TURN) return false;

        // Try catch with server

        BoardManager.Instance.DropCard(indexPos, indexOfCard, true);

        return true;
    }

    void UpdateBoard(PositionState state)
    {
        BoardManager.Instance.DropCard(state.Position, state.IndexOfCard, false);
    }

    void UpdateTurn(string sessionIdOfPlayerTurn)
    {
        if (sessionIdOfPlayerTurn == _localPresence.SessionId)
        {
            _matchState = MATCH_STATE.PLAYER_TURN;

            UIManager.Instance.GamePanel.ActivePlayerCards();

            BoardManager.Instance.UpdateTurnText(_localPresence.Username + " turn's");
        }
        else
        {
            _matchState = MATCH_STATE.OPONENT_TURN;

            UIManager.Instance.GamePanel.DisablePlayerCards();

            BoardManager.Instance.UpdateTurnText(_oponentPresence.Username + " turn's");
        }
    }

    public void MatchEnd(bool playerHaveWin)
    {
        _matchState = MATCH_STATE.ENDED;

        UIManager.Instance.GamePanel.DisablePlayerCards();

        if (playerHaveWin)
        {
            BoardManager.Instance.UpdateTurnText("You win");
        }
        else
        {
            BoardManager.Instance.UpdateTurnText("You loose");
        }

        GameManager.Instance.UpdateStateToEnd();
    }


}

[Serializable]
public class PositionState
{
    public int Position;
    public int IndexOfCard;
}
