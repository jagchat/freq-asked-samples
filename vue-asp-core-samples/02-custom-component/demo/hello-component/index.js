import hello from "./hello.vue";

const HelloComponent = {
  install(app) {
    app.component("hello", hello);
  },
};

export default HelloComponent;
