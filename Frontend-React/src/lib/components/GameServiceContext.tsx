import React, { Dispatch, FC, useState } from "react";
import { GameService } from "$lib/services/GameService";

interface Test {
  gameService: GameService | undefined;
  setGameService: Dispatch<React.SetStateAction<GameService | undefined>> | undefined;
}
const initial: Test = {
  gameService: undefined,
  setGameService: undefined,
};
const GameServiceContext = React.createContext<Test>(initial);

export const GameServiceProvider: FC = ({ children }) => {
  const [gameService, setGameService] = useState<GameService>();

  return <GameServiceContext.Provider value={{ gameService, setGameService }}>{children}</GameServiceContext.Provider>;
};
