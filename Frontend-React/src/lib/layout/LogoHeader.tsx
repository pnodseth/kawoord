import React from "react";
import { Account } from "$lib/components/Account";

export function LogoHeader() {
  return (
    <div className="max-w-2xl m-auto">
      <div className="spacer h-1 lg:h-6" />
      <div className="flex w-full items-center justify-center">
        <h1 className="text-6xl text-center font-kawoord text-white">Kawoord</h1>
        <div className="account absolute right-8">
          <Account />
        </div>
      </div>
      <div className="spacer h-4" />
    </div>
  );
}
