import React, { FC } from "react";

interface ButtonProps {
  onClick?: () => void;
  width?: string;
  secondary?: boolean;
}

const Button: FC<ButtonProps> = ({ children, onClick, width, secondary }) => {
  return (
    <button
      className={
        "text-2xl border-white border-2 rounded p-2 py-4 min-w-[200px] bg-kawoordLilla text-white font-bold font-kawoord " +
        `${width ? width : ""}` +
        " " +
        `${secondary ? "bg-white border-kawoordLilla text-kawoordLilla text-lg" : ""}`
      }
      onClick={onClick}
    >
      {children}
    </button>
  );
};

export default Button;
