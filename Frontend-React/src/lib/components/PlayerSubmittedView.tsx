import React, { useEffect, useRef, useState } from "react";
import { motion } from "framer-motion";
import useWordDefinition from "$lib/hooks/useWordDefinition";
import FixedBottomContent from "$lib/layout/FixedBottomContent";

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
    <motion.div animate={{ opacity: [0, 1] }} transition={{ duration: 0.4, type: "spring" }}>
      <h2 className="font-kawoord text-2xl">Great job!</h2>
      <h2 className="font-kawoord text-xl">You submitted: {submittedWord.toUpperCase()}</h2>
      <div className="spacer sm:8 xl:h-20"></div>
      {/* <div className="flex justify-center items-center">
        <ClimbingBoxLoader color="#593b99" />
      </div>*/}
      <FixedBottomContent>
        {definitions && definitions.length > 0 && (
          <h2 className="italic text-md xl:text-2xl px-4">
            {submittedWord.toUpperCase()}: {definitions[definitionIdx].substr(0, 190)}...
          </h2>
        )}
        <p className=" mt-6 animate-bounce font-sans italic">Waiting for other players...</p>
      </FixedBottomContent>
      {/*      <div className="spacer h-10 xl:h-20"></div>*/}
    </motion.div>
  );
};
