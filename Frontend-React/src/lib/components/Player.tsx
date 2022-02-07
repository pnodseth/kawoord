import React, { useState } from "react";
import { usePlayerName } from "$lib/hooks/hooks";

export function Player() {
  const [nameInput, setNameInput] = useState<string>("");
  const [name, setName] = useState<string>("");
  const player = usePlayerName(name);
  return (
    <>
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
          <button className="border-black border-2 p-2" onClick={() => setName(nameInput)}>
            Set name
          </button>
        </>
      ) : (
        <p>Player name: {player?.name}</p>
      )}
    </>
  );
}
