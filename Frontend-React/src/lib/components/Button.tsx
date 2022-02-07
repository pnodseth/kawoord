import React, { FC } from "react";

interface ButtonProps {
  onClick?: () => void;
}
const Button: FC<ButtonProps> = ({ onClick, children }) => {
  return (
    <button className="border-black border-2 p-2" onClick={onClick}>
      {children}
    </button>
  );
};

export default Button;
