import hello from "./hello.vue";

const HelloDirective = {
  install(app) {
    app.component("hello", hello);
  },
};

export default HelloDirective;
