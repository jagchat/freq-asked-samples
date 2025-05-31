import { createApp } from "vue";
import HelloDirective from "../hello-directive/dist/hello-directive.es.js";
import App from "./App.vue";

const app = createApp(App);
app.use(HelloDirective);
app.mount("#app");
