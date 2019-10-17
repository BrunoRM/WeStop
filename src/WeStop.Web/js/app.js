angular.module('WeStop', [
    'ngRoute',
    'ngMessages',
    'ngAnimate',
    'angular-uuid',
    'ngMaterial',
    'md.data.table'
])

.value('API_SETTINGS', { uri: 'http://localhost:5000/api' })

.config(['$routeProvider', '$locationProvider', '$mdThemingProvider', function ($routeProvider, $locationProvider, $mdThemingProvider) {

    $locationProvider.hashPrefix('');

    $mdThemingProvider.theme('default')
        .dark();

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
    .run(['$user', '$rootScope', '$location', '$window', 'googleService', ($user, $rootScope, $location, $window, googleService) => {
    
    $rootScope.user = $user.get();

    $rootScope.$on('$routeChangeStart', (e, next, current) => {
        if (next.$$route.secure && !$rootScope.user) {
            $location.path('/');
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

    let firebaseConfig = {
        apiKey: "AIzaSyAMtxk9YGof82udlAC_LkkB4mgc-JxFmAI",
        authDomain: "westop-1571110414274.firebaseapp.com",
        databaseURL: "https://westop-1571110414274.firebaseio.com",
        projectId: "westop-1571110414274",
        storageBucket: "westop-1571110414274.appspot.com",
        messagingSenderId: "418166283498",
        appId: "1:418166283498:web:7ab90ab027f742f6b80b93"
    };
    
    firebase.initializeApp(firebaseConfig);

    if ('serviceWorker' in navigator) {
        navigator.serviceWorker.register('./service-worker.js').then(function (registration) {
        }, function(err) {
            console.log('ServiceWorker registration failed: ', err);
        });
    }

}]);