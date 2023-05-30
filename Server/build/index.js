"use strict";
var InitModule = function (ctx, logger, nk, initializer) {
    // Hooks
    initializer.registerAfterAuthenticateDevice(afterAuthenticate);
    initializer.registerAfterAuthenticateFacebook(afterAuthenticate);
    logger.info('AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA');
};
function afterAuthenticate(ctx, logger, nk, data) {
    if (!data.created) {
        logger.info('User with id: %s account data already existing', ctx.userId);
        return;
    }
    var user_id = ctx.userId;
    var username = "Player_" + ctx.username;
    var metadata = {
        vip: false,
        win: 0,
        loose: 0,
        blast_captured: 0,
        blast_defeated: 0,
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
