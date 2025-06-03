import { createApp } from "vue";
import HelloComponent from "../hello-component/dist/hello-component.es.js";
import App from "./App.vue";

const app = createApp(App);
app.use(HelloComponent);
app.mount("#app");
