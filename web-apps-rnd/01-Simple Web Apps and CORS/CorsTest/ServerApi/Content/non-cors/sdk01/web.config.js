const path = require('path');
const webpack = require('webpack');

module.exports = {
  entry: {
    externals: ["jquery"],
    core: './src/core.js'
  },
  plugins: [
    new webpack.optimize.CommonsChunkPlugin({
      name: 'externals', //common bundle's name.
      minChunks: Infinity
    })
  ],
  devtool: 'eval-source-map',
  output: {
    path: path.resolve(__dirname, 'dist'),
    library: ["psfy", "[name]"],
    libraryTarget: "var",
    filename: 'psfy.[name].js',
    libraryExport: 'default'
  }
};