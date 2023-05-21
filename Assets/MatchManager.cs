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

    public void Init(IMatch newMatch, IUserPresence localPresence, IUserPresence oponentPresence, IUserPresence hostPresence)
    {
        _nakamaManager = NakamaManager.Instance;

        _nakamaManager.Socket.ReceivedMatchState += m => UnityMainThreadDispatcher.Instance.Enqueue(() => OnReceivedMatchState(m));

        _match = newMatch;
        _localPresence = localPresence;
        _oponentPresence = oponentPresence;
        _hostPresence = hostPresence;

        UIManager.Instance.GamePanel.DisablePlayerCards();

        _myDeck = CardManager.Instance.GetRandomHand();

        for (int i = 0; i < _myDeck.Count; i++)
        {
            BoardManager.Instance.PlayerCard[i].Init(_myDeck[i], true);
        }

        BoardManager.Instance.Init();

        //Update UI

        if (_localPresence.SessionId == _hostPresence.SessionId)
        {
            var opCode = 1;
            var whoStartString = "";

            var whoStart = Random.Range(0, 2);

            if (whoStart == 0)
            {
                _matchState = MATCH_STATE.PLAYER_TURN;
                whoStartString = localPresence.SessionId;
            }
            else
            {
                _matchState = MATCH_STATE.OPONENT_TURN;
                whoStartString = oponentPresence.SessionId;
            }

            UpdateTurn(whoStartString);

            _nakamaManager.Socket.SendMatchStateAsync(_match.Id, opCode, whoStartString);
        }
    }

    private void OnReceivedMatchState(IMatchState matchState)
    {
        string messageJson = System.Text.Encoding.UTF8.GetString(matchState.State);

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

    public async void DropCard(int indexPos, int indexOfCard)
    {
        if (_matchState != MATCH_STATE.PLAYER_TURN) return;

        var opCode = 2;

        var state = new PositionState
        {
            Position = indexPos,
            IndexOfCard = indexOfCard,
        };

        BoardManager.Instance.DropCard(state.Position, state.IndexOfCard, true);

        await _nakamaManager.Socket.SendMatchStateAsync(_match.Id, opCode, JsonWriter.ToJson(state));

        if (BoardManager.Instance.UpdateAdjacentCard(state.Position, false) == false)
        {
            //Update Turn
            opCode = 1;
            await _nakamaManager.Socket.SendMatchStateAsync(_match.Id, opCode, _oponentPresence.SessionId);

            UpdateTurn(_oponentPresence.SessionId);
        }
    }

    void UpdateBoard(PositionState state)
    {
        BoardManager.Instance.DropCard(state.Position, state.IndexOfCard, false);

        BoardManager.Instance.UpdateAdjacentCard(state.Position, true);
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

        //GameManager.Instance.UpdateStateToEnd();
    }
}

[Serializable]
public class PositionState
{
    public int Position;
    public int IndexOfCard;
}
