import { createApp } from "vue";
import HelloDirective from "../index.js";
import App from "./App.vue";

const app = createApp(App);
app.use(HelloDirective);
app.mount("#app");
