import React, { useContext } from "react";
import { playerContext } from "$lib/contexts/PlayerContext";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faUser } from "@fortawesome/free-solid-svg-icons";
import { useIsAuthenticated } from "@azure/msal-react";

interface ILogoHeader {
  headerSize: "small" | "large";
  setShowMenu: React.Dispatch<React.SetStateAction<boolean>>;
}

interface IPlayerAccount {
  setShowMenu: React.Dispatch<React.SetStateAction<boolean>>;
}

const PlayerAccount: React.FC<IPlayerAccount> = ({ setShowMenu }) => {
  const isAuthenticated = useIsAuthenticated();
  const player = useContext(playerContext);

  console.log("is auth", isAuthenticated);
  return (
    <div className="flex justify-between items-center font-kawoord pr-4 relative">
      <h1 className="text-xl text-left font-kawoord text-white"> Kawoord </h1>
      <button className="flex items-center gap-1" onClick={() => setShowMenu((prev) => !prev)}>
        <div>{player.name}</div>
        <FontAwesomeIcon icon={faUser} />
      </button>
    </div>
  );
};

export const LogoHeader: React.FC<ILogoHeader> = ({ headerSize, setShowMenu }) => {
  return (
    <div className="w-full m-auto pt-4 pl-4">
      {headerSize === "large" ? (
        <h1 className="text-6xl text-center font-kawoord text-white">Kawoord</h1>
      ) : (
        <div>
          <PlayerAccount setShowMenu={setShowMenu} />
        </div>
      )}
    </div>
  );
};
