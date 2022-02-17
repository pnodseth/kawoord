import React, { useEffect, useRef, useState } from "react";
import { allowed } from "$lib/components/constants";

interface InputGridParams {
  handleSubmit: (word: string) => void;
}

export function InputGrid({ handleSubmit }: InputGridParams) {
  const inputRef = useRef<HTMLInputElement>(null);
  const [letterArr, setLetterArr] = useState<string[]>(["", "", "", "", ""]);
  const [letterIdx, setLetterIdx] = useState(0);

  useEffect(() => {
    inputRef.current?.focus();
  }, []);

  function handleKeyPress(e: React.KeyboardEvent<HTMLInputElement>) {
    if (allowed.includes(e.key)) {
      if (letterIdx < 5) {
        const arr = [...letterArr];
        arr[letterIdx] = e.key;
        setLetterArr(arr);
        setLetterIdx(letterIdx + 1);
      }
    } else if (e.key === "Backspace") {
      if (letterIdx >= 0) {
        const arr = [...letterArr];
        arr[letterIdx - 1] = "";
        setLetterArr(arr);
        if (letterIdx > 0) {
          setLetterIdx(letterIdx - 1);
        }
      }
    } else if (e.key === "Enter" && letterIdx === 5) {
      console.log("allowed to submit");
      handleSubmit(letterArr.join(""));
    }
  }

  return (
    <>
      <input
        type="text"
        ref={inputRef}
        onKeyDown={(e) => handleKeyPress(e)}
        onBlur={() => inputRef.current?.focus()}
        className="opacity-0 absolute top-0 left-0 w-0"
      />
      <div className="letters grid grid-cols-5 h-12  gap-3 mb-6">
        <p className="border-black border-2 flex justify-center items-center font-kawoord text-xl">
          {letterArr[0].toUpperCase()}
        </p>
        <p className="border-black border-2 flex justify-center items-center font-kawoord text-xl">
          {letterArr[1].toUpperCase()}
        </p>
        <p className="border-black border-2 flex justify-center items-center font-kawoord text-xl">
          {letterArr[2].toUpperCase()}
        </p>
        <p className="border-black border-2 flex justify-center items-center font-kawoord text-xl">
          {letterArr[3].toUpperCase()}
        </p>
        <p className="border-black border-2 flex justify-center items-center font-kawoord text-xl">
          {letterArr[4].toUpperCase()}
        </p>
      </div>
    </>
  );
}
