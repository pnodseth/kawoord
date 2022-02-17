import Button from "$lib/components/Button";
import React, { useEffect, useState } from "react";
import { PlayerSection } from "$lib/components/PlayerSection";
import { Player } from "../../interface";

export function NoGame(props: {
  onClick: () => Promise<void>;
  onJoin: (id: string) => void;
  setPlayer: React.Dispatch<React.SetStateAction<Player | undefined>>;
  player: Player | undefined;
}) {
  const [input, setInput] = useState<string>("");
  /*get cached Player on first mount*/
  useEffect(() => {
    const cachedPlayerString = localStorage.getItem("player");
    if (cachedPlayerString) {
      props.setPlayer(JSON.parse(cachedPlayerString));
    }
  }, [props.setPlayer]);

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

  return (
    <section className="text-center h-[70vh]  pb-6">
      <div className="bg-white rounded p-8 h-[70vh]">
        <p>Join game with pin</p>
        <div className="join flex flex-col w-80 m-auto">
          <input
            type="text"
            className="border-2 border-gray-200 rounded p-2 py-4 text-black text-center"
            value={input}
            onChange={(e) => setInput(e.target.value)}
            placeholder="Enter Game Id"
          />
          <div className="spacer h-2" />
          <Button onClick={() => props.onJoin(input)}>Join Game</Button>
        </div>
        <p className="text-gray-500 my-2 ">Or...</p>
        <Button secondary width="w-80" onClick={props.onClick}>
          Create Game
        </Button>
      </div>
    </section>
  );
}
