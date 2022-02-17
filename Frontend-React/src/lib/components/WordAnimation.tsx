import React, { useState } from "react";
import { LetterEvaluation } from "../../interface";
import { a, config, useTrail } from "@react-spring/web";
import { LetterTile } from "$lib/components/LetterTile";

export const WordAnimation: React.FC<{ letters: LetterEvaluation[] }> = ({ letters }) => {
  const [showColor, setShowColor] = useState(false);
  const items = letters;
  const trail = useTrail(items.length, {
    config: config.gentle,
    from: { y: -200, x: -50, opacity: 1 },
    to: { y: 0, x: 0, opacity: 1 },
    onRest: () => {
      setTimeout(() => {
        setShowColor(true);
      }, 600);
    },
  });
  return (
    <ul className="grid grid-cols-5 h-12  gap-3 px-12">
      {trail.map((style, index) => (
        <a.li key={index} style={style}>
          <LetterTile e={items[index]} showLetter={showColor} />
        </a.li>
      ))}
    </ul>
  );
};
