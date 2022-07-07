import React, { FC } from "react";

interface ButtonProps {
  onClick?: () => void;
  width?: string;
  disabled?: boolean;
  variant?: Variant;
}

type Variant = "secondary" | "ghost";

const Button: FC<ButtonProps> = ({ children, onClick, width, disabled = false, variant }) => {
  return (
    <button
      className={
        "text-xl border-darkLilla hover:bg-kawoordDarkYellow  border-2 rounded-xl p-2 py-4 min-w-[240px] drop-shadow-kawoord h-16 bg-kawoordYellow text-white font-bold text-darkLilla font-kawoord disabled:bg-gray-400" +
        `${width ? width : " "}` +
        " " +
        `${variant === "secondary" ? "border-kawoordLilla text-xl" : ""}` +
        " " +
        `${
          variant === "ghost"
            ? "border-0 bg-inherit hover:bg-inherit hover:text-kawoordLilla text-gray-600 drop-shadow-none"
            : ""
        }`
      }
      onClick={onClick}
      disabled={disabled}
    >
      <p className="drop-shadow-lg drop-shadow-kawoord bg-white "></p>
      {children}
    </button>
  );
};

export default Button;

/*
*     accent-color: currentcolor;
    box-shadow: 0 6px 0 currentColor;
    border-radius: 12px;
    width: 240px;
    height: 58px;
    display: flex;
    align-items: center;
    justify-content: center;
    background: #ffd43b;
    color: #371688;
*
* */
