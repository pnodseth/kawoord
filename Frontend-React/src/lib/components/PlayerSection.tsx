import React, { useState } from "react";
import { nanoid } from "nanoid";
import { Player } from "../../interface";
import Button from "$lib/components/Button";
import AppLayout from "$lib/layout/AppLayout";

interface Created {
  player: Player | undefined;
  persistPlayer: (player: Player) => void;
}

export function PlayerSection({ player, persistPlayer }: Created) {
  const [nameInput, setNameInput] = useState<string>("");

  return (
    <AppLayout>
      <h2 className="font-kawoord text-4xl mb-4">Just one thing...</h2>
      <p className="mb-4">Before you can start playing, we need to know what to call you</p>
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
    </AppLayout>
  );
}
