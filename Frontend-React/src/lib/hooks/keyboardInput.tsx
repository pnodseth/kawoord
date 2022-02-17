import React, { FC, useEffect, useRef } from "react";

interface KeyboardInputProps {
  handleTap: (letter: string) => void;
}

const KeyboardInput: FC<KeyboardInputProps> = ({ handleTap }: KeyboardInputProps) => {
  const inputRef = useRef<HTMLInputElement>(null);

  useEffect(() => {
    inputRef.current?.focus();
  }, []);

  function handleKeyPress(e: React.KeyboardEvent<HTMLInputElement>) {
    handleTap(e.key);
  }

  return (
    <input
      type="text"
      id="type-input"
      ref={inputRef}
      onKeyDown={(e) => handleKeyPress(e)}
      onBlur={() => inputRef.current?.focus()}
      className="opacity-0 absolute top-0 left-0 w-0"
    />
  );
};

export default KeyboardInput;
