import Button from "$lib/components/Button";
import React, { useContext, useRef, useState } from "react";
import { Player } from "../../interface";
import { gameServiceContext } from "$lib/components/GameServiceContext";
import AppLayout from "$lib/layout/AppLayout";
import { motion } from "framer-motion";
import { BarLoader } from "react-spinners";
import FixedBottomContent from "$lib/layout/FixedBottomContent";

interface INoGame {
  player: Player;
}

export const NoGame: React.FC<INoGame> = ({ player }) => {
  const gameService = useContext(gameServiceContext);
  const [gameIdInput, setGameIdInput] = useState<string>("");
  const [loading, setLoading] = useState(false);
  const [showInput, setShowInput] = useState(false);
  const inputRef = useRef<HTMLInputElement>(null);

  const joinGame = async () => {
    //todo: Notification if gameId is empty
    if (gameIdInput === "") return;
    setLoading(true);
    await gameService.joinGame(player, gameIdInput);
    setLoading(false);
  };

  const createGame = async () => {
    setLoading(true);
    await gameService.createGame(player);
  };

  const findGame = async () => {
    setLoading(true);
    await gameService.findPublicGame(player);
  };

  const setJoinView = () => {
    setShowInput(true);
    inputRef.current?.focus();
  };

  return (
    <AppLayout>
      <div className="spacer h-6"></div>
      {!loading ? (
        <div>
          <h2 className="text-2xl">How do you want to play?</h2>
          <div className="spacer h-8 md:h-12 xl:h-20"></div>
          <div className="join flex flex-col w-full m-auto">
            {!showInput && (
              <>
                <Button onClick={() => setJoinView()} disabled={showInput}>
                  Join with code
                </Button>
                <p className="font-sans text-sm italic mb-4">Enter a game code to play</p>
              </>
            )}
            {showInput && (
              <motion.div
                animate={{ height: ["1px", "200px"] }}
                transition={{ ease: "easeInOut", duration: 0.2 }}
                style={{ overflow: "hidden" }}
              >
                <>
                  <div className="spacer h-2" />
                  <input
                    autoFocus={true}
                    ref={inputRef}
                    type="text"
                    className="border-2 border-gray-200 rounded p-2 py-4 text-black text-center block mt-auto w-full"
                    value={gameIdInput}
                    onChange={(e) => setGameIdInput(e.target.value.toUpperCase())}
                    placeholder="Enter Game Id"
                  />
                  <Button width="w-full" onClick={joinGame}>
                    Join Game
                  </Button>
                  <div className="spacer h-4"></div>
                  <Button variant="ghost" onClick={() => setShowInput(false)}>
                    Back
                  </Button>
                </>
              </motion.div>
            )}
          </div>
          {!showInput && (
            <>
              <Button variant="secondary" width="w-full" onClick={createGame}>
                Host a private game
              </Button>
              <p className="font-sans text-sm italic mb-4">Play with only your friends</p>
              <Button variant="secondary" width="w-full" onClick={findGame}>
                Find game
              </Button>
              <p className="font-sans text-sm italic mb-4">Join a public game</p>
            </>
          )}
        </div>
      ) : (
        <>
          <div className="spacer h-24"></div>
          <div className="flex items-center justify-center flex-col">
            <div className="spacer h-8 "></div>
            <BarLoader color="#593b99" />
            <div className="spacer h-16 "></div>
            <p className="italic font-sans text-xl lg:text-2xl animate-bounce">Hold tight while we find a game...</p>
            <FixedBottomContent></FixedBottomContent>
          </div>
        </>
      )}
    </AppLayout>
  );
};
