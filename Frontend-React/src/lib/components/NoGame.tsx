import Button from "$lib/components/Button";
import React, { useContext, useEffect, useState } from "react";
import { PlayerSection } from "$lib/components/PlayerSection";
import { Game, Player } from "../../interface";
import { SyncLoader } from "react-spinners";
import { gameServiceContext } from "$lib/components/GameServiceContext";

export function NoGame(props: {
  game: Game | undefined;
  setPlayer: React.Dispatch<React.SetStateAction<Player | undefined>>;
  player: Player | undefined;
}) {
  const gameService = useContext(gameServiceContext);
  const [gameIdInput, setGameIdInput] = useState<string>("");
  const [loading, setLoading] = useState(false);

  /*get cached Player on first mount*/
  useEffect(() => {
    const cachedPlayerString = localStorage.getItem("player");
    if (cachedPlayerString && !props.player) {
      props.setPlayer(JSON.parse(cachedPlayerString));
    }
  }, [props, props.setPlayer]);

  /*Store updated player in local storage*/
  useEffect(() => {
    if (props.player) {
      localStorage.setItem("player", JSON.stringify(props.player));
    }
  }, [props.player]);

  if (!props.player) {
    return (
      <>
        <PlayerSection player={props.player} setPlayer={props.setPlayer} />
      </>
    );
  }

  const joinGame = async () => {
    //todo: Notification if gameId is empty
    if (gameIdInput === "" || !props.player) return;
    setLoading(true);
    await gameService.joinGame(props.player, gameIdInput);
    setLoading(false);
  };

  const createGame = async () => {
    setLoading(true);
    await gameService.createGame(props.player);
    setLoading(false);
  };

  return (
    <section className="text-center h-[70vh]  pb-6">
      <div className="bg-white rounded p-8 h-[70vh]">
        <p>Join game with pin</p>
        <div className="join flex flex-col w-full m-auto">
          <input
            type="text"
            className="border-2 border-gray-200 rounded p-2 py-4 text-black text-center"
            value={gameIdInput}
            onChange={(e) => setGameIdInput(e.target.value)}
            placeholder="Enter Game Id"
          />
          <div className="spacer h-6" />
          <Button onClick={() => joinGame()}>{!loading ? "Join Game" : <SyncLoader color="#593b99" />}</Button>
        </div>
        <p className="text-gray-500 my-2 ">Or...</p>
        <Button secondary width="w-full" onClick={() => createGame()}>
          {!loading ? "Create Game" : <SyncLoader color="#593b99" />}
        </Button>
      </div>
    </section>
  );
}
