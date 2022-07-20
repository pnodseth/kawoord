import React from "react";
import Button from "./Button";
import { useMsal } from "@azure/msal-react";
import { b2cPolicies } from "../../auth/policies";
import { RedirectRequest } from "@azure/msal-browser";
import { loginRequest } from "../../auth/authConfig";

export const SignedInMenu = () => {
  const { instance } = useMsal();

  function signOut() {
    instance.logout().then();
  }

  function editProfile() {
    const test: RedirectRequest = {
      authority: b2cPolicies.authorities.editProfile.authority,
      scopes: loginRequest.scopes,
    };
    instance.loginRedirect(test).then();
  }

  return (
    <div className="flex flex-col">
      <div className="spacer h-4"></div>
      <Button onClick={editProfile} variant="ghost">
        Change Display Name
      </Button>
      <Button onClick={signOut} variant="ghost">
        Sign out
      </Button>
    </div>
  );
};
