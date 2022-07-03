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
        "text-2xl border-white border-2 rounded p-2 py-4 min-w-[200px] bg-kawoordLilla text-white font-bold font-kawoord disabled:bg-gray-400 " +
        `${width ? width : " "}` +
        " " +
        `${variant === "secondary" ? "bg-white border-kawoordLilla text-kawoordLilla text-2xl" : ""}` +
        " " +
        `${variant === "ghost" ? "border-0 bg-white text-gray-600" : ""}`
      }
      onClick={onClick}
      disabled={disabled}
    >
      {children}
    </button>
  );
};

export default Button;
