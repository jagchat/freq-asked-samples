import Vue from 'psfyVue'
//import Vue from 'vue';
import App from './App.vue'

const plugin = {
  install(Vue, options) {
    Vue.component('psfy-app02', App)
  }
}

export default plugin;
