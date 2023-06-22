"use strict";
var InitModule = function (ctx, logger, nk, initializer) {
    // Set up hooks.
    initializer.registerAfterAuthenticateDevice(afterAuthenticate);
    initializer.registerAfterAuthenticateFacebook(afterAuthenticate);
    initializer.registerAfterAuthenticateEmail(afterAuthenticate);
    // Set up RPCs
    // Blast
    initializer.registerRpc('swap_deck_card', rpcSwapDeckCard);
    initializer.registerRpc('upgrade_card', rpcUpgradeCard);
    initializer.registerRpc('load_user_cards', rpcLoadUserCards);
    initializer.registerRpc('add_random_card', rpcBuyRandomCard);
    // Match
    initializer.registerRpc('search_match', rpcFindOrCreateMatch);
    initializer.registerMatch('duel', {
        matchInit: matchInit,
        matchJoinAttempt: matchJoinAttempt,
        matchJoin: matchJoin,
        matchLeave: matchLeave,
        matchLoop: matchLoop,
        matchSignal: matchSignal,
        matchTerminate: matchTerminate
    });
    logger.warn('XXXXXXXXXXXXXXXXXXXX - Blast Labs TypeScript loaded - XXXXXXXXXXXXXXXXXXXX');
};
function afterAuthenticate(ctx, logger, nk, data) {
    if (!data.created) {
        logger.info('User with id: %s account data already existing', ctx.userId);
        return;
    }
    var user_id = ctx.userId;
    var username = "Player" + ctx.username;
    var metadata = {
        battle_pass: false,
        win: 0,
        loose: 0,
        total_card: 0,
    };
    var displayName = "NewPlayer";
    var timezone = null;
    var location = null;
    var langTag = "EN";
    var avatarUrl = null;
    var changeset = {
        'coins': 0,
        'gems': 0,
        'rank': 0,
    };
    var writeCards = {
        collection: DeckCollectionName,
        key: DeckCollectionKey,
        permissionRead: DeckPermissionRead,
        permissionWrite: DeckPermissionWrite,
        value: defaultCardCollection(nk, logger, ctx.userId),
        userId: ctx.userId,
    };
    try {
        nk.storageWrite([writeCards]);
    }
    catch (error) {
        logger.error('storageWrite error: %q', error);
        throw error;
    }
    try {
        nk.walletUpdate(user_id, changeset);
    }
    catch (error) {
        logger.error('Error init new wallet : %s', error);
    }
    try {
        nk.accountUpdateId(user_id, username, displayName, timezone, location, langTag, avatarUrl, metadata);
    }
    catch (error) {
        logger.error('Error init update account : %s', error);
    }
    logger.debug('new user id: %s account data initialised', ctx.userId);
}
var DeckPermissionRead = 2;
var DeckPermissionWrite = 0;
var DeckCollectionName = 'card_collection';
var DeckCollectionKey = 'user_cards';
var DefaultDeck = [
    {
        id: 0,
        level: 1,
    },
    {
        id: 1,
        level: 1,
    },
    {
        id: 2,
        level: 1,
    },
    {
        id: 3,
        level: 1,
    },
    {
        id: 4,
        level: 1,
    },
    {
        id: 5,
        level: 1,
    },
    {
        id: 6,
        level: 1,
    },
    {
        id: 7,
        level: 1,
    },
    {
        id: 8,
        level: 1,
    },
    {
        id: 9,
        level: 1,
    }
];
/**
 * Swap a card in the user deck with one in its collection.
 */
var rpcSwapDeckCard = function (ctx, logger, nk, payload) {
    var request = JSON.parse(payload);
    var userCards = loadUserBlast(nk, logger, ctx.userId);
    // Check the cards being swapper are valid.
    if (Object.keys(userCards.deckCards).indexOf(request.cardOutId) < 0) {
        throw Error('invalid out blast card');
    }
    if (Object.keys(userCards.storedCards).indexOf(request.cardInId) < 0) {
        throw Error('invalid in blast card');
    }
    // Swap the cards
    var outCard = userCards.deckCards[request.cardOutId];
    var inCard = userCards.storedCards[request.cardInId];
    delete (userCards.deckCards[request.cardOutId]);
    delete (userCards.storedCards[request.cardInId]);
    userCards.deckCards[request.cardInId] = inCard;
    userCards.storedCards[request.cardOutId] = outCard;
    // Store the changes
    storeUserCards(nk, logger, ctx.userId, userCards);
    logger.debug("user '%s' deck blast card '%s' swapped with '%s'", ctx.userId, outCard, inCard);
    return JSON.stringify(userCards);
};
/**
 * Upgrade the level of a given card in the user collection.
 */
