import React from "react";
import { motion } from "framer-motion";
import { ClimbingBoxLoader } from "react-spinners";

interface PlayerHasSubmittedProps {
  submittedWord: string;
}

export const PlayerSubmittedView = ({ submittedWord }: PlayerHasSubmittedProps) => (
  <motion.div animate={{ opacity: [0, 1] }} transition={{ duration: 0.4, type: "spring" }}>
    <h2 className="font-kawoord text-2xl">Great job!</h2>
    <div className="spacer h-6"></div>
    <h2 className="font-kawoord text-xl">You submitted: {submittedWord.toUpperCase()}</h2>
    <div className="spacer h-10 xl:h-20"></div>
    <div className="flex justify-center items-center">
      <ClimbingBoxLoader color="#593b99" />
    </div>
    <div className="spacer h-10 xl:h-20"></div>
    <p className=" mt-6 animate-bounce font-sans italic">Waiting for other players...</p>
  </motion.div>
);
