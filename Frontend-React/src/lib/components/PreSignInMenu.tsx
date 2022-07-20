import Button from "$lib/components/Button";
import React from "react";
import { useMsal } from "@azure/msal-react";
import { loginRequest } from "../../auth/authConfig";

export const PreSignInMenu = () => {
  const { instance } = useMsal();

  function login() {
    instance.loginRedirect(loginRequest).then();
  }

  return (
    <div className="player-menu font-sans p-6">
      <p>Sign in or create an account to change your Display Name, see game history, stats and more!</p>
      <div className="spacer h-8"></div>
      <Button onClick={login}>Login / Create Account</Button>
    </div>
  );
};
