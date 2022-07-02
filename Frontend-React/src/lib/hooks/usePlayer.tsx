import { useContext, useEffect, useState } from "react";
import { Player } from "../../interface";
import { gameServiceContext } from "$lib/components/GameServiceContext";
import { nanoid } from "nanoid";

export const usePlayer = () => {
  const [player, setPlayer] = useState<Player>({ name: "", id: "" });
  const gameService = useContext(gameServiceContext);

  /*get cached Player on first mount*/
  useEffect(() => {
    const getRandomName = async () => {
      return await gameService.GetRandomName();
    };

    const cachedPlayerString = localStorage.getItem("player");

    if (cachedPlayerString) {
      setPlayer(JSON.parse(cachedPlayerString));
    } else {
      getRandomName().then((name) => {
        const player: Player = {
          name,
          id: nanoid(),
        };

        localStorage.setItem("player", JSON.stringify(player));
        setPlayer(player);
      });
    }
  }, [gameService]);

  return player;
};
