import React from "react";
import { LetterEvaluation } from "../../interface";
import { LetterInputTile } from "$lib/components/LetterInputTile";

interface InputGridProps {
  letterArr: string[];
  correctLetters: LetterEvaluation[];
  invalidWord: boolean;
}

export function InputGrid({ letterArr, correctLetters, invalidWord }: InputGridProps) {
  return (
    <>
      <div className="letters grid grid-cols-5 h-12  gap-3 mb-2">
        {letterArr.map((tile, idx) => {
          return (
            <LetterInputTile
              key={idx}
              tile={tile}
              tilePosition={idx}
              correctLetters={correctLetters}
              invalidWord={invalidWord}
            />
          );
        })}
      </div>
      <p className={`transition-opacity opacity-0 ${invalidWord && "opacity-100"}`}>Not a valid word</p>
    </>
  );
}
