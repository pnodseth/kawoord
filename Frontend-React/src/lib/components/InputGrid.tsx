import React from "react";

export function InputGrid({ letterArr }: { letterArr: string[] }) {
  return (
    <>
      <div className="letters grid grid-cols-5 h-12  gap-3 mb-6">
        <p className="border-black border-2 flex justify-center items-center font-kawoord text-xl">
          {letterArr[0]?.toUpperCase() || ""}
        </p>
        <p className="border-black border-2 flex justify-center items-center font-kawoord text-xl">
          {letterArr[1]?.toUpperCase() || ""}
        </p>
        <p className="border-black border-2 flex justify-center items-center font-kawoord text-xl">
          {letterArr[2]?.toUpperCase() || ""}
        </p>
        <p className="border-black border-2 flex justify-center items-center font-kawoord text-xl">
          {letterArr[3]?.toUpperCase() || ""}
        </p>
        <p className="border-black border-2 flex justify-center items-center font-kawoord text-xl">
          {letterArr[4]?.toUpperCase() || ""}
        </p>
      </div>
    </>
  );
}
