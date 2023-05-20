"use strict";
let InitModule = function (ctx, logger, nk, initializer) {
    // Hooks
    initializer.registerAfterAuthenticateFacebook(afterAuthenticate);
    logger.info('AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA');
};
function afterAuthenticate(ctx, logger, nk, data) {
    if (!data.created) {
        logger.info('User with id: %s account data already existing', ctx.userId);
        return;
    }
    let user_id = ctx.userId;
    let username = "Player_" + ctx.username;
    let metadata = {
        vip: false,
        win: 0,
        loose: 0,
        blast_captured: 0,
        blast_defeated: 0,
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
