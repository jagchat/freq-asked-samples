// ViteApp/vite.config.js
import { defineConfig } from "vite";
import vue from "@vitejs/plugin-vue";
import path from "path";

export default defineConfig({
  plugins: [vue()],
  build: {
    lib: {
      entry: {
        mount: path.resolve(__dirname, "./src/mount.js"),
        MyWidget: path.resolve(__dirname, "./src/MyWidget.vue"),
      },
      formats: ["es"],
      name: "MyApp", // global name for UMD builds
    },
    rollupOptions: {
      external: ["vue"],
      output: {
        entryFileNames: "assets/[name].js",
        globals: {
          vue: "Vue",
        },
      },
    },
    outDir: "../AspNetCoreApp/wwwroot/js",
    emptyOutDir: true,
  },
});

