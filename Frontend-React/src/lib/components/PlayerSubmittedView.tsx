import React from "react";

interface PlayerHasSubmittedProps {
  submittedWord: string;
}

export const PlayerSubmittedView = ({ submittedWord }: PlayerHasSubmittedProps) => (
  <>
    <h2 className="font-kawoord text-2xl">Great job!</h2>
    <div className="spacer h-6"></div>
    <h2 className="font-kawoord text-xl">You submitted: {submittedWord.toUpperCase()}</h2>
    <div className="spacer h-10"></div>
    <p className=" mt-6 animate-bounce">Waiting for other players to submit their word also...</p>
  </>
);
