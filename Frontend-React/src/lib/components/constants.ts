export const GameViewEnum = {
  Lobby: 0,
  Starting: 1,
  Started: 2,
  EndedUnsolved: 3,
  Solved: 4,
  Abandoned: 5,
};

export const RoundViewEnum = {
  NotStarted: 0,
  Playing: 1,
  Summary: 2,
};

export const PlayerEventEnum = {
  WordSubmission: 0,
  PlayerDisconnected: 1,
};

export const DEV_URL = "http://localhost:3000";
export const PROD_URL = "https://kawoord.com";
