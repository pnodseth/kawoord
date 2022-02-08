import { LetterEvaluation } from "../../interface";
import React from "react";

interface LetterTileParams {
  e: LetterEvaluation;
  showLetter?: boolean;
}

export function LetterTile(props: LetterTileParams) {
  function style() {
    switch (props.e.type.value) {
      case "Wrong":
        return "bg-gray-500";

      case "WrongPlacement":
        return "bg-yellow-500";

      case "Correct":
        return "bg-green-400";
    }
  }

  return (
    <li className={`${style()} flex items-center justify-center text-xl`}>
      {props.showLetter && props.e.letter.toUpperCase()}
    </li>
  );
}
