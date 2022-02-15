import React, { useState } from "react";
import { nanoid } from "nanoid";
import { Player } from "../../interface";

interface Created {
  player: Player | undefined;
  setPlayer: React.Dispatch<React.SetStateAction<Player | undefined>>;
}

export function PlayerSection({ player, setPlayer }: Created) {
  const [nameInput, setNameInput] = useState<string>("");

  return (
    <section className="text-center">
      {!player?.name ? (
        <>
          <label htmlFor="name">Enter player name</label>
          <input
            type="text"
            className="border-black border-2"
            id="name"
            value={nameInput}
            onChange={(e) => setNameInput(e.target.value)}
          />
          <button className="border-black border-2 p-2" onClick={() => setPlayer({ name: nameInput, id: nanoid() })}>
            Set name
          </button>
        </>
      ) : (
        <p>Your name: {player?.name}</p>
      )}
    </section>
  );
}
