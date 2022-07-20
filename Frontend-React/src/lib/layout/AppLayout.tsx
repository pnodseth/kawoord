import { LogoHeader } from "$lib/layout/LogoHeader";
import { ContentLayout } from "$lib/layout/ContentLayout";
import React, { useState } from "react";
import { useIsAuthenticated, useMsal } from "@azure/msal-react";
import { loginRequest } from "../../auth/authConfig";
import Button from "$lib/components/Button";
import { AnimatePresence, motion } from "framer-motion";

interface IAppLayout {
  noBg?: boolean;
  padding?: string;
  headerSize?: "small" | "large";
}

const AppLayout: React.FC<IAppLayout> = ({ children, noBg, padding, headerSize = "large" }) => {
  const isAuthenticated = useIsAuthenticated();
  const [showMenu, setShowMenu] = useState(false);
  const { instance } = useMsal();

  function login() {
    instance.loginRedirect(loginRequest).then();
  }

  return (
    <div
      className={`app-layout px-4 grid ${
        headerSize === "large" ? "grid-rows-gridApp" : "grid-rows-gridAppSmallHeader"
      } h-screen gap-4 justify-center pb-2 grid-cols-1 md:max-w-2xl m-auto`}
    >
      <LogoHeader headerSize={headerSize} setShowMenu={setShowMenu} />
      <AnimatePresence>
        {/*Game Content */}
        {!showMenu ? (
          <motion.div>
            <ContentLayout noBg={noBg} padding={padding}>
              {children}
            </ContentLayout>
          </motion.div>
        ) : (
          /* Player menu */
          <ContentLayout>
            {!isAuthenticated ? (
              <div className="player-menu font-sans p-6">
                <p>Sign in or create an account to change your username, see stats and more!</p>
                <div className="spacer h-8"></div>
                <Button onClick={login}>Login / Create Account</Button>
              </div>
            ) : (
              <h1>Logged in</h1>
            )}
          </ContentLayout>
        )}
      </AnimatePresence>
    </div>
  );
};

export default AppLayout;
