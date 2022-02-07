import Button from "$lib/components/Button";
import React, { useState } from "react";

export function NoGame(props: { onClick: () => Promise<void>; onJoin: (id: string) => void }) {
  const [input, setInput] = useState<string>("");

  return (
    <>
      <Button onClick={props.onClick}>Create Game</Button>
      <p>Or join game:</p>
      <input type="text" className="border-2 border-black" value={input} onChange={(e) => setInput(e.target.value)} />
      <Button onClick={() => props.onJoin(input)}>Join</Button>
    </>
  );
}
