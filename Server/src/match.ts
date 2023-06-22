const matchInit = function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, params: { [key: string]: string }): { state: nkruntime.MatchState, tickRate: number, label: string } {
    logger.debug('Lobby match created');


    return {
        state: { presences: {}, emptyTicks: 0, started: false, countReadyPing: 0 },
        tickRate: 1, // 1 tick per second = 1 MatchLoop func invocations per second
        label: 'duel',
    };
};


const matchJoinAttempt = function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, dispatcher: nkruntime.MatchDispatcher, tick: number, state: nkruntime.MatchState, presence: nkruntime.Presence, metadata: { [key: string]: any }): { state: nkruntime.MatchState, accept: boolean, rejectMessage?: string | undefined } | null {
    logger.debug('%q attempted to join Lobby match', ctx.userId);

    if (state.started) {
        return {
            state,
            accept: false
        };
    }

    return {
        state,
        accept: true
    };
}

const matchJoin = function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, dispatcher: nkruntime.MatchDispatcher, tick: number, state: nkruntime.MatchState, presences: nkruntime.Presence[]): { state: nkruntime.MatchState } | null {
    presences.forEach(function (presence) {
        state.presences[presence.userId] = presence;
        logger.debug('%q joined Lobby match', presence.userId);
    });

    logger.debug('presences LENGTHHHHHHHHHHHHH :' + Object.keys(state.presences).length);


    // if (Object.keys(state.presences).length === 2) {

    //     // Match Started
    //     state.started = true;
    //     dispatcher.broadcastMessage(OPCODE_matchStart, JSON.stringify(matchStartData), null, null, true);


    //     // Who Start
    //     const playerIDs = Object.keys(state.presences);
    //     const randomIndex = Math.floor(Math.random() * playerIDs.length);
    //     const starterUserID = playerIDs[randomIndex];
    //     dispatcher.broadcastMessage(OPCODE_whoStart, JSON.stringify(starterUserID), null, null, true);

    //     // Players deck

    //     presences.forEach(function (presence) {
    //         // get random hand
    //         presence.userId
    //     });

    // }

    return {
        state
    };
}

const matchLeave = function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, dispatcher: nkruntime.MatchDispatcher, tick: number, state: nkruntime.MatchState, presences: nkruntime.Presence[]): { state: nkruntime.MatchState } | null {
    presences.forEach(function (presence) {
        delete (state.presences[presence.userId]);
        logger.debug('%q left Lobby match', presence.userId);
    });

    return {
        state
    };
}

const matchLoop = function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, dispatcher: nkruntime.MatchDispatcher, tick: number, state: nkruntime.MatchState, messages: nkruntime.MatchMessage[]): { state: nkruntime.MatchState } | null {

    if (Object.keys(state.presences).length === 0) {
        state.emptyTicks++;
    }

    if (state.emptyTicks > 5) {
        return null;
    }

    messages.forEach(function (message) {
        logger.info('Received %v from %v', message.data, message.sender.userId);

        if (message.opCode === OPCODE_readyState) {
            state.countReadyPing++;

            if (state.countReadyPing == 2) {
                dispatcher.broadcastMessage(OPCODE_matchStart, JSON.stringify(matchStartData), null, null, true);

                const playerIDs = Object.keys(state.presences);
                const randomIndex = Math.floor(Math.random() * playerIDs.length);
                const starterUserID = playerIDs[randomIndex];

                dispatcher.broadcastMessage(OPCODE_whoStart, starterUserID, null, null, true);
            }
        }

        if (message.opCode === OPCODE_playerDropCard) {

        }
    });

    return {
        state
    };
}

const matchTerminate = function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, dispatcher: nkruntime.MatchDispatcher, tick: number, state: nkruntime.MatchState, graceSeconds: number): { state: nkruntime.MatchState } | null {
    logger.debug('Lobby match terminated');

    const message = `Server shutting down in ${graceSeconds} seconds.`;
    dispatcher.broadcastMessage(2, message, null, null);

    return {
        state
    };
}

const matchSignal = function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, dispatcher: nkruntime.MatchDispatcher, tick: number, state: nkruntime.MatchState, data: string): { state: nkruntime.MatchState, data?: string } | null {
    logger.debug('Lobby match signal received: ' + data);

    return {
        state,
        data: "Lobby match signal received: " + data
    };
}

const matchStartData = {
    roundTimer: 45,
};