var path = require("path");
var webpack = require("webpack");
var fableUtils = require("fable-utils");

function resolve(filePath) {
  return path.join(__dirname, filePath)
}

var babelOptions = fableUtils.resolveBabelOptions({
  presets: [
    ["env", {
      "targets": { "browsers": ["last 2 versions", "safari >= 7"] },
      "modules": false
    }]
  ]
});

var isProduction = process.argv.indexOf("-p") >= 0;
console.log("Bundling for " + (isProduction ? "production" : "development") + "...");

module.exports = {
  devtool: "source-map",
  entry: resolve('src/personalweb.fsproj'),
  output: {
    filename: 'bundle.js',
    path: resolve('public/build'),
    publicPath: "/build/",
  },
  resolve: {
    modules: [resolve("./node_modules/")]
  },
  devServer: {
    contentBase: resolve('public'),
    port: 8080,
    host: "0.0.0.0",
    hot: true,
    inline: true
  },
  plugins : isProduction ? [] : [
    new webpack.HotModuleReplacementPlugin(),
    new webpack.NamedModulesPlugin()
  ],
  module: {
    rules: [
      {
        test: /\.fs(x|proj)?$/,
        use: {
          loader: "fable-loader",
          options: {
            babel: babelOptions,
            define: isProduction ? [] : ["DEBUG"],
          }
        }
      },
      {
        test: /\.js$/,
        exclude: /node_modules|paket-files/,
        use: {
          loader: 'babel-loader',
          options: babelOptions
        },
      },
    ]
  }
};
