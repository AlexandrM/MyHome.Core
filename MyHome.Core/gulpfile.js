var gulp = require("gulp");
var uglify = require("gulp-uglify");
var concat = require("gulp-concat");
var watch = require('gulp-watch');
var gutil = require('gulp-util');
var batch = require('gulp-batch');
var exec = require('gulp-exec');
var jsValidate = require('gulp-jsvalidate');

gulp.task("combine", function () {
    console.log("dotnet run");
});

gulp.task("watch", function () {
    gulp.run(["combine"]);
    gulp.watch('**/*.cs', ["combine"]);
});