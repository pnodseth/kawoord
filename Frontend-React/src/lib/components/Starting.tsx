import { Game } from "../../interface";
import React, { useEffect, useState } from "react";
import { formatDistanceToNowStrict, isBefore } from "date-fns";

export function Starting({ game }: { game: Game }) {
  const [startingIn, setStartingIn] = useState("");
  // eslint-disable-next-line no-undef

  /*Countdown timer*/
  useEffect(() => {
    const intervalId = setInterval(() => {
      if (isBefore(new Date(), new Date(game.startedTime))) {
        setStartingIn(`Starting in: ${formatDistanceToNowStrict(new Date(game.startedTime))}`);
      } else {
        clearInterval(intervalId);
        setStartingIn("Game is starting!");
      }
    }, 1000);

    return function cleanup() {
      clearInterval(intervalId);
    };
  }, [game.startedTime]);

  return (
    <section id="starting">
      <p>{startingIn}</p>
    </section>
  );
}
