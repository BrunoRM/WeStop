const gulp = require('gulp');
const jshint = require('gulp-jshint');
const clean = require('gulp-clean');
const concat = require('gulp-concat');
const uglify = require('gulp-uglify-es').default;
const es = require('event-stream');
const htmlmin = require('gulp-htmlmin');
const cleanCSS = require('gulp-clean-css');
const runSequence = require('run-sequence');
const rename = require('gulp-rename');
const imagemin = require('gulp-imagemin');
const merge2 = require('merge2');

gulp.task('clean', function () {
    return gulp.src('dist/')
        .pipe(clean());
});

gulp.task('jshint', function () {
    return gulp.src('js/**/*.js')
        .pipe(jshint())
        .pipe(jshint.reporter('default'));
});

gulp.task('uglifyAndConcatJs', function(){
    return es.merge([
        gulp.src(
            [
                'node_modules/@aspnet/signalr/dist/browser/signalr.min.js',
                'node_modules/angular/angular.min.js', 
                'node_modules/angular-route/angular-route.min.js', 
                'node_modules/angular-aria/angular-aria.min.js', 
                'node_modules/angular-animate/angular-animate.min.js', 
                'node_modules/angular-messages/angular-messages.min.js', 
                'node_modules/angular-cookies/angular-cookies.min.js',
                'node_modules/angular-material/angular-material.min.js',
                'node_modules/angular-material-data-table/dist/md-data-table.min.js',
                'node_modules/angular-uuid/angular-uuid.js'
            ]
        ),
        gulp.src('js/**/*.js').pipe(concat('scripts.js')).pipe(uglify().on('error', function (e) {
            console.log(e);
        }))
    ])
    .pipe(concat('all.min.js'))
    .pipe(gulp.dest('dist/js'));
});

// Minificar os arquivos html
gulp.task('htmlmin', function(){
    return gulp.src('views/**/*.html')
    .pipe(htmlmin({collapseWhitespace: true}))
    .pipe(gulp.dest('dist/views'));
});

gulp.task('cssmin', function(){
    return merge2(
        gulp.src([
            'node_modules/font-awesome/css/font-awesome.min.css',
            'node_modules/angular-material/angular-material.min.css', 
            'node_modules/angular-material-data-table/dist/md-data-table.min.css'
        ]),
        gulp.src('css/*.css').pipe(cleanCSS())
    )
    .pipe(cleanCSS())
    .pipe(concat('styles.min.css'))
    .pipe(gulp.dest('dist/css'));
});

gulp.task('imagesmin', function(){
    return gulp.src('images/**/*')
    .pipe(imagemin())
    .pipe(gulp.dest('dist/images'));
});

gulp.task('copy-images', function () {
    return gulp.src('images/**/*')
    .pipe(gulp.dest('dist/images'))
})

gulp.task('copy', function(){
    return es.merge(
        [
            gulp.src('index-prod.html')
                .pipe(htmlmin({ collapseWhitespace: true }))
                .pipe(rename('index.html')),
            gulp.src('service-worker-prod.js')
                .pipe(uglify().on('error', function (e) {
                    console.log(e);
                }))
                .pipe(rename('service-worker.js')),
            gulp.src(['favicon.ico', 'manifest.json']),            
        ]
    ) 
    .pipe(gulp.dest('dist/'));
});

gulp.task('copy-fonts', function() {
    return gulp.src(['node_modules/font-awesome/fonts/*', 'fonts/*']).pipe(gulp.dest('dist/fonts'));
});

gulp.task('copy-icons', function() {
    return gulp.src('icons/*').pipe(gulp.dest('dist/icons'))
});

gulp.task('default', function(cb){
    return runSequence('clean', ['jshint', 'uglifyAndConcatJs', 'htmlmin', 'cssmin', 'copy', 'copy-fonts', 'copy-icons', 'imagesmin'], cb);
});