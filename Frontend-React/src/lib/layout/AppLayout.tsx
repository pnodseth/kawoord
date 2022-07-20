import { LogoHeader } from "$lib/layout/LogoHeader";
import { ContentLayout } from "$lib/layout/ContentLayout";
import React, { useState } from "react";
import { useIsAuthenticated } from "@azure/msal-react";
import { AnimatePresence, motion } from "framer-motion";
import { PreSignInMenu } from "$lib/components/PreSignInMenu";
import { SignedInMenu } from "$lib/components/SignedInMenu";

interface IAppLayout {
  noBg?: boolean;
  padding?: string;
  headerSize?: "small" | "large";
}

const AppLayout: React.FC<IAppLayout> = ({ children, noBg, padding, headerSize = "large" }) => {
  const isAuthenticated = useIsAuthenticated();
  const [showMenu, setShowMenu] = useState(false);

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
          <ContentLayout>{!isAuthenticated ? <PreSignInMenu /> : <SignedInMenu />}</ContentLayout>
        )}
      </AnimatePresence>
    </div>
  );
};

export default AppLayout;
