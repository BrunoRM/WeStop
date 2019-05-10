angular.module('WeStop', [
    'ngRoute'
])
.config(['$routeProvider', '$locationProvider', function ($routeProvider, $locationProvider) {

    $locationProvider.hashPrefix('');

    $routeProvider

        .when('/lobby', {
            controller: 'lobbyController',
            templateUrl: 'views/lobby.html',
            secure: true
        })

        .when('/game/create', {
            controller: 'createGameController',
            templateUrl: 'views/create-game.html',
            secure: true
        })
        
        .when('/game/:id', {
            controller: 'gameController',
            templateUrl: 'views/game.html',
            secure: true
        })
        .otherwise({
            redirectTo: '/lobby'
        });
        

}])
.run(['$user', '$rootScope', '$location', ($user, $rootScope, $location) => {

    $rootScope.user = $user.get();

    $rootScope.$on('$routeChangeStart', (e, next, current) => {

        if (next.$$route.secure == true && !$rootScope.user) {
            $location.path('/lobby');
        }
    });

}]);