import { useEffect, useState } from "react";
import { Player } from "../../interface";

export const usePlayer = () => {
  const [player, setPlayer] = useState<Player>();

  const persistPlayer = (player: Player) => {
    localStorage.setItem("player", JSON.stringify(player));
    setPlayer(player);
  };

  /*get cached Player on first mount*/
  useEffect(() => {
    const cachedPlayerString = localStorage.getItem("player");
    if (cachedPlayerString) {
      setPlayer(JSON.parse(cachedPlayerString));
    }
  }, []);

  return { player, persistPlayer };
};
