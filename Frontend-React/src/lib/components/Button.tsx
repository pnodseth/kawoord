import React, { FC } from "react";

interface ButtonProps {
  onClick?: () => void;
}

const Button: FC<ButtonProps> = ({ children, onClick }) => {
  return (
    <button
      className="border-white border-2 rounded p-2 py-4 min-w-[200px] bg-kawoordLilla text-white font-bold"
      onClick={onClick}
    >
      {children}
    </button>
  );
};

export default Button;
