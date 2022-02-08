import React, { FC, useState } from "react";
import { GameService } from "$lib/services/GameService";

export const gameServiceContext = React.createContext(new GameService());

export const GameServiceProvider: FC = ({ children }) => {
  const [gameService] = useState<GameService>(new GameService());

  return <gameServiceContext.Provider value={gameService}>{children}</gameServiceContext.Provider>;
};
