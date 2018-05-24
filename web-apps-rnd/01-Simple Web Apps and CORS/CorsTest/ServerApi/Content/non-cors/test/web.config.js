const path = require('path');
const webpack = require('webpack');

module.exports = {
  entry: {
    app1: './src/app.js'
  },
  externals: {
    psfy: 'psfy',
    jq: 'psfy.core.externals.jQuery'
  },
  devtool: 'eval-source-map',
  output: {
    path: path.resolve(__dirname, 'dist'),
    library: ["psfy", "[name]"],
    libraryTarget: "var",
    filename: 'psfy.[name].js',
    libraryExport: 'default'
  }
};