var rpcUpgradeCard = function (ctx, logger, nk, payload) {
    var request = JSON.parse(payload);
    var userCards = loadUserBlast(nk, logger, ctx.userId);
    if (!userCards) {
        logger.error('user %s card blast collection not found', ctx.userId);
        throw Error('Internal server error');
    }
    var card_id = request.id;
    var card = userCards.deckCards[card_id];
    if (card) {
        card.level += 1;
        userCards.deckCards[card_id] = card;
    }
    card = userCards.storedCards[card_id];
    if (card) {
        card.level += 1;
        userCards.storedCards[card_id] = card;
    }
    if (!card) {
        logger.error('invalid blast card');
        throw Error('invalid blast card');
    }
    try {
        storeUserCards(nk, logger, ctx.userId, userCards);
    }
    catch (error) {
        // Error logged in storeUserCards
        throw Error('Internal server error');
    }
    logger.debug('user %s blast card %s upgraded', ctx.userId, JSON.stringify(card));
    return JSON.stringify(card);
};
/**
 * Get user card collection.
 */
var rpcLoadUserCards = function (ctx, logger, nk, payload) {
    return JSON.stringify(loadUserBlast(nk, logger, ctx.userId));
};
/**
 * Add a random card to the user collection
 */
var rpcBuyRandomCard = function (ctx, logger, nk, payload) {
    var _a;
    var id = Math.floor(Math.random() * BlastMaxID);
    var userCards;
    try {
        userCards = loadUserBlast(nk, logger, ctx.userId);
    }
    catch (error) {
        logger.error('error loading user blast cards: %s', error);
        throw Error('Internal server error');
    }
    var cardId = nk.uuidv4();
    var newCard = {
        id: id,
        level: 1,
    };
    userCards.storedCards[cardId] = newCard;
    try {
        storeUserCards(nk, logger, ctx.userId, userCards);
    }
    catch (error) {
        logger.error('error getting random blast card: %s', error);
        throw error;
    }
    logger.debug('user %s successfully add a random new card', ctx.userId);
    return JSON.stringify((_a = {}, _a[cardId] = newCard, _a));
};
function loadUserBlast(nk, logger, userId) {
    var storageReadReq = {
        key: DeckCollectionKey,
        collection: DeckCollectionName,
        userId: userId,
    };
    var objects;
    try {
        objects = nk.storageRead([storageReadReq]);
    }
    catch (error) {
        logger.error('blast : storageRead error: %s', error);
        throw error;
    }
    if (objects.length === 0) {
        throw Error('user blast cards storage object not found');
    }
    var storedCardCollection = objects[0].value;
    return storedCardCollection;
}
function storeUserCards(nk, logger, userId, cards) {
    try {
        nk.storageWrite([
            {
                key: DeckCollectionKey,
                collection: DeckCollectionName,
                userId: userId,
                value: cards,
                permissionRead: DeckPermissionRead,
                permissionWrite: DeckPermissionWrite,
            }
        ]);
    }
    catch (error) {
        logger.error('storageWrite blast error: %s', error);
        throw error;
    }
}
function getRandomHand(cardCollection, logger) {
    var deckCards = cardCollection.deckCards;
    var deckCardIds = Object.keys(deckCards);
    if (deckCardIds.length < 5) {
        throw new Error("Insufficient cards in the deck to create a random hand.");
    }
    var hand = [];
    var selectedCardIds = [];
    while (hand.length < 5) {
        var randomIndex = Math.floor(Math.random() * deckCardIds.length);
        var randomCardId = deckCardIds[randomIndex];
        var randomCard = deckCards[randomCardId];
        if (selectedCardIds.indexOf(randomCardId) > 0) {
            hand.push(randomCard);
            selectedCardIds.push(randomCardId);
        }
    }
    logger.info('RRRRRRRRRRRRRRRRRRRRRandom Hand : %s', hand);
    return hand;
}
function defaultCardCollection(nk, logger, userId) {
    var deck = {};
    DefaultDeck.forEach(function (c) {
        deck[nk.uuidv4()] = c;
    });
    var stored = {};
    var cards = {
        deckCards: deck,
        storedCards: stored,
    };
    storeUserCards(nk, logger, userId, cards);
    return {
        deckCards: deck,
        storedCards: stored,
    };
}
// ///////// Bag /////////
var kitchi0 = {
    id: 0,
    list: [
        [1, 2, 1, 1],
        [2, 4, 2, 2],
        [3, 6, 3, 3]
    ]
};
var kitchi1 = {
    id: 1,
    list: [
        [1, 2, 1, 1],
        [2, 4, 2, 2],
        [3, 6, 3, 3]
    ]
};
var kitchi2 = {
    id: 2,
    list: [
        [1, 2, 1, 1],
        [2, 4, 2, 2],
        [3, 6, 3, 3]
    ]
};
var kitchi3 = {
    id: 3,
    list: [
        [1, 2, 1, 1],
        [2, 4, 2, 2],
        [3, 6, 3, 3]
    ]
};
var kitchi4 = {
    id: 4,
    list: [
        [1, 2, 1, 1],
        [2, 4, 2, 2],
        [3, 6, 3, 3]
    ]
};
var kitchi5 = {
    id: 5,
    list: [
        [1, 2, 1, 1],
        [2, 4, 2, 2],
        [3, 6, 3, 3]
    ]
};
var kitchi6 = {
    id: 6,
    list: [
        [1, 2, 1, 1],
        [2, 4, 2, 2],
        [3, 6, 3, 3]
    ]
};
var kitchi7 = {
    id: 7,
    list: [
        [1, 2, 1, 1],
        [2, 4, 2, 2],
        [3, 6, 3, 3]
    ]
};
var kitchi8 = {
    id: 8,
    list: [
        [1, 2, 1, 1],
        [2, 4, 2, 2],
        [3, 6, 3, 3]
    ]
};
var kitchi9 = {
    id: 9,
    list: [
        [1, 2, 1, 1],
        [2, 4, 2, 2],
        [3, 6, 3, 3]
    ]
};
var kitchi10 = {
    id: 10,
    list: [
        [1, 2, 1, 1],
        [2, 4, 2, 2],
        [3, 6, 3, 3]
    ]
};
var BlastMaxID = 10;
///////// Shop /////////
// const staticGoldGain1 = 2000;
// const staticGoldCost1 = -100;
// const staticGoldGain2 = 5000;
// const staticGoldCost2 = -200;
// const staticGoldGain3 = 10000;
// const staticGoldCost3 = -300;
// const staticTrapGain1 = 5;
// const staticTrapCost1 = -1000;
// const staticTrapGain2 = 10;
// const staticTrapCost2 = -2000;
// const staticTrapGain3 = 20;
// const staticTrapCost3 = -4000;
///////// Daily Log /////////
// const dailyReward1 = 500;
// const dailyReward2 = 3;
// const dailyReward3 = 1000;
// const dailyReward4 = 100;
// const dailyReward5 = 1500;
// const dailyReward6 = 5;
// const dailyReward7 = 300;
function updateWallet(nk, userId, currencyKeyName, amount) {
    var _a;
    var changeset = (_a = {},
        _a[currencyKeyName] = amount,
        _a);
    var result = nk.walletUpdate(userId, changeset, {}, true);
    return result;
}
function updateMetadata(nk, userId, metadatKeyName, amount) {
    //Load metadata
    // Modify metadata
    // let metadataUpdateResult = nk.accountUpdateId(userId, null, null, null, null, null, null, JSON.parse(payload));
    // let updateString = JSON.stringify(metadataUpdateResult);
}
var matchInit = function (ctx, logger, nk, params) {
    logger.debug('Lobby match created');
    return {
        state: { presences: {}, emptyTicks: 0, started: false, countReadyPing: 0 },
        tickRate: 1,
        label: 'duel',
    };
};
var matchJoinAttempt = function (ctx, logger, nk, dispatcher, tick, state, presence, metadata) {
    logger.debug('%q attempted to join Lobby match', ctx.userId);
    if (state.started) {
        return {
            state: state,
            accept: false
        };
    }
    return {
        state: state,
        accept: true
    };
};
var matchJoin = function (ctx, logger, nk, dispatcher, tick, state, presences) {
    presences.forEach(function (presence) {
        state.presences[presence.userId] = presence;
        logger.debug('%q joined Lobby match', presence.userId);
    });
    return {
        state: state
    };
};
var matchLeave = function (ctx, logger, nk, dispatcher, tick, state, presences) {
    presences.forEach(function (presence) {
        delete (state.presences[presence.userId]);
        logger.debug('%q left Lobby match', presence.userId);
    });
    return {
        state: state
    };
};
var matchLoop = function (ctx, logger, nk, dispatcher, tick, state, messages) {
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
            }
        }
    });
    return {
        state: state
    };
};
var matchTerminate = function (ctx, logger, nk, dispatcher, tick, state, graceSeconds) {
    logger.debug('Lobby match terminated');
    var message = "Server shutting down in ".concat(graceSeconds, " seconds.");
    dispatcher.broadcastMessage(2, message, null, null);
    return {
        state: state
    };
};
var matchSignal = function (ctx, logger, nk, dispatcher, tick, state, data) {
    logger.debug('Lobby match signal received: ' + data);
    return {
        state: state,
        data: "Lobby match signal received: " + data
    };
};
var matchStartData = {
    roundTimer: 45,
};
function rpcFindOrCreateMatch(context, logger, nk) {
    var limit = 10;
    var isAuthoritative = true;
    var minSize = 1;
    var maxSize = 1;
    var matches = nk.matchList(limit, isAuthoritative, "duel", minSize, maxSize, "");
    if (matches.length > 0) {
        matches.sort(function (a, b) {
            return a.size >= b.size ? 1 : -1;
        });
        var matchId = matches[0].matchId;
        return JSON.stringify({ matchId: matchId });
    }
    var matchId = nk.matchCreate('duel', {});
    return JSON.stringify({ matchId: matchId }); // Changer pas besoin de faire du JSON
}
var OPCODE_readyState = 1;
var OPCODE_matchStart = 2;
var OPCODE_whoStart = 3;
var OPCODE_playersDeck = 4;
var OPCODE_playerDropCard = 5;
var OPCODE_matchEnd = 6;
