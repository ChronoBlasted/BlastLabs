const DeckPermissionRead = 2;
const DeckPermissionWrite = 0;
const DeckCollectionName = 'card_collection';
const DeckCollectionKey = 'user_cards';

const DefaultDeck = [
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

type CardMap = { [id: string]: Card }

interface Card {
  id: number
  level: number
}

interface CardCollection {
  deckCards: CardMap
  storedCards: CardMap
}

interface SwapDeckCardRequest {
  cardInId: string
  cardOutId: string
}

interface UpgradeCardRequest {
  id: string
}

/**
 * Swap a card in the user deck with one in its collection.
 */
const rpcSwapDeckCard: nkruntime.RpcFunction =
  function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, payload: string): string {
    const request: SwapDeckCardRequest = JSON.parse(payload);

    const userCards = loadUserBlast(nk, logger, ctx.userId);

    // Check the cards being swapper are valid.
    if (Object.keys(userCards.deckCards).indexOf(request.cardOutId) < 0) {
      throw Error('invalid out blast card');
    }
    if (Object.keys(userCards.storedCards).indexOf(request.cardInId) < 0) {
      throw Error('invalid in blast card');
    }


    // Swap the cards
    let outCard = userCards.deckCards[request.cardOutId];
    let inCard = userCards.storedCards[request.cardInId];
    delete (userCards.deckCards[request.cardOutId]);
    delete (userCards.storedCards[request.cardInId]);
    userCards.deckCards[request.cardInId] = inCard;
    userCards.storedCards[request.cardOutId] = outCard;

    // Store the changes
    storeUserCards(nk, logger, ctx.userId, userCards);

    logger.debug("user '%s' deck blast card '%s' swapped with '%s'", ctx.userId, outCard, inCard);

    return JSON.stringify(userCards);
  }


/**
 * Upgrade the level of a given card in the user collection.
 */
const rpcUpgradeCard: nkruntime.RpcFunction =
  function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, payload: string): string {

    let request: UpgradeCardRequest = JSON.parse(payload);
    let userCards = loadUserBlast(nk, logger, ctx.userId);

    if (!userCards) {
      logger.error('user %s card blast collection not found', ctx.userId);
      throw Error('Internal server error');
    }

    let card_id = request.id;
    let card = userCards.deckCards[card_id];

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
    } catch (error) {
      // Error logged in storeUserCards
      throw Error('Internal server error');
    }

    logger.debug('user %s blast card %s upgraded', ctx.userId, JSON.stringify(card));

    return JSON.stringify(card);
  }


/**
 * Get user card collection.
 */
const rpcLoadUserCards: nkruntime.RpcFunction =
  function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, payload: string): string {
    return JSON.stringify(loadUserBlast(nk, logger, ctx.userId));
  }

/**
 * Add a random card to the user collection
 */
const rpcBuyRandomCard: nkruntime.RpcFunction =
  function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, payload: string) {
    let id = Math.floor(Math.random() * BlastMaxID);

    let userCards: CardCollection;
    try {
      userCards = loadUserBlast(nk, logger, ctx.userId);
    } catch (error) {
      logger.error('error loading user blast cards: %s', error);
      throw Error('Internal server error');
    }

    let cardId = nk.uuidv4();
    let newCard: Card = {
      id,
      level: 1,
    }

    userCards.storedCards[cardId] = newCard;

    try {
      storeUserCards(nk, logger, ctx.userId, userCards);
    } catch (error) {
      logger.error('error getting random blast card: %s', error);
      throw error;
    }

    logger.debug('user %s successfully add a random new card', ctx.userId);

    return JSON.stringify({ [cardId]: newCard });
  }


function loadUserBlast(nk: nkruntime.Nakama, logger: nkruntime.Logger, userId: string): CardCollection {
  let storageReadReq: nkruntime.StorageReadRequest = {
    key: DeckCollectionKey,
    collection: DeckCollectionName,
    userId: userId,
  }

  let objects: nkruntime.StorageObject[];
  try {
    objects = nk.storageRead([storageReadReq]);
  } catch (error) {
    logger.error('blast : storageRead error: %s', error);
    throw error;
  }

  if (objects.length === 0) {
    throw Error('user blast cards storage object not found');
  }

  let storedCardCollection = objects[0].value as CardCollection;
  return storedCardCollection;
}

function storeUserCards(nk: nkruntime.Nakama, logger: nkruntime.Logger, userId: string, cards: CardCollection) {
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
  } catch (error) {
    logger.error('storageWrite blast error: %s', error);
    throw error;
  }
}

function getRandomHand(cardCollection: CardCollection,logger : nkruntime.Logger): Card[] {
  const { deckCards } = cardCollection;
  const deckCardIds = Object.keys(deckCards);

  if (deckCardIds.length < 5) {
    throw new Error("Insufficient cards in the deck to create a random hand.");
  }

  const hand: Card[] = [];
  const selectedCardIds: string[] = [];

  while (hand.length < 5) {
    const randomIndex = Math.floor(Math.random() * deckCardIds.length);
    const randomCardId = deckCardIds[randomIndex];
    const randomCard = deckCards[randomCardId];

    if (selectedCardIds.indexOf(randomCardId) > 0 ) {
      hand.push(randomCard);
      selectedCardIds.push(randomCardId);
    }
  }

  logger.info('RRRRRRRRRRRRRRRRRRRRRandom Hand : %s', hand);

  return hand;
}


function defaultCardCollection(nk: nkruntime.Nakama, logger: nkruntime.Logger, userId: string): CardCollection {

  let deck: CardMap = {};

  DefaultDeck.forEach(c => {
    deck[nk.uuidv4()] = c;
  });

  let stored: CardMap = {};

  let cards: CardCollection = {
    deckCards: deck,
    storedCards: stored,
  }

  storeUserCards(nk, logger, userId, cards);

  return {

    deckCards: deck,
    storedCards: stored,
  }
}
