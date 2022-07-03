import React, { FC } from "react";
import { gameService, GameService } from "$lib/services/GameService";

export const gameServiceContext = React.createContext<GameService>(gameService);

export const GameServiceProvider: FC = ({ children }) => {
  return <gameServiceContext.Provider value={gameService}>{children}</gameServiceContext.Provider>;
};
