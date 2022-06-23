import { LetterEvaluation } from "../../interface";
import React from "react";

export function LetterInputTile(props: {
  tile: string;
  correctLetters: LetterEvaluation[];
  tilePosition: number;
  invalidWord: boolean;
}) {
  const hasCorrectPreviousSubmission = props.correctLetters.find((e) => e.wordIndex === props.tilePosition);

  return (
    <div
      className={`border-black border-2 flex justify-center items-center font-kawoord relative text-xl ${
        props.invalidWord ? "animate-wiggle border-red-500" : ""
      }  ${hasCorrectPreviousSubmission && props.tile === hasCorrectPreviousSubmission.letter ? "text-green-400" : ""}`}
    >
      <p>{props.tile?.toUpperCase() || ""}</p>
      {hasCorrectPreviousSubmission && props.tile === "" && (
        <p className="text-green-400 absolute top-8 left-50% border-gray-400 bg-white border-2 px-2">
          {hasCorrectPreviousSubmission?.letter.toUpperCase()}
        </p>
      )}
    </div>
  );
}
