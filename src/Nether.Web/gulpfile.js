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
    console.log('compile ts ui files into js');
    var tsResult = gulp.src(["./wwwroot/Features/AdminWebUi/app/**/*.ts"], {base: "."}) 
        .pipe(tsProject()); 
    return tsResult.js.pipe(gulp.dest('.'));
});

gulp.task("npmtolib", () => {
    // TODO - tighten this up so that we copy less
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
        .pipe(gulp.dest("./wwwroot/Features/AdminWebUi/lib"));

    gulp.src([
        "bootstrap/dist/**",
        "jquery/dist/**",
        "jquery-ui/**"
    ], {
            cwd: "bower_components/**"
        })
        .pipe(gulp.dest("./wwwroot/Features/AdminWebUi/lib"));
});