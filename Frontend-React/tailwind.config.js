// eslint-disable-next-line no-undef
module.exports = {
  content: ["./src/**/*.{js,jsx,ts,tsx}"],
  theme: {
    extend: {
      colors: {
        kawoordLilla: "#593b99",
        kawoordYellow: "rgb(255, 212, 59)",
        kawoordDarkYellow: "#fab005",
        darkLilla: "#3c0776",
      },
      dropShadow: {
        kawoord: "0 5px 0 #3f1794",
      },
      fontFamily: {
        kawoord: ['"Francois One"', "sans-serif"],
      },
      keyframes: {
        wiggle: {
          "0%, 100%": { transform: "rotate(-3deg)" },
          "50%": { transform: "rotate(3deg)" },
        },
      },
      animation: {
        wiggle: "wiggle 200ms ease-in-out infinite",
      },
    },
  },
  plugins: [],
};
