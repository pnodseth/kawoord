import { LogoHeader } from "$lib/layout/LogoHeader";
import { ContentLayout } from "$lib/layout/ContentLayout";
import Button from "$lib/components/Button";
import React from "react";
import { Player } from "../../interface";
import { useNavigate } from "react-router-dom";

interface IHome {
  player: Player;
}

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
      <ContentLayout noBg>
        <h1 className="text-2xl">Welcome, friend!</h1>
        <div className="spacer h-6"></div>
        <p> Join billions of others in this fun multiplayer word battle</p>
        <div className="spacer h-6"></div>
        <p>Be the first to guess the correct word - you only have six rounds!</p>
        <div className="spacer h-20"></div>
        <Button onClick={startPlaying}>Start playing </Button>
      </ContentLayout>
    </div>
  );
};
