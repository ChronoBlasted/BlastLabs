function rpcFindOrCreateMatch(context: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama): string {
    const limit = 10
    const isAuthoritative = true;
    const minSize = 1;
    const maxSize = 1;

    var matches = nk.matchList(limit, isAuthoritative, "duel", minSize, maxSize, "");

    if (matches.length > 0) {
        matches.sort(function (a, b) {
            return a.size >= b.size ? 1 : -1;
        });
        var matchId = matches[0].matchId;
        return JSON.stringify({ matchId });
    }

    var matchId = nk.matchCreate('duel', {});
    return JSON.stringify({ matchId });
}

