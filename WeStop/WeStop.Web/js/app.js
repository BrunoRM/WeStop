angular.module('WeStop', [
    'ngRoute'
]);

angular.module('WeStop').config(['$routeProvider', '$locationProvider', function ($routeProvider, $locationProvider) {

    $locationProvider.hashPrefix('');

    $routeProvider

        .when('/login', {
            controller: 'loginController',
            templateUrl: 'views/login.html',
            secure: false
        })

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
        

}]);