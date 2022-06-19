import React, { FC, useEffect, useState } from "react";
import { KeyIndicatorDict, LetterEvaluation, LetterIndicator } from "../../interface";

interface KeyProps {
  clickHandler: () => void;
  letter: string;
  letterHints: LetterEvaluation[];
}

const Key: FC<KeyProps> = ({ letter, clickHandler, letterHints }) => {
  const found = letterHints.find((e) => e.letter === letter);

  return (
    <button
      onClick={clickHandler}
      className={`font-bold rounded border-0 p-0 mt-0 mr-1 mb-0 ml-0 flex-1 flex justify-center items-center cursor-pointer uppercase bg-gray-200 h-[58px] ${
        found?.letterValueType.value === "Correct" ? "bg-green-400" : ""
      } ${found?.letterValueType.value === "WrongPlacement" ? "bg-yellow-300" : ""} ${
        found?.letterValueType.value === "Wrong" ? "bg-gray-400" : ""
      }`}
    >
      {letter}
    </button>
  );
};

export default Key;
