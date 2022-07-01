import React from "react";
import { motion } from "framer-motion";

interface PlayerHasSubmittedProps {
  submittedWord: string;
}

export const PlayerSubmittedView = ({ submittedWord }: PlayerHasSubmittedProps) => (
  <motion.div animate={{ opacity: [0, 1] }} transition={{ duration: 0.4, type: "spring" }}>
    <h2 className="font-kawoord text-2xl">Great job!</h2>
    <div className="spacer h-6"></div>
    <h2 className="font-kawoord text-xl">You submitted: {submittedWord.toUpperCase()}</h2>
    <div className="spacer h-10"></div>
    <p className=" mt-6 animate-bounce">Waiting for other players to submit their word also...</p>
  </motion.div>
);
