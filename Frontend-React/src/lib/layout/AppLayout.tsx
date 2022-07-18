import { LogoHeader } from "$lib/layout/LogoHeader";
import { ContentLayout } from "$lib/layout/ContentLayout";
import React from "react";

interface IAppLayout {
  noBg?: boolean;
  padding?: string;
}

const AppLayout: React.FC<IAppLayout> = ({ children, noBg, padding }) => {
  return (
    <div className="app-layout px-4 grid grid-rows-gridApp h-screen gap-4 justify-center pb-2 grid-cols-1 md:max-w-2xl m-auto">
      <LogoHeader />
      <ContentLayout noBg={noBg} padding={padding}>
        {children}
      </ContentLayout>
    </div>
  );
};

export default AppLayout;
