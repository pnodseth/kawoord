import React from "react";
import { useCountDownTo } from "$lib/hooks/useCountDownTo";
import { Round } from "../../interface";
import { motion } from "framer-motion";
import AppLayout from "$lib/layout/AppLayout";

interface PreRoundViewProps {
  round: Round;
}

export const PreRoundView: React.FC<PreRoundViewProps> = ({ round }) => {
  const countDown = useCountDownTo(round.preRoundEndsUtc);

  console.log("co", countDown);

  const heading =
    round.roundNumber === 1 ? "The game is about to start..." : `Round ${round.roundNumber} is starting...`;

  return (
    <AppLayout noBg={true}>
      <div className="overflow-hidden h-[400px]">
        <div className="spacer h-8"></div>
        <motion.div
          animate={{ opacity: [0, 1] }}
          transition={{ duration: 0.2 }}
          className="font-kawoord text-center text-xl"
        >
          {heading}
        </motion.div>
        <div className="spacer h-24"></div>
        <div className="container text-center"></div>
        <motion.div
          key={countDown}
          animate={{ scale: [4, 0.9, 1], opacity: [0, 0.9, 1] }}
          transition={{ duration: 0.5, type: "spring" }}
          className="font-kawoord text-center text-8xl"
        >
          {countDown}
        </motion.div>
      </div>
    </AppLayout>
  );
};
