import { useContext, useEffect, useState } from "react";
import { Player } from "../../interface";
import { gameServiceContext } from "$lib/contexts/GameServiceContext";
import { nanoid } from "nanoid";
import { useAccount, useIsAuthenticated, useMsal } from "@azure/msal-react";

export const usePlayer = () => {
  const [player, setPlayer] = useState<Player>({ name: "", id: "", signedIn: false });
  const gameService = useContext(gameServiceContext);

  const isAuthenticated = useIsAuthenticated();
  const { accounts } = useMsal();
  const accountIdentifier = {
    homeAccountId: accounts[0]?.homeAccountId || "",
  };
  const accountInfo = useAccount(accountIdentifier);

  /* useEffect(() => {
     if (isAuthenticated && accountInfo && accountInfo.name && accountInfo.name !== player.name) {
       setPlayer((prev) => {
         return { ...prev, name: accountInfo.name as string };
       });
     }
   }, [accountInfo, isAuthenticated, player.name]);*/

  useEffect(() => {
    if (isAuthenticated && accountInfo) {
      localStorage.removeItem("player");

      setPlayer((prev) => {
        const signedInPlayer: Player = {
          name: accountInfo.name || prev.name,
          id: accountInfo.homeAccountId,
          signedIn: true,
        };
        return signedInPlayer;
      });
    }
  }, [accountInfo, isAuthenticated]);

  /*get cached Player on first mount*/
  useEffect(() => {
    if (isAuthenticated) return;

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
          signedIn: false,
        };

        localStorage.setItem("player", JSON.stringify(player));
        setPlayer(player);
      });
    }
  }, [gameService, isAuthenticated]);

  return player;
};
