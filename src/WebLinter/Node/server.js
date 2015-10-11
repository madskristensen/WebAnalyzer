var http = require("http"),
    fs = require("fs");

var start = function (port) {
    http.createServer(function (req, res) {

        if (!req.url || req.url.length < 2) {
            response.writeHead(200, { 'Content-Type': 'text/plain' });
            response.end();
            return;
        }

        var path = req.url.substring(1);
        var body = "";

        req.on('data', function (data) {
            body += data;
        });

        req.on('end', function () {
            try {
                if (body === "")
                    return;

                var linter = linters[path];

                if (linter) {
                    var data = JSON.parse(body);
                    var result = linter(data.config, data.files);

                    res.writeHead(200, { 'Content-Type': 'application/json' });
                    res.write(JSON.stringify(result));
                }
            }
            catch (e) {
                response.writeHead(500, { 'Content-Type': 'text/plain' });
                response.write("Server error: " + e.message);
            }
            finally {
                res.end();
            }
        });

    }).listen(port);
};

var linters = {

    eslint: function (configFile, files) {
        var CLIEngine = require("eslint").CLIEngine;
        var cli = new CLIEngine({ configFile: configFile });
        var report = cli.executeOnFiles(files);
        return report.results;
    },

    tslint: function (configFile, files) {
        var tslint = require("tslint");
        var options = {
            formatter: "json",
            configuration: JSON.parse(fs.readFileSync(configFile, "utf8"))
        };

        var results = [];

        for (var i = 0; i < files.length; i++) {
            var file = files[i];
            var ll = new tslint(file, fs.readFileSync(file, "utf8"), options);
            results = results.concat(JSON.parse(ll.lint().output));
        }

        return results;
    },

    coffeelint: function (configFile, files) {
        var linter = require("coffeelint");
        var options = {};
        var results = {};

        var config = JSON.parse(fs.readFileSync(configFile, "utf8"));
        options.configFile = undefined;
        for (var key in config) {
            options[key] = config[key];
        }

        for (var i = 0; i < files.length; i++) {
            var file = files[i];
            var literate = !!file.match(/\.(litcoffee|coffee\.md)$/i);
            var result = linter.lint(fs.readFileSync(file, "utf8"), options, literate);
            results[file] = result;
        }

        return results;
    },

    csslint: function (configFile, files) {
        var linter = require("csslint").CSSLint;

        var options = JSON.parse(fs.readFileSync(configFile, "utf8"));
        var results = {};

        for (var i = 0; i < files.length; i++) {
            var file = files[i];
            var result = linter.verify(fs.readFileSync(file, "utf8"), options);

            results[file] = result.messages;
        }

        return results;
    }
};

start(process.argv[2]);