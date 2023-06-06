let InitModule: nkruntime.InitModule = function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, initializer: nkruntime.Initializer) {

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
        matchInit,
        matchJoinAttempt,
        matchJoin,
        matchLeave,
        matchLoop,
        matchSignal,
        matchTerminate
    });
    
    logger.warn('XXXXXXXXXXXXXXXXXXXX - Blast Labs TypeScript loaded - XXXXXXXXXXXXXXXXXXXX');
}

function afterAuthenticate(ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, data: nkruntime.Session) {
    if (!data.created) {
        logger.info('User with id: %s account data already existing', ctx.userId);
        return
    }

    let user_id = ctx.userId;
    let username = "Player" + ctx.username;
    let metadata = {
        battle_pass: false,
        win: 0,
        loose: 0,
        total_card: 0,
    };

    let displayName = "NewPlayer";
    let timezone = null;
    let location = null;
    let langTag = "EN";
    let avatarUrl = null;

    let changeset = {
        'coins': 0,
        'gems': 0,
        'rank': 0,
    };

    const writeCards: nkruntime.StorageWriteRequest = {
        collection: DeckCollectionName,
        key: DeckCollectionKey,
        permissionRead: DeckPermissionRead,
        permissionWrite: DeckPermissionWrite,
        value: defaultCardCollection(nk, logger, ctx.userId),
        userId: ctx.userId,
    }

    try {
        nk.storageWrite([writeCards]);
    } catch (error) {
        logger.error('storageWrite error: %q', error);
        throw error;
    }

    try {
        nk.walletUpdate(user_id, changeset);
    } catch (error) {
        logger.error('Error init new wallet : %s', error);
    }

    try {
        nk.accountUpdateId(user_id, username, displayName, timezone, location, langTag, avatarUrl, metadata);
    } catch (error) {
        logger.error('Error init update account : %s', error);
    }

    logger.debug('new user id: %s account data initialised', ctx.userId);
}