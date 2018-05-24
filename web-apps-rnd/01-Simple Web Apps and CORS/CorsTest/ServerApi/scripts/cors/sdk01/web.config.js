//build: 
//  --> npm run build:dev
//  --> npm run build:watch

const path = require('path');
const webpack = require('webpack');

module.exports = {
  entry: {
    externals: ["jquery", "underscore", "vue"],
    core: './src/core.js'
  },
  plugins: [
    new webpack.optimize.CommonsChunkPlugin({
      name: 'externals', //common bundle's name.
      minChunks: Infinity
    }),
    new webpack.DefinePlugin({
      'SERVICE_URL': JSON.stringify("http://localhost:48724")
    })
  ],
  devtool: 'eval-source-map',
  output: {
    path: path.resolve(__dirname, 'dist'),
    library: ["psfy", "[name]"],
    libraryTarget: "var",
    filename: 'psfy.[name].js',
    libraryExport: 'default'
  },
  resolve: {
    alias: {
      vue$: 'vue/dist/vue.esm.js'
    }
  }
};