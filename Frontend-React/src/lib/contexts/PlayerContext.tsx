import React, { FC } from "react";
import { Player } from "../../interface";
import { usePlayer } from "$lib/hooks/usePlayer";

export const playerContext = React.createContext<Player>({ name: "", id: "" });

export const PlayerProvider: FC = ({ children }) => {
  const player = usePlayer();

  return <playerContext.Provider value={player}>{children}</playerContext.Provider>;
};
