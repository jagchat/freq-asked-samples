import { defineConfig } from "vite";
import vue from "@vitejs/plugin-vue";
import path from "path";

export default defineConfig({
  plugins: [vue()],
  build: {
    sourcemap: true,
    rollupOptions: {
      external: ["vue"], // Vue provided externally
      output: {
        globals: {
          vue: "Vue",
        },
        entryFileNames: `assets/index.js`, // disables hash for entry
        chunkFileNames: `assets/[name].js`, // disables hash for chunks
        assetFileNames: `assets/[name][extname]`, // disables hash for assets
      },
    },
  },
  server: {
    port: 3000,
  },
  resolve: {
    alias: {
      "@": path.resolve(__dirname, "./"),
    },
  },
  root: "./",
});
