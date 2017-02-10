/*
This file in the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkId=518007
*/

var gulp = require("gulp");
var ts = require("gulp-typescript");
var tsProject = ts.createProject("tsconfig.json");

gulp.task('default', function () {
   // place code for your default task here
});

// compiles ts files in js and outputs them in the same folder
gulp.task("compiletsforadminui", function () {
    return tsProject.src()
        .pipe(tsProject())
        .js.pipe(gulp.dest("."));
});

gulp.task("npmtolib", () => {
   gulp.src([
      "systemjs/dist/*.js",
      "reflect-metadata/*.js",
      "rxjs/**",
      "zone.js/dist/**",
      "@angular/**",
      "core-js/client/*.min.js",
      "ng2-cookies/**"
   ], {
      cwd: "node_modules/**"
   })
   .pipe(gulp.dest("./wwwroot/lib"));
});