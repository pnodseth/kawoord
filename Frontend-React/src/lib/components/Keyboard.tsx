import React, { FC } from "react";
import Key from "$lib/components/Key";
import { KeyIndicatorDict } from "../../interface";
import Button from "$lib/components/Button";

interface KeyboardProps {
  keyIndicators: KeyIndicatorDict;
  handleSubmit: (word: string) => void;
}

const Keyboard: FC<KeyboardProps> = ({ keyIndicators, handleSubmit }) => {
  const keys = [
    ["q", "w", "e", "r", "t", "y", "u", "i", "o", "p", "å"],
    ["a", "s", "d", "f", "g", "h", "j", "k", "l", "ø", "æ"],
    ["z", "x", "c", "v", "b", "n", "m", "Del"],
  ];

  function handleTap(letter: string) {
    console.log("letter: ", letter);
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
      <div className="spacer h-8"></div>
      <Button onClick={() => handleSubmit("sdfsfddf")}>Submit</Button>
    </>
  );
};

export default Keyboard;
