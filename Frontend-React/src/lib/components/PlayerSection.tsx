import React, { useState } from "react";
import { nanoid } from "nanoid";
import { Player } from "../../interface";
import Button from "$lib/components/Button";

interface Created {
  player: Player | undefined;
  persistPlayer: (player: Player) => void;
}

export function PlayerSection({ player, persistPlayer }: Created) {
  const [nameInput, setNameInput] = useState<string>("");

  return (
    <div className="text-gray-600 text-center h-[70vh]  pb-6 bg-white rounded p-8">
      <h2 className="font-kawoord text-4xl mb-4">Welcome, friend!</h2>
      <p className="mb-4">Enter your name to start playing Kawoord</p>
      {!player?.name ? (
        <>
          <input
            type="text"
            className="text-2xl border-2 border-gray-200 rounded p-2 py-4 text-black text-center w-full "
            id="name"
            value={nameInput}
            placeholder="Your name here"
            onChange={(e) => setNameInput(e.target.value)}
          />
          <div className="mt-2">
            <Button width="w-full" onClick={() => persistPlayer({ name: nameInput, id: nanoid() })}>
              Do it
            </Button>
          </div>
        </>
      ) : (
        <p>Your name: {player?.name}</p>
      )}
    </div>
  );
}
