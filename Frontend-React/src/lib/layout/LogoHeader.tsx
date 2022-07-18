import React, { useContext } from "react";
import { playerContext } from "$lib/contexts/PlayerContext";

interface ILogoHeader {
  headerSize: "small" | "large";
}

export const LogoHeader: React.FC<ILogoHeader> = ({ headerSize }) => {
  const player = useContext(playerContext);
  return (
    <div className="w-full m-auto pt-4 pl-4">
      {headerSize === "large" ? (
        <h1 className="text-6xl text-center font-kawoord text-white">Kawoord</h1>
      ) : (
        <div className="flex justify-between items-center font-kawoord pr-8">
          <h1 className="text-xl text-left font-kawoord text-white">Kawoord</h1>
          <div>{player.name}</div>
        </div>
      )}
    </div>
  );
};
