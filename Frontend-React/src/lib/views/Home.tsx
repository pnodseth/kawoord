import { LogoHeader } from "$lib/layout/LogoHeader";
import { ContentLayout } from "$lib/layout/ContentLayout";
import Button from "$lib/components/Button";
import React from "react";
import { Player } from "../../interface";
import { useNavigate } from "react-router-dom";
import FixedBottomContent from "$lib/layout/FixedBottomContent";

interface IHome {
  player: Player;
}

// React MSAL Hooks: https://github.com/AzureAD/microsoft-authentication-library-for-js/blob/dev/lib/msal-react/docs/hooks.md

export const Home: React.FC<IHome> = ({ player }) => {
  const navigate = useNavigate();

  const startPlaying = () => {
    if (!player) {
      navigate("/player");
    }
    navigate("/play");
  };
  return (
    <div>
      <LogoHeader />
      {/*<button onClick={() => login()}>Login</button>*/}
      <ContentLayout noBg>
        <h1 className="text-2xl md:text-3xl xl:mt-12">Welcome, friend!</h1>
        <div className="spacer h-16 md:h-20"></div>
        <p className="text-xl md:text-3xl">
          {" "}
          Join <span className="italic">billions</span> of others in this fun multiplayer word battle
        </p>
        <div className="spacer h-16 xl:h-20"></div>
        <p className="text-xl  md:text-3xl">Be the first to guess the correct word - can you beat your friends?</p>
        <FixedBottomContent>
          <Button onClick={startPlaying}>Start playing </Button>
        </FixedBottomContent>
      </ContentLayout>
    </div>
  );
};
