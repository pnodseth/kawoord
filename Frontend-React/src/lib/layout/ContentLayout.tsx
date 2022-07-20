import React from "react";
import { motion } from "framer-motion";

interface ContentLayoutProps {
  noBg?: boolean;
  padding?: string;
}

export const ContentLayout: React.FC<ContentLayoutProps> = ({ children, noBg, padding }) => (
  <motion.div
    initial={{ opacity: 0 }}
    exit={{ opacity: 0 }}
    animate={{ opacity: [0, 1] }}
    transition={{ duration: 0.2 }}
    className={`relative  px-2 text-center pb-6 h-full ${
      !noBg ? "bg-kawoordWhite text-gray-600 " : "text-white"
    } rounded-3xl ${padding ? padding : "p-8"} font-kawoord`}
  >
    {children}
  </motion.div>
);
