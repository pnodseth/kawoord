import React from "react";

interface ContentLayoutProps {
  noBg?: boolean;
  padding?: string;
}

export const ContentLayout: React.FC<ContentLayoutProps> = ({ children, noBg, padding }) => (
  <div
    className={`max-w-2xl m-auto text-center h-[70vh]  pb-6  ${
      !noBg ? "bg-white text-gray-600 " : "text-white"
    } rounded ${padding ? padding : "p-8"} font-kawoord`}
  >
    {children}
  </div>
);