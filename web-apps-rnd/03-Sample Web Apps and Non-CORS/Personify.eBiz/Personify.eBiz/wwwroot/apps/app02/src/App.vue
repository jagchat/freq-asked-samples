<template>
  <div id="app" class="app-container">
    <h1>{{ msg }}</h1>
    <psfy-app01></psfy-app01>
  </div>
</template>

<script>
import psfy from "psfy";
import jQuery from "jQuery";
import Vue from "vue";

import app01 from "app01/src/app"; //TODO: fix this
Vue.use(app01); //import plugin

export default {
  name: "app",
  data() {
    return {
      msg: "Welcome to Personify eBiz App 02"
    };
  }
};
</script>

<style>
.app-container {
  font-family: "Avenir", Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  border: 1px solid gray;
  margin: 5px;
  margin-top: 25px;
}

h1,
h2 {
  font-weight: normal;
}

.logo {
  width: 200px;
  height: 45px;
}
</style>
