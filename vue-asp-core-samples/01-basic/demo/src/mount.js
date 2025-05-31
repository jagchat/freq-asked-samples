// VueApp/mount.js
import { createApp } from "vue";
import MyWidget from "./MyWidget.vue";

export function mountMyWidget(el) {
  createApp(MyWidget).mount(el);
}
