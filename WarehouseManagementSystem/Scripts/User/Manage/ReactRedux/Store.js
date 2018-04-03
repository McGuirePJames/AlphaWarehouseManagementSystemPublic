const logger = reduxLogger.createLogger();
const createStoreWithMiddleware = Redux.applyMiddleware(ReduxThunk.default, logger);
const store = Redux.createStore(reducer, createStoreWithMiddleware);