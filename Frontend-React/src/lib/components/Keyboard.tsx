import React, { FC } from "react";
import Key from "$lib/components/Key";
import { KeyIndicatorDict } from "../../interface";
import KeyboardInput from "$lib/hooks/keyboardInput";

interface KeyboardProps {
  keyIndicators: KeyIndicatorDict;
  handleSubmit: (word: string) => void;
  letterArr: string[];
  setLetterArr: React.Dispatch<React.SetStateAction<string[]>>;
  setLetterIdx: React.Dispatch<React.SetStateAction<number>>;
  letterIdx: number;
}

const Keyboard: FC<KeyboardProps> = ({
  keyIndicators,
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
    console.log("letter: ", letter);
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
      console.log("allowed to submit");
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
                return <Key key={l} letter={l} clickHandler={() => handleTap(l)} keyIndicators={keyIndicators} />;
              })}
            </div>
          );
        })}
      </div>
      <div className="spacer h-8" />
      <KeyboardInput handleTap={handleTap} />
    </>
  );
};

export default Keyboard;
