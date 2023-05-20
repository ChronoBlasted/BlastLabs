using BaseTemplate.Behaviours;
using Nakama;
using Nakama.TinyJson;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public enum MATCH_STATE { WAIT, PLAYER_TURN, OPONENT_TURN, ENDED }

public class MatchManager : MonoSingleton<MatchManager>
{
    NakamaManager _nakamaManager;

    MATCH_STATE _matchState;

    IMatch _match;
    IUserPresence _localPresence, _oponentPresence, _hostPresence;

    public void Init(IMatch newMatch, IUserPresence localPresence, IUserPresence oponentPresence, IUserPresence hostPresence)
    {
        _nakamaManager = NakamaManager.Instance;

        _nakamaManager.Socket.ReceivedMatchState += m => UnityMainThreadDispatcher.Instance.Enqueue(() => OnReceivedMatchState(m));

        _match = newMatch;
        _localPresence = localPresence;
        _oponentPresence = oponentPresence;
        _hostPresence = hostPresence;

        //DrawHand

        if (_localPresence.SessionId == _hostPresence.SessionId)
        {
            var opCode = 1;
            var whoStartString = "";

            var whoStart = Random.Range(0, 2);

            if (whoStart == 0)
            {
                _matchState = MATCH_STATE.PLAYER_TURN;
                whoStartString = localPresence.ToString();
                Debug.Log("Player First");
            }
            else
            {
                _matchState = MATCH_STATE.OPONENT_TURN;
                whoStartString = oponentPresence.ToString();
                Debug.Log("Oponents Turn");
            }

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
            case OPCodes.WHO_START:
                UpdateWhoStart(messageJson);
                break;
            case OPCodes.PLAYER_DROP_CARD:
                UpdateBoard(messageJson);
                break;
            default:
                break;
        }
    }

    void UpdateBoard(string state)
    {
    }

    void UpdateWhoStart(string state)
    {
        if (state == _localPresence.SessionId)
        {
            _matchState = MATCH_STATE.PLAYER_TURN;
            Debug.Log("Player First");
        }
        else
        {
            _matchState = MATCH_STATE.OPONENT_TURN;
            Debug.Log("Oponents Turn");
        }
    }
}
