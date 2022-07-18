import { LogoHeader } from "$lib/layout/LogoHeader";
import { ContentLayout } from "$lib/layout/ContentLayout";
import React from "react";

interface IAppLayout {
  noBg?: boolean;
  padding?: string;
  headerSize?: "small" | "large";
}

const AppLayout: React.FC<IAppLayout> = ({ children, noBg, padding, headerSize = "large" }) => {
  return (
    <div
      className={`app-layout px-4 grid ${
        headerSize === "large" ? "grid-rows-gridApp" : "grid-rows-gridAppSmallHeader"
      } h-screen gap-4 justify-center pb-2 grid-cols-1 md:max-w-2xl m-auto`}
    >
      <LogoHeader headerSize={headerSize} />
      <ContentLayout noBg={noBg} padding={padding}>
        {children}
      </ContentLayout>
    </div>
  );
};

export default AppLayout;
