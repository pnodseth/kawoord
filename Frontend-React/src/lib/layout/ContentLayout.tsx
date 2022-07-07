import React from "react";

interface ContentLayoutProps {
  noBg?: boolean;
  padding?: string;
}

export const ContentLayout: React.FC<ContentLayoutProps> = ({ children, noBg, padding }) => (
  <div
    className={`relative max-w-xl m-auto text-center h-[70vh]  pb-6  ${
      !noBg ? "bg-kawoordWhite text-gray-600 " : "text-white"
    } rounded-3xl ${padding ? padding : "p-8"} font-kawoord`}
  >
    {children}
  </div>
);
