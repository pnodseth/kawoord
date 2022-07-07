import React, { useContext, useState } from "react";
import { gameServiceContext } from "$lib/components/GameServiceContext";
import { PlayerEventData } from "../../interface";
import { AnimatePresence, motion } from "framer-motion";
import { PlayerEventEnum } from "$lib/components/constants";

export function PlayerEvents() {
  const gameService = useContext(gameServiceContext);
  const [playerEvents, setPlayerEvents] = useState<PlayerEventData[]>([]);
  const [playerJoinAudio] = useState(new Audio("/audio/player_join.wav")); //convert to mp3

  const submittedWords = playerEvents.filter((e) => e.type === PlayerEventEnum.WordSubmission);
  const disconnectedPlayers = playerEvents.filter((e) => e.type === PlayerEventEnum.Disconnected);

  gameService.registerCallbacks({
    onPlayerEvent: (data) => {
      setPlayerEvents((prev) => [...prev, data]);
      playerJoinAudio.play().then();

      setTimeout(() => {
        setPlayerEvents((prev) => prev.filter((e) => e.id !== data.id));
      }, 5000);
    },
  });

  return (
    <div className="player-events absolute left-4 top-0 text-black">
      <ul>
        <AnimatePresence>
          {submittedWords.map((e) => {
            return (
              <motion.div
                animate={{ x: [0, 0], y: [0, 30], opacity: [0, 1] }}
                exit={{ opacity: [1, 0] }}
                transition={{ duration: 0.5, type: "spring" }}
                key={e.id}
                className="px-4 py-1 bg-kawoordLilla text-white rounded-xl mb-1 text-sm"
              >
                <h2>{e.playerName} has submitted</h2>
              </motion.div>
            );
          })}
        </AnimatePresence>
      </ul>
      <ul>
        {disconnectedPlayers.map((e) => {
          return (
            <motion.div
              animate={{ x: [0, 0], y: [0, 30], opacity: [0, 1] }}
              exit={{ opacity: [1, 0] }}
              transition={{ duration: 0.5, ease: "easeInOut", type: "tween" }}
              key={e.id}
              className="px-4 py-1 bg-kawoordLilla text-white rounded-xl mb-1 text-sm"
            >
              <h2>{e.playerName} disconnected..</h2>
            </motion.div>
          );
        })}
      </ul>
    </div>
  );
}
