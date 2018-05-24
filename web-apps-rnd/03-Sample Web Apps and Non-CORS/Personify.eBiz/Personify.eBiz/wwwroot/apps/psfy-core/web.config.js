//build: 
//  --> npm run build:dev
//  --> npm run build:watch

const path = require('path');
const webpack = require('webpack');
const CopyWebpackPlugin = require('copy-webpack-plugin');

module.exports = {
    entry: {
        externals: ["jquery", "post-robot", "lodash", "vue"],
        core: './src/app.js',
        host: './src/host.js',
        proxy: './src/proxy.js'
    },
    plugins: [
        new webpack.optimize.CommonsChunkPlugin({
            name: 'externals', //common bundle's name.
            minChunks: Infinity
        }),
        new webpack.DefinePlugin({
            'SERVICE_URL': JSON.stringify("http://localhost:12602/"),
            'POSTROBOT_LOG_LEVEL': JSON.stringify("error")
        }),
        new CopyWebpackPlugin([{
            from: './src/*.htm',
            flatten: true
        }])
    ],
    resolve: {

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