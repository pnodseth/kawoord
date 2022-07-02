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
      <ContentLayout noBg={noBg} padding={padding}>
        {children}
      </ContentLayout>
    </>
  );
};

export default AppLayout;
