import React, { FC } from "react";
import Key from "$lib/components/Key";
import { LetterEvaluation } from "../../interface";
import KeyboardInput from "$lib/hooks/keyboardInput";

interface KeyboardProps {
  letterHints: LetterEvaluation[];
  handleSubmit: (word: string) => void;
  letterArr: string[];
  setLetterArr: React.Dispatch<React.SetStateAction<string[]>>;
  setLetterIdx: React.Dispatch<React.SetStateAction<number>>;
  letterIdx: number;
}

const Keyboard: FC<KeyboardProps> = ({
  letterHints,
  handleSubmit,
  letterIdx,
  letterArr,
  setLetterArr,
  setLetterIdx,
}) => {
  const keys = [
    ["q", "w", "e", "r", "t", "y", "u", "i", "o", "p"],
    ["a", "s", "d", "f", "g", "h", "j", "k", "l"],
    ["z", "x", "c", "v", "b", "n", "m", "Del"],
  ];

  function allowedKeys() {
    return keys.flat().filter((e) => e !== "Del");
  }

  function handleTap(letter: string) {
    if (allowedKeys().includes(letter)) {
      if (letterIdx < 5) {
        const arr = [...letterArr];
        arr[letterIdx] = letter;
        setLetterArr(arr);
        setLetterIdx(letterIdx + 1);
      }
    } else if (letter === "Del" || letter === "Backspace") {
      if (letterIdx >= 0) {
        const arr = [...letterArr];
        arr[letterIdx - 1] = "";
        setLetterArr(arr);
        if (letterIdx > 0) {
          setLetterIdx(letterIdx - 1);
        }
      }
    } else if (letter === "Enter" && letterIdx === 5) {
      handleSubmit(letterArr.join(""));
    }
  }

  return (
    <>
      <div id="keyboard" className="w-full">
        {keys.map((row, idx) => {
          return (
            <div key={idx} className="row flex w-full mt-0 mx-auto mb-0.5">
              {row.map((l) => {
                return <Key key={l} letter={l} clickHandler={() => handleTap(l)} letterHints={letterHints} />;
              })}
            </div>
          );
        })}
      </div>
      <KeyboardInput handleTap={handleTap} />
    </>
  );
};

export default Keyboard;
