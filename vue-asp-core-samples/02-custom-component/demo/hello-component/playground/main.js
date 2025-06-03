import { createApp } from "vue";
import HelloComponent from "../index.js";
import App from "./App.vue";

const app = createApp(App);
app.use(HelloComponent);
app.mount("#app");
