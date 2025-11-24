/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        primary: {
          DEFAULT: '#0d1b2a', // Azul Marino Oscuro
          dark: '#1b263b',
          light: '#415a77',
        },
        secondary: {
          DEFAULT: '#FF8C00', // Naranja Intenso
          dark: '#e67e00',
          light: '#ffa500',
        },
        accent: '#FF8C00',
        text: {
          primary: '#ffffff',
          secondary: '#343a40',
          light: '#e0e1dd',
        },
      },
    },
  },
  plugins: [],
}
