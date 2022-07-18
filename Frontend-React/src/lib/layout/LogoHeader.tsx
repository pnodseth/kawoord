import React, { useContext, useState } from "react";
import { playerContext } from "$lib/contexts/PlayerContext";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faUser } from "@fortawesome/free-solid-svg-icons";
import { useIsAuthenticated, useMsal } from "@azure/msal-react";
import { loginRequest } from "../../auth/authConfig";
import Button from "$lib/components/Button";

interface ILogoHeader {
  headerSize: "small" | "large";
}

const PlayerAccount = () => {
  const isAuthenticated = useIsAuthenticated();
  const player = useContext(playerContext);
  const [showDropdown, setShowDropdown] = useState(false);
  const { instance } = useMsal();

  function login() {
    instance.loginRedirect(loginRequest).then();
  }

  console.log("is auth", isAuthenticated);
  return (
    <div className="flex justify-between items-center font-kawoord pr-4 relative">
      <h1 className="text-xl text-left font-kawoord text-white"> Kawoord </h1>
      <button className="flex items-center gap-1" onClick={() => setShowDropdown((prev) => !prev)}>
        <div>{player.name}</div>
        <FontAwesomeIcon icon={faUser} />
      </button>
      {showDropdown && (
        <div className="popout absolute min-w-60 right-0 top-8 text-gray-700 z-10 bg-white font-sans p-4">
          {!isAuthenticated ? (
            <div>
              <h1>No auth</h1>
              <p>Sign in or create an account to change your username, see stats and more!</p>
              <Button onClick={login}>Click here to login!</Button>
            </div>
          ) : (
            <h1>Logged in</h1>
          )}
        </div>
      )}
    </div>
  );
};

export const LogoHeader: React.FC<ILogoHeader> = ({ headerSize }) => {
  return (
    <div className="w-full m-auto pt-4 pl-4">
      {headerSize === "large" ? (
        <h1 className="text-6xl text-center font-kawoord text-white">Kawoord</h1>
      ) : (
        <div>
          <PlayerAccount />
        </div>
      )}
    </div>
  );
};
