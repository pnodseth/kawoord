import { useEffect, useRef, useState } from "react";
import { differenceInSeconds } from "date-fns";

export const useCountDownTo = (countDownTo: Date) => {
  const [countDown, setCountDown] = useState<number>();
  const intervalRef = useRef<number>();

  /* Set countdown timer */
  useEffect(() => {
    setCountDown(differenceInSeconds(new Date(countDownTo), Date.now()));
    intervalRef.current = setInterval(() => {
      setCountDown(differenceInSeconds(new Date(countDownTo), Date.now()));
    }, 1000);

    return function cleanup() {
      clearInterval(intervalRef.current);
    };
  }, [countDownTo]);
  return countDown;
};
