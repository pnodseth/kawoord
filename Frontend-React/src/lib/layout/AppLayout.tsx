import { LogoHeader } from "$lib/layout/LogoHeader";
import { ContentLayout } from "$lib/layout/ContentLayout";
import React from "react";

interface IAppLayout {
  noBg?: boolean;
  padding?: string;
}

const AppLayout: React.FC<IAppLayout> = ({ children, noBg, padding }) => {
  return (
    <>
      <LogoHeader />
      <div className="spacer lg:h-4 xl:h-8"></div>
      <ContentLayout noBg={noBg} padding={padding}>
        {children}
      </ContentLayout>
    </>
  );
};

export default AppLayout;
