import React from "react";

interface ILogoHeader {
  headerSize: "small" | "large";
}

export const LogoHeader: React.FC<ILogoHeader> = ({ headerSize }) => (
  <div className="w-full m-auto pt-4 pl-4">
    {headerSize === "large" ? (
      <h1 className="text-6xl text-center font-kawoord text-white">Kawoord</h1>
    ) : (
      <h1 className="text-xl text-left font-kawoord text-white">Kawoord</h1>
    )}
  </div>
);
