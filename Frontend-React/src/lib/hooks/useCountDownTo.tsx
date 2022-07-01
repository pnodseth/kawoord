import { useEffect, useState } from "react";
import { formatDistanceToNowStrict, isBefore } from "date-fns";

export const useCountDownTo = (countDownTo: Date) => {
  const [countDown, setCountDown] = useState("");

  /* Set countdown timer */
  useEffect(() => {
    setCountDown(`${formatDistanceToNowStrict(new Date(countDownTo))}`);

    if (countDownTo) {
      const intervalId = setInterval(() => {
        if (isBefore(new Date(), new Date(countDownTo))) {
          setCountDown(`${formatDistanceToNowStrict(new Date(countDownTo))}`);
        } else {
          clearInterval(intervalId);
          setCountDown("");
        }
      }, 1000);

      return function cleanup() {
        clearInterval(intervalId);
      };
    }
  }, [countDownTo]);

  return countDown;
};
