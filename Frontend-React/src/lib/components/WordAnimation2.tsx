import React, { FC, useEffect, useRef, useState } from "react";
import { LetterEvaluation } from "../../interface";
import { animated, config, useTransition } from "@react-spring/web";
import { LetterTile } from "$lib/components/LetterTile";

export const WordAnimation2: FC<{ evalArr: LetterEvaluation[] }> = ({ evalArr }) => {
  const [wordArr, setWordArr] = useState<LetterEvaluation[]>([]);
  const intervalRef = useRef<number[]>();

  useEffect(() => {
    const arr: number[] = [];

    for (let i = 0; i <= 4; i++) {
      const id = setTimeout(() => {
        const newArr = evalArr.slice(0, i + 1);
        setWordArr(newArr);
      }, 500 * i + 1);
      arr.push(id);
    }

    intervalRef.current = arr;

    return function () {
      intervalRef.current?.forEach((id) => {
        clearInterval(id);
      });
    };
  }, [evalArr]);

  //
  const transitions = useTransition(wordArr, {
    from: { x: 0, y: -80, opacity: 1, showBg: false },
    enter: () => async (next) => {
      /*await next({ x: 0 });*/
      await next({ y: 0, showBg: true });
    },
    leave: { opacity: 0, x: 0, y: 0 },
    delay: 0,
    config: config.wobbly,
  });

  return (
    <>
      <div className="grid grid-cols-5 h-12  gap-3 px-12">
        {transitions((styles, item) => (
          <animated.div style={styles}>
            <LetterTile e={item} showLetter={styles.showBg.goal} />
          </animated.div>
        ))}
      </div>
    </>
  );
};
