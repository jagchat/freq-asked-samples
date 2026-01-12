import { defineConfig } from "vite";
import vue from "@vitejs/plugin-vue";
import cssInjectedByJs from "vite-plugin-css-injected-by-js";
import path from "path";

export default defineConfig({
  plugins: [vue(), cssInjectedByJs()],
  build: {
    sourcemap: true,
    minify: true,
    lib: {
      entry: "./index.js",
      name: "HelloComponent",
      fileName: "hello-component",
      formats: ["es", "umd"],
    },
    rollupOptions: {
      external: ["vue"],
      output: {
        globals: {
          vue: "Vue",
        },
        entryFileNames: "hello-component.[format].js",
      },
    },
  },
  resolve: {
    alias: {
      "@": path.resolve(__dirname, "./"),
    },
  },
  root: process.env.NODE_ENV === "development" ? "./playground" : "./",
});
