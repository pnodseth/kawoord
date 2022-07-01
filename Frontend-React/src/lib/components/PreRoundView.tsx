import React from "react";
import { useCountDownTo } from "$lib/hooks/useCountDownTo";
import { Round } from "../../interface";
import { animated, config, useTransition } from "@react-spring/web";

interface PreRoundViewProps {
  round: Round;
}

export const PreRoundView: React.FC<PreRoundViewProps> = ({ round }) => {
  const countDown = useCountDownTo(round.preRoundEndsUtc);

  const transitions = useTransition(countDown, {
    from: { opacity: 0, transform: "scale(3)" },
    enter: { opacity: 1, transform: "scale(1)" },
    leave: { opacity: 0 },
    delay: 200,
    config: config.molasses,
    exitBeforeEnter: true,
    reset: true,
  });
  console.log("co", countDown);

  return (
    <>
      <div className="spacer h-8"></div>
      <h1 className="font-kawoord text-center text-xl">The game is about to start...</h1>
      <div className="spacer h-32"></div>
      <div className="container text-center"></div>
      {transitions((styles, item) => {
        return (
          <animated.div style={styles} className="font-kawoord text-center text-8xl">
            {item}
          </animated.div>
        );
      })}
    </>
  );
};
