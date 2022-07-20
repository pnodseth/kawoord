import Button from "$lib/components/Button";
import React, { useContext } from "react";
import { useNavigate } from "react-router-dom";
import FixedBottomContent from "$lib/layout/FixedBottomContent";
import { playerContext } from "$lib/contexts/PlayerContext";
import AppLayout from "$lib/layout/AppLayout";

// React MSAL Hooks: https://github.com/AzureAD/microsoft-authentication-library-for-js/blob/dev/lib/msal-react/docs/hooks.md

export const Home: React.FC = () => {
  const navigate = useNavigate();
  const player = useContext(playerContext);

  const startPlaying = () => {
    if (!player) {
      navigate("/player");
    }
    navigate("/play");
  };
  return (
    <div>
      <AppLayout noBg={true}>
        <h1 className="text-2xl md:text-3xl xl:mt-12">Welcome, friend!</h1>
        <div className="spacer h-16 md:h-20"></div>
        <p className="text-xl md:text-3xl">
          {" "}
          Join <span className="italic">billions</span> of others in this fun multiplayer word battle
        </p>
        <div className="spacer h-16 xl:h-20"></div>
        <p className="text-xl  md:text-3xl">Be the first to guess the correct word - can you beat your friends?</p>
        <div className="spacer h-8"></div>
        <Button onClick={startPlaying}>Start playing </Button>
        <FixedBottomContent></FixedBottomContent>
      </AppLayout>
    </div>
  );
};
