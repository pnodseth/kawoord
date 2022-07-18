import { useIsAuthenticated, useMsal } from "@azure/msal-react";
import React, { useState } from "react";
import { motion } from "framer-motion";
import { loginRequest } from "../../auth/authConfig";

export const Account = () => {
  const [showMenu, setShowMenu] = useState(false);
  const isAuthenticated = useIsAuthenticated();
  const { instance } = useMsal();

  function login() {
    instance.loginRedirect(loginRequest);
  }

  return (
    <>
      <button onClick={() => setShowMenu((showMenu) => !showMenu)}>Icon</button>
      {showMenu && (
        <motion.div
          animate={{ opacity: [0, 1] }}
          transition={{ duration: 0.2, type: "spring" }}
          className="popover relative z-10"
        >
          {!isAuthenticated ? (
            <div className="absolute w-40 bg-kawoordWhite text-kawoordLilla p-4 right-0 rounded-lg border-kawoordLilla border-2">
              <h1>No account</h1>
              <div>Sign up to change your display name, see stats etc.</div>
              <button onClick={login}>Sign up</button>
            </div>
          ) : (
            <div className="absolute w-40 bg-kawoordWhite text-kawoordLilla p-4 right-0 rounded-lg border-kawoordLilla border-2">
              <h1>Logged in</h1>
              <div>See stats</div>
            </div>
          )}
        </motion.div>
      )}
    </>
  );
};
