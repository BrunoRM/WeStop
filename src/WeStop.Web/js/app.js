angular.module('WeStop', [
    'ngRoute',
    'ngMessages',
    'ngAnimate',
    'angular-uuid',
    'ngMaterial',
    'md.data.table'
])

    .value('API_SETTINGS', { uri: 'http://localhost:5000' })

.config(['$routeProvider', '$locationProvider', '$mdThemingProvider', '$httpProvider', '$mdGestureProvider', '$mdAriaProvider', function ($routeProvider, $locationProvider, $mdThemingProvider, $httpProvider, $mdGestureProvider, $mdAriaProvider) {

    $httpProvider.interceptors.push('httpInterceptor');

    $locationProvider.hashPrefix('');

    $mdGestureProvider.skipClickHijack();

    $mdThemingProvider.theme('default')
        .dark();

    $mdThemingProvider
        .theme('default')
        .primaryPalette('blue')
        .accentPalette('teal')
        .warnPalette('red');
        // .backgroundPalette('grey');

    $mdAriaProvider.disableWarnings();

    $routeProvider
        .when('/', {
            controller: 'createUserController',
            templateUrl: 'views/user/create.html',
            secure: false
        })
        .when('/lobby', {
            controller: 'lobbyController',
            templateUrl: 'views/lobby.html',
            secure: true
        })

        .when('/game/create', {
            controller: 'createGameController',
            templateUrl: 'views/game/create.html',
            secure: true
        })
        
        .when('/game/:id', {
            controller: 'gameController',
            templateUrl: 'views/game/game.html',
            secure: true
        })
        .otherwise({
            redirectTo: '/'
        });


}])

.run(['$user', '$rootScope', '$location', '$window', ($user, $rootScope, $location, $window) => {

    $rootScope.user = $user.get();

    $rootScope.$on('$routeChangeStart', (e, next, current) => {
        if (next.$$route.secure && !$rootScope.user) {
            $location.path('/');
        } else if (!next.$$route.secure && $rootScope.user) {
            $location.path('/lobby');
        }
    });

    $window.fbAsyncInit = function () {
        FB.init({
            appId: '659220371267596',
            status: true,
            cookie: true,
            xfbml: true,
            version: 'v2.6'
        });
    };

    if ('serviceWorker' in navigator) {
        navigator.serviceWorker.register('./service-worker.js').then(function (registration) {
        }, function(err) {
            console.log('ServiceWorker registration failed: ', err);
        });
    }

}]);