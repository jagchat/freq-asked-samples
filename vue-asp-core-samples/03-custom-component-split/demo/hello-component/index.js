import hello from "./src/hello.vue";

const HelloComponent = {
  install(app) {
    app.component("hello", hello);
  },
};

export default HelloComponent;
