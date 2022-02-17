import React, { FC, useEffect, useState } from "react";
import { KeyIndicatorDict, LetterIndicator } from "../../interface";

interface KeyProps {
  clickHandler: () => void;
  letter: string;
  keyIndicators: KeyIndicatorDict;
}

const Key: FC<KeyProps> = ({ letter, clickHandler, keyIndicators }) => {
  const [indicator, setIndicator] = useState<LetterIndicator | "">();

  useEffect(() => {
    if (letter && keyIndicators[letter]) {
      setIndicator(keyIndicators[letter]);
    } else {
      setIndicator("");
    }
  }, [keyIndicators, letter]);

  return (
    <button
      onClick={clickHandler}
      className={`font-bold rounded border-0 p-0 mt-0 mr-1 mb-0 ml-0 flex-1 flex justify-center items-center cursor-pointer uppercase bg-gray-200 h-[58px] ${
        indicator === "correct" ? "bg-green-400" : ""
      } ${indicator === "notPresent" ? "bg-gray-700" : ""}`}
    >
      {letter}
    </button>
  );
};

export default Key;
