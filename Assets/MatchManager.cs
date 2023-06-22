using BaseTemplate.Behaviours;
using Nakama;
using Nakama.TinyJson;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public enum MATCH_STATE { WAIT, PLAYER_TURN, OPONENT_TURN, ENDED }

public class MatchManager : MonoSingleton<MatchManager>
{
    NakamaManager _nakamaManager;
    GameManager _gameManager;
    UIManager _uIManager;

    MATCH_STATE _matchState;

    IMatch _match;
    IUserPresence _localPresence, _opponentPresence;

    List<CardData> _myDeck;

    public void Init()
    {
        _nakamaManager = NakamaManager.Instance;
        _gameManager = GameManager.Instance;
        _uIManager = UIManager.Instance;

        _nakamaManager.Socket.ReceivedMatchState += m => UnityMainThreadDispatcher.Instance.Enqueue(() => OnReceivedMatchState(m));
    }

    public void StartNewMatch(IMatch newMatch)
    {
        _match = newMatch;

        UIManager.Instance.GamePanel.DisablePlayerCards();
    }

    private void OnReceivedMatchState(IMatchState matchState)
    {
        string messageJson = Encoding.UTF8.GetString(matchState.State);

        Debug.Log(messageJson);
        Debug.Log(matchState.OpCode);

        switch (matchState.OpCode)
        {
            case OPCodes.MATCH_START:
                UIManager.Instance.GamePanel.DisablePlayerCards();

                _localPresence = _match.Self;

                foreach (var user in _match.Presences)
                {
                    Debug.Log("userName : " + user.Username);
                    if (user.UserId != _localPresence.UserId)
                    {
                        _opponentPresence = user;
                    }
                }

                _gameManager.UpdateStateToGame();
                break;

            case OPCodes.WHO_START:
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
        if (_localPresence.UserId == sessionIdOfPlayerTurn)
        {
            _matchState = MATCH_STATE.PLAYER_TURN;

            _uIManager.GamePanel.ActivePlayerCards();

            _uIManager.GamePanel.UpdateTurnText("your turn");
        }
        else
        {
            _matchState = MATCH_STATE.OPONENT_TURN;

            _uIManager.GamePanel.DisablePlayerCards();

            _uIManager.GamePanel.UpdateTurnText(_opponentPresence.Username + " turn's");
        }
    }

    public void MatchEnd(bool playerHaveWin)
    {
        _matchState = MATCH_STATE.ENDED;

        _uIManager.GamePanel.DisablePlayerCards();

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

[Serializable]
public class MatchStartData
{
    public int RoundTimer;
    public List<IUserPresence> Presences;
}
