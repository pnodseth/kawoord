import React, { useEffect, useState } from "react";
import { formatDistanceToNowStrict, isBefore } from "date-fns";

interface CountdownProps {
  countDownTo: Date | undefined;
}

export const CountDown: React.FC<CountdownProps> = ({ countDownTo }) => {
  const [countDown, setCountDown] = useState("");

  /* Set countdown timer */
  useEffect(() => {
    if (countDownTo) {
      const intervalId = setInterval(() => {
        if (isBefore(new Date(), new Date(countDownTo))) {
          setCountDown(`${formatDistanceToNowStrict(new Date(countDownTo))}`);
        } else {
          clearInterval(intervalId);
          setCountDown("Round has ended!");
        }
      }, 1000);

      return function cleanup() {
        clearInterval(intervalId);
      };
    }
  });

  return <p className="font-kawoord absolute right-0 top-0">{countDown}</p>;
};