import Button from "$lib/components/Button";
import React, { useState } from "react";

export function NoGame(props: { onClick: () => Promise<void>; onJoin: (id: string) => void }) {
  const [input, setInput] = useState<string>("");

  return (
    <section className="text-center bg-white rounded w-96 m-auto pb-6">
      <p>Join game with pin</p>
      <div className="join flex flex-col w-80 m-auto">
        <input
          type="text"
          className="border-2 border-gray-200 rounded p-2 py-4 text-black text-center"
          value={input}
          onChange={(e) => setInput(e.target.value)}
          placeholder="Enter Game Id"
        />
        <div className="spacer h-2"></div>
        <Button onClick={() => props.onJoin(input)}>Start</Button>
      </div>
      <p className="text-gray-500 my-2 ">Or...</p>
      <button className="text-gray-600 border-gray-400 border-2 px-4" onClick={props.onClick}>
        Create Game
      </button>
    </section>
  );
}
