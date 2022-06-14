import React from "react";
import { LetterEvaluation } from "../../interface";

function LetterInputTile(props: { tile: string; correctLetters: LetterEvaluation[]; tilePosition: number }) {
  const hasCorrectPreviousSubmission = props.correctLetters.find((e) => e.wordIndex === props.tilePosition);

  console.log("has correct previous: ", hasCorrectPreviousSubmission);

  return (
    <div
      className={`border-black border-2 flex justify-center items-center font-kawoord text-xl relative ${
        hasCorrectPreviousSubmission && props.tile === hasCorrectPreviousSubmission.letter ? "text-green-400" : ""
      }`}
    >
      <p>{props.tile?.toUpperCase() || ""}</p>
      {hasCorrectPreviousSubmission && props.tile === "" && (
        <p className="text-green-400 absolute top-9 left-2 border-gray-400 bg-white border-2 px-2">
          {hasCorrectPreviousSubmission?.letter.toUpperCase()}
        </p>
      )}
    </div>
  );
}

export function InputGrid({ letterArr, correctLetters }: { letterArr: string[]; correctLetters: LetterEvaluation[] }) {
  return (
    <>
      <div className="letters grid grid-cols-5 h-12  gap-3 mb-6">
        {letterArr.map((tile, idx) => {
          return <LetterInputTile key={idx} tile={tile} tilePosition={idx} correctLetters={correctLetters} />;
        })}
      </div>
    </>
  );
}
