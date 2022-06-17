import React, { useState } from "react";
import { LetterEvaluation } from "../../interface";
import { a, config, useTrail } from "@react-spring/web";
import { LetterTile } from "$lib/components/LetterTile";

interface WordAnimationProps {
  letters: LetterEvaluation[];
  delayMs?: number;
  player: string;
  showLetters: boolean;
}

export const WordAnimation: React.FC<WordAnimationProps> = ({ letters, delayMs, player, showLetters = true }) => {
  const [showName, setShowName] = useState(false);
  const items = letters;
  const trail = useTrail(items.length, {
    config: config.gentle,
    delay: delayMs,
    from: { y: -200, x: -50, opacity: 0 },
    to: { y: 0, x: 0, opacity: 1 },
    onStart: () => {
      setShowName(true);
    },
  });
  return (
    <ul className="grid grid-cols-5 h-12  gap-3 px-12 relative">
      <h2 className="absolute top-[-2rem] left-0 font-kawoord text-xl">{showName && player}</h2>
      {trail.map((style, index) => (
        <a.li key={index} style={style}>
          <LetterTile e={items[index]} showLetter={showLetters} />
        </a.li>
      ))}
    </ul>
  );
};
