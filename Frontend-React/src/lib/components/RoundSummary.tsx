import { LetterEvaluation, RoundEvaluation, RoundSummaryParams } from "../../interface";
import React, { FC, useEffect, useState } from "react";
import { PlayerEvaluationWord } from "$lib/components/PlayerEvaluationWord";
import { WordAnimation } from "$lib/components/WordAnimation";
import { config, useTransition, animated } from "@react-spring/web";

const WordAnimation2: FC<{ evalArr: LetterEvaluation[] }> = ({ evalArr }) => {
  const [wordArr, setWordArr] = useState<LetterEvaluation[]>([]);
  console.log("evalarr: ", evalArr);

  useEffect(() => {
    let interval: number | undefined;
    setWordArr([evalArr[0]]);

    for (let i = 0; i <= 4; i++) {
      interval = setTimeout(() => {
        const newArr = evalArr.slice(0, i + 1);
        setWordArr(newArr);
      }, 500 * i + 1);
    }

    return function () {
      clearInterval(interval);
    };
  }, []);

  //
  const transitions = useTransition(wordArr, {
    from: { x: -200, y: 0, opacity: 1 },
    enter: { y: 0, x: 0, opacity: 1 },
    leave: { opacity: 0, x: 0, y: 0 },
    delay: 0,
    config: config.wobbly,
  });

  return (
    <>
      <div style={{ display: "flex" }}>
        {transitions((styles, item) => (
          <animated.div style={styles}>{item.letter}</animated.div>
        ))}
      </div>
    </>
  );
};

export function RoundSummary({ gameState: { evaluations }, player }: RoundSummaryParams) {
  const [wordLetters, setWordLetters] = useState<RoundEvaluation>();

  useEffect(() => {
    function isCurrentPlayer(e: RoundEvaluation) {
      return e.player.id === player.id;
    }

    const evaluation = evaluations?.roundEvaluations.find((e) => isCurrentPlayer(e));
    if (evaluation) {
      setWordLetters(evaluation);
    }
  }, [evaluations?.roundEvaluations, player.id]);

  function isOtherPlayers(e: RoundEvaluation) {
    return e.player.id !== player.id;
  }

  function sortEvaluations(a: LetterEvaluation, b: LetterEvaluation) {
    if (a.wordIndex < b.wordIndex) return -1;
    return 1;
  }

  return (
    <>
      <section className="summary">
        <h2>Summary</h2>
        <h3>Your Word:</h3>
        {wordLetters && (
          <>
            <WordAnimation letters={wordLetters?.evaluation.sort(sortEvaluations)}>
              {wordLetters?.evaluation.map((l) => {
                return <span key={l.wordIndex}>{l.letter}</span>;
              })}
            </WordAnimation>
            <WordAnimation2 evalArr={wordLetters.evaluation.sort(sortEvaluations)} />
          </>
        )}

        {/*OTHER PLAYERS WORD*/}
        <h3>Other players:</h3>
        {evaluations?.roundEvaluations
          .filter((e) => isOtherPlayers(e))
          .map((p) => {
            return (
              <div key={p.player.id}>
                <h4>{p.player.name}</h4>
                <PlayerEvaluationWord evaluation={p} />
              </div>
            );
          })}
      </section>
    </>
  );
}
