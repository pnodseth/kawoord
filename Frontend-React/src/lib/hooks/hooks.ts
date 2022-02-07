import { useEffect, useState } from "react";
import { nanoid } from "nanoid";
import { Player } from "../../interface";

export const usePlayerName = (name: string) => {
  const CACHED_PLAYER = "player";
  const [player, setPlayer] = useState<Player>({ name: "", id: "" });

  useEffect(() => {
    if (name !== "" && player?.id) {
      const newplayer: Player = { id: player.id, name: name };
      localStorage.setItem(CACHED_PLAYER, JSON.stringify(newplayer));

      setPlayer(newplayer);
    }
  }, [name, player?.id]);

  useEffect(() => {
    const cachedPlayer = localStorage.getItem(CACHED_PLAYER);

    if (!cachedPlayer) {
      const p = {
        name: "",
        id: nanoid(),
      };
      setPlayer(p);

      localStorage.setItem(CACHED_PLAYER, JSON.stringify(p));
    } else {
      setPlayer(JSON.parse(cachedPlayer));
    }
  }, []);
  return player;
};
