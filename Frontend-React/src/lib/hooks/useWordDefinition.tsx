import { useEffect, useState } from "react";

interface DefinitionData {
  meanings: DefinitionMeaning[];
}

interface DefinitionMeaning {
  definitions: Definition[];
}

interface Definition {
  definition: string;
}

const useWordDefinition = (word: string) => {
  const [definition, setDefinition] = useState<string[]>();

  useEffect(() => {
    async function getWordDefinition() {
      const res = await fetch(`https://api.dictionaryapi.dev/api/v2/entries/en/${word}`);
      if (res.ok) {
        const data: DefinitionData[] = await res.json();

        setDefinition(data[0].meanings[0].definitions.map((e) => e.definition));
      } else {
        console.log(res.status);
      }
    }

    getWordDefinition().then();
  }, [word]);

  return definition;
};

export default useWordDefinition;
