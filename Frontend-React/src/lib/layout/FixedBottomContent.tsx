import React from "react";

const FixedBottomContent: React.FC = ({ children }) => {
  return <div className="absolute bottom-2 w-full left-0">{children}</div>;
};

export default FixedBottomContent;
