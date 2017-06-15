var gulp = require("gulp");
var uglify = require("gulp-uglify");
var concat = require("gulp-concat");
var watch = require('gulp-watch');
var gutil = require('gulp-util');
var batch = require('gulp-batch');
var exec = require('gulp-exec');
var jsValidate = require('gulp-jsvalidate');
var exec2 = require('child_process').exec;

gulp.task("combine", function () {
    console.log("dotnet run");
});

gulp.task("watch", function () {
    gulp.run(["combine"]);
    gulp.watch('**/*.cs', ["combine"]);
});

gulp.task("publish", function (cb) {
    exec2('dotnet publish -c Release', function (err, stdout, stderr) {
        console.log(stdout);
        console.log(stderr);
        cb(err);
    });
});
