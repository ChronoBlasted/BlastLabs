function updateWallet(nk: nkruntime.Nakama, userId: string, currencyKeyName: string, amount: number): nkruntime.WalletUpdateResult {
    const changeset = {
        [currencyKeyName]: amount,
    }
    let result = nk.walletUpdate(userId, changeset, {}, true);

    return result;
}

function updateMetadata(nk: nkruntime.Nakama, userId: string, metadatKeyName: string, amount: number) {

    //Load metadata

    // Modify metadata

    // let metadataUpdateResult = nk.accountUpdateId(userId, null, null, null, null, null, null, JSON.parse(payload));

    // let updateString = JSON.stringify(metadataUpdateResult);
}
