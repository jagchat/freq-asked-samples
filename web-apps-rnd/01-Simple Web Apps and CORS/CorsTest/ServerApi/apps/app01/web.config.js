//build: 
//  --> npm run build:dev
//  --> npm run build:watch
const path = require('path');
const webpack = require('webpack');
const CopyWebpackPlugin = require('copy-webpack-plugin');

module.exports = {
  entry: {
    app01: './src/app.js'
  },
  externals: {
    psfy: 'psfy',
    jq: 'psfy.core.externals.jQuery'
  },
  plugins: [
    new CopyWebpackPlugin([{
      from: './src/*.htm',
      flatten: true
    }])
  ],
  devtool: 'eval-source-map',
  output: {
    path: path.resolve(__dirname, 'dist'),
    library: ["psfy", "[name]"],
    //library: "app01",
    libraryTarget: "var",
    filename: 'psfy.[name].js',
    //filename: 'psfy.app01.js',
    libraryExport: 'default'
  }
};