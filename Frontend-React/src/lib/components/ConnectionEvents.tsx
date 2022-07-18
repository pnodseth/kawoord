import React, { useContext, useState } from "react";
import { gameServiceContext } from "$lib/contexts/GameServiceContext";
import { ConnectionEvent } from "../../interface";

const ConnectionEvents = () => {
  const gameService = useContext(gameServiceContext);
  const [eventType, setEventType] = useState<ConnectionEvent>();

  gameService.registerCallbacks({
    onConnectionEvent: (type) => {
      setEventType(type);

      if (type == "reconnected") {
        setTimeout(() => {
          setEventType(undefined);
        }, 3000);
      }
    },
  });

  const connectionText = () => {
    if (eventType == "reconnecting") {
      return "Connection lost. Reconnecting...";
    }
    return "Connected!";
  };

  if (!eventType) return null;
  return (
    <div
      className={`z-10 ${
        eventType == "reconnecting" ? "bg-red-600" : "bg-green-600"
      } h-8 absolute w-full left-0  top-0 flex justify-center items-center text-white`}
    >
      <h1>{connectionText()}</h1>
    </div>
  );
};

export default ConnectionEvents;
