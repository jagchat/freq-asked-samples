//import Vue from 'psfyVue'
import Vue from 'vue'

import app from './app'
Vue.use(app); //import plugin

var app01 = function () {
  this.self = this;
  this.v = null;
  this.options = null;

  this.show = function (options) {
    self.options = options;
    self.v = new Vue({
      el: options.target,
      template: '<psfy-app01></psfy-app01>',
    });
  }
};

export default app01;
