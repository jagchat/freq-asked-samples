<template>
  <div id="app" class="app-container">
    <!-- <img class="logo" :src="require('./assets/logo.png')"> -->
    <h1>{{ msg }}</h1>

    First number:
    <!-- one way binding-->
    <!--<input type="text" :value="x" />-->
    <!-- two way binding-->
    <input type="text" v-model="x" />

    <br/> Second number:
    <input type="text" v-model="y" />
    <br/>
    <button id="btnSum" v-on:click="doSum">Show Sum</button>
    <div>{{resultMsg}}</div>
  </div>
</template>

<script>
import psfy from "psfy";
import jQuery from "jQuery";

export default {
  name: "app",
  // beforeCreate: function() {
  //   console.log("jQuery version: " + jQuery.fn.jquery);
  //   psfy.host.ready(function() {
  //     console.log("jQuery version: " + psfy.core.externals.jQuery.fn.jquery);
  //   });
  // },
  data() {
    return {
      msg: "Welcome to Personify eBiz App",
      x: 3,
      y: 5,
      resultMsg: ""
    };
  },
  methods: {
    doSum: function() {
      var vself = this;

      psfy.host.ajax
        .post({
          url: "/test/sum",
          data: {
            a: this.x,
            b: this.y
          }
        })
        .done(function(o) {
          vself.resultMsg = "Result = " + o;
        });
    }
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
