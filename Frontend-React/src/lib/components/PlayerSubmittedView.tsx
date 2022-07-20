import React, { useEffect, useRef, useState } from "react";
import { motion } from "framer-motion";
import useWordDefinition from "$lib/hooks/useWordDefinition";

interface PlayerHasSubmittedProps {
  submittedWord: string;
}

export const PlayerSubmittedView = ({ submittedWord }: PlayerHasSubmittedProps) => {
  const definitions = useWordDefinition(submittedWord);
  const [definitionIdx, setDefinitionIdx] = useState(0);
  const intervalRef = useRef<number>();

  useEffect(() => {
    if (definitions && definitions.length > 0) {
      intervalRef.current = setInterval(() => {
        setDefinitionIdx((e) => (e < definitions.length - 1 ? e + 1 : 0));
      }, 6000);
    }

    return () => {
      clearInterval(intervalRef.current);
    };
  }, [definitions]);
  return (
    <motion.div
      animate={{ opacity: [0, 1] }}
      transition={{ duration: 0.4, type: "spring" }}
      className="flex flex-col h-full"
    >
      <div className="flex-grow-0">
        <h2 className=" text-xl">You submitted: </h2>
        <div className="spacer h-2"></div>
        <h2 className="font-kawoord text-6xl text-kawoordLilla">{submittedWord.toUpperCase()}</h2>
        <div className="spacer h-8  lg:h-20"></div>
      </div>
      <div className="definition flex-1">
        {definitions && definitions.length > 0 && (
          <h2 className="italic text-md lg:text-2xl px-4 font-sans">{definitions[definitionIdx].substr(0, 190)}...</h2>
        )}
      </div>
      <div className="waiting">
        <p className=" mt-6 animate-bounce font-sans italic">Waiting for other players...</p>
      </div>
    </motion.div>
  );
};
