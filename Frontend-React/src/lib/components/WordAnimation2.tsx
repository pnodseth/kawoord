import React, { FC, useEffect, useState } from "react";
import { LetterEvaluation } from "../../interface";
import { animated, config, useTransition } from "@react-spring/web";
import { LetterTile } from "$lib/components/LetterTile";

interface WordAnimation2Props {
  evalArr: LetterEvaluation[];
  showLetters: boolean;
}

export const WordAnimation2: FC<WordAnimation2Props> = ({ evalArr, showLetters }) => {
  const [wordArr, setWordArr] = useState<LetterEvaluation[]>([]);

  useEffect(() => {
    for (let i = 0; i <= 4; i++) {
      setTimeout(() => {
        const newArr = evalArr.slice(0, i + 1);
        setWordArr(newArr);
      }, 500 * i + 1);
    }
  }, [evalArr]);

  //
  const transitions = useTransition(wordArr, {
    from: { x: 0, y: -80, opacity: 1 },
    enter: () => async (next) => {
      await next({ y: 0 });
    },
    leave: { opacity: 0, x: 0, y: 0 },
    delay: 0,
    config: config.wobbly,
    keys: wordArr.map((e) => e.wordIndex),
  });

  return (
    <>
      <div className="grid grid-cols-5 h-12  gap-3 px-12">
        {transitions((styles, item) => (
          <animated.div style={styles}>
            <LetterTile e={item} showLetter={showLetters} />
          </animated.div>
        ))}
      </div>
    </>
  );
};